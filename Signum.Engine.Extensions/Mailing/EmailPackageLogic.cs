using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities.Mailing;
using Signum.Engine.Processes;
using Signum.Engine.Operations;
using Signum.Entities.Processes;
using Signum.Engine.Maps;
using Signum.Engine.DynamicQuery;
using System.Reflection;
using Signum.Entities;
using System.Linq.Expressions;
using Signum.Utilities;
using Signum.Engine.Scheduler;

namespace Signum.Engine.Mailing
{
    public static class EmailPackageLogic
    {
        static Expression<Func<EmailPackageDN, IQueryable<EmailMessageDN>>> MessagesExpression =
            p => Database.Query<EmailMessageDN>().Where(a => a.Package.RefersTo(p));
        public static IQueryable<EmailMessageDN> Messages(this EmailPackageDN p)
        {
            return MessagesExpression.Evaluate(p);
        }

        static Expression<Func<EmailPackageDN, IQueryable<EmailMessageDN>>> RemainingMessagesExpression =
            p => p.Messages().Where(a => a.State == EmailMessageState.RecruitedForSending);
        public static IQueryable<EmailMessageDN> RemainingMessages(this EmailPackageDN p)
        {
            return RemainingMessagesExpression.Evaluate(p);
        }

        static Expression<Func<EmailPackageDN, IQueryable<EmailMessageDN>>> ExceptionMessagesExpression =
            p => p.Messages().Where(a => a.State == EmailMessageState.SentException);
        public static IQueryable<EmailMessageDN> ExceptionMessages(this EmailPackageDN p)
        {
            return ExceptionMessagesExpression.Evaluate(p);
        }

        public static void Start(SchemaBuilder sb, DynamicQueryManager dqm)
        {
            if (sb.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                sb.Include<EmailPackageDN>();

                dqm.RegisterExpression((EmailPackageDN ep) => ep.Messages(), () => EmailMessageMessage.Messages.NiceToString());
                dqm.RegisterExpression((EmailPackageDN ep) => ep.RemainingMessages(), () => EmailMessageMessage.RemainingMessages.NiceToString());
                dqm.RegisterExpression((EmailPackageDN ep) => ep.ExceptionMessages(), () => EmailMessageMessage.ExceptionMessages.NiceToString());

                ProcessLogic.AssertStarted(sb);
                ProcessLogic.Register(EmailMessageProcess.SendEmails, new SendEmailProcessAlgorithm());

                new Graph<ProcessDN>.ConstructFromMany<EmailMessageDN>(EmailMessageOperation.ReSendEmails)
                {
                    Construct = (messages, args) =>
                    {
                        EmailPackageDN emailPackage = new EmailPackageDN()
                        {
                            Name = args.TryGetArgC<string>()
                        }.Save();

                        foreach (var m in messages.Select(m => m.RetrieveAndForget()))
                        {
                            new EmailMessageDN()
                            {
                                Package = emailPackage.ToLite(),
                                From = m.From,
                                Recipients = m.Recipients,
                                Target = m.Target,
                                Body = m.Body,
                                IsBodyHtml = m.IsBodyHtml,
                                Subject = m.Subject,
                                Template = m.Template,
                                SmtpConfiguration = m.SmtpConfiguration,
                                EditableMessage = m.EditableMessage,
                                State = EmailMessageState.RecruitedForSending
                            }.Save();
                        }

                        return ProcessLogic.Create(EmailMessageProcess.SendEmails, emailPackage);
                    }
                }.Register();

                dqm.RegisterQuery(typeof(EmailPackageDN), () =>
                    from e in Database.Query<EmailPackageDN>()
                    select new
                    {
                        Entity = e,
                        e.Id,
                        e.Name,
                    });
            }
        }
    }

    public class SendEmailProcessAlgorithm : IProcessAlgorithm
    {
        public void Execute(ExecutingProcess executingProcess)
        {
            EmailPackageDN package = (EmailPackageDN)executingProcess.Data;

            List<Lite<EmailMessageDN>> emails = package.RemainingMessages()
                                                .OrderBy(e => e.CreationTime)
                                                .Select(e => e.ToLite())
                                                .ToList();
            int counter = 0;
            foreach (var group in emails.GroupsOf(EmailLogic.Configuration.ChunkSizeToProcessEmails))
            {
                var retrieved = group.RetrieveFromListOfLite();
                foreach (var m in retrieved)
                {
                    executingProcess.CancellationToken.ThrowIfCancellationRequested();
                    counter++;
                    try
                    {
                        using (Transaction tr = Transaction.ForceNew())
                        {
                            m.Execute(EmailMessageOperation.Send);
                            tr.Commit();
                        }
                        executingProcess.ProgressChanged(counter, emails.Count);
                    }
                    catch
                    {
                        try
                        {
                            if (m.SendRetries <= EmailLogic.Configuration.MaxEmailSendRetries)
                            {
                                using (Transaction tr = Transaction.ForceNew())
                                {
                                    var nm = m.ToLite().Retrieve();
                                    nm.SendRetries += 1;
                                    nm.Execute(EmailMessageOperation.ReadyToSend);
                                    tr.Commit();
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
    }
}

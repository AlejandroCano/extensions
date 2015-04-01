using Signum.Engine.Maps;
using Signum.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Signum.Engine;
using Signum.Engine.Basics;
using Signum.Entities;
using Signum.Entities.Mailing;
using Signum.Engine.Authorization;

namespace Signum.Engine.Mailing
{
    public static class EmailAsyncSenderLogic
    {
        static Timer timer;
        public static int DelayBetweenProcessesMilliseconds; //60 * 1000;
        internal static DateTime? nextPlannedExecution;
        static bool running = false;
        static CancellationTokenSource CancelProcess;
        static long queuedItems;
        static Guid processIdentifier;

        public static EmailAsyncProcessState ExecutionState()
        {
            return new EmailAsyncProcessState
            {
                Running = running,
                CurrentProcessIdentifier = processIdentifier,
                DelayBetweenProcessesMilliseconds = DelayBetweenProcessesMilliseconds,
                NextPlannedExecution = nextPlannedExecution,
                IsCancelationRequested = CancelProcess.IsCancellationRequested,
                QueuedItems = queuedItems,
                MachineName = Environment.MachineName,
                ApplicationName = Schema.Current.ApplicationName
            };
        }

        public static void StartRunningProcess(int initialDelayMilliseconds)
        {
            if (running)
                throw new InvalidOperationException("EmailAsyncSender process is already running");

            DelayBetweenProcessesMilliseconds = EmailLogic.Configuration.AsyncSenderPeriodMilliseconds;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    running = true;

                    CancelProcess = new CancellationTokenSource();

                    timer = new Timer(new TimerCallback(ExecuteProcessAsync),
                         null, 
                         initialDelayMilliseconds,
                         Timeout.Infinite);
                }
                catch (Exception e)
                {
                    e.LogException(edn =>
                    {
                        edn.ControllerName = "EmailAsyncSender";
                        edn.ActionName = "StartRunningProcess";
                    });
                    throw;
                }
            });
        }

        private static void ExecuteProcessAsync(object state) //obj is ignored
        {
            Task.Factory.StartNew(() => ExecuteProcess(), TaskCreationOptions.LongRunning);
        }

        private static void ExecuteProcess() 
        {
            try
            {
                if (CancelProcess.IsCancellationRequested)
                    return;

                timer.Change(Timeout.Infinite, Timeout.Infinite);
                nextPlannedExecution = null;

                using (HeavyProfiler.Log("EmailAsyncSender", () => "Execute process"))
                using (AuthLogic.Disable())
                {
                    processIdentifier = Guid.NewGuid();
                    DateTime? firstDate = EmailLogic.Configuration.CreationDateHoursLimitToSendEmails == 0 ?
                        null : (DateTime?)TimeZoneManager.Now.AddHours(-EmailLogic.Configuration.CreationDateHoursLimitToSendEmails);
                    queuedItems = Database.Query<EmailMessageDN>().Where(m =>
                        m.State == EmailMessageState.ReadyToSend &&
                        (firstDate == null ? true : m.CreationTime >= firstDate)).UnsafeUpdate()
                            .Set(m => m.RecruitingGuid, m => processIdentifier)
                            .Set(m => m.State, m => EmailMessageState.RecruitedForSending)
                            .Execute();
                    while (queuedItems > 0)
                    {
                        var items = Database.Query<EmailMessageDN>().Where(m =>
                            m.RecruitingGuid == processIdentifier &&
                            m.State == EmailMessageState.RecruitedForSending)
                            .Take(EmailLogic.Configuration.ChunkSizeToProcessEmails).ToList();
                        queuedItems = items.Count;
                        foreach (var email in items)
                        {
                            CancelProcess.Token.ThrowIfCancellationRequested();

                            try
                            {
                                using (Transaction tr = Transaction.ForceNew())
                                {
                                    EmailLogic.SenderManager.Send(email);
                                    tr.Commit();
                                }
                            }
                            catch
                            {
                                try
                                {
                                    if (email.SendRetries < EmailLogic.Configuration.MaxEmailSendRetries)
                                    {
                                        using (Transaction tr = Transaction.ForceNew())
                                        {
                                            var nm = email.ToLite().Retrieve();
                                            nm.SendRetries += 1;
                                            nm.State = EmailMessageState.ReadyToSend;
                                            nm.Save();
                                            tr.Commit();
                                        }
                                    }
                                }
                                catch { }
                            }
                            queuedItems--;
                        }
                        queuedItems = Database.Query<EmailMessageDN>().Where(m =>
                            m.RecruitingGuid == processIdentifier &&
                            m.State == EmailMessageState.RecruitedForSending).Count();
                    }
                }
            }
            catch (Exception e)
            {
                try
                {
                    e.LogException(edn =>
                    {
                        edn.ControllerName = "EmailAsyncSender";
                        edn.ActionName = "ExecuteProcess";
                    });
                }
                catch { }
            }
            SetTimer();
        }

        private static void SetTimer()
        {
            nextPlannedExecution = TimeZoneManager.Now.AddMilliseconds(DelayBetweenProcessesMilliseconds);
            timer.Change(Timeout.Infinite, DelayBetweenProcessesMilliseconds);
        }


        public static void Stop()
        {
            if (!running)
                throw new InvalidOperationException("ProcessLogic is not running");

            using (HeavyProfiler.Log("EmailAsyncSender", () => "Sopping process"))
            {
                timer.Dispose();
                CancelProcess.Cancel();
                nextPlannedExecution = null;
                running = false;
            }
        }
    }


    public class EmailAsyncProcessState
    {
        public int DelayBetweenProcessesMilliseconds;
        public bool Running;
        public bool IsCancelationRequested;
        public DateTime? NextPlannedExecution;
        public long QueuedItems;
        public string MachineName;
        public string ApplicationName;
        public Guid CurrentProcessIdentifier;
    }
}

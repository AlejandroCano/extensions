﻿using Signum.Engine.Maps;
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
using Signum.Engine.Cache;
using System.Data.SqlClient;
using Signum.Engine.Isolation;
using Signum.Entities.Isolation;

namespace Signum.Engine.Mailing
{
    public static class EmailAsyncSenderLogic
    {
        static Timer timer;
        internal static DateTime? nextPlannedExecution;
        static bool running = false;
        static CancellationTokenSource CancelProcess;
        static long queuedItems;
        static Guid processIdentifier;
        static AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        public static Func<EmailConfigurationEntity> GetConfiguration = () => EmailLogic.Configuration;

        public static EmailAsyncProcessState ExecutionState()
        {
            return new EmailAsyncProcessState
            {
                Running = running,
                CurrentProcessIdentifier = processIdentifier,
                DelayBetweenProcessesMilliseconds = GetConfiguration().AsyncSenderPeriodMilliseconds,
                NextPlannedExecution = nextPlannedExecution,
                IsCancelationRequested = CancelProcess != null ? CancelProcess.IsCancellationRequested : false,
                QueuedItems = queuedItems,
                MachineName = Environment.MachineName,
                ApplicationName = Schema.Current.ApplicationName
            };
        }

        public static void StartRunningEmailSenderAsync(int initialDelayMilliseconds)
        {
            if (initialDelayMilliseconds == 0)
                ExecuteProcess();
            else
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(initialDelayMilliseconds);
                    ExecuteProcess();
                });
            }
        }

        private static void ExecuteProcess()
        {
            if (running)
                throw new InvalidOperationException("EmailAsyncSender process is already running");

            Task.Factory.StartNew(() =>
            {
                try
                {
                    running = true;
                    CancelProcess = new CancellationTokenSource();
                    autoResetEvent.Set();

                    timer = new Timer(ob => WakeUp("TimerNextExecution", null),
                         null,
                         Timeout.Infinite,
                         Timeout.Infinite);

                    GC.KeepAlive(timer);

                    using (AuthLogic.Disable())
                    {
                        if (GetConfiguration().CreationDateHoursLimitToSendEmails.HasValue)
                        {
                            DateTime firstDate = TimeZoneManager.Now.AddHours(-GetConfiguration().CreationDateHoursLimitToSendEmails.Value);
                            Database.Query<EmailMessageEntity>().Where(m =>
                                m.State == EmailMessageState.ReadyToSend &&
                                m.CreationTime < firstDate).UnsafeUpdate()
                                .Set(m => m.State, m => EmailMessageState.Outdated)
                                .Execute();
                        }

                        if (CacheLogic.WithSqlDependency)
                        {
                            SetSqlDepndency();
                        }

                        while (autoResetEvent.WaitOne())
                        {
                            if (!GetConfiguration().SendEmails)
                                throw new ApplicationException("Email configuration does not allow email sending");

                            if (CancelProcess.IsCancellationRequested)
                                return;

                            timer.Change(Timeout.Infinite, Timeout.Infinite);
                            nextPlannedExecution = null;

                            using (HeavyProfiler.Log("EmailAsyncSender", () => "Execute process"))
                            {
                                processIdentifier = Guid.NewGuid();
                                if (RecruitQueuedItems())
                                {
                                    while (queuedItems > 0 || RecruitQueuedItems())
                                    {
                                        var items = Database.Query<EmailMessageEntity>().Where(m =>
                                            m.ProcessIdentifier == processIdentifier &&
                                            m.State == EmailMessageState.RecruitedForSending)
                                            .Take(GetConfiguration().ChunkSizeToProcessEmails).ToList();
                                        queuedItems = items.Count;
                                        foreach (var email in items)
                                        {
                                            CancelProcess.Token.ThrowIfCancellationRequested();

                                            try
                                            {
                                                using (Transaction tr = Transaction.ForceNew())
                                                {
                                                    if (MixinDeclarations.IsDeclared(typeof(EmailMessageEntity), typeof(IsolationMixin)) && email.Isolation() != null)
                                                    {
                                                        using (IsolationEntity.Override(email.Isolation()))
                                                            EmailLogic.SenderManager.Send(email);
                                                    }
                                                    else 
                                                        EmailLogic.SenderManager.Send(email);
                                                    
                                                    tr.Commit();
                                                }
                                            }
                                            catch
                                            {
                                                try
                                                {
                                                    if (email.SendRetries < GetConfiguration().MaxEmailSendRetries)
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
                                        queuedItems = Database.Query<EmailMessageEntity>().Where(m =>
                                            m.ProcessIdentifier == processIdentifier &&
                                            m.State == EmailMessageState.RecruitedForSending).Count();
                                    }
                                }
                                SetTimer();
                                SetSqlDepndency();
                            }
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
                finally
                {
                    running = false;
                }
            }, TaskCreationOptions.LongRunning);
        }

        private static void SetSqlDepndency()
        {
            var query = Database.Query<EmailMessageEntity>().Where(m => m.State == EmailMessageState.RecruitedForSending).Select(m => m.Id);
            query.ToListWithInvalidation(typeof(EmailMessageEntity), "EmailAsyncSender ReadyToSend dependency", a => WakeUp("EmailAsyncSender ReadyToSend dependency", a));
        }

        private static bool RecruitQueuedItems()
        {
            DateTime? firstDate = GetConfiguration().CreationDateHoursLimitToSendEmails == null ?
                null : (DateTime?)TimeZoneManager.Now.AddHours(-GetConfiguration().CreationDateHoursLimitToSendEmails.Value);
            queuedItems = Database.Query<EmailMessageEntity>().Where(m =>
                m.State == EmailMessageState.ReadyToSend &&
                (firstDate == null ? true : m.CreationTime >= firstDate)).UnsafeUpdate()
                    .Set(m => m.ProcessIdentifier, m => processIdentifier)
                    .Set(m => m.State, m => EmailMessageState.RecruitedForSending)
                    .Execute();
            return queuedItems > 0;
        }

        internal static bool WakeUp(string reason, SqlNotificationEventArgs args)
        {
            using (HeavyProfiler.Log("EmailAsyncSender WakeUp", () => "WakeUp! " + reason + ToString(args)))
            {
                return autoResetEvent.Set();
            }
        }

        private static string ToString(SqlNotificationEventArgs args)
        {
            if (args == null)
                return null;

            return " ({0} {1} {2})".FormatWith(args.Type, args.Source, args.Info);
        }

        private static void SetTimer()
        {
            nextPlannedExecution = TimeZoneManager.Now.AddMilliseconds(GetConfiguration().AsyncSenderPeriodMilliseconds);
            timer.Change(GetConfiguration().AsyncSenderPeriodMilliseconds, System.Threading.Timeout.Infinite);
        }

        public static void Stop()
        {
            if (!running)
                throw new InvalidOperationException("EmailAsyncSender is not running");

            using (HeavyProfiler.Log("EmailAsyncSender", () => "Stopping process"))
            {
                timer.Dispose();
                CancelProcess.Cancel();
                WakeUp("Stop", null);
                nextPlannedExecution = null;
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

﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using Signum.Services;
using Signum.Utilities;
using Signum.Entities;
using Signum.Web;
using Signum.Engine;
using Signum.Engine.Operations;
using Signum.Engine.Basics;
using Signum.Entities.Mailing;
using Signum.Engine.Mailing;
using Signum.Engine.Authorization;
using Signum.Web.Operations;

namespace Signum.Web.Mailing
{
    public class AsyncEmailSenderController : Controller
    {
        [HttpGet]
        public new ActionResult View()
        {
            EmailAsyncProcessState state = EmailAsyncSenderLogic.ExecutionState();

            if (Request.IsAjaxRequest())
                return PartialView(MailingClient.ViewPrefix.Formato("AsyncEmailSenderDashboard"), state);
            else
                return View(MailingClient.ViewPrefix.Formato("AsyncEmailSenderDashboard"), state);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Start()
        {
            AsyncEmailSenderPermission.ViewAsyncEmailSenderPanel.AssertAuthorized();

            EmailAsyncSenderLogic.StartRunningProcess(0);

            Thread.Sleep(1000);

            return null;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Stop()
        {
            AsyncEmailSenderPermission.ViewAsyncEmailSenderPanel.AssertAuthorized();

            EmailAsyncSenderLogic.Stop();

            Thread.Sleep(1000);

            return null;
        }
    }
}
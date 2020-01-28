﻿using Signum.Engine;
using Signum.Engine.Basics;
using Signum.Engine.DynamicQuery;
using Signum.Engine.Mailing;
using Signum.Engine.Operations;
using Signum.Entities;
using Signum.Entities.DynamicQuery;
using Signum.Entities.Mailing;
using Signum.Utilities;
using Signum.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Signum.Web.Operations;
using Signum.Engine.Mailing.Pop3;
using Signum.Entities.Authorization;
using Signum.Web.Auth;

namespace Signum.Web.Mailing
{
    public class MailingController : Controller
    {


        [HttpGet]
        public bool GettingCancelGet()
        {
            return Pop3ConfigurationLogic.GettingCancel;
            //return new JsonResult{ Data = new { GettingCancel = Pop3ConfigurationLogic.GettingCancel } };
        }

        [HttpGet]
        public void GettingCancelSet(bool cancel)
        {
             Pop3ConfigurationLogic.GettingCancel=cancel;
        }


        [HttpPost]
        public ContentResult NewSubTokensCombo(string webQueryName, string tokenName, string prefix, int options)
        {
            object queryName = Finder.ResolveQueryName(webQueryName);
            QueryDescription qd = DynamicQueryManager.Current.QueryDescription(queryName);
            var token = QueryUtils.Parse(tokenName, qd, (SubTokensOptions)options);

            var combo = FinderController.CreateHtmlHelper(this)
                .QueryTokenBuilderOptions(token, new Context(null, prefix), MailingClient.GetQueryTokenBuilderSettings(qd, (SubTokensOptions)options));

            return Content(combo.ToHtmlString());
        }

        [HttpPost]
        public ActionResult RemoveRecipientsExecute()
        {
            var deliveries = this.ParseLiteKeys<NewsletterDeliveryEntity>();

            var newsletter = this.ExtractEntity<NewsletterEntity>();
            
            newsletter.Execute(NewsletterOperation.RemoveRecipients, deliveries);

            return this.DefaultExecuteResult(newsletter);
        }

        [HttpPost]
        public ActionResult CreateMailFromTemplateAndEntity()
        {
            var entity = Lite.Parse(Request["keys"]).Retrieve();

            var emailMessage = this.ExtractEntity<EmailTemplateEntity>()
                .ConstructFrom(EmailMessageOperation.CreateMailFromTemplate, entity);

            return this.DefaultConstructResult(emailMessage);
        }

        [HttpPost]
        public JsonResult GetEmailTemplateEntityImplementations()
        {
            var template = Lite.Parse<EmailTemplateEntity>(Request["template"]);

            var implementations = SendEmailTaskLogic.GetImplementations(template.InDB(a => a.Query));

            return new JsonResult
            {
                Data = implementations == null ?
                new JsExtensions.JsTypeInfo[0] :
                implementations.Value.ToJsTypeInfos(isSearch: false, prefix: "")
            };
        }

        [HttpPost]
        public ActionResult SetSmtpPasswordOnOk()
        {
            var newPassword = this.ParseValue<string>("newPassword");

            var smtp = this.ExtractLite<SmtpConfigurationEntity>()
                .ExecuteLite(SmtpConfigurationOperation.SetPassword, newPassword);
            
            return this.DefaultExecuteResult(smtp);
        }

        [HttpPost]
        public ActionResult SetPop3PasswordOnOk()
        {
            var newPassword = this.ParseValue<string>("newPassword");

            var pop3 = this.ExtractLite<Pop3ConfigurationEntity>()
                .ExecuteLite(Pop3ConfigurationOperation.SetPassword, newPassword);
            
            return this.DefaultExecuteResult(pop3);
        }
    }
}

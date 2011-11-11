﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using Signum.Utilities;
    using Signum.Entities;
    using Signum.Web;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Caching;
    using System.Web.DynamicData;
    using System.Web.SessionState;
    using System.Web.Profile;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.Web.UI.HtmlControls;
    using System.Xml.Linq;
    using Signum.Entities.Mailing;
    using Signum.Entities.Reflection;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("MvcRazorClassGenerator", "1.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Mailing/Views/Newsletter.cshtml")]
    public class _Page_Mailing_Views_Newsletter_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {


        public _Page_Mailing_Views_Newsletter_cshtml()
        {
        }
        protected System.Web.HttpApplication ApplicationInstance
        {
            get
            {
                return ((System.Web.HttpApplication)(Context.ApplicationInstance));
            }
        }
        public override void Execute()
        {



Write(Html.ScriptCss("~/Mailing/Content/Mail.css"));

WriteLiteral("\r\n");


 using (var nc = Html.TypeContext<NewsletterDN>())
{
    bool editable = nc.Value.State == NewsletterState.Saved;
    

WriteLiteral("    <div class=\"");


            Write(editable ? "" : "sf-tabs");

WriteLiteral("\">\r\n");


         if (!editable)
        {

WriteLiteral("            <ul>\r\n                <li><a href=\"#emTabMain\">Newsletter</a></li>\r\n " +
"               <li><a href=\"#emTabSend\">Deliveries</a></li>\r\n            </ul>\r\n" +
"");


        }

WriteLiteral("        <div id=\"emTabMain\">\r\n            ");


       Write(Html.ValueLine(nc, n => n.Name));

WriteLiteral("\r\n           \r\n            ");


       Write(Html.HiddenRuntimeInfo(nc, n => n.Query));

WriteLiteral("\r\n        \r\n            ");


       Write(Html.ValueLine(nc, n => n.State, vl => vl.ReadOnly = true));

WriteLiteral("\r\n            ");


       Write(Html.ValueLine(nc, n => n.Subject));

WriteLiteral("\r\n");


             if (!editable)
            {
                
           Write(Html.Hidden("htmlBodyContent", nc.Value.HtmlBody));

                                                                  

WriteLiteral(@"                <fieldset>
                    <legend>Message</legend>
                    <iframe id=""newsBodyContent"" name=""frameNewImage"" src=""about:blank"" class=""sf-email-htmlbody"" frameborder=""0"">
                    </iframe>
                </fieldset>
");


            }
            else
            {

WriteLiteral("                <div id=\"newsEditContent\">\r\n                    ");


               Write(Html.ValueLine(nc, n => n.HtmlBody, vl =>
                    {
                        vl.ValueLineType = ValueLineType.TextArea;
                        vl.ValueHtmlProps["cols"] = "30";
                        vl.ValueHtmlProps["rows"] = "6";
                        vl.ValueHtmlProps["class"] = "sf-email-htmlwrite";
                        vl.ReadOnly = !editable;
                    }));

WriteLiteral("\r\n                    <br />\r\n                    <input type=\"button\" class=\"sf-" +
"button\" id=\"newsPreviewContentButton\" value=\"Preview content\" />\r\n              " +
"  </div>\r\n");



WriteLiteral(@"                <div id=""newsPreviewContent"">
                    <fieldset>
                        <legend>Message</legend>
                        <iframe id=""newsBodyContentPreview"" name=""frameNewImage"" src=""about:blank"" class=""sf-email-htmlbody""
                            frameborder=""0""></iframe>
                        <br />
                        <input type=""button"" class=""sf-button"" id=""newsEditContentButton"" value=""Edit content"" />
                    </fieldset>
                </div>
");


            }

WriteLiteral("            ");


       Write(Html.EntityCombo(nc, n => n.SMTPConfig));

WriteLiteral("\r\n            ");


       Write(Html.ValueLine(nc, n => n.From));

WriteLiteral("\r\n        </div>\r\n");


         if (!editable)
        {

WriteLiteral("            <div id=\"emTabSend\">\r\n                ");


           Write(Html.ValueLine(nc, n => n.NumLines));

WriteLiteral("\r\n                ");


           Write(Html.ValueLine(nc, n => n.NumErrors));

WriteLiteral("\r\n                <fieldset>\r\n                    <legend>Email recipients</legen" +
"d>\r\n                    ");


               Write(Html.SearchControl(new FindOptions(typeof(NewsletterDeliveryDN))
               {
                   FilterOptions = { new FilterOption("Newsletter", nc.Value) { Frozen = true } },
                   SearchOnLoad = true,
               }, new Context(nc, "ncSent")));

WriteLiteral("\r\n                </fieldset>\r\n            </div>\r\n");


        }

WriteLiteral("    </div>    \r\n");


}


Write(Html.ScriptsJs("~/Mailing/Scripts/SF_Mail.js"));

WriteLiteral("\r\n");


        }
    }
}

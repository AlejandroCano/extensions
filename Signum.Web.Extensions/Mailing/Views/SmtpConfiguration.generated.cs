﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
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
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using Signum.Entities;
    
    #line 1 "..\..\Mailing\Views\SmtpConfiguration.cshtml"
    using Signum.Entities.Mailing;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Mailing/Views/SmtpConfiguration.cshtml")]
    public partial class _Mailing_Views_SmtpConfiguration_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {
        public _Mailing_Views_SmtpConfiguration_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\n");

            
            #line 3 "..\..\Mailing\Views\SmtpConfiguration.cshtml"
 using (var sc = Html.TypeContext<SmtpConfigurationEntity>())
{
    
            
            #line default
            #line hidden
            
            #line 5 "..\..\Mailing\Views\SmtpConfiguration.cshtml"
Write(Html.ValueLine(sc, s => s.Name));

            
            #line default
            #line hidden
            
            #line 5 "..\..\Mailing\Views\SmtpConfiguration.cshtml"
                                    
    
            
            #line default
            #line hidden
            
            #line 6 "..\..\Mailing\Views\SmtpConfiguration.cshtml"
Write(Html.ValueLine(sc, s => s.DeliveryFormat));

            
            #line default
            #line hidden
            
            #line 6 "..\..\Mailing\Views\SmtpConfiguration.cshtml"
                                              
    
            
            #line default
            #line hidden
            
            #line 7 "..\..\Mailing\Views\SmtpConfiguration.cshtml"
Write(Html.ValueLine(sc, s => s.DeliveryMethod));

            
            #line default
            #line hidden
            
            #line 7 "..\..\Mailing\Views\SmtpConfiguration.cshtml"
                                              

    
            
            #line default
            #line hidden
            
            #line 9 "..\..\Mailing\Views\SmtpConfiguration.cshtml"
Write(Html.EntityDetail(sc, s => s.Network));

            
            #line default
            #line hidden
            
            #line 9 "..\..\Mailing\Views\SmtpConfiguration.cshtml"
                                          

    
            
            #line default
            #line hidden
            
            #line 11 "..\..\Mailing\Views\SmtpConfiguration.cshtml"
Write(Html.ValueLine(sc, s => s.PickupDirectoryLocation));

            
            #line default
            #line hidden
            
            #line 11 "..\..\Mailing\Views\SmtpConfiguration.cshtml"
                                                       
    
            
            #line default
            #line hidden
            
            #line 12 "..\..\Mailing\Views\SmtpConfiguration.cshtml"
Write(Html.EntityDetail(sc, s => s.DefaultFrom));

            
            #line default
            #line hidden
            
            #line 12 "..\..\Mailing\Views\SmtpConfiguration.cshtml"
                                              
    
            
            #line default
            #line hidden
            
            #line 13 "..\..\Mailing\Views\SmtpConfiguration.cshtml"
Write(Html.EntityRepeater(sc, s => s.AdditionalRecipients));

            
            #line default
            #line hidden
            
            #line 13 "..\..\Mailing\Views\SmtpConfiguration.cshtml"
                                                         
}
            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591

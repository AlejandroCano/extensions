﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18010
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Signum.Web.Extensions.Mailing.Views
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
    
    #line 1 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
    using Signum.Entities.Mailing;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.5.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Mailing/Views/SMTPConfiguration.cshtml")]
    public class SMTPConfiguration : System.Web.Mvc.WebViewPage<dynamic>
    {
        public SMTPConfiguration()
        {
        }
        public override void Execute()
        {

WriteLiteral("\r\n");


            
            #line 3 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
 using (var sc = Html.TypeContext<SMTPConfigurationDN>()) 
{
	
            
            #line default
            #line hidden
            
            #line 5 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
Write(Html.ValueLine(sc, s => s.Name));

            
            #line default
            #line hidden
            
            #line 5 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
                                 
	
            
            #line default
            #line hidden
            
            #line 6 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
Write(Html.ValueLine(sc, s => s.Port));

            
            #line default
            #line hidden
            
            #line 6 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
                                 
	
            
            #line default
            #line hidden
            
            #line 7 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
Write(Html.ValueLine(sc, s => s.Host));

            
            #line default
            #line hidden
            
            #line 7 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
                                 
	
            
            #line default
            #line hidden
            
            #line 8 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
Write(Html.ValueLine(sc, s => s.UseDefaultCredentials));

            
            #line default
            #line hidden
            
            #line 8 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
                                                  
	
            
            #line default
            #line hidden
            
            #line 9 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
Write(Html.ValueLine(sc, s => s.Username));

            
            #line default
            #line hidden
            
            #line 9 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
                                     
	
            
            #line default
            #line hidden
            
            #line 10 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
Write(Html.ValueLine(sc, s => s.Password));

            
            #line default
            #line hidden
            
            #line 10 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
                                     
	
            
            #line default
            #line hidden
            
            #line 11 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
Write(Html.ValueLine(sc, s => s.EnableSSL));

            
            #line default
            #line hidden
            
            #line 11 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
                                      
    
            
            #line default
            #line hidden
            
            #line 12 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
Write(Html.EntityList(sc, s => s.ClientCertificationFiles));

            
            #line default
            #line hidden
            
            #line 12 "..\..\Mailing\Views\SMTPConfiguration.cshtml"
                                                         
}

            
            #line default
            #line hidden

        }
    }
}
#pragma warning restore 1591

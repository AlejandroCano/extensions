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
    using Signum.Utilities;
    
    #line 1 "..\..\Profiler\Views\HeavyDetails.cshtml"
    using Signum.Utilities.ExpressionTrees;
    
    #line default
    #line hidden
    using Signum.Web;
    
    #line 2 "..\..\Profiler\Views\HeavyDetails.cshtml"
    using Signum.Web.Profiler;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Profiler/Views/HeavyDetails.cshtml")]
    public partial class _Profiler_Views_HeavyDetails_cshtml : System.Web.Mvc.WebViewPage<HeavyProfilerEntry>
    {
        public _Profiler_Views_HeavyDetails_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("    <div");

WriteLiteral(" id=\"profilerExternalDiv\"");

WriteLiteral(">\r\n    <h2>\r\n        Profiler Entry (\r\n");

            
            #line 7 "..\..\Profiler\Views\HeavyDetails.cshtml"
        
            
            #line default
            #line hidden
            
            #line 7 "..\..\Profiler\Views\HeavyDetails.cshtml"
         foreach (var e in Model.Follow(a => a.Parent).Skip(1).Reverse())
        {
            
            
            #line default
            #line hidden
            
            #line 9 "..\..\Profiler\Views\HeavyDetails.cshtml"
       Write(Html.ProfilerEntry(e.Index.ToString(), e.FullIndex()));

            
            #line default
            #line hidden
WriteLiteral(".\r\n");

            
            #line 10 "..\..\Profiler\Views\HeavyDetails.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("        ");

            
            #line 11 "..\..\Profiler\Views\HeavyDetails.cshtml"
   Write(Model.Index.ToString());

            
            #line default
            #line hidden
WriteLiteral(")\r\n    </h2>\r\n");

WriteLiteral("    ");

            
            #line 13 "..\..\Profiler\Views\HeavyDetails.cshtml"
Write(Html.ActionLink("(View all)", (ProfilerController pc) => pc.Heavy(false)));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 14 "..\..\Profiler\Views\HeavyDetails.cshtml"
Write(Html.ActionLink("Download", (ProfilerController pc) => pc.DownloadFile(Model.FullIndex()), new { @class = "btn btn-default pull-right" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <br />\r\n    <h3>Breakdown</h3>\r\n    <div");

WriteLiteral(" class=\"sf-profiler-chart\"");

WriteLiteral(" data-detail-url=\"");

            
            #line 17 "..\..\Profiler\Views\HeavyDetails.cshtml"
                                               Write(Url.Action("HeavyRoute", "Profiler"));

            
            #line default
            #line hidden
WriteLiteral("\"");

WriteLiteral(">\r\n    </div>\r\n    <br />\r\n    <table");

WriteLiteral(" class=\"table table-nonfluid\"");

WriteLiteral(">\r\n        <tr>\r\n            <th>\r\n                Role\r\n            </th>\r\n     " +
"       <td>\r\n");

WriteLiteral("                ");

            
            #line 26 "..\..\Profiler\Views\HeavyDetails.cshtml"
           Write(Model.Role);

            
            #line default
            #line hidden
WriteLiteral("\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <th>\r\n             " +
"   Time\r\n            </th>\r\n            <td>\r\n");

WriteLiteral("                ");

            
            #line 34 "..\..\Profiler\Views\HeavyDetails.cshtml"
           Write(Model.ElapsedToString());

            
            #line default
            #line hidden
WriteLiteral("\r\n            </td>\r\n        </tr>\r\n    </table>\r\n    <br />\r\n    <h3>Aditional D" +
"ata</h3>\r\n    <div>\r\n        <pre><code>");

            
            #line 41 "..\..\Profiler\Views\HeavyDetails.cshtml"
              Write(Model.AdditionalData);

            
            #line default
            #line hidden
WriteLiteral("</code></pre>\r\n    </div>\r\n    <br />\r\n    <h3>StackTrace</h3>\r\n");

            
            #line 45 "..\..\Profiler\Views\HeavyDetails.cshtml"
    
            
            #line default
            #line hidden
            
            #line 45 "..\..\Profiler\Views\HeavyDetails.cshtml"
     if (Model.StackTrace == null)
    {

            
            #line default
            #line hidden
WriteLiteral("        <span>No StackTrace</span>\r\n");

            
            #line 48 "..\..\Profiler\Views\HeavyDetails.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <table");

WriteLiteral(" class=\"table table-condensed\"");

WriteLiteral(@">
            <thead>
                <tr>
                    <th>
                        Type
                    </th>
                    <th>
                        Method
                    </th>
                    <th>
                        FileLine
                    </th>
                </tr>
            </thead>
            <tbody>
");

            
            #line 66 "..\..\Profiler\Views\HeavyDetails.cshtml"
                
            
            #line default
            #line hidden
            
            #line 66 "..\..\Profiler\Views\HeavyDetails.cshtml"
                 for (int i = 0; i < Model.StackTrace.FrameCount; i++)
                {
                    var frame = Model.StackTrace.GetFrame(i);
                    var type = frame.GetMethod().DeclaringType;

            
            #line default
            #line hidden
WriteLiteral("                    <tr>\r\n                        <td>\r\n");

            
            #line 72 "..\..\Profiler\Views\HeavyDetails.cshtml"
                            
            
            #line default
            #line hidden
            
            #line 72 "..\..\Profiler\Views\HeavyDetails.cshtml"
                             if (type != null)
                            {
                                var color = ColorExtensions.ToHtmlColor(type.Assembly.FullName.GetHashCode());


            
            #line default
            #line hidden
WriteLiteral("                                <span");

WriteAttribute("style", Tuple.Create(" style=\"", 2306), Tuple.Create("\"", 2326)
, Tuple.Create(Tuple.Create("", 2314), Tuple.Create("color:", 2314), true)
            
            #line 76 "..\..\Profiler\Views\HeavyDetails.cshtml"
, Tuple.Create(Tuple.Create("", 2320), Tuple.Create<System.Object, System.Int32>(color
            
            #line default
            #line hidden
, 2320), false)
);

WriteLiteral(">");

            
            #line 76 "..\..\Profiler\Views\HeavyDetails.cshtml"
                                                      Write(frame.GetMethod().DeclaringType.Try(t => t.TypeName()));

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n");

            
            #line 77 "..\..\Profiler\Views\HeavyDetails.cshtml"
                            }

            
            #line default
            #line hidden
WriteLiteral("                        </td>\r\n                        <td>\r\n");

WriteLiteral("                            ");

            
            #line 80 "..\..\Profiler\Views\HeavyDetails.cshtml"
                       Write(frame.GetMethod().Name);

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </td>\r\n                        <td>\r\n");

WriteLiteral("                            ");

            
            #line 83 "..\..\Profiler\Views\HeavyDetails.cshtml"
                       Write(frame.GetFileName());

            
            #line default
            #line hidden
WriteLiteral(" (");

            
            #line 83 "..\..\Profiler\Views\HeavyDetails.cshtml"
                                             Write(frame.GetFileLineNumber());

            
            #line default
            #line hidden
WriteLiteral(")\r\n                        </td>\r\n                    </tr>\r\n");

            
            #line 86 "..\..\Profiler\Views\HeavyDetails.cshtml"
                }

            
            #line default
            #line hidden
WriteLiteral("            </tbody>\r\n        </table>\r\n");

            
            #line 89 "..\..\Profiler\Views\HeavyDetails.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("    <br />\r\n");

WriteLiteral("    ");

            
            #line 91 "..\..\Profiler\Views\HeavyDetails.cshtml"
Write(Html.ScriptCss("~/Profiler/Content/Profiler.css"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 92 "..\..\Profiler\Views\HeavyDetails.cshtml"
    
            
            #line default
            #line hidden
            
            #line 92 "..\..\Profiler\Views\HeavyDetails.cshtml"
      
        var fullTree = Model.Follow(e => e.Parent).ToList();
        fullTree.AddRange(Model.Descendants());

        int max = fullTree.Max(a => a.Depth);
        while (fullTree.Count > ProfilerClient.MaxEntriesToDisplay && Model.Depth + 1 < max)
        {
            fullTree.RemoveAll(a => a.Depth == max);
            max--;
        }
    
            
            #line default
            #line hidden
WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        $(function () {\r\n");

WriteLiteral("            ");

            
            #line 105 "..\..\Profiler\Views\HeavyDetails.cshtml"
        Write(ProfilerClient.Module["heavyDetailsChart"](fullTree.HeavyDetailsToJson(), Model.Depth));

            
            #line default
            #line hidden
WriteLiteral(";\r\n        });\r\n    </script>\r\n</div>");

        }
    }
}
#pragma warning restore 1591

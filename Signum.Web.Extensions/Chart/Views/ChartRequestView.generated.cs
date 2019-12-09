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
    
    #line 4 "..\..\Chart\Views\ChartRequestView.cshtml"
    using System.Configuration;
    
    #line default
    #line hidden
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
    
    #line 3 "..\..\Chart\Views\ChartRequestView.cshtml"
    using Signum.Engine.DynamicQuery;
    
    #line default
    #line hidden
    
    #line 6 "..\..\Chart\Views\ChartRequestView.cshtml"
    using Signum.Entities;
    
    #line default
    #line hidden
    
    #line 7 "..\..\Chart\Views\ChartRequestView.cshtml"
    using Signum.Entities.Chart;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Chart\Views\ChartRequestView.cshtml"
    using Signum.Entities.DynamicQuery;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Chart\Views\ChartRequestView.cshtml"
    using Signum.Entities.Reflection;
    
    #line default
    #line hidden
    using Signum.Utilities;
    
    #line 1 "..\..\Chart\Views\ChartRequestView.cshtml"
    using Signum.Web;
    
    #line default
    #line hidden
    
    #line 8 "..\..\Chart\Views\ChartRequestView.cshtml"
    using Signum.Web.Chart;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Chart/Views/ChartRequestView.cshtml")]
    public partial class _Chart_Views_ChartRequestView_cshtml : System.Web.Mvc.WebViewPage<TypeContext<ChartRequest>>
    {
        public _Chart_Views_ChartRequestView_cshtml()
        {
        }
        public override void Execute()
        {
            
            #line 10 "..\..\Chart\Views\ChartRequestView.cshtml"
Write(Html.ScriptCss("~/Chart/Content/Chart.css"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 11 "..\..\Chart\Views\ChartRequestView.cshtml"
   
    QueryDescription queryDescription = (QueryDescription)ViewData[ViewDataKeys.QueryDescription];
    if (queryDescription == null)
    {
        queryDescription = DynamicQueryManager.Current.QueryDescription(Model.Value.QueryName);
        ViewData[ViewDataKeys.QueryDescription] = queryDescription;
    }

    List<FilterOption> filterOptions = (List<FilterOption>)ViewData[ViewDataKeys.FilterOptions];

    var entityColumn = queryDescription.Columns.SingleEx(a => a.IsEntity);
    Type entitiesType = entityColumn.Type.CleanType();

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

            
            #line 25 "..\..\Chart\Views\ChartRequestView.cshtml"
 using (Html.BeginForm())
{

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"sf-chart-control-container\"");

WriteLiteral(">\r\n        <h2>\r\n            <span");

WriteLiteral(" class=\"sf-entity-title\"");

WriteLiteral(">");

            
            #line 29 "..\..\Chart\Views\ChartRequestView.cshtml"
                                      Write(ViewBag.Title);

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n            <a");

WriteAttribute("id", Tuple.Create(" id=\"", 1044), Tuple.Create("\"", 1079)
            
            #line 30 "..\..\Chart\Views\ChartRequestView.cshtml"
, Tuple.Create(Tuple.Create("", 1049), Tuple.Create<System.Object, System.Int32>(Model.Compose("sfFullScreen")
            
            #line default
            #line hidden
, 1049), false)
);

WriteLiteral(" class=\"sf-popup-fullscreen\"");

WriteLiteral(" href=\"#\"");

WriteLiteral(">\r\n                <span");

WriteLiteral(" class=\"glyphicon glyphicon-new-window\"");

WriteLiteral("></span>\r\n            </a>\r\n        </h2>\r\n");

WriteLiteral("        ");

            
            #line 34 "..\..\Chart\Views\ChartRequestView.cshtml"
   Write(Html.ValidationSummaryAjax());

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 35 "..\..\Chart\Views\ChartRequestView.cshtml"
   Write(Html.AntiForgeryToken());

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n        <div");

WriteAttribute("id", Tuple.Create(" id=\"", 1310), Tuple.Create("\"", 1347)
            
            #line 37 "..\..\Chart\Views\ChartRequestView.cshtml"
, Tuple.Create(Tuple.Create("", 1315), Tuple.Create<System.Object, System.Int32>(Model.Compose("sfChartControl")
            
            #line default
            #line hidden
, 1315), false)
);

WriteLiteral(" class=\"sf-chart-control SF-control-container\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 38 "..\..\Chart\Views\ChartRequestView.cshtml"
       Write(Html.HiddenRuntimeInfo(Model));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 39 "..\..\Chart\Views\ChartRequestView.cshtml"
       Write(Html.Hidden(Model.Compose("sfOrders"), Model.Value.Orders.IsNullOrEmpty() ? "" :
        (Model.Value.Orders.ToString(oo => (oo.OrderType == OrderType.Ascending ? "" : "-") + oo.Token.FullKey(), ";") + ";")));

            
            #line default
            #line hidden
WriteLiteral("\r\n            <div>\r\n");

            
            #line 42 "..\..\Chart\Views\ChartRequestView.cshtml"
                
            
            #line default
            #line hidden
            
            #line 42 "..\..\Chart\Views\ChartRequestView.cshtml"
                  
                    ViewData[ViewDataKeys.FilterOptions] = filterOptions;
                    ViewData[ViewDataKeys.QueryTokenSettings] = ChartClient.GetQueryTokenBuilderSettings(queryDescription,
                        SubTokensOptions.CanAnyAll | SubTokensOptions.CanElement | (Model.Value.GroupResults ? SubTokensOptions.CanAggregate : 0));
                    Html.RenderPartial(Finder.Manager.FilterBuilderView);
                
            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n            </div>\r\n            <div");

WriteAttribute("id", Tuple.Create(" id=\"", 2183), Tuple.Create("\"", 2229)
            
            #line 50 "..\..\Chart\Views\ChartRequestView.cshtml"
, Tuple.Create(Tuple.Create("", 2188), Tuple.Create<System.Object, System.Int32>(Model.Compose("sfChartBuilderContainer")
            
            #line default
            #line hidden
, 2188), false)
);

WriteLiteral(" class=\"SF-control-container\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 51 "..\..\Chart\Views\ChartRequestView.cshtml"
           Write(Html.Partial(ChartClient.ChartBuilderView, Model));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n                require([\"");

            
            #line 54 "..\..\Chart\Views\ChartRequestView.cshtml"
                     Write(ChartClient.Module);

            
            #line default
            #line hidden
WriteLiteral("\"], function (Chart) {\r\n                    var options = ");

            
            #line 55 "..\..\Chart\Views\ChartRequestView.cshtml"
                              Write(MvcHtmlString.Create(Model.Value.ToChartRequest(Url, "", ChartRequestMode.complete).ToString()));

            
            #line default
            #line hidden
WriteLiteral(";\r\n                    new Chart.ChartRequest(options);\r\n                });\r\n   " +
"         </script>\r\n            <div");

WriteLiteral(" class=\"sf-query-button-bar\"");

WriteLiteral(">\r\n                <button");

WriteLiteral(" type=\"submit\"");

WriteLiteral(" class=\"sf-query-button sf-chart-draw btn btn-primary\"");

WriteAttribute("id", Tuple.Create(" id=\"", 2835), Tuple.Create("\"", 2864)
            
            #line 60 "..\..\Chart\Views\ChartRequestView.cshtml"
                , Tuple.Create(Tuple.Create("", 2840), Tuple.Create<System.Object, System.Int32>(Model.Compose("qbDraw")
            
            #line default
            #line hidden
, 2840), false)
);

WriteLiteral(">");

            
            #line 60 "..\..\Chart\Views\ChartRequestView.cshtml"
                                                                                                                     Write(ChartMessage.Chart_Draw.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("</button>\r\n                <button");

WriteLiteral(" class=\"sf-query-button sf-chart-script-edit btn btn-default\"");

WriteAttribute("id", Tuple.Create(" id=\"", 3000), Tuple.Create("\"", 3029)
            
            #line 61 "..\..\Chart\Views\ChartRequestView.cshtml"
         , Tuple.Create(Tuple.Create("", 3005), Tuple.Create<System.Object, System.Int32>(Model.Compose("qbEdit")
            
            #line default
            #line hidden
, 3005), false)
);

WriteLiteral(">");

            
            #line 61 "..\..\Chart\Views\ChartRequestView.cshtml"
                                                                                                              Write(ChartMessage.EditScript.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("</button>\r\n");

WriteLiteral("                ");

            
            #line 62 "..\..\Chart\Views\ChartRequestView.cshtml"
           Write(UserChartClient.GetChartMenu(this.ViewContext, Url, queryDescription.QueryName, entitiesType, Model.Prefix, (Lite<UserChartDN>)ViewData["UserChart"]).ToStringButton(Html));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <br />\r\n            <div");

WriteAttribute("id", Tuple.Create(" id=\"", 3326), Tuple.Create("\"", 3359)
            
            #line 65 "..\..\Chart\Views\ChartRequestView.cshtml"
, Tuple.Create(Tuple.Create("", 3331), Tuple.Create<System.Object, System.Int32>(Model.Compose("divResults")
            
            #line default
            #line hidden
, 3331), false)
);

WriteLiteral(" class=\"sf-search-results-container\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 66 "..\..\Chart\Views\ChartRequestView.cshtml"
           Write(JavascriptMessage.searchForResults.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </div>\r\n    </div>\r\n");

            
            #line 70 "..\..\Chart\Views\ChartRequestView.cshtml"
                    }

            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591

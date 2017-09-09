using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SamWeb.Code.Extensions
{
    public static class HtmlHelpers
    {
        public static IHtmlString Frame(this HtmlHelper helper, string title, string content)
        {
            string html = "";

            html += "<div class=\"frame\">";
            #region title:
            html += "<h2>" + 
                        helper.Encode(title) +
                    "</h2>";
            #endregion
            #region content:
            html += "<div>" +
                        helper.Encode(content) +
                    "</div>";
            #endregion
            html += "</div>";

            return new MvcHtmlString(html);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SamWeb.Code.Extensions
{
    public static class HtmlHelpers
    {
        public static IHtmlString Frame(this HtmlHelper helper, string title, string content, bool allowHtml = false)
        {
            string html = "";

            html += "<div class=\"frame\">";
            #region title:
            html += "<h2>" + 
                        (allowHtml ? title : helper.Encode(title)) +
                    "</h2>";
            #endregion
            #region content:
            html += "<div>" +
                        (allowHtml ? content : helper.Encode(content)) +
                    "</div>";
            #endregion
            html += "</div>";

            return new MvcHtmlString(html);
        }
    }
}
using System.Collections.Generic;
using System.Web.Mvc;

namespace ExecViewHrk.WebUI.Helpers
{
    public static class StMetaHtmlExtensions
    {
        public static string StLabel(this HtmlHelper html, string fieldKey, string fallback)
        {
            var dict = html.ViewBag.StFieldLabels as IDictionary<string, string>;
            if (dict != null)
            {
                string label;
                if (dict.TryGetValue(fieldKey, out label) && !string.IsNullOrWhiteSpace(label))
                    return label;
            }
            return fallback ?? fieldKey;
        }

        public static bool StVisible(this HtmlHelper html, string fieldKey)
        {
            var dict = html.ViewBag.StFieldVisible as IDictionary<string, bool>;
            if (dict != null)
            {
                bool visible;
                if (dict.TryGetValue(fieldKey, out visible))
                    return visible;
            }
            return true;
        }

        public static bool StRequired(this HtmlHelper html, string fieldKey, bool fallback = false)
        {
            var dict = html.ViewBag.StFieldRequired as IDictionary<string, bool>;
            if (dict != null)
            {
                bool required;
                if (dict.TryGetValue(fieldKey, out required))
                    return required;
            }
            return fallback;
        }

        public static MvcHtmlString StFieldLabel(this HtmlHelper html, string fieldKey, string fallback, bool defaultRequired = false)
        {
            string label = html.StLabel(fieldKey, fallback);
            bool required = html.StRequired(fieldKey, defaultRequired);
            if (required)
                return MvcHtmlString.Create("<span style=\"color:red\">*</span>" + System.Web.HttpUtility.HtmlEncode(label));
            return MvcHtmlString.Create(System.Web.HttpUtility.HtmlEncode(label));
        }
    }
}

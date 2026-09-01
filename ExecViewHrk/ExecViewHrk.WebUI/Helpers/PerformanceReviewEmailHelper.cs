using System;
using System.Configuration;
using System.Web;

namespace ExecViewHrk.WebUI.Helpers
{
    /// <summary>HTML email templates for performance review workflow notifications.</summary>
    public static class PerformanceReviewEmailHelper
    {
        public static string GetCompanyName()
        {
            return ConfigurationManager.AppSettings["CompanyName"]
                ?? ConfigurationManager.AppSettings["ApplicationName"]
                ?? "HRnest";
        }

        public static string FormatApproverRoleLabel(string reviewerRole)
        {
            if (string.IsNullOrWhiteSpace(reviewerRole)) return "Approver";
            var role = reviewerRole.Trim();
            if (string.Equals(role, "Approver1", StringComparison.OrdinalIgnoreCase)) return "Approver 1";
            if (string.Equals(role, "Approver2", StringComparison.OrdinalIgnoreCase)) return "Approver 2";
            if (string.Equals(role, "Approver3", StringComparison.OrdinalIgnoreCase)) return "Approver 3";
            if (string.Equals(role, "HR", StringComparison.OrdinalIgnoreCase)) return "HR";
            if (string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase)) return "Employee";
            return role;
        }

        /// <summary>
        /// Assignment email when a review step is ready (Start Review or after prior approver submits).
        /// </summary>
        public static string BuildAssignmentEmail(
            string recipientName,
            string employeeName,
            string approverRoleLabel,
            string approverName,
            string reviewName,
            string loginUrl)
        {
            string company = GetCompanyName();
            string safeRecipient = HttpUtility.HtmlEncode(recipientName ?? "Colleague");
            string safeEmployee = HttpUtility.HtmlEncode(employeeName ?? "");
            string safeApprover = HttpUtility.HtmlEncode(approverName ?? "");
            string safeRole = HttpUtility.HtmlEncode(approverRoleLabel ?? "Approver");
            string safeReview = HttpUtility.HtmlEncode(reviewName ?? "");
            string safeLogin = HttpUtility.HtmlAttributeEncode(loginUrl ?? "#");
            string safeCompany = HttpUtility.HtmlEncode(company);

            var sb = new System.Text.StringBuilder();
            sb.Append("<html><body style='margin:0;padding:24px;background:#f0f0f0;font-family:Arial,Helvetica,sans-serif;font-size:14px;color:#333;'>");
            sb.Append("<table role='presentation' width='100%' cellpadding='0' cellspacing='0'><tr><td align='center'>");
            sb.Append("<table role='presentation' width='600' cellpadding='0' cellspacing='0' style='max-width:600px;background:#fff;border:1px solid #e0e0e0;border-radius:4px;padding:32px 40px;'>");
            sb.Append("<tr><td style='padding-bottom:16px;'>Dear ").Append(safeRecipient).Append(",</td></tr>");
            sb.Append("<tr><td style='padding-bottom:20px;line-height:1.5;'>");
            sb.Append("A new performance review has been launched. Please complete the review process.");
            sb.Append("</td></tr>");
            sb.Append("<tr><td style='padding-bottom:8px;'><strong>Employee:</strong> ").Append(safeEmployee).Append("</td></tr>");
            sb.Append("<tr><td style='padding-bottom:8px;'><strong>").Append(safeRole).Append(":</strong> ").Append(safeApprover).Append("</td></tr>");
            sb.Append("<tr><td style='padding-bottom:24px;'><strong>Review:</strong> ").Append(safeReview).Append("</td></tr>");
            sb.Append("<tr><td style='padding-bottom:24px;line-height:1.6;'>");
            sb.Append("Please ");
            sb.Append("<a href='").Append(safeLogin).Append("' style='display:inline-block;background:#337ab7;color:#fff!important;text-decoration:none;padding:8px 20px;border-radius:3px;font-weight:bold;'>Login</a>");
            sb.Append(" to complete the performance review.");
            sb.Append("</td></tr>");
            sb.Append("<tr><td style='padding-top:8px;'>With regards,<br/>").Append(safeCompany).Append(".</td></tr>");
            sb.Append("</table></td></tr></table></body></html>");
            return sb.ToString();
        }

        public static string AssignmentSubject(string reviewName)
        {
            return "Performance Review Assignment - Action Required"
                + (string.IsNullOrWhiteSpace(reviewName) ? "" : " (" + reviewName.Trim() + ")");
        }
    }
}

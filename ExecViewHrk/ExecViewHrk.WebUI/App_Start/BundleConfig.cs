using System.Web;
using System.Web.Optimization;

namespace ExecViewHrk.WebUI.App_Start
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/Common").Include(
                      "~/Scripts/toastr.min.js",
                      "~/Scripts/moment-with-locales.min.js",
                      "~/Scripts/moment.min.js",
                      "~/Scripts/application/EvHelpers.js",
                      "~/Scripts/application/Common.js",
                      "~/Scripts/application/MessagesBox.js",
                      "~/Scripts/application/FormServices.js",
                      "~/Scripts/application/Person.js",
                      "~/Scripts/application/Manager.js",
                      "~/Scripts/application/PersonAdaMatrixPartial.js",
                      "~/Scripts/application/PersonMembershipsMatrixPartial.js",
                      "~/Scripts/application/PersonExaminationsMatrixPartial.js",
                      "~/Scripts/application/PersonInnoculationsMatrixPartial.js",
                      "~/Scripts/application/PersonLicensesMatrixPartial.js",
                      "~/Scripts/application/PersonPassportsMatrixPartial.js",
                      "~/Scripts/application/PersonPropertiesMatrixPartial.js",
                      "~/Scripts/application/PersonVehiclesMatrixPartial.js",
                      "~/Scripts/application/PersonWorkPermitsMatrixPartial.js",
                      "~/Scripts/application/PersonAddressesMatrixPartial.js",
                      "~/Scripts/application/PersonRelationshipsMatrixPartial.js",
                      "~/Scripts/application/PersonEmployeesMatrixPartial.js",
                      "~/Scripts/application/PersonPhoneNumbersMatrixPartial.js",
                      "~/Scripts/application/PersonSkillsMatrixPartial.js",
                      "~/Scripts/application/PersonTestsMatrixPartial.js",
                      "~/Scripts/application/EmployeeI9DocumentsMatrixPartial.js",
                      "~/Scripts/application/PersonTrainingsMatrixPartial.js",
                      "~/Scripts/application/PersonEducationsMatrixPartial.js",
                      "~/Scripts/application/E_PositionsMatrixPartial.js",
                      "~/Scripts/application/PerformanceReviewMatrixPartial.js",
                      "~/Scripts/application/PersonEmergencyContactsMatrixPartial.js",
                      "~/Scripts/application/PositionsMatrixPartial.js",
                      "~/Scripts/application/JobSetup.js",
                      "~/Scripts/application/BusinessLevels.js",
                      "~/Scripts/application/PositionBudgets.js",
                      "~/Scripts/application/FedralEIN.js",
                      "~/Scripts/application/PositionBudgetSchedules.js",
                      "~/Scripts/application/TimeCardsMatrixPartial.js",
                      "~/Scripts/application/PayPeriodsMatrixPartial.js",
                      "~/Scripts/application/Departments.js",
                      "~/Scripts/application/ClientConfiguration.js",
                      "~/Scripts/application/HoursEditMatrix.js",
                      "~/Scripts/application/EarningsCodes.js",
                      "~/Scripts/application/ManagerDepartment.js",
                      "~/Scripts/application/TimeCardApprovalReportMatrixPartial.js",
                      "~/Scripts/application/TimeCardSessionConfig.js",
                      "~/Scripts/application/EmployeeStatus.js",
                      "~/Scripts/application/EditManagerRecord.js",
                      "~/Scripts/application/TimeCardSummaryReportMatrixPartial.js",
                      "~/Scripts/application/TimeCardUnApprovedReport.js",
                      "~/Scripts/application/DesignateSupervisor.js"));
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/toastr.min.css"));

            bundles.Add(new StyleBundle("~/Content/kendo/2016.1.226/bundles").Include(
                        "~/Content/kendo/2016.1.226/kendo.common-fiori.min.css",
                        "~/Content/kendo/2016.1.226/kendo.fiori.min.css"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                        "~/Scripts/kendo/2016.1.226/jquery.min.js",
                        "~/Scripts/kendo/2016.1.226/kendo.all.min.js",
                        "~/Scripts/kendo/2016.1.226/kendo.aspnetmvc.min.js",
                        "~/Scripts/kendo/2016.1.226/jszip.min.js",
                        "~/Scripts/kendo.modernizr.custom.js"));

#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif

        }
    }
}

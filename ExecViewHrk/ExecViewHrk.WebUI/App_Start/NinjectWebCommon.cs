[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(ExecViewHrk.WebUI.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(ExecViewHrk.WebUI.App_Start.NinjectWebCommon), "Stop")]

namespace ExecViewHrk.WebUI.App_Start
{
    using ExecViewHrk.Domain.Interface;
    using ExecViewHrk.Domain.Repositories;
    using ExecViewHrk.WebUI.Implementation;
    using ExecViewHrk.WebUI.Interface;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using System;
    using System.Configuration;
    using System.Web;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<ITestRepository>().To<TestRepository>();
            kernel.Bind<ISalaryComponent>().To<SalaryComponenRepository>();
            kernel.Bind<IPersonLicenseRepository>().To<PersonLicenseRepository>();
            kernel.Bind<IPersonPassportRepository>().To<PersonPassportRepository>();
            kernel.Bind<IPositionBudgetSchedulesRepository>().To<PositionBudgetSchedulesRepository>();
            kernel.Bind<IPersonRepository>().To<PersonRepository>();
            kernel.Bind<IReportRepository>().To<ReportRepository>();
            kernel.Bind<ILookupTablesRepository>().To<LookupTablesRepository>();
            kernel.Bind<IPersonEmployeeRepository>().To<PersonEmployeeRepository>();
            kernel.Bind<IPersonPhoneNumbersRepository>().To<PersonPhoneNumbersRepository>();
            kernel.Bind<IPerformanceRepository>().To<PerformanceRepository>();
            kernel.Bind<IPersonAddress>().To<PersonAddressRepository>();
            kernel.Bind<IPayPeriodRepository>().To<PayPeriodRepository>();
            kernel.Bind<ITimeCardConfigurationsRepository>().To<TimeCardConfigurationsRepository>();
            kernel.Bind<IPersonVehicleRepository>().To<PersonVehicleRepository>();
            kernel.Bind<IClientConfigurationRepository>().To<ClientConfigurationRepository>();
            kernel.Bind<IEPositionRepository>().To<EPositionRepository>();
            kernel.Bind<ITimeCardsRepository>().To<TimeCardsRepository>();
            kernel.Bind<IDepartmetsRepository>().To<DepartmetsRepository>();
            kernel.Bind<IEarningsCodeRepository>().To<EarningsCodeRepository>();
            kernel.Bind<IPositionImportRepository>().To<PositionImportRepository>();
            kernel.Bind<IEmployeeForecast>().To<EmployeeForecastRepository>();
            kernel.Bind<ITimeCardMatrixReposotory>().To<TimeCardMatrixReposotory> ();
            kernel.Bind<IManagerDepartmentRepository>().To<ManagerDepartmentRepository>();
            kernel.Bind<ITimeOffEmployeesAllowedTakenRepository>().To<TimeOffEmployeesAllowedTakenRepository>();
            kernel.Bind<ITimeOffRequestsRepository>().To<TimeOffRequestsRepository>();
            kernel.Bind<ITimeCardArchiveRerpository>().To<TimeCardArchiveRerpository>();
            kernel.Bind<ITimeCardApprovalReportRepository>().To<TimeCardApprovalReportRepository>();
            kernel.Bind<ITimeCardSessionInOutRepository>().To<TimeCardSessionInOutRepository>();
            kernel.Bind<ITimeCardAuditsRepository>().To<TimeCardAuditsRepository>();
            kernel.Bind<ITimeReportRepository>().To<TimeReportRepository>();
            kernel.Bind<ITimeCardUnApprovedReportRepository>().To<TimeCardUnApprovedReportRepository>();
            kernel.Bind<IFinancialRepository>().To<FinancialRepository>();
            kernel.Bind<IHrkAdminRepository>().To<HrkAdminRepository>();


            #region
            kernel.Bind<IHoursCodesRepository>().To<HoursCodesRepository>();
            kernel.Bind<IEmployeeRetroHours>().To<EmployeeRetroHoursRepository>();
            #endregion

            // Added for Web API 
            kernel.Bind<IHttpClientWrapper>().To<HttpClientWrapper>();
            kernel.Bind<IServiceLocator>().To<ServiceLocator>().InRequestScope().WithConstructorArgument("baseUrl", ConfigurationManager.AppSettings["ApiBaseUrl"]);

            // Created to accept punch details for mobile application
            kernel.Bind<ITimeCardsMobileRepository>().To<TimeCardsMobileRepository>();
            // #1484 Designated Supervisor
            kernel.Bind<IDesignatedSupervisorRepository>().To<DesignatedSupervisorRepository>();

            System.Web.Mvc.DependencyResolver.SetResolver(new  ExecViewHrk.WebUI.Infrastructure.NinjectDependencyResolver(kernel));

            
        }        
    }
}

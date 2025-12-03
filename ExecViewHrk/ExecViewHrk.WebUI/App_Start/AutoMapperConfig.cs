using AutoMapper;
using ExecViewHrk.Domain.Models;
using ExecViewHrk.Models;
using ExecViewHrk.WebUI.Models;
using System;

namespace ExecViewHrk.WebUI.App_Start
{
    public class AutoMapperConfig
    {
        private AutoMapperConfig()
        { }

        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg =>
            {
                // Timecard Unapproval VM <-> DM Conversion
                cfg.CreateMap<TimeCardUnApprovedReportVM, TimeCardUnApprovedReportDM>();
                cfg.CreateMap<TimeCardUnApprovedReportDM, TimeCardUnApprovedReportVM>()
                .ForMember(dest => dest.UnApprovedHours, opts => opts.MapFrom(src => src.UnApprovedHours.HasValue ? Math.Round(src.UnApprovedHours.Value, 2) : 0))
                .ForMember(dest => dest.ApprovedHours, opts => opts.MapFrom(src => src.ApprovedHours.HasValue ? Math.Round(src.ApprovedHours.Value, 2) : 0))
                .ForMember(dest => dest.DepartmentTotalHours, opts => opts.MapFrom(src => src.DepartmentTotalHours.HasValue ? Math.Round(src.DepartmentTotalHours.Value, 2) : 0))
                .ForMember(dest => dest.PayPeriodHours, opts => opts.MapFrom(src => src.PayPeriodHours.HasValue ? Math.Round(src.PayPeriodHours.Value, 2) : 0))
                ;

                // Designated Supervisor Override VM <-> DM Conversion
                cfg.CreateMap<AddDesignatedSupervisorVM, AddDesignatedSupervisorDM>();
                cfg.CreateMap<AddDesignatedSupervisorDM, AddDesignatedSupervisorVM>();


            });
        }
    }
}
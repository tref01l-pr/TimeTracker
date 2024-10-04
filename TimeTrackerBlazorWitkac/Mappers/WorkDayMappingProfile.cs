using AutoMapper;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Mappers.DataAccessMappingProfiles;

public class WorkDayMappingProfile : Profile
{
    public WorkDayMappingProfile()
    {
        CreateMap<AbsenceEntity, WorkDayResponse>()
            .ForMember(
                dst => dst.WorkDayType,
                opt => opt.MapFrom(_ => WorkDayType.Absence))
            .ForMember(dst => dst.StartDate, opt => 
                opt.MapFrom(src => new DateTime(
                    src.StartDate.Year, 
                    src.StartDate.Month, 
                    src.StartDate.Day, 
                    src.StartHour, 
                    src.StartMinute, 
                    0)))
            .ForMember(dst => dst.EndDate, opt => 
                opt.MapFrom(src => new DateTime(
                    src.EndDate.Year, 
                    src.EndDate.Month, 
                    src.EndDate.Day, 
                    src.EndHour, 
                    src.EndMinute, 
                    0)))
            .ForMember(dst => dst.User, opt => 
                opt.MapFrom(src => src.User))
            .ReverseMap();
        
        CreateMap<AttendanceEntity, WorkDayResponse>()
            .ForMember(
                dst => dst.WorkDayType,
                opt => opt.MapFrom(_ => WorkDayType.Attendance))
            .ForMember(dst => dst.StartDate, opt => 
                opt.MapFrom(src => new DateTime(
                    src.StartDate.Year, 
                    src.StartDate.Month, 
                    src.StartDate.Day, 
                    src.StartHour, 
                    src.StartMinute, 
                    0)))
            .ForMember(dst => dst.EndDate, opt => 
                opt.MapFrom(src => 
                    src.EndDate.HasValue
                        ? new DateTime(
                            src.EndDate.Value.Year, 
                            src.EndDate.Value.Month, 
                            src.EndDate.Value.Day, 
                            src.EndHour, 
                            src.EndMinute,
                            0)
                        : (DateTime?)null))
            .ReverseMap();
        
        CreateMap<HolidayEntity, WorkDayResponse>()
        .ForMember(
            dst => dst.WorkDayType,
            opt => opt.MapFrom(_ => WorkDayType.Holiday))
        .ForMember(dst => dst.StartDate, opt =>
            opt.MapFrom(src => new DateTime(
                src.StartDate.Year,
                src.StartDate.Month,
                src.StartDate.Day,
                0,
                0,
                0)))
        .ForMember(dst => dst.EndDate, opt =>
            opt.MapFrom(src => new DateTime(
                src.EndDate.Year,
                src.EndDate.Month,
                src.EndDate.Day,
                0,
                0,
                0)))
        .ReverseMap();
        CreateMap<WorkDayResponse, Absence>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
           .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
           .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
           .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
           .ForMember(dest => dest.StatusOfType, opt => opt.MapFrom(src => src.StatusOfType))
           .ForMember(dest => dest.StatusOfDates, opt => opt.MapFrom(src => src.StatusOfDates))
           .ForMember(dest => dest.IsFullyConfirmed, opt => opt.MapFrom(src => src.IsFullyConfirmed))
           .ReverseMap();
     
    }
}
using AutoMapper;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Mappers.DataAccessMappingProfiles;

public class AbsenceMappingProfile : Profile
{
    public AbsenceMappingProfile()
    {
        CreateMap<AbsenceEntity, Absence>().ReverseMap();
        CreateMap<AbsenceEntity, AbsenceResponse>()
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
            .ForMember(
                dst => dst.AbsenceType,
                opt => opt.MapFrom(src => src.AbsenceType))
            .ForMember(
                dst => dst.User,
                opt => opt.MapFrom(src => src.User))
            .ForMember(
            dst => dst.StatusOfDates,
            opt => opt.MapFrom(src => src.StatusOfDates))
            .ForMember(
            dst => dst.StatusOfType,
            opt => opt.MapFrom(src => src.StatusOfType))
            .ForMember(
            dst => dst.ConfirmationStatus,
            opt => opt.MapFrom(src => src.IsFullyConfirmed))
            .ReverseMap();

    }
}
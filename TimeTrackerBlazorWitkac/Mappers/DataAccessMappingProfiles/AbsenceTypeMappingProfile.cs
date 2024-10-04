using AutoMapper;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Mappers.DataAccessMappingProfiles;

public class AbsenceTypeMappingProfile : Profile
{
    public AbsenceTypeMappingProfile()
    {
        CreateMap<AbsenceTypeEntity, AbsenceType>().ReverseMap();
        CreateMap<AbsenceTypeEntity, AbsenceTypeResponse>().ReverseMap();
        CreateMap<AbsenceType, AbsenceTypeResponse>().ReverseMap();
    }
}
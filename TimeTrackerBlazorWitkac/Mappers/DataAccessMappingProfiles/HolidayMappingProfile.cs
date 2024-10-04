using AutoMapper;
using TimeTrackerBlazorWitkac.Data;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Mappers.DataAccessMappingProfiles;

public class HolidayMappingProfile : Profile
{
    public HolidayMappingProfile()
    {
        CreateMap<HolidayEntity, Holiday>().ReverseMap();
    }
}
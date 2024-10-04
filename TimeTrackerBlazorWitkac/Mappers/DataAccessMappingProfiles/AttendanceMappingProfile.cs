using AutoMapper;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Mappers.DataAccessMappingProfiles;

public class AttendanceMappingProfile : Profile
{
    public AttendanceMappingProfile()
    {
        CreateMap<Attendance, AttendanceResponse>()
           .ReverseMap();

        CreateMap<AttendanceEntity, Attendance>()
            .ReverseMap();

        CreateMap<AttendanceEntity, AttendanceResponse>()
            .ReverseMap();
    }
}
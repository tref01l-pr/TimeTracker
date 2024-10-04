using AutoMapper;
using TimeTrackerBlazorWitkac.Data.Entities;

namespace TimeTrackerBlazorWitkac.Mappers.DataAccessMappingProfiles;

public class RoleMappingProfiles : Profile
{
    public RoleMappingProfiles()
    {
        CreateMap<RoleEntity, string>()
            .ConvertUsing(r => r.Name!);
    }
}
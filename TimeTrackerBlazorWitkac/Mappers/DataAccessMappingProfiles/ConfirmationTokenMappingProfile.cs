using AutoMapper;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Mappers.DataAccessMappingProfiles;

public class ConfirmationTokenMappingProfile : Profile
{
    public ConfirmationTokenMappingProfile()
    {
        CreateMap<ConfirmationTokenEntity, ConfirmationToken>().ReverseMap();
        CreateMap<ConfirmationTokenEntity, ConfirmationTokenResponse>().ReverseMap();
    }
}
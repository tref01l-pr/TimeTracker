using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Dto.UserDto;

namespace TimeTrackerBlazorWitkac.Mappers.DataAccessMappingProfiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserEntity, UserEntity>().ReverseMap();
        CreateMap<UserEntity, User>().ReverseMap();

        CreateMap<UserEntity, UserDtoUserCards>()
            .ForMember(
                dst => dst.UserCards,
                opt => opt.MapFrom(src => src.UserCards))
            .ReverseMap();

        CreateMap<UserDtoUserCards, UserDetailsResponse>()
            .ForMember(
                dst => dst.UserCards,
                opt => opt.MapFrom(stc => stc.UserCards.Where(uc => string.IsNullOrEmpty(uc.UserDeletedId))))
            .ReverseMap();

        CreateMap<UserEntity, UserDetailsResponse>()
            .ForMember(
                dst => dst.UserCards,
                opt => opt.MapFrom(stc => stc.UserCards.Where(uc => string.IsNullOrEmpty(uc.UserDeletedId))))
            .ReverseMap();
        
        CreateMap<UserEntity, UserResponse>()
            .ForMember(dst => dst.Id,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dst => dst.Roles, 
                opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name)))
            .ReverseMap();
    }
}
using AutoMapper;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;
using TimeTrackerBlazorWitkac.Dto.UserCardDto;

namespace TimeTrackerBlazorWitkac.Mappers.DataAccessMappingProfiles;

public class UserCardMappingProfile : Profile
{
    public UserCardMappingProfile()
    {
        CreateMap<UserCardEntity, UserCard>().ReverseMap();
        CreateMap<UserCardResponse, UserCard>().ReverseMap();

        /*CreateMap<UserCardEntity, UserCardDtoAllInfo>()
            .ForMember(
                dst => dst.User,
                opt => opt.MapFrom((src => src.User)))
            .ReverseMap();*/

        CreateMap<UserCardEntity, UserCardDtoAllInfo>()
            .ForMember(
                dst => dst.User,
                opt => opt.MapFrom(src => src.User))
            .ForMember(
                dst => dst.Company,
                opt => opt.MapFrom(src => src.Company))
            .ForMember(
                dst => dst.DeletedUser,
                opt => opt.MapFrom(src => src.UserDeleted))
            .ReverseMap();

        CreateMap<UserCardEntity, UserCardDtoCompany>()
            .ForMember(
                dst => dst.Company,
                opt => opt.MapFrom(src => src.Company))
            .ReverseMap();

        CreateMap<UserCardDtoCompany, UserCardResponse>()
            .ForMember(
                dst => dst.Company,
                opt => opt.MapFrom(src => src.Company))
            .ReverseMap();
        
        CreateMap<UserCardEntity, UserCardResponse>()
            .ForMember(
                dst => dst.Company,
                opt => opt.MapFrom(src => src.Company))
            .ReverseMap();

    }
}
using AutoMapper;
using TimeTrackerBlazorWitkac.Contracts.Responses;
using TimeTrackerBlazorWitkac.Data.Entities;
using TimeTrackerBlazorWitkac.Data.Models;

namespace TimeTrackerBlazorWitkac.Mappers.DataAccessMappingProfiles;

public class CompanyMappingProfile : Profile
{
    public CompanyMappingProfile()
    {
        CreateMap<CompanyEntity, Company>().ReverseMap();
        CreateMap<Company, CompanyResponse>().ReverseMap();
        CreateMap<CompanyEntity, CompanyResponse>().ReverseMap();
    }
}
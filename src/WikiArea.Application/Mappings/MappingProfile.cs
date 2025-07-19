using AutoMapper;
using WikiArea.Application.DTOs;
using WikiArea.Core.Entities;

namespace WikiArea.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<WikiPage, WikiPageDto>()
            .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name));

        CreateMap<WikiPage, WikiPageSummaryDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name));

        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name));

        CreateMap<User, UserSummaryDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name));

        CreateMap<WikiFolder, WikiFolderDto>();
        CreateMap<WikiFolder, WikiFolderTreeDto>();

        CreateMap<Comment, CommentDto>();
    }
} 
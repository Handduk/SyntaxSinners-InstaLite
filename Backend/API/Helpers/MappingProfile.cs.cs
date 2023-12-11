using AutoMapper;
using Api.Dtos;
using Entity;

namespace Api.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<Post, PostDto>();
            CreateMap<PostDto, Post>();
            CreateMap<PostWithImageDto, Post>();
            CreateMap<Post, UserDto>();
            CreateMap<UpdateUserModel, User>();
            CreateMap<User, UpdateUserModel>();
            CreateMap<Comment, CommentDto>();
            CreateMap<CommentDto, Comment>();
        }
    }
}

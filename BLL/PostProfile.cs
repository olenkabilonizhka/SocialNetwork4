using AutoMapper;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Post, PostDynamo>()
                .ForMember(x => x.PostId, s => s.MapFrom(x => x.Id))
                .ForMember(x => x.UserId, s => s.MapFrom(x => x.UserIdPost))
                .ForMember(x => x.Title, s => s.MapFrom(x => x.Title))
                .ForMember(x => x.Body, s => s.MapFrom(x => x.Body))
                .ForMember(x => x.CreatedTime, s => s.MapFrom(x => x.CreatedTime))
                .ForMember(x => x.ModifiedTime, s => s.MapFrom(x => x.CreatedTime));
            CreateMap<PostRedis, Post>()
                .ForMember(x => x.Id, s => s.MapFrom(x => x.PostId))
                .ForMember(x => x.UserIdPost, s => s.MapFrom(x => x.UserId))
                .ForMember(x => x.Title, s => s.MapFrom(x => x.Title))
                .ForMember(x => x.Body, s => s.MapFrom(x => x.Body))
                .ForMember(x => x.LikesPost, s => s.MapFrom(x => x.LikesPost))
                .ForMember(x => x.CreatedTime, s => s.MapFrom(x => x.CreatedTime)).ReverseMap();
        }
    }
}

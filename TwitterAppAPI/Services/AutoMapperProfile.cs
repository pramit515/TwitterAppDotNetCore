using AutoMapper;
using TwitterAppAPI.Dtos;
using TwitterAppAPI.Models;

namespace TwitterAppAPI.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserRegister, UserDto>();
            CreateMap<UserDto, UserRegister>();
        

            CreateMap<UserTweet, TweetDto>();
            CreateMap<TweetDto, UserTweet>();


            CreateMap<GetTweetDto, UserTweet>();
            CreateMap<UserTweet, GetTweetDto>();
        }
    }
}

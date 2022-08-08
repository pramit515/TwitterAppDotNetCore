using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TwitterAppAPI.Data;
using TwitterAppAPI.Dtos;
using TwitterAppAPI.Models;

namespace TwitterAppAPI.Services
{
    public class UserTweetService : IUserTweets
    {
        private readonly DataContext _context;
        private readonly IMapper mapper;

        public UserTweetService(DataContext context, IMapper mapper)
        {            
            _context = context;
            this.mapper = mapper;

        }
        public async Task<ServiceResponse<int>> AddTweet(TweetDto newusertweet,string userName)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();
            if (string.IsNullOrEmpty(userName))
            {
                response.Success = false;
                response.Message = "User name is required.";
            }
            else
            {
                var user = _context.UserRegister.FirstOrDefault(x => x.LoginId.ToLower() == userName.ToLower());

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not exists.";
                }
                else
                {
                    UserTweet tweet = this.mapper.Map<UserTweet>(newusertweet);
                    tweet.User = user;
                    tweet.Parent = null;

                    _context.UserTweet.Add(tweet);
                    await _context.SaveChangesAsync();

                    response.Data = tweet.Id;
                    response.Message = "Tweet added successfully.";
                }
            }

            return response;
        }

        public async Task<ServiceResponse<int>> DeleteTweet(int tweetid, string userName)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();
            if (string.IsNullOrEmpty(userName))
            {
                response.Success = false;
                response.Message = "User name is required.";
            }
            else
            {
                var user = _context.UserRegister.FirstOrDefault(x => x.LoginId.ToLower() == userName.ToLower());

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not exists.";
                }
                else
                {
                    UserTweet oldTweet = _context.UserTweet.FirstOrDefault(x => x.Id == tweetid && x.User.Id == user.Id);
                    if (oldTweet == null)
                    {
                        response.Success = false;
                        response.Message = "Tweet not exists.";
                    }
                    else
                    {
                        await DeleteChild(tweetid);

                        response.Message = "Tweet deleted successfully.";
                    }
                }
            }

            return response;
        }

        public async Task<ServiceResponse<List<GetTweetDto>>> GetAllUserTweets()
        {
            var response = new ServiceResponse<List<GetTweetDto>>();
            var dbtweet = await _context.UserTweet.ToListAsync();
            response.Data = mapper.Map<List<GetTweetDto>>(dbtweet);
            return response;
        }

        public async Task<ServiceResponse<GetTweetDto>> GetTweetById(int id)
        {
            var response = new ServiceResponse<GetTweetDto>();
            var dbUserTweeet = await _context.UserTweet.FirstOrDefaultAsync(c => c.Id == id);
            response.Data = mapper.Map<GetTweetDto>(dbUserTweeet);
            return response;
        }

        public async Task<ServiceResponse<int>> UpdateTweet(int tweetid, TweetDto tweetDto, string userName)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();
            if (string.IsNullOrEmpty(userName))
            {
                response.Success = false;
                response.Message = "User name is required.";
            }
            else
            {
                var user = _context.UserRegister.FirstOrDefault(x => x.LoginId.ToLower() == userName.ToLower());

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not exists.";
                }
                else
                {
                    UserTweet oldTweet = _context.UserTweet.FirstOrDefault(x => x.Id == tweetid && x.User.Id == user.Id);
                    if (oldTweet == null)
                    {
                        response.Success = false;
                        response.Message = "Tweet not exists.";
                    }
                    else
                    {
                        oldTweet.Message = tweetDto.Message;
                        oldTweet.Tag = tweetDto.Tag;

                        await _context.SaveChangesAsync();

                        response.Data = oldTweet.Id;
                        response.Message = "Tweet updated successfully.";
                    }
                }
            }

            return response;
        }

        public async Task<ServiceResponse<int>> ReplyTweet(TweetDto tweetDto, string userName, int tweetId)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();
            if (string.IsNullOrEmpty(userName))
            {
                response.Success = false;
                response.Message = "User name is required.";
            }
            else
            {
                var user = _context.UserRegister.FirstOrDefault(x => x.LoginId.ToLower() == userName.ToLower());

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not exists.";
                }
                else
                {
                    UserTweet tweetReply = _context.UserTweet.FirstOrDefault(x => x.Id == tweetId);
                    UserTweet tweet = this.mapper.Map<UserTweet>(tweetDto);
                    if (tweetReply != null)
                    {
                        tweet.User = user;
                        tweet.Parent = tweetReply;

                        _context.UserTweet.Add(tweet);
                        await _context.SaveChangesAsync();

                        response.Data = tweet.Id;
                        response.Message = "Tweet replyed successfully.";
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Tweet not exists.";
                    }
                }
            }

            return response;
        }

        public async Task<ServiceResponse<List<GetTweetDto>>> GetAllTweetsByUserId(int userid)
        {
            var response = new ServiceResponse<List<GetTweetDto>>();
            var dbUserTweeet = await _context.UserTweet.Where(c => c.User.Id == userid).ToListAsync();
            response.Data = mapper.Map<List<GetTweetDto>>(dbUserTweeet);
            return response;
        }

        public async Task<ServiceResponse<List<GetTweetDto>>> GetAllTweetsByUsername(string username)
        {
            var response = new ServiceResponse<List<GetTweetDto>>();
            var dbUserTweeet = await _context.UserTweet.Where(c => c.User.LoginId == username).ToListAsync();
            response.Data = mapper.Map<List<GetTweetDto>>(dbUserTweeet);
            return response;
        }

        private async Task DeleteChild(int tweetId)
        {
            var tweets = _context.UserTweet.Where(x => x.Parent != null && x.Parent.Id == tweetId).ToList();
            if (tweets.Any() && tweets.Count > 0)
            {
                foreach (var item in tweets)
                {
                    await DeleteChild(item.Id);
                }
            }

            var tweet = _context.UserTweet.FirstOrDefault(x => x.Id == tweetId);
            _context.UserTweet.Remove(tweet);
            await _context.SaveChangesAsync();
        }
    }
}

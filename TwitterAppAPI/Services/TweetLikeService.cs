using AutoMapper;
using TwitterAppAPI.Data;
using TwitterAppAPI.Models;

namespace TwitterAppAPI.Services
{
    public class TweetLikeService : ITweetLikeService
    {
        private readonly DataContext dataContext;
        private readonly IMapper mapper;

        public TweetLikeService(IMapper mapper, DataContext dataContext)
        {
            this.mapper = mapper;
            this.dataContext = dataContext;
        }

        public async Task<ServiceResponse<int>> TweetLike(int tweetId, string userName)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();
            if (string.IsNullOrEmpty(userName))
            {
                response.Success = false;
                response.Message = "User name is required.";
            }
            else
            {
                var user = dataContext.UserRegister.FirstOrDefault(x => x.LoginId.ToLower() == userName.ToLower());

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not exists.";
                }
                else
                {
                    UserTweet oldTweet = dataContext.UserTweet.FirstOrDefault(x => x.Id == tweetId);
                    if (oldTweet == null)
                    {
                        response.Success = false;
                        response.Message = "Tweet not exists.";
                    }
                    else
                    {
                        TweetLike tweetLike = dataContext.UserTweetLike.FirstOrDefault(x => x.Tweet.Id == tweetId && x.UserId == user.Id);
                        if (tweetLike != null)
                        {
                            dataContext.UserTweetLike.Remove(tweetLike);
                            oldTweet.Likes--;
                            await dataContext.SaveChangesAsync();
                            response.Data = tweetLike.Id;
                            response.Message = "Tweet dislike successfully.";
                        }
                        else
                        {
                            tweetLike = new TweetLike();
                            tweetLike.Tweet = oldTweet;
                            tweetLike.UserId = user.Id;
                            dataContext.UserTweetLike.Add(tweetLike);
                            oldTweet.Likes++;
                            await dataContext.SaveChangesAsync();
                            response.Data = tweetLike.Id;
                            response.Message = "Tweet like successfully.";
                        }
                    }
                }
            }

            return response;
        }
    }
}

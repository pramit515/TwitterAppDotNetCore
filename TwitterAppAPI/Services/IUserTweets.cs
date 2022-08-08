using TwitterAppAPI.Dtos;
using TwitterAppAPI.Models;

namespace TwitterAppAPI.Services
{
    public interface IUserTweets
    {
        Task<ServiceResponse<List<GetTweetDto>>> GetAllUserTweets();
        Task<ServiceResponse<GetTweetDto>> GetTweetById(int id);
        Task<ServiceResponse<List<GetTweetDto>>> GetAllTweetsByUserId(int id);
        Task<ServiceResponse<List<GetTweetDto>>> GetAllTweetsByUsername(string username);
        Task<ServiceResponse<int>> AddTweet(TweetDto newusertweet, string username);
        Task<ServiceResponse<int>> UpdateTweet(int tweetid, TweetDto tweetDto, string userName);
        Task<ServiceResponse<int>> ReplyTweet(TweetDto tweetDto, string userName, int tweetId);
        Task<ServiceResponse<int>> DeleteTweet(int tweetid, string userName);

    }
}

using TwitterAppAPI.Models;

namespace TwitterAppAPI.Services
{
    public interface ITweetLikeService
    {
        Task<ServiceResponse<int>> TweetLike(int tweetId, string userName);
    }
}

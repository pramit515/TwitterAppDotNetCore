using Microsoft.EntityFrameworkCore;
using TwitterAppAPI.Models;

namespace TwitterAppAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) :base(options)
        {

        }

        public DbSet<UserRegister> UserRegister { get; set; }

        public DbSet<UserTweet> UserTweet { get; set; }

        public DbSet<TweetLike> UserTweetLike { get; set; }

    }
}

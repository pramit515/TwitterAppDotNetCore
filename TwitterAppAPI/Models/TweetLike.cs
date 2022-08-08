namespace TwitterAppAPI.Models
{
    public class TweetLike
    {
        public int Id { get; set; }

        public UserTweet Tweet { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}

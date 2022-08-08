using System.ComponentModel.DataAnnotations;

namespace TwitterAppAPI.Models
{
    public class UserTweet
    {
        public int Id { get; set; }      

        [Required]
        [MaxLength(144)]
        public string Message { get; set; }

        [Required]
        [MaxLength(50)]
        public string Tag { get; set; }
        public int Likes { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set;} = DateTime.Now;
        public UserRegister User { get; set; }
        public List<TweetLike> TweetLike { get; set; }
        public UserTweet? Parent { get; set; }
    }
}

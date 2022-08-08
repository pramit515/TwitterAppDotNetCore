namespace TwitterAppAPI.Dtos
{
    public class GetTweetDto
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public string Tag { get; set; }

        public int Likes { get; set; }

        public GetTweetDto? Parent { get; set; }

        public UserDto User { get; set; }
    }
}

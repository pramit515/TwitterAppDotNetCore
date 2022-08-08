using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TwitterAppAPI.Data;
using TwitterAppAPI.Dtos;
using TwitterAppAPI.Models;
using TwitterAppAPI.Services;

namespace TwitterAppAPI.Controllers
{
    [Authorize]
    [Route("api/v1.0/tweets")]
    [ApiController]
    public class TweetController : ControllerBase
    {
        private readonly IUserTweets tweetservice;
        private readonly ITweetLikeService tweetlikeservice;
        public TweetController(IUserTweets tweetservice, ITweetLikeService tweetlikeservice)
        {
            this.tweetservice = tweetservice;
            this.tweetlikeservice = tweetlikeservice;
        }

        [HttpGet("all")]
        public async Task<IActionResult> Get()
        {

            return Ok(await tweetservice.GetAllUserTweets());
        }       

        [HttpGet("{username}")]
        public async Task<IActionResult> GetAllTweetsByUsername(string username)
        {

            return Ok(await tweetservice.GetAllTweetsByUsername(username));
        }

        [HttpPost("{username}/add")]
        public async Task<IActionResult> Add(string username, TweetDto objTweet)
        {

            return Ok(await tweetservice.AddTweet(objTweet, username));
        }

       

        [HttpPut("/{username}/udpate/{id}")]
        public async Task<IActionResult> Update(string username, int id, TweetDto tweetDto)
        {
            var response = await tweetservice.UpdateTweet(id, tweetDto, username);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("/{username}/delete/{id}")]
        public async Task<IActionResult> Delete(string username, int id)
        {
            var response = await tweetservice.DeleteTweet(id, username);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("/{username}/like/{id}")]
        public async Task<IActionResult> Update(string username, int id)
        {
            var response = await this.tweetlikeservice.TweetLike(id, username);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("/{username}/reply/{id}")]
        public async Task<IActionResult> Reply(string username, int id, TweetDto tweetDto)
        {
            var response = await tweetservice.ReplyTweet(tweetDto, username, id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

    }
}

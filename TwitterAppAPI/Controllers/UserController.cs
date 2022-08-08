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
    public class UserController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public UserController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto user,string password,string confirmpassword)
        {
            var response = await _authRepository.Register(user,password,confirmpassword);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLogin userlogin)
        {
            var response = await _authRepository.Login(userlogin);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }



       
        [HttpGet("users/all")]
        public async Task<IActionResult> GetAllUsers()
        {

            return Ok(await _authRepository.GetAllUsers());
        }


        [HttpGet("user/search/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {

            return Ok(await _authRepository.GetUserByName(username));
        }

        [HttpPost("/{userName}/forgot")]
        public async Task<IActionResult> Forgot(string userName, ForgotPasswordDto forgotDto)
        {
            var response = await this._authRepository.ForgotPassword(userName, forgotDto.NewPassword, forgotDto.ConfirmPassword);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

    }
}

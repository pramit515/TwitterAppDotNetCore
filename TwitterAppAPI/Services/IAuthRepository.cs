using TwitterAppAPI.Dtos;
using TwitterAppAPI.Models;

namespace TwitterAppAPI.Services
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<int>> Register(UserDto newuser, string password, string confirmpassword);
        Task<ServiceResponse<string>> Login(UserLogin userlogin);
        Task<bool> UserExists(string username);
        Task<ServiceResponse<List<UserDto>>> GetAllUsers();
        Task<ServiceResponse<UserDto>> GetUserById(int id);
        Task<ServiceResponse<UserDto>> GetUserByName(string name);
        Task<ServiceResponse<UserDto>> UpdateUser(UserDto updateuser);
        Task<ServiceResponse<int>> DeleteUser(int id);
        Task<ServiceResponse<int>> ForgotPassword(string loginId, string password, string confirmPassword);
    }
}

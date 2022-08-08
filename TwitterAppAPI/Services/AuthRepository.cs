using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TwitterAppAPI.Data;
using TwitterAppAPI.Dtos;
using TwitterAppAPI.Models;

namespace TwitterAppAPI.Services
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _Context;
        private readonly IConfiguration _Configuration;
        private readonly IMapper _mapper;

        public AuthRepository(DataContext context, IConfiguration configuration, IMapper mapper)
        {
            _Context = context;
            _Configuration = configuration;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<string>> Login(UserLogin userlogin)
        {
            var response = new ServiceResponse<string>();
            var user = await _Context.UserRegister.FirstOrDefaultAsync(u => u.LoginId.ToLower().Equals(userlogin.Username.ToLower()));
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
            }
            else if (!VerifyPasswordHash(userlogin.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password";
            }
            else
            {
                response.Data = "Bearer " + CreateToken(user);
                // response.Data = user.Id.ToString();
            }
            return response;
        }
        public async Task<ServiceResponse<int>> Register(UserDto newuser, string password, string confirmpassword)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            if (await UserExists(newuser.LoginId))
            {
                response.Success = false;
                response.Message = "User already exist";
                return response;
            }

            if (string.IsNullOrEmpty(password) && string.IsNullOrEmpty(confirmpassword) && password != confirmpassword)
            {
                response.Success = false;
                response.Message = "Password and ConfirmPassword must be same";
                return response;
            }
            else if (string.IsNullOrEmpty(newuser.LoginId) && string.IsNullOrEmpty(newuser.LoginId) && newuser.LoginId != newuser.Email)
            {
                response.Success = false;
                response.Message = "LoginId and Email could not be same";
                return response;
            }
            else if (await LoginIdExists(newuser.LoginId))
            {
                response.Success = false;
                response.Message = "Username already exist";
                return response;
            }
            else if (await EmailExists(newuser.Email))
            {
                response.Success = false;
                response.Message = "Email already exist";
                return response;
            }
            else {
                UserRegister objUser = _mapper.Map<UserRegister>(newuser);
                objUser.PasswordHash = passwordHash;
                objUser.PasswordSalt = passwordSalt;
                _Context.UserRegister.Add(objUser);
                await _Context.SaveChangesAsync();

                response.Data = objUser.Id;
                response.Message = "User registerd succesfully";
            }

            return response;
        }   

        public async Task<bool> UserExists(string username)
        {
            if (await _Context.UserRegister.AnyAsync(u => u.LoginId.ToLower() == username.ToLower()))
                return true;
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(UserRegister user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.LoginId)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_Configuration.GetSection("AppSettings:Token").Value));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<ServiceResponse<List<UserDto>>> GetAllUsers()
        {
            ServiceResponse<List<UserDto>> response = new ServiceResponse<List<UserDto>>();
            List<UserRegister> dbUsers = await _Context.UserRegister.ToListAsync();
            response.Data = dbUsers.Select(c => _mapper.Map<UserDto>(c)).ToList();
            return response;
        }

        public async Task<ServiceResponse<UserDto>> GetUserById(int id)
        {
            var response = new ServiceResponse<UserDto>();
            var dbUser = await _Context.UserRegister.FirstOrDefaultAsync(c => c.Id == id);
            response.Data = _mapper.Map<UserDto>(dbUser);
            return response;
        }

        public async Task<ServiceResponse<UserDto>> GetUserByName(string name)
        {
            var response = new ServiceResponse<UserDto>();
            var dbUser = await _Context.UserRegister.FirstOrDefaultAsync(c => c.LoginId == name);
            response.Data = _mapper.Map<UserDto>(dbUser);
            return response;
        }       

        public async Task<ServiceResponse<UserDto>> UpdateUser(UserDto updateuser)
        {
            ServiceResponse<UserDto> serviceResponse = new ServiceResponse<UserDto>();
            try
            {
                UserRegister userT = await _Context.UserRegister.FirstOrDefaultAsync(c => c.Id == updateuser.Id);
                _mapper.Map(updateuser, userT);
                await _Context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<UserDto>(userT);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<int>> DeleteUser(int id)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();
            try
            {
                UserRegister user = await _Context.UserRegister.FirstOrDefaultAsync(c => c.Id == id);
                _Context.UserRegister.Remove(user);
                await _Context.SaveChangesAsync();
                response.Message = "User has been successfully deleted";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<int>> ForgotPassword(string loginId, string password, string confirmPassword)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();
            if (string.IsNullOrEmpty(loginId))
            {
                response.Success = false;
                response.Message = "Login id is required.";
            }
            else if (string.IsNullOrEmpty(password))
            {
                response.Success = false;
                response.Message = "Password is required.";
            }
            else if (string.IsNullOrEmpty(confirmPassword))
            {
                response.Success = false;
                response.Message = "Confirm password is required.";
            }
            else if (password != confirmPassword)
            {
                response.Success = false;
                response.Message = "Password and confirm password should be same.";
            }
            else
            {
                UserRegister user = _Context.UserRegister.FirstOrDefault(x => x.LoginId == loginId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not exists.";
                }
                else
                {
                    CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;

                    await _Context.SaveChangesAsync();

                    response.Data = user.Id;
                    response.Message = "User password changed successfully.";
                }
            }
            return response;
        }


        public async Task<bool> LoginIdExists(string loginId)
        {
            if (await _Context.UserRegister.AnyAsync(x => x.LoginId.ToLower() == loginId.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> EmailExists(string email)
        {
            if (await _Context.UserRegister.AnyAsync(x => x.Email.ToLower() == email.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

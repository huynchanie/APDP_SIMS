using SIMS.Data;
using SIMS.Models;
using Microsoft.EntityFrameworkCore;
using SIMS.Repositories;

namespace SIMS.Facades
{
    public class UserFacade : IUserFacade
    {
        private readonly IUserRepository _userRepository;

        public UserFacade(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public string RegisterUser(string fullName, string email, string password, int roleId, string address, string phoneNumber)
        {
            if (_userRepository.GetUserByEmail(email) != null)
            {
                return "Email already exists!";
            }

            var newUser = new User
            {
                FullName = fullName,
                Email = email,
                Password = password, // Không mã hóa mật khẩu
                RoleId = roleId,
                Address = address,
                PhoneNumber = phoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            _userRepository.CreateUser(newUser);
            return "User registered successfully!";
        }

        public User LoginUser(string email, string password)
        {
            return _userRepository.AuthenticateUser(email, password);
        }
    }
}


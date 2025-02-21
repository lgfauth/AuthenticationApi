using Domain.Entities;
using Domain.Models;
using Domain.Models.Envelope;
using Domain.Settings;
using Microsoft.Extensions.Options;
using Repository.Interfaces;
using ServicesApplication.Interfaces;
using ServicesApplication.Utils;

namespace ServicesApplication.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly JwtSettings _jwtSettings;

        public RegisterService(IRegisterRepository registerRepository, IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
            _registerRepository = registerRepository;
        }

        public async Task<IResponse<bool>> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _registerRepository.GetUserByUsernameAndEmailAsync(request.Username, request.Email);

            if (existingUser is not null)
                throw new Exception("User already exists.");

            var passwordHash = Encryptor.HashPassword(request.Password);

            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                LastName = request.LastName,
                Email = request.Email,
                Name = request.Name
            };

            await _registerRepository.CreateUserAsync(newUser);

            return new ResponseOk<bool>(true);
        }

        public async Task<IResponse<bool>> UnregisterAsync(UnregisterRequest request)
        {
            var existingUser = await _registerRepository.GetUserByUsernameAndEmailAsync(request.Username, request.Email);

            if (existingUser is null)
                throw new Exception("User not exists.");

            var response = await _registerRepository.DeleteUserByIdAsync(existingUser.Id);

            return new ResponseOk<bool>(response);
        }
    }
}

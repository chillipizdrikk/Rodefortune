using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using RodeFortune.BLL.Dto;
using RodeFortune.BLL.Mappers;
using RodeFortune.BLL.Services.Interfaces;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace RodeFortune.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserResponseDto> GetUserByIdAsync(string id)
        {
            try
            {
                var objectId = new ObjectId(id);
                var user = await _userRepository.GetByIdAsync(objectId);
                return user?.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні користувача за ID: {Id}", id);
                return null;
            }
        }

        public async Task<UserResponseDto> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                return user?.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні користувача за email: {Email}", email);
                return null;
            }
        }

        public async Task<UserResponseDto> GetUserByUsernameAsync(string username)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                return user?.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні користувача за ім'ям: {Username}", username);
                return null;
            }
        }

        public async Task<UserResponseDto> CreateUserAsync(UserRequestDto userDto)
        {
            try
            {
                var user = userDto.ToModel();
                await _userRepository.CreateAsync(user);
                return user.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при створенні користувача: {Username}", userDto.Username);
                return null;
            }
        }

        public async Task<UserResponseDto> UpdateUserAsync(string id, UserRequestDto userDto)
        {
            try
            {
                var objectId = new ObjectId(id);
                var existingUser = await _userRepository.GetByIdAsync(objectId);
                
                if (existingUser == null)
                {
                    return null;
                }
                
                // Оновлення полів
                existingUser.Username = userDto.Username;
                existingUser.Email = userDto.Email;
                if (!string.IsNullOrEmpty(userDto.PasswordHash))
                {
                    existingUser.PasswordHash = userDto.PasswordHash;
                }
                existingUser.BirthDate = userDto.BirthDate;
                existingUser.ZodiacSign = userDto.ZodiacSign;
                existingUser.Role = userDto.Role;
                
                await _userRepository.UpdateAsync(objectId, existingUser);
                return existingUser.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні користувача: {Id}", id);
                return null;
            }
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                var objectId = new ObjectId(id);
                return await _userRepository.DeleteAsync(objectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при видаленні користувача: {Id}", id);
                return false;
            }
        }

        public async Task<bool> ValidateUserCredentialsAsync(string email, string passwordHash)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                return user != null && user.PasswordHash == passwordHash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при валідації облікових даних: {Email}", email);
                return false;
            }
        }

        public async Task SavePasswordResetTokenAsync(string userId, string token, DateTime expirationDate)
        {
            try
            {
                var objectId = new ObjectId(userId);
                var user = await _userRepository.GetByIdAsync(objectId);
                
                if (user == null)
                {
                    _logger.LogWarning("Користувача з ID {UserId} не знайдено при збереженні токена скидання пароля", userId);
                    return;
                }
                
                user.PasswordResetToken = token;
                user.PasswordResetTokenExpiration = expirationDate;
                
                await _userRepository.UpdateAsync(objectId, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при збереженні токена скидання пароля: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ValidatePasswordResetTokenAsync(string userId, string token)
        {
            try
            {
                var objectId = new ObjectId(userId);
                var user = await _userRepository.GetByIdAsync(objectId);
                
                if (user == null)
                    return false;
                    
                if (user.PasswordResetToken != token)
                    return false;
                    
                if (user.PasswordResetTokenExpiration < DateTime.UtcNow)
                    return false;
                    
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при валідації токена скидання пароля: {UserId}", userId);
                return false;
            }
        }

        public async Task UpdateUserPasswordAsync(string userId, string newPasswordHash)
        {
            try
            {
                var objectId = new ObjectId(userId);
                var user = await _userRepository.GetByIdAsync(objectId);
                
                if (user == null)
                {
                    _logger.LogWarning("Користувача з ID {UserId} не знайдено при оновленні пароля", userId);
                    return;
                }
                
                user.PasswordHash = newPasswordHash;
                
                await _userRepository.UpdateAsync(objectId, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні пароля користувача: {UserId}", userId);
                throw;
            }
        }

        public async Task InvalidatePasswordResetTokenAsync(string userId)
        {
            try
            {
                var objectId = new ObjectId(userId);
                var user = await _userRepository.GetByIdAsync(objectId);
                
                if (user == null)
                {
                    _logger.LogWarning("Користувача з ID {UserId} не знайдено при видаленні токена скидання пароля", userId);
                    return;
                }
                
                user.PasswordResetToken = null;
                user.PasswordResetTokenExpiration = null;
                
                await _userRepository.UpdateAsync(objectId, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при видаленні токена скидання пароля: {UserId}", userId);
                throw;
            }
        }
    }
}
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
                _logger.LogInformation("Початок оновлення користувача: {Id}", id);
                var objectId = new ObjectId(id);
                var existingUser = await _userRepository.GetByIdAsync(objectId);

                if (existingUser == null)
                {
                    _logger.LogWarning("Користувача з ID {Id} не знайдено", id);
                    return null;
                }

               
                var originalCreatedAt = existingUser.CreatedAt;
                var originalPasswordHash = existingUser.PasswordHash;
                var originalAvatar = existingUser.Avatar;

                _logger.LogInformation("Поточні дані користувача: Avatar={HasAvatar}, Size={AvatarSize}",
                    originalAvatar != null, originalAvatar?.Length ?? 0);

              
                existingUser.Username = userDto.Username;
                existingUser.Email = userDto.Email;

              
                if (userDto.BirthDate != default)
                {
                    existingUser.BirthDate = DateTime.SpecifyKind(userDto.BirthDate, DateTimeKind.Utc);
                }

                existingUser.ZodiacSign = userDto.ZodiacSign;
                existingUser.Role = userDto.Role;

                
                existingUser.CreatedAt = originalCreatedAt;

                
                if (!string.IsNullOrEmpty(userDto.PasswordHash))
                {
                    existingUser.PasswordHash = userDto.PasswordHash;
                }
                else
                {
                    existingUser.PasswordHash = originalPasswordHash;
                }

               
                if (userDto.Avatar != null && userDto.Avatar.Length > 0)
                {
                    _logger.LogInformation("Оновлюємо аватар. Новий розмір: {Size} байт", userDto.Avatar.Length);
                    existingUser.Avatar = userDto.Avatar;
                }
                else
                {
                    _logger.LogInformation("Зберігаємо поточний аватар. Розмір: {Size} байт", originalAvatar?.Length ?? 0);
                    existingUser.Avatar = originalAvatar;
                }

                _logger.LogInformation("Підготовлені дані для оновлення: Avatar={HasAvatar}, Size={AvatarSize}",
                    existingUser.Avatar != null, existingUser.Avatar?.Length ?? 0);

                var updateResult = await _userRepository.UpdateAsync(objectId, existingUser);
                _logger.LogInformation("Результат оновлення: {Success}", updateResult);

                if (updateResult)
                {
                   
                    var updatedUser = await _userRepository.GetByIdAsync(objectId);
                    _logger.LogInformation("Оновлений користувач: Avatar={HasAvatar}, Size={AvatarSize}",
                        updatedUser.Avatar != null, updatedUser.Avatar?.Length ?? 0);
                    return updatedUser.ToDto();
                }

                _logger.LogWarning("Оновлення не відбулося");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні користувача: {Id}", id);
                return null;
            }
        }

        public async Task<UserResponseDto> UpdateUserAvatarAsync(string userId, byte[] avatarData)
        {
            try
            {
                if (avatarData == null || avatarData.Length == 0)
                {
                    _logger.LogWarning("Спроба оновити аватар порожніми даними");
                    throw new ArgumentException("Дані аватару відсутні або порожні");
                }

                _logger.LogInformation("Оновлення аватару користувача {UserId}, розмір: {Size} байт",
                    userId, avatarData.Length);

                var objectId = new ObjectId(userId);
                var user = await _userRepository.GetByIdAsync(objectId);
                if (user == null)
                {
                    _logger.LogWarning("Користувача з ID {UserId} не знайдено", userId);
                    return null;
                }

                user.Avatar = avatarData;

                var updateResult = await _userRepository.UpdateAsync(objectId, user);
                _logger.LogInformation("Результат оновлення аватару: {Success}", updateResult);

                if (updateResult)
                {
                    var updatedUser = await _userRepository.GetByIdAsync(objectId);
                    _logger.LogInformation("Оновлений аватар, розмір: {Size} байт",
                        updatedUser.Avatar?.Length ?? 0);
                    return updatedUser.ToDto();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні аватару: {UserId}", userId);
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
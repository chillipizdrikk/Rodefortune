using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using RodeFortune.BLL.Dto;
using RodeFortune.BLL.Services;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;

namespace RodeFortune.UnitTests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private UserService _userService;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<ILogger<UserService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(
                _mockUserRepository.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task CreateUserAsync_ShouldReturnUserDto_WhenSuccessful()
        {
            var userDto = new UserRequestDto
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                BirthDate = new DateTime(1990, 1, 1),
                ZodiacSign = "Capricorn",
                Role = "User"
            };

            User capturedUser = null;
            _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
                .Callback<User>(user => capturedUser = user)
                .Returns(Task.CompletedTask);

            var result = await _userService.CreateUserAsync(userDto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo(userDto.Username));
            Assert.That(result.Email, Is.EqualTo(userDto.Email));
            Assert.That(capturedUser, Is.Not.Null);
            Assert.That(capturedUser.Username, Is.EqualTo(userDto.Username));
        }


        [Test]
        public async Task UpdateUserAsync_ShouldReturnUpdatedUser_WhenUserExists()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var objectId = new ObjectId(userId);

            var existingUser = new User
            {
                Id = objectId,
                Username = "oldusername",
                Email = "old@example.com",
                PasswordHash = "oldhash",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                Avatar = new byte[] { 1, 2, 3 }
            };

            var updateDto = new UserRequestDto
            {
                Username = "newusername",
                Email = "new@example.com",
                BirthDate = new DateTime(1995, 5, 5),
                ZodiacSign = "Taurus",
                Role = "Admin"
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(objectId)).ReturnsAsync(existingUser);
            _mockUserRepository.Setup(r => r.UpdateAsync(objectId, It.IsAny<User>())).ReturnsAsync(true);

            var updatedUser = new User
            {
                Id = objectId,
                Username = updateDto.Username,
                Email = updateDto.Email,
                BirthDate = updateDto.BirthDate,
                ZodiacSign = updateDto.ZodiacSign,
                Role = updateDto.Role,
                PasswordHash = existingUser.PasswordHash,
                CreatedAt = existingUser.CreatedAt,
                Avatar = existingUser.Avatar
            };
            var getByIdSetup = _mockUserRepository.SetupSequence(r => r.GetByIdAsync(objectId))
                .ReturnsAsync(existingUser)
                .ReturnsAsync(updatedUser);

            var result = await _userService.UpdateUserAsync(userId, updateDto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo(updateDto.Username));
            Assert.That(result.Email, Is.EqualTo(updateDto.Email));
            _mockUserRepository.Verify(r => r.UpdateAsync(objectId, It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task UpdateUserAsync_ShouldUpdatePasswordHash_WhenProvided()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var objectId = new ObjectId(userId);

            var existingUser = new User
            {
                Id = objectId,
                Username = "username",
                Email = "email@example.com",
                PasswordHash = "oldhash",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            };

            var updateDto = new UserRequestDto
            {
                Username = "username",
                Email = "email@example.com",
                PasswordHash = "newhash"
            };

            User capturedUser = null;
            _mockUserRepository.Setup(r => r.GetByIdAsync(objectId)).ReturnsAsync(existingUser);
            _mockUserRepository.Setup(r => r.UpdateAsync(objectId, It.IsAny<User>()))
                .Callback<ObjectId, User>((id, user) => capturedUser = user)
                .ReturnsAsync(true);

            _mockUserRepository.SetupSequence(r => r.GetByIdAsync(objectId))
                .ReturnsAsync(existingUser)
                .ReturnsAsync(existingUser);

            var result = await _userService.UpdateUserAsync(userId, updateDto);

            Assert.That(capturedUser, Is.Not.Null);
            Assert.That(capturedUser.PasswordHash, Is.EqualTo("newhash"));
        }

        [Test]
        public async Task UpdateUserAsync_ShouldKeepOriginalPassword_WhenNotProvided()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var objectId = new ObjectId(userId);

            var existingUser = new User
            {
                Id = objectId,
                Username = "username",
                Email = "email@example.com",
                PasswordHash = "originalhash",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            };

            var updateDto = new UserRequestDto
            {
                Username = "username",
                Email = "email@example.com",
                PasswordHash = ""
            };

            User capturedUser = null;
            _mockUserRepository.Setup(r => r.GetByIdAsync(objectId)).ReturnsAsync(existingUser);
            _mockUserRepository.Setup(r => r.UpdateAsync(objectId, It.IsAny<User>()))
                .Callback<ObjectId, User>((id, user) => capturedUser = user)
                .ReturnsAsync(true);

            _mockUserRepository.SetupSequence(r => r.GetByIdAsync(objectId))
                .ReturnsAsync(existingUser)
                .ReturnsAsync(existingUser);

            var result = await _userService.UpdateUserAsync(userId, updateDto);

            Assert.That(capturedUser, Is.Not.Null);
            Assert.That(capturedUser.PasswordHash, Is.EqualTo("originalhash"));
        }

        [Test]
        public async Task UpdateUserAsync_ShouldUpdateAvatar_WhenProvided()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var objectId = new ObjectId(userId);

            var existingUser = new User
            {
                Id = objectId,
                Username = "username",
                Email = "email@example.com",
                Avatar = new byte[] { 1, 2, 3 }
            };

            var newAvatar = new byte[] { 4, 5, 6, 7 };
            var updateDto = new UserRequestDto
            {
                Username = "username",
                Email = "email@example.com",
                Avatar = newAvatar
            };

            User capturedUser = null;
            _mockUserRepository.Setup(r => r.GetByIdAsync(objectId)).ReturnsAsync(existingUser);
            _mockUserRepository.Setup(r => r.UpdateAsync(objectId, It.IsAny<User>()))
                .Callback<ObjectId, User>((id, user) => capturedUser = user)
                .ReturnsAsync(true);

            _mockUserRepository.SetupSequence(r => r.GetByIdAsync(objectId))
                .ReturnsAsync(existingUser)
                .ReturnsAsync(existingUser);

            var result = await _userService.UpdateUserAsync(userId, updateDto);

            Assert.That(capturedUser, Is.Not.Null);
            Assert.That(capturedUser.Avatar, Is.EqualTo(newAvatar));
        }

        [Test]
        public async Task UpdateUserAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var updateDto = new UserRequestDto
            {
                Username = "newusername",
                Email = "new@example.com"
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<ObjectId>())).ReturnsAsync((User)null);

            var result = await _userService.UpdateUserAsync(userId, updateDto);

            Assert.That(result, Is.Null);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<ObjectId>(), It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task UpdateUserAsync_ShouldReturnNull_WhenUpdateFails()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var objectId = new ObjectId(userId);

            var existingUser = new User
            {
                Id = objectId,
                Username = "oldusername",
                Email = "old@example.com"
            };

            var updateDto = new UserRequestDto
            {
                Username = "newusername",
                Email = "new@example.com"
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(objectId)).ReturnsAsync(existingUser);
            _mockUserRepository.Setup(r => r.UpdateAsync(objectId, It.IsAny<User>())).ReturnsAsync(false);

            var result = await _userService.UpdateUserAsync(userId, updateDto);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateUserAvatarAsync_ShouldReturnUpdatedUser_WhenSuccessful()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var objectId = new ObjectId(userId);
            var avatarData = new byte[] { 1, 2, 3, 4 };

            var existingUser = new User
            {
                Id = objectId,
                Username = "username",
                Email = "email@example.com"
            };

            var updatedUser = new User
            {
                Id = objectId,
                Username = "username",
                Email = "email@example.com",
                Avatar = avatarData
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(objectId)).ReturnsAsync(existingUser);
            _mockUserRepository.Setup(r => r.UpdateAsync(objectId, It.IsAny<User>())).ReturnsAsync(true);

            _mockUserRepository.SetupSequence(r => r.GetByIdAsync(objectId))
                .ReturnsAsync(existingUser)
                .ReturnsAsync(updatedUser);

            var result = await _userService.UpdateUserAvatarAsync(userId, avatarData);

            Assert.That(result, Is.Not.Null);
            _mockUserRepository.Verify(r => r.UpdateAsync(objectId, It.Is<User>(u => u.Avatar == avatarData)), Times.Once);
        }

        [Test]
        public async Task UpdateUserAvatarAsync_ShouldReturnNull_WhenUserNotFound()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var avatarData = new byte[] { 1, 2, 3, 4 };

            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<ObjectId>())).ReturnsAsync((User)null);

            var result = await _userService.UpdateUserAvatarAsync(userId, avatarData);

            Assert.That(result, Is.Null);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<ObjectId>(), It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task UpdateUserAvatarAsync_ShouldReturnNull_WhenUpdateFails()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var avatarData = new byte[] { 1, 2, 3, 4 };

            var existingUser = new User
            {
                Id = new ObjectId(userId),
                Username = "username",
                Email = "email@example.com"
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<ObjectId>())).ReturnsAsync(existingUser);
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<ObjectId>(), It.IsAny<User>())).ReturnsAsync(false);

            var result = await _userService.UpdateUserAvatarAsync(userId, avatarData);
            Assert.That(result, Is.Null);
        }


        [Test]
        public async Task DeleteUserAsync_ShouldReturnTrue_WhenSuccessful()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var objectId = new ObjectId(userId);

            _mockUserRepository.Setup(r => r.DeleteAsync(objectId)).ReturnsAsync(true);

            var result = await _userService.DeleteUserAsync(userId);

            Assert.That(result, Is.True);
            _mockUserRepository.Verify(r => r.DeleteAsync(objectId), Times.Once);
        }

        [Test]
        public async Task DeleteUserAsync_ShouldReturnFalse_WhenRepositoryReturnsFalse()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var objectId = new ObjectId(userId);

            _mockUserRepository.Setup(r => r.DeleteAsync(objectId)).ReturnsAsync(false);

            var result = await _userService.DeleteUserAsync(userId);

            Assert.That(result, Is.False);
        }


        [Test]
        public async Task ValidateUserCredentialsAsync_ShouldReturnTrue_WhenCredentialsMatch()
        {
            var email = "user@example.com";
            var passwordHash = "correctHash";

            var user = new User
            {
                Email = email,
                PasswordHash = passwordHash
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            var result = await _userService.ValidateUserCredentialsAsync(email, passwordHash);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ValidateUserCredentialsAsync_ShouldReturnFalse_WhenPasswordDoesNotMatch()
        {
            var email = "user@example.com";
            var correctPasswordHash = "correctHash";
            var incorrectPasswordHash = "incorrectHash";

            var user = new User
            {
                Email = email,
                PasswordHash = correctPasswordHash
            };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            var result = await _userService.ValidateUserCredentialsAsync(email, incorrectPasswordHash);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ValidateUserCredentialsAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            var email = "nonexistent@example.com";
            var passwordHash = "someHash";

            _mockUserRepository.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User)null);

            var result = await _userService.ValidateUserCredentialsAsync(email, passwordHash);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task SavePasswordResetTokenAsync_ShouldUpdateUser_WhenUserExists()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var objectId = new ObjectId(userId);
            var token = "reset-token";
            var expirationDate = DateTime.UtcNow.AddHours(24);

            var user = new User { Id = objectId };
            User capturedUser = null;

            _mockUserRepository.Setup(r => r.GetByIdAsync(objectId)).ReturnsAsync(user);
            _mockUserRepository.Setup(r => r.UpdateAsync(objectId, It.IsAny<User>()))
                .Callback<ObjectId, User>((id, u) => capturedUser = u)
                .ReturnsAsync(true);

            await _userService.SavePasswordResetTokenAsync(userId, token, expirationDate);

            Assert.That(capturedUser, Is.Not.Null);
            Assert.That(capturedUser.PasswordResetToken, Is.EqualTo(token));
            Assert.That(capturedUser.PasswordResetTokenExpiration, Is.EqualTo(expirationDate));
            _mockUserRepository.Verify(r => r.UpdateAsync(objectId, It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task ValidatePasswordResetTokenAsync_ShouldReturnTrue_WhenTokenIsValid()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var objectId = new ObjectId(userId);
            var token = "valid-token";
            var expirationDate = DateTime.UtcNow.AddHours(1); // Not expired

            var user = new User
            {
                Id = objectId,
                PasswordResetToken = token,
                PasswordResetTokenExpiration = expirationDate
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(objectId)).ReturnsAsync(user);

            var result = await _userService.ValidatePasswordResetTokenAsync(userId, token);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ValidatePasswordResetTokenAsync_ShouldReturnFalse_WhenTokenExpired()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var objectId = new ObjectId(userId);
            var token = "expired-token";
            var expirationDate = DateTime.UtcNow.AddHours(-1); // Expired

            var user = new User
            {
                Id = objectId,
                PasswordResetToken = token,
                PasswordResetTokenExpiration = expirationDate
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(objectId)).ReturnsAsync(user);

            var result = await _userService.ValidatePasswordResetTokenAsync(userId, token);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateUserPasswordAsync_ShouldUpdatePassword_WhenUserExists()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var objectId = new ObjectId(userId);
            var newPasswordHash = "new-hashed-password";

            var user = new User { Id = objectId, PasswordHash = "old-hash" };
            User capturedUser = null;

            _mockUserRepository.Setup(r => r.GetByIdAsync(objectId)).ReturnsAsync(user);
            _mockUserRepository.Setup(r => r.UpdateAsync(objectId, It.IsAny<User>()))
                .Callback<ObjectId, User>((id, u) => capturedUser = u)
                .ReturnsAsync(true);

            await _userService.UpdateUserPasswordAsync(userId, newPasswordHash);

            Assert.That(capturedUser, Is.Not.Null);
            Assert.That(capturedUser.PasswordHash, Is.EqualTo(newPasswordHash));
            _mockUserRepository.Verify(r => r.UpdateAsync(objectId, It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task InvalidatePasswordResetTokenAsync_ShouldClearToken_WhenUserExists()
        {
            var userId = ObjectId.GenerateNewId().ToString();
            var objectId = new ObjectId(userId);

            var user = new User
            {
                Id = objectId,
                PasswordResetToken = "token",
                PasswordResetTokenExpiration = DateTime.UtcNow.AddHours(1)
            };

            User capturedUser = null;

            _mockUserRepository.Setup(r => r.GetByIdAsync(objectId)).ReturnsAsync(user);
            _mockUserRepository.Setup(r => r.UpdateAsync(objectId, It.IsAny<User>()))
                .Callback<ObjectId, User>((id, u) => capturedUser = u)
                .ReturnsAsync(true);

            await _userService.InvalidatePasswordResetTokenAsync(userId);

            Assert.That(capturedUser, Is.Not.Null);
            Assert.That(capturedUser.PasswordResetToken, Is.Null);
            Assert.That(capturedUser.PasswordResetTokenExpiration, Is.Null);
            _mockUserRepository.Verify(r => r.UpdateAsync(objectId, It.IsAny<User>()), Times.Once);
        }

    }
}
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace RodeFortune.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IMongoDatabase database, ILogger<UserRepository> logger)
        {
            _users = database.GetCollection<User>("Users");
            _logger = logger;
        }

        public async Task<User> GetByIdAsync(ObjectId id)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(u => u.Id, id);
                return await _users.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by ID: {Id}", id);
                throw;
            }
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(u => u.Email, email);
                return await _users.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by email: {Email}", email);
                throw;
            }
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(u => u.Username, username);
                return await _users.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by username: {Username}", username);
                throw;
            }
        }

        public async Task CreateAsync(User user)
        {
            try
            {
                await _users.InsertOneAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {Username}", user.Username);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ObjectId id, User user)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(u => u.Id, id);
                var result = await _users.ReplaceOneAsync(filter, user);
                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(ObjectId id)
        {
            try
            {
                var filter = Builders<User>.Filter.Eq(u => u.Id, id);
                var result = await _users.DeleteOneAsync(filter);
                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {Id}", id);
                throw;
            }
        }
    }
}
using RodeFortune.BLL.Dto;
using System.Threading.Tasks;

namespace RodeFortune.BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> GetUserByIdAsync(string id);
        Task<UserResponseDto> GetUserByEmailAsync(string email);
        Task<UserResponseDto> GetUserByUsernameAsync(string username);
        Task<UserResponseDto> CreateUserAsync(UserRequestDto userDto);
        Task<UserResponseDto> UpdateUserAsync(string id, UserRequestDto userDto);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> ValidateUserCredentialsAsync(string email, string passwordHash);
    }
}
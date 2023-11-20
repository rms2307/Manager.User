using Manager.Services.Dtos;

namespace Manager.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> CreateAsync(UserDto userDto);
        Task<UserDto> UpdateAsync(UserDto userDto);
        Task RemoveAsync(long id);
        Task<UserDto> GetAsync(long id);
        Task<IList<UserDto>> GetAllAsync();
        Task<IList<UserDto>> SearchByNameAsync(string name);
        Task<IList<UserDto>> SearchByEmailAsync(string email);
        Task<UserDto> GetByEmailAsync(string email);
    }
}

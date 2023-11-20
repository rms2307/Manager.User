using AutoMapper;
using Manager.Core.Exceptions;
using Manager.Domain.Entities;
using Manager.Infra.Interfaces;
using Manager.Services.Dtos;
using Manager.Services.Interfaces;

namespace Manager.Services.Services
{
    public class UserService(IMapper mapper, IUserRepository userResository) : IUserService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IUserRepository _userResository = userResository;

        public async Task<UserDto> CreateAsync(UserDto userDto)
        {
            User? userExists = await _userResository.GetByEmailAsync(userDto.Email);
            if (userExists != null)
                throw new DomainException("E-mail já cadastrado.");

            User user = _mapper.Map<User>(userDto);
            user.Validate();

            User userCreated = await _userResository.CreateAsync(user);

            return _mapper.Map<UserDto>(userCreated);
        }

        public async Task<UserDto> UpdateAsync(UserDto userDto)
        {
            _ = await _userResository.GetAsync(userDto.Id)
                ?? throw new DomainException("Usuário não encontrado.");

            User user = _mapper.Map<User>(userDto);
            user.Validate();

            User userUpdated = await _userResository.CreateAsync(user);

            return _mapper.Map<UserDto>(userUpdated);
        }

        public async Task RemoveAsync(long id)
            => await _userResository.RemoveAsync(id);

        public async Task<UserDto> GetAsync(long id)
            => _mapper.Map<UserDto>(await _userResository.GetAsync(id));

        public async Task<IList<UserDto>> GetAllAsync()
            => _mapper.Map<List<UserDto>>(await _userResository.GetAllAsync());

        public async Task<UserDto> GetByEmailAsync(string email)
            => _mapper.Map<UserDto>(await _userResository.GetByEmailAsync(email));

        public async Task<IList<UserDto>> SearchByEmailAsync(string email)
            => _mapper.Map<List<UserDto>>(await _userResository.SearchByEmailAsync(email));

        public async Task<IList<UserDto>> SearchByNameAsync(string name)
            => _mapper.Map<List<UserDto>>(await _userResository.SearchByNameAsync(name));
    }
}

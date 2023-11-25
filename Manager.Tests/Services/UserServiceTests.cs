using AutoMapper;
using Bogus;
using Bogus.DataSets;
using FluentAssertions;
using Manager.Core.Exceptions;
using Manager.Core.Services.Interfaces;
using Manager.Domain.Entities;
using Manager.Infra.Interfaces;
using Manager.Services.Dtos;
using Manager.Services.Interfaces;
using Manager.Services.Services;
using Manager.Tests.Configurations;
using Manager.Tests.Fixtures;
using Moq;
using System.Linq.Expressions;

namespace Manager.Tests.Services
{
    public class UserServiceTests
    {
        private readonly IUserService _userService;

        private readonly IMapper _mapper;

        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ICryptographyService> _cryptographyServiceMock;

        public UserServiceTests()
        {
            _mapper = AutoMapperConfiguration.GetConfiguration();

            _userRepositoryMock = new Mock<IUserRepository>();
            _cryptographyServiceMock = new Mock<ICryptographyService>();

            _userService = new UserService(_mapper, _userRepositoryMock.Object, _cryptographyServiceMock.Object);
        }

        [Fact]
        [Trait("Category", "Services")]
        public async Task Create_WhenUserIsValid_ReturnsUserDto()
        {
            // Arrange
            UserDto userToCreate = UserFixture.CreateValidUserDto();

            string encryptedPassword = new Lorem().Sentence();
            User userCreated = _mapper.Map<User>(userToCreate);
            userCreated.SetPassword(encryptedPassword);

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            _cryptographyServiceMock.Setup(x => x.Encrypt(It.IsAny<string>()))
                .Returns(encryptedPassword);

            _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(() => userCreated);

            // Act
            var result = await _userService.CreateAsync(userToCreate);

            // Assert
            result.Should().BeEquivalentTo(_mapper.Map<UserDto>(userCreated));
        }

        [Fact]
        [Trait("Category", "Services")]
        public async Task Create_WhenUserExists_ReturnsDomainException()
        {
            // Arrange
            var userToCreate = UserFixture.CreateValidUserDto();
            var userExists = UserFixture.CreateValidUser();

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                 .ReturnsAsync(() => userExists);

            // Act
            Func<Task<UserDto>> act = async () =>
            {
                return await _userService.CreateAsync(userToCreate);
            };

            // Assert
            await act.Should()
                   .ThrowAsync<DomainException>()
                   .WithMessage("E-mail já cadastrado.");
        }

        [Fact]
        [Trait("Category", "Services")]
        public async Task Create_WhenUserIsInvalid_ReturnsDomainException()
        {
            // Arrange
            var userToCreate = UserFixture.CreateInvalidUserDto();

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
               .ReturnsAsync(() => null);

            // Act
            Func<Task<UserDto>> act = async () =>
            {
                return await _userService.CreateAsync(userToCreate);
            };

            // Assert
            await act.Should()
                   .ThrowAsync<DomainException>()
                   .WithMessage("Alguns erros ocorreram: ");
        }

        [Fact]
        [Trait("Category", "Services")]
        public async Task Update_WhenUserIsValid_ReturnsUserDto()
        {
            // Arrange
            User oldUser = UserFixture.CreateValidUser();
            UserDto userToUpdate = UserFixture.CreateValidUserDto();
            User userUpdated = _mapper.Map<User>(userToUpdate);

            string encryptedPassword = new Lorem().Sentence();

            _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(() => oldUser);

            _cryptographyServiceMock.Setup(x => x.Encrypt(It.IsAny<string>()))
                .Returns(encryptedPassword);

            _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(() => userUpdated);

            // Act
            var result = await _userService.UpdateAsync(userToUpdate);

            // Assert
            result.Should().BeEquivalentTo(_mapper.Map<UserDto>(userUpdated));
        }

        [Fact]
        [Trait("Category", "Services")]
        public async Task Update_WhenUserNotExists_ReturnsDomainException()
        {
            // Arrange
            var userUpdated = UserFixture.CreateValidUserDto();
            _userRepositoryMock.Setup(x => x.GetAsync(It.IsAny<long>()))
                 .ReturnsAsync(() => null);

            // Act
            Func<Task<UserDto>> act = async () =>
            {
                return await _userService.UpdateAsync(userUpdated);
            };

            // Assert
            await act.Should()
                   .ThrowAsync<DomainException>()
                   .WithMessage("Usuário não encontrado.");
        }

        [Fact(DisplayName = "Remove User")]
        [Trait("Category", "Services")]
        public async Task Remove_WhenUserExists_RemoveUser()
        {
            // Arrange
            var userId = new Randomizer().Int(0, 1000);

            _userRepositoryMock.Setup(x => x.RemoveAsync(It.IsAny<int>()))
                .Verifiable();

            // Act
            await _userService.RemoveAsync(userId);

            // Assert
            _userRepositoryMock.Verify(x => x.RemoveAsync(userId), Times.Once);
        }

        [Fact]
        [Trait("Category", "Services")]
        public async Task GetById_WhenUserExists_ReturnsUserDto()
        {
            // Arrange
            var userId = new Randomizer().Int(0, 1000);
            var userFound = UserFixture.CreateValidUser();

            _userRepositoryMock.Setup(x => x.GetAsync(userId))
            .ReturnsAsync(() => userFound);

            // Act
            var result = await _userService.GetAsync(userId);

            // Assert
            result.Should()
                .BeEquivalentTo(_mapper.Map<UserDto>(userFound));
        }

        [Fact]
        [Trait("Category", "Services")]
        public async Task GetById_WhenUserNotExists_ReturnsNull()
        {
            // Arrange
            var userId = new Randomizer().Int(0, 1000);

            _userRepositoryMock.Setup(x => x.GetAsync(userId))
                .ReturnsAsync(() => null);

            // Act
            var result = await _userService.GetAsync(userId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        [Trait("Category", "Services")]
        public async Task GetByEmail_WhenUserExists_ReturnsUserDto()
        {
            // Arrange
            var userEmail = new Internet().Email();
            var userFound = UserFixture.CreateValidUser();

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => userFound);

            // Act
            var result = await _userService.GetByEmailAsync(userEmail);

            // Assert
            result.Should()
                .BeEquivalentTo(_mapper.Map<UserDto>(userFound));
        }

        [Fact]
        [Trait("Category", "Services")]
        public async Task GetByEmail_WhenUserNotExists_ReturnsNull()
        {
            // Arrange
            var userEmail = new Internet().Email();

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _userService.GetByEmailAsync(userEmail);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        [Trait("Category", "Services")]
        public async Task GetAllUsers_WhenUsersExists_ReturnsAListOfUserDto()
        {
            // Arrange
            var usersFound = UserFixture.CreateListValidUser();

            _userRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => usersFound);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(_mapper.Map<List<UserDto>>(usersFound));
        }

        [Fact]
        [Trait("Category", "Services")]
        public async Task GetAllUsers_WhenNoneUserFound_ReturnsEmptyList()
        {
            // Arrange

            _userRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => null);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            result.Should()
                .BeEmpty();
        }

        [Fact]
        [Trait("Category", "Services")]
        public async Task SearchByName_WhenAnyUserFound_ReturnsAListOfUserDto()
        {
            // Arrange
            var nameToSearch = new Name().FirstName();
            var usersFound = UserFixture.CreateListValidUser();

            _userRepositoryMock.Setup(x => x.SearchByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(() => usersFound);

            // Act
            var result = await _userService.SearchByNameAsync(nameToSearch);

            // Assert
            result.Should()
                .BeEquivalentTo(_mapper.Map<List<UserDto>>(usersFound));
        }

        [Fact]
        [Trait("Category", "Services")]
        public async Task SearchByName_WhenNoneUserFound_ReturnsEmptyList()
        {
            // Arrange
            var nameToSearch = new Name().FirstName();

            _userRepositoryMock.Setup(x => x.SearchByNameAsync(It.IsAny<string>()))
                 .ReturnsAsync(() => null);

            // Act
            var result = await _userService.SearchByNameAsync(nameToSearch);

            // Assert
            result.Should()
                .BeEmpty();
        }

        [Fact(DisplayName = "Search By Email")]
        [Trait("Category", "Services")]
        public async Task SearchByEmail_WhenAnyUserFound_ReturnsAListOfUserDto()
        {
            // Arrange
            var emailSoSearch = new Internet().Email();
            var usersFound = UserFixture.CreateListValidUser();

            _userRepositoryMock.Setup(x => x.SearchByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => usersFound);

            // Act
            var result = await _userService.SearchByEmailAsync(emailSoSearch);

            // Assert
            result.Should()
                .BeEquivalentTo(_mapper.Map<List<UserDto>>(usersFound));
        }

        [Fact]
        [Trait("Category", "Services")]
        public async Task SearchByEmail_WhenNoneUserFound_ReturnsEmptyList()
        {
            // Arrange
            var emailSoSearch = new Internet().Email();

            _userRepositoryMock.Setup(x => x.SearchByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            var result = await _userService.SearchByEmailAsync(emailSoSearch);

            // Assert
            result.Should().BeEmpty();
        }
    }
}

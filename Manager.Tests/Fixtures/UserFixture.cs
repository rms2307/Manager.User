using Bogus.DataSets;
using Bogus;
using Manager.Domain.Entities;
using Manager.Services.Dtos;

namespace Manager.Tests.Fixtures
{
    public static class UserFixture
    {
        public static User CreateValidUser()
        {
            return new User(
                name: new Name().FirstName(),
                email: new Internet().Email(),
                password: new Internet().Password());
        }

        public static List<User> CreateListValidUser(int limit = 5)
        {
            var list = new List<User>();

            for (int i = 0; i < limit; i++)
                list.Add(CreateValidUser());

            return list;
        }

        public static UserDto CreateValidUserDto(bool newId = false)
        {
            return new UserDto
            {
                Id = newId ? new Randomizer().Int(0, 1000) : 0,
                Name = new Name().FirstName(),
                Email = new Internet().Email(),
                Password = new Internet().Password()
            };
        }

        public static UserDto CreateInvalidUserDto()
        {
            return new UserDto
            {
                Id = 0,
                Name = "",
                Email = "",
                Password = ""
            };
        }
    }
}

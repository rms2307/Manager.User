using System.Text.Json.Serialization;

namespace Manager.Services.Dtos
{
    public class UserDto
    {
        public UserDto() { }

        public UserDto(int id, string name, string email, string password)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [JsonIgnore]
        public string Password { get; set; } = string.Empty;
    }
}

using MessagePack;

namespace RealTimeChat.Api.ViewModels
{
    [MessagePackObject]
    public class RegisterViewModel
    {
        public RegisterViewModel(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }

        [Key("name")]
        public string Name { get; set; }

        [Key("email")]
        public string Email { get; set; }

        [Key("password")]
        public string Password { get; set; }
    }
}

using MessagePack;

namespace RealTimeChat.Api.ViewModels
{
    [MessagePackObject]
    public class LogInViewModel
    {
        public LogInViewModel(string email, string password)
        {
            Email = email;
            Password = password;
        }

        [Key("email")]
        public string Email { get; set; }

        [Key("password")]
        public string Password { get; set; }
    }
}

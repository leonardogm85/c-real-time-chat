namespace RealTimeChat.Api.ViewModels
{
    public class LogInViewModel
    {
        public LogInViewModel(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; set; }
        public string Password { get; set; }
    }
}

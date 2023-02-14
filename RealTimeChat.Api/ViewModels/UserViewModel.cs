using RealTimeChat.Api.Models;

namespace RealTimeChat.Api.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel(Guid id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public static explicit operator UserViewModel(User user)
        {
            return new(user.Id, user.Name, user.Email);
        }
    }
}

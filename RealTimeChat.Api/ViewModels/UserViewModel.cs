using MessagePack;
using RealTimeChat.Api.Models;

namespace RealTimeChat.Api.ViewModels
{
    [MessagePackObject]
    public class UserViewModel
    {
        public UserViewModel(Guid id, string name, string email, bool isOnline)
        {
            Id = id;
            Name = name;
            Email = email;
            IsOnline = isOnline;
        }

        [Key("id")]
        public Guid Id { get; set; }

        [Key("name")]
        public string Name { get; set; }

        [Key("email")]
        public string Email { get; set; }

        [Key("isOnline")]
        public bool IsOnline { get; set; }

        public static explicit operator UserViewModel(User user)
        {
            return new(user.Id, user.Name, user.Email, user.IsOnline);
        }
    }
}

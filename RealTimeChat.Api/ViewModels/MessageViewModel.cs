using MessagePack;
using RealTimeChat.Api.Models;

namespace RealTimeChat.Api.ViewModels
{
    [MessagePackObject]
    public class MessageViewModel
    {
        public MessageViewModel(Guid id, string groupName, Guid userId, string userName, string text)
        {
            Id = id;
            GroupName = groupName;
            UserId = userId;
            UserName = userName;
            Text = text;
        }

        [Key("id")]
        public Guid Id { get; set; }

        [Key("groupName")]
        public string GroupName { get; set; }

        [Key("userId")]
        public Guid UserId { get; set; }

        [Key("userName")]
        public string UserName { get; set; }

        [Key("text")]
        public string Text { get; set; }

        public static explicit operator MessageViewModel(Message message)
        {
            return new(message.Id, message.Group!.Name, message.User!.Id, message.User.Name, message.Text);
        }
    }
}

using RealTimeChat.Api.Models;

namespace RealTimeChat.Api.ViewModels
{
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

        public Guid Id { get; set; }
        public string GroupName { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }

        public static explicit operator MessageViewModel(Message message)
        {
            return new(message.Id, message.Group!.Name, message.User!.Id, message.User.Name, message.Text);
        }
    }
}

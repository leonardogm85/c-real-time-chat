namespace RealTimeChat.Api.Models
{
    public class Message
    {
        public Message(Guid groupId, Guid userId, string text)
        {
            Id = Guid.NewGuid();
            GroupId = groupId;
            UserId = userId;
            Text = text;
            CreateAt = DateTime.Now;
        }

        public Guid Id { get; private set; }
        public Guid GroupId { get; private set; }
        public Guid UserId { get; private set; }
        public string Text { get; private set; }
        public DateTime CreateAt { get; private set; }

        public virtual Group? Group { get; private set; }
        public virtual User? User { get; private set; }

        public void SetGroup(Group group) => Group = group;
        public void SetUser(User user) => User = user;
    }
}

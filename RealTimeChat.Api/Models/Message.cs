namespace RealTimeChat.Api.Models
{
    public class Message
    {
        public Message(string gruop, string user, string text)
        {
            Id = Guid.NewGuid();
            Gruop = gruop;
            User = user;
            Text = text;
        }

        public Guid Id { get; private set; }
        public string Gruop { get; private set; }
        public string User { get; private set; }
        public string Text { get; private set; }
        public DateTime CreateAt { get; private set; }
    }
}

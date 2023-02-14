namespace RealTimeChat.Api.Models
{
    public class User
    {
        public User(string name, string email, string password)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            Password = password;
            ConnectionsId = string.Empty;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public bool IsOnline { get; private set; }
        public string ConnectionsId { get; private set; }

        public void SetConnectionsId(string connectionsId) => ConnectionsId = connectionsId;
    }
}

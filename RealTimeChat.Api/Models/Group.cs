namespace RealTimeChat.Api.Models
{
    public class Group
    {
        public Group(string name, string users)
        {
            Id = Guid.NewGuid();
            Name = name;
            Users = users;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Users { get; private set; }
    }
}

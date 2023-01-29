namespace RealTimeChat.Models;

public sealed class Group
{
    public Group(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Users { get; private set; } = string.Empty;
}

namespace ChatApp.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    private readonly List<User> _contacts = new();
    public IReadOnlyCollection<User> Contacts => _contacts.AsReadOnly();

    private readonly List<Group> _groups = new();
    public IReadOnlyCollection<Group> Groups => _groups.AsReadOnly();

    private readonly List<Message> _sentMessages = new();
    public IReadOnlyCollection<Message> SentMessages => _sentMessages.AsReadOnly();

    private readonly List<Message> _receivedMessages = new();
    public IReadOnlyCollection<Message> ReceivedMessages => _receivedMessages.AsReadOnly();

    private User() { } // For EF Core

    public User(string username, string name, string passwordHash)
    {
        Id = Guid.NewGuid();
        Username = username;
        Name = name;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddContact(User contact)
    {
        if (!_contacts.Contains(contact))
        {
            _contacts.Add(contact);
            contact._contacts.Add(this);
        }
    }

    public void RemoveContact(User contact)
    {
        if (_contacts.Contains(contact))
        {
            _contacts.Remove(contact);
            contact._contacts.Remove(this);
        }
    }
}
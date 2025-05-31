namespace ChatApp.Domain.Entities;

public class Message
{
    public Guid Id { get; private set; }
    public string Content { get; private set; }
    public Guid SenderId { get; private set; }
    public User Sender { get; private set; } = null!;
    public DateTime SentAt { get; private set; }

    // Recipient can be either a user or a group
    public Guid? RecipientUserId { get; private set; }
    public User? RecipientUser { get; private set; }
    public Guid? RecipientGroupId { get; private set; }
    public Group? RecipientGroup { get; private set; }

    private Message() { } // For EF Core

    public Message(string content, User sender, User? recipientUser = null, Group? recipientGroup = null)
    {
        if (recipientUser == null && recipientGroup == null)
            throw new ArgumentException("Message must have either a user or group recipient");

        if (recipientUser != null && recipientGroup != null)
            throw new ArgumentException("Message cannot have both user and group recipient");

        Id = Guid.NewGuid();
        Content = content;
        SenderId = sender.Id;
        Sender = sender;
        SentAt = DateTime.UtcNow;

        if (recipientUser != null)
        {
            RecipientUserId = recipientUser.Id;
            RecipientUser = recipientUser;
        }

        if (recipientGroup != null)
        {
            RecipientGroupId = recipientGroup.Id;
            RecipientGroup = recipientGroup;
        }
    }
}
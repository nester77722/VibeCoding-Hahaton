using System;

namespace ChatApp.Domain.Entities;

public class Message
{
    public Guid Id { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public User Sender { get; private set; }
    public Guid SenderId { get; private set; }
    
    // For direct messages
    public User? Recipient { get; private set; }
    public Guid? RecipientId { get; private set; }
    
    // For group messages
    public Group? Group { get; private set; }
    public Guid? GroupId { get; private set; }
    
    private Message() { } // For EF Core
    
    // Constructor for direct messages
    public Message(string content, User sender, User recipient)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content cannot be empty", nameof(content));
            
        Id = Guid.NewGuid();
        Content = content;
        CreatedAt = DateTime.UtcNow;
        Sender = sender;
        SenderId = sender.Id;
        Recipient = recipient;
        RecipientId = recipient.Id;
    }
    
    // Constructor for group messages
    public Message(string content, User sender, Group group)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content cannot be empty", nameof(content));
            
        if (!group.Members.Contains(sender))
            throw new InvalidOperationException("Sender must be a member of the group");
            
        Id = Guid.NewGuid();
        Content = content;
        CreatedAt = DateTime.UtcNow;
        Sender = sender;
        SenderId = sender.Id;
        Group = group;
        GroupId = group.Id;
    }
} 
using System;
using System.Collections.Generic;

namespace ChatApp.Domain.Entities;

public class Group
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public User Creator { get; private set; }
    public Guid CreatorId { get; private set; }
    
    private readonly List<User> _members = new();
    public IReadOnlyCollection<User> Members => _members.AsReadOnly();
    
    private readonly List<Message> _messages = new();
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();
    
    private Group() { } // For EF Core
    
    public Group(string name, User creator)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name cannot be empty", nameof(name));
            
        Id = Guid.NewGuid();
        Name = name;
        Creator = creator;
        CreatorId = creator.Id;
        _members.Add(creator);
    }
    
    public void AddMember(User user)
    {
        if (_members.Count >= 300)
            throw new InvalidOperationException("Group has reached maximum capacity of 300 members");
            
        if (!_members.Contains(user))
            _members.Add(user);
    }
    
    public void RemoveMember(User user)
    {
        if (user.Id == CreatorId)
            throw new InvalidOperationException("Cannot remove the group creator");
            
        _members.Remove(user);
    }
} 
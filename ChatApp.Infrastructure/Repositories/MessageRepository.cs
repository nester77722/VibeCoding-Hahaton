using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ChatAppDbContext _context;
    
    public MessageRepository(ChatAppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .Include(m => m.Group)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }
    
    public async Task<IEnumerable<Message>> GetDirectMessagesAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Recipient)
            .Where(m => 
                (m.SenderId == userId1 && m.RecipientId == userId2) ||
                (m.SenderId == userId2 && m.RecipientId == userId1))
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Message>> GetGroupMessagesAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Group)
            .Where(m => m.GroupId == groupId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }
    
    public async Task AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        await _context.Messages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
} 
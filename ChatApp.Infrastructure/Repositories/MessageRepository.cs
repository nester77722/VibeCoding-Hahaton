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
            .Include(m => m.RecipientUser)
            .Include(m => m.RecipientGroup)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetDirectMessagesAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.RecipientUser)
            .Where(m =>
                (m.SenderId == userId1 && m.RecipientUserId == userId2) ||
                (m.SenderId == userId2 && m.RecipientUserId == userId1))
            .OrderBy(m => m.SentAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetGroupMessagesAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.RecipientGroup)
            .Where(m => m.RecipientGroupId == groupId)
            .OrderBy(m => m.SentAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        await _context.Messages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
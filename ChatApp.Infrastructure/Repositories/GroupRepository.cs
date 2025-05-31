using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly ChatAppDbContext _context;

    public GroupRepository(ChatAppDbContext context)
    {
        _context = context;
    }

    public async Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Include(g => g.Creator)
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Group>> GetUserGroupsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Include(g => g.Creator)
            .Include(g => g.Members)
            .Where(g => g.Members.Any(m => m.Id == userId))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Group group, CancellationToken cancellationToken = default)
    {
        await _context.Groups.AddAsync(group, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Group group, CancellationToken cancellationToken = default)
    {
        _context.Groups.Update(group);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Group group)
    {
        if (group is null)
        {
            throw new ArgumentNullException(nameof(group), "Group cannot be null");
        }

        var exists = _context.Groups.Any(g => g.Id == group.Id);
        if (!exists)
        {
            throw new InvalidOperationException("Group does not exist");
        }

        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();
    }
}
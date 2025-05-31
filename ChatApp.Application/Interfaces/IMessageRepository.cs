using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Message>> GetDirectMessagesAsync(Guid userId1, Guid userId2, CancellationToken cancellationToken = default);
    Task<IEnumerable<Message>> GetGroupMessagesAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task AddAsync(Message message, CancellationToken cancellationToken = default);
} 
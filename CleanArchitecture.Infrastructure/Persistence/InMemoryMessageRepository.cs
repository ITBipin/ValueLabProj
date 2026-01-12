using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Infrastructure.Persistence
{
    public class InMemoryMessageRepository : IMessageRepository
    {
        private readonly ConcurrentDictionary<Guid, Message> _messages = new();

        public Task<IEnumerable<Message>> GetAllAsync(Guid organizationId)
        {
            var result = _messages.Values
                .Where(m => m.OrganizationId == organizationId)
                .AsEnumerable();

            return Task.FromResult(result);
        }

        public Task<Message?> GetAsync(Guid organizationId, Guid id)
        {
            _messages.TryGetValue(id, out var message);

            if (message?.OrganizationId != organizationId)
                return Task.FromResult<Message?>(null);

            return Task.FromResult(message);
        }

        public Task AddAsync(Message message)
        {
            _messages[message.Id] = message;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Message message)
        {
            _messages[message.Id] = message;
            return Task.CompletedTask;
        }

        public Task<bool> DeleteAsync(Guid organizationId, Guid id)
        {
            if (_messages.TryGetValue(id, out var message) &&
                message.OrganizationId == organizationId)
            {
                return Task.FromResult(_messages.TryRemove(id, out _));
            }

            return Task.FromResult(false);
        }

        public Task<bool> ExistsTitleAsync(
            Guid organizationId,
            string title,
            Guid? excludeMessageId = null)
        {
            var exists = _messages.Values.Any(m =>
                m.OrganizationId == organizationId &&
                m.Title.Equals(title, StringComparison.OrdinalIgnoreCase) &&
                (!excludeMessageId.HasValue || m.Id != excludeMessageId));

            return Task.FromResult(exists);
        }
    }
}

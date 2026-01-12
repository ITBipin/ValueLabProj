using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetAllAsync(Guid organizationId);

        Task<Message?> GetAsync(Guid organizationId, Guid id);

        Task AddAsync(Message message);

        Task UpdateAsync(Message message);

        Task<bool> DeleteAsync(Guid organizationId, Guid id);

        Task<bool> ExistsTitleAsync(
            Guid organizationId,
            string title,
            Guid? excludeMessageId = null);
    }
}

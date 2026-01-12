using System;
using System.Collections.Generic;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Application.Common.Results;

namespace CleanArchitecture.Application.Services
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetAllAsync(Guid organizationId);
        Task<Message?> GetByIdAsync(Guid organizationId, Guid id);
        Task<Result> CreateAsync(Message message);
        Task<Result> UpdateAsync(Message message);
        Task<Result> DeleteAsync(Guid organizationId, Guid id);
    }


}

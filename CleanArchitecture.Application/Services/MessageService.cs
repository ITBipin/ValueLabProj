using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Results;

namespace CleanArchitecture.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repo;

        public MessageService(IMessageRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Message>> GetAllAsync(Guid organizationId)
            => await _repo.GetAllAsync(organizationId);

        public async Task<Message?> GetByIdAsync(Guid organizationId, Guid id)
            => await _repo.GetAsync(organizationId, id);

        public async Task<Result> CreateAsync(Message message)
        {
            var errors = Validate(message, isNew: true);
            if (errors.Any())
                return new ValidationErrorResult { Errors = errors };

            if (await _repo.ExistsTitleAsync(message.OrganizationId, message.Title))
                return new ConflictResult
                {
                    Text = "Title must be unique per organization"
                };

            message.Id = Guid.NewGuid();
            message.CreatedAt = DateTime.UtcNow;

            await _repo.AddAsync(message);

            return new CreatedResult<Message> { Value = message };
        }

        public async Task<Result> UpdateAsync(Message message)
        {
            var existing = await _repo.GetAsync(message.OrganizationId, message.Id);
            if (existing == null)
                return new NotFoundResult();

            if (!existing.IsActive)
                return new ValidationErrorResult
                {
                    Errors = new Dictionary<string, string[]>
                    {
                        { "IsActive", new[] { "Cannot update inactive message" } }
                    }
                };

            var errors = Validate(message, isNew: false);
            if (errors.Any())
                return new ValidationErrorResult { Errors = errors };

            if (await _repo.ExistsTitleAsync(
                    message.OrganizationId,
                    message.Title,
                    message.Id))
            {
                return new ConflictResult
                {
                    Text = "Title must be unique per organization"
                };
            }

            message.CreatedAt = existing.CreatedAt;
            message.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(message);

            return new SuccessResult();
        }

        public async Task<Result> DeleteAsync(Guid organizationId, Guid id)
        {
            var existing = await _repo.GetAsync(organizationId, id);
            if (existing == null)
                return new NotFoundResult();

            if (!existing.IsActive)
                return new ValidationErrorResult
                {
                    Errors = new Dictionary<string, string[]>
                    {
                        { "IsActive", new[] { "Cannot delete inactive message" } }
                    }
                };

            var ok = await _repo.DeleteAsync(organizationId, id);
            return ok ? new SuccessResult() : new NotFoundResult();
        }

        private Dictionary<string, string[]> Validate(Message message, bool isNew)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(message.Title) ||
                message.Title.Length < 3 ||
                message.Title.Length > 200)
            {
                errors["Title"] = new[]
                {
                    "Title is required and must be between 3 and 200 characters"
                };
            }

            if (string.IsNullOrWhiteSpace(message.Content) ||
                message.Content.Length < 10 ||
                message.Content.Length > 1000)
            {
                errors["Content"] = new[]
                {
                    "Content must be between 10 and 1000 characters"
                };
            }

            return errors;
        }
    }
}

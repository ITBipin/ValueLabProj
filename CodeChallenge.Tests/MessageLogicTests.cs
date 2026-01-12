using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Application.Common.Results;

namespace CodeChallenge.Tests
{
    public class MessageServiceTests
    {
        [Fact]
        public async Task Create_Should_Create_Message_Successfully()
        {
            var repo = new Mock<IMessageRepository>();
            repo.Setup(r => r.ExistsTitleAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    null))
                .ReturnsAsync(false);

            var service = new MessageService(repo.Object);

            var msg = new Message
            {
                OrganizationId = Guid.NewGuid(),
                Title = "Hello",
                Content = "This is valid content"
            };

            var res = await service.CreateAsync(msg);

            res.Should().BeOfType<CreatedResult<Message>>();

            repo.Verify(r => r.AddAsync(It.IsAny<Message>()), Times.Once);
        }

        [Fact]
        public async Task Create_DuplicateTitle_Returns_Conflict()
        {
            var repo = new Mock<IMessageRepository>();
            repo.Setup(r => r.ExistsTitleAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    null))
                .ReturnsAsync(true);

            var service = new MessageService(repo.Object);

            var msg = new Message
            {
                OrganizationId = Guid.NewGuid(),
                Title = "Hello",
                Content = "This is valid content"
            };

            var res = await service.CreateAsync(msg);

            res.Should().BeOfType<ConflictResult>();

            repo.Verify(r => r.AddAsync(It.IsAny<Message>()), Times.Never);
        }

        [Fact]
        public async Task Create_InvalidContent_Returns_ValidationError()
        {
            var repo = new Mock<IMessageRepository>();
            var service = new MessageService(repo.Object);

            var msg = new Message
            {
                OrganizationId = Guid.NewGuid(),
                Title = "Hi",
                Content = "short"
            };

            var res = await service.CreateAsync(msg);

            res.Should().BeOfType<ValidationErrorResult>();

            repo.Verify(r => r.AddAsync(It.IsAny<Message>()), Times.Never);
        }

        [Fact]
        public async Task Update_NonExistent_Returns_NotFound()
        {
            var repo = new Mock<IMessageRepository>();
            repo.Setup(r => r.GetAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>()))
                .ReturnsAsync((Message?)null);

            var service = new MessageService(repo.Object);

            var msg = new Message
            {
                OrganizationId = Guid.NewGuid(),
                Id = Guid.NewGuid(),
                Title = "Title",
                Content = "Valid content here"
            };

            var res = await service.UpdateAsync(msg);

            res.Should().BeOfType<NotFoundResult>();

            repo.Verify(r => r.UpdateAsync(It.IsAny<Message>()), Times.Never);
        }

        [Fact]
        public async Task Update_Inactive_Returns_ValidationError()
        {
            var existing = new Message
            {
                Id = Guid.NewGuid(),
                OrganizationId = Guid.NewGuid(),
                Title = "T",
                Content = "Long enough content",
                IsActive = false
            };

            var repo = new Mock<IMessageRepository>();
            repo.Setup(r => r.GetAsync(existing.OrganizationId, existing.Id))
                .ReturnsAsync(existing);

            var service = new MessageService(repo.Object);

            var msg = new Message
            {
                OrganizationId = existing.OrganizationId,
                Id = existing.Id,
                Title = "T2",
                Content = "Valid content here"
            };

            var res = await service.UpdateAsync(msg);

            res.Should().BeOfType<ValidationErrorResult>();

            repo.Verify(r => r.UpdateAsync(It.IsAny<Message>()), Times.Never);
        }

        [Fact]
        public async Task Delete_NonExistent_Returns_NotFound()
        {
            var repo = new Mock<IMessageRepository>();
            repo.Setup(r => r.GetAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>()))
                .ReturnsAsync((Message?)null);

            var service = new MessageService(repo.Object);

            var res = await service.DeleteAsync(Guid.NewGuid(), Guid.NewGuid());

            res.Should().BeOfType<NotFoundResult>();

            repo.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }
    }
}

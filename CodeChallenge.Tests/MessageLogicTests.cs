using System;
using System.Collections.Generic;
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
        public void Create_Should_Create_Message_Successfully()
        {
            var repo = new Mock<IMessageRepository>();
            repo.Setup(r => r.ExistsTitle(It.IsAny<Guid>(), It.IsAny<string>(), null)).Returns(false);
            var service = new MessageService(repo.Object);

            var msg = new Message { OrganizationId = Guid.NewGuid(), Title = "Hello", Content = "This is valid content" };
            var res = service.Create(msg);
            res.Should().BeOfType<CreatedResult<Message>>();
        }

        [Fact]
        public void Create_DuplicateTitle_Returns_Conflict()
        {
            var repo = new Mock<IMessageRepository>();
            repo.Setup(r => r.ExistsTitle(It.IsAny<Guid>(), It.IsAny<string>(), null)).Returns(true);
            var service = new MessageService(repo.Object);

            var msg = new Message { OrganizationId = Guid.NewGuid(), Title = "Hello", Content = "This is valid content" };
            var res = service.Create(msg);
            res.Should().BeOfType<ConflictResult>();
        }

        [Fact]
        public void Create_InvalidContent_Returns_ValidationError()
        {
            var repo = new Mock<IMessageRepository>();
            var service = new MessageService(repo.Object);

            var msg = new Message { OrganizationId = Guid.NewGuid(), Title = "Hi", Content = "short" };
            var res = service.Create(msg);
            res.Should().BeOfType<ValidationErrorResult>();
        }

        [Fact]
        public void Update_NonExistent_Returns_NotFound()
        {
            var repo = new Mock<IMessageRepository>();
            repo.Setup(r => r.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns((Message?)null);
            var service = new MessageService(repo.Object);

            var msg = new Message { OrganizationId = Guid.NewGuid(), Id = Guid.NewGuid(), Title = "Title", Content = "Valid content here" };
            var res = service.Update(msg);
            res.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void Update_Inactive_Returns_ValidationError()
        {
            var existing = new Message { Id = Guid.NewGuid(), OrganizationId = Guid.NewGuid(), Title = "T", Content = "Long enough content", IsActive = false };
            var repo = new Mock<IMessageRepository>();
            repo.Setup(r => r.Get(existing.OrganizationId, existing.Id)).Returns(existing);
            var service = new MessageService(repo.Object);

            var msg = new Message { OrganizationId = existing.OrganizationId, Id = existing.Id, Title = "T2", Content = "Valid content here" };
            var res = service.Update(msg);
            res.Should().BeOfType<ValidationErrorResult>();
        }

        [Fact]
        public void Delete_NonExistent_Returns_NotFound()
        {
            var repo = new Mock<IMessageRepository>();
            repo.Setup(r => r.Get(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns((Message?)null);
            var service = new MessageService(repo.Object);

            var res = service.Delete(Guid.NewGuid(), Guid.NewGuid());
            res.Should().BeOfType<NotFoundResult>();
        }
    }
}

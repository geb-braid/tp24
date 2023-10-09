using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;
using Moq;
using tp24_api.Controllers;
using tp24_api.Models;

namespace tp24_api.Tests.Controllers;

public class ReceivablesControllerTests
{
    private const long RECEIVABLE_ID = 1;
    private const long DIFFERENT_RECEIVABLE_ID = 2;

    public class EmptyDb
    {
        private readonly Mock<IReceivablesRepository> mockRepo;

        private readonly ReceivablesReport report = new(
            Summary: ImmutableDictionary<string, ImmutableDictionary<string, Stats>>.Empty,
            Receivables: Enumerable.Empty<Receivable>());

        public EmptyDb()
        {
            mockRepo = new();
            mockRepo.Setup(repo => repo.Create(It.IsAny<Receivable>())).Returns(Task.CompletedTask);
            mockRepo.Setup(repo => repo.Read(RECEIVABLE_ID).Result).Returns((Receivable?)null);
            mockRepo.Setup(repo => repo.Read(It.IsAny<DateTime>(), It.IsAny<bool>()).Result).Returns(report);
            mockRepo.Setup(repo => repo.Update(It.IsAny<Receivable>()).Result).Returns(false);
            mockRepo.Setup(repo => repo.Delete(RECEIVABLE_ID)).Returns(Task.CompletedTask);
        }

        [Fact]
        public async Task GetReceivables_ReturnsReport()
        {
            var controller = new ReceivablesController(mockRepo.Object);
            var result = await controller.GetReceivables(DateTime.MinValue, summaryOnly: false);
            Assert.Equal(report, result.Value);
        }

        [Fact]
        public async Task GetReceivable_ReturnsNothingFound()
        {
            var controller = new ReceivablesController(mockRepo.Object);
            var result = await controller.GetReceivable(RECEIVABLE_ID);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PutReceivable_NullId_IsOkButNotFound()
        {
            var controller = new ReceivablesController(mockRepo.Object);
            var result = await controller.PutReceivable(RECEIVABLE_ID, new Receivable() { Id = null });
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task PutReceivable_ConflictingId_ReturnsBadRequest()
        {
            var controller = new ReceivablesController(mockRepo.Object);
            var result =
                await controller.PutReceivable(RECEIVABLE_ID, new Receivable { Id = DIFFERENT_RECEIVABLE_ID });
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PostReceivable_ConflictingId_ReturnsBadRequest()
        {
            var controller = new ReceivablesController(mockRepo.Object);
            var result = await controller.PostReceivable(new Receivable { Id = RECEIVABLE_ID });
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task PostReceivable_ReturnsCreatedAtActionResult()
        {
            var receivable = new Receivable();
            var controller = new ReceivablesController(mockRepo.Object);
            var result = await controller.PostReceivable(receivable);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedReceivable = Assert.IsType<Receivable>(createdAtActionResult.Value);
            Assert.Equal(receivable, returnedReceivable);
        }

        [Fact]
        public async Task DeleteReceivable_ReturnsNoContent()
        {
            var controller = new ReceivablesController(mockRepo.Object);
            var result = await controller.DeleteReceivable(RECEIVABLE_ID);
            Assert.IsType<NoContentResult>(result);
        }
    }

    public class SingleItemDb
    {
        private readonly Mock<IReceivablesRepository> mockRepo;

        private readonly Receivable receivable = new();

        public SingleItemDb()
        {
            mockRepo = new();
            mockRepo.Setup(repo => repo.Read(RECEIVABLE_ID).Result).Returns(receivable);
            mockRepo.Setup(repo => repo.Update(It.IsAny<Receivable>()).Result).Returns(true);
        }

        [Fact]
        public async Task GetReceivable_ReturnsReceivable()
        {
            var controller = new ReceivablesController(mockRepo.Object);
            var result = await controller.GetReceivable(RECEIVABLE_ID);
            Assert.Equal(receivable, result.Value);
        }

        [Fact]
        public async Task PutReceivable_NullId_IsOkAndReturnsNoContent()
        {
            var controller = new ReceivablesController(mockRepo.Object);
            var result = await controller.PutReceivable(RECEIVABLE_ID, new Receivable { Id = null });
            Assert.IsType<NoContentResult>(result);
        }
    }
}
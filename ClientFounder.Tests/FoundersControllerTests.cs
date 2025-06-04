using ClientFounder.Data;
using ClientFounder.Models;
using ClientFounder.Models.Dtos;
using ClientFounder.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientFounder.Tests
{
    public class FoundersControllerTests
    {
        private ApplicationDbContext GetContextWithData()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            var client = new Client
            {
                Id = 1,
                INN = 123456789012,
                Name = "Test Client",
                Type = ClientType.LegalEntity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Founders = new List<Founder>()
            };

            var founder = new Founder
            {
                Id = 1,
                INN = 111111111111,
                FullName = "Test Founder",
                ClientId = 1,
                Client = client,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Clients.Add(client);
            context.Founders.Add(founder);
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetById_ReturnsFounder()
        {
            var context = GetContextWithData();
            var controller = new FoundersController(context);

            var result = await controller.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var founder = Assert.IsType<Founder>(ok.Value);
            Assert.Equal(1, founder.Id);
        }

        [Fact]
        public async Task Create_CreatesFounder()
        {
            var context = GetContextWithData();
            var controller = new FoundersController(context);

            var dto = new FounderCreateDto
            {
                INN = 222222222222,
                FullName = "New Founder"
            };

            var result = await controller.Create(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var founder = Assert.IsType<Founder>(created.Value);
            Assert.Equal(dto.INN, founder.INN);
        }

        [Fact]
        public async Task Update_ChangesClientLink()
        {
            var context = GetContextWithData();
            var controller = new FoundersController(context);

            var newClient = new Client
            {
                Id = 2,
                INN = 888888888888,
                Name = "Second",
                Type = ClientType.LegalEntity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Clients.Add(newClient);
            context.SaveChanges();

            var dto = new FounderUpdateDto
            {
                INN = 111111111111,
                FullName = "Updated Name",
                ClientId = 2
            };

            var result = await controller.Update(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            var updated = Assert.IsType<Founder>(ok.Value);
            Assert.Equal(dto.FullName, updated.FullName);
            Assert.Equal(2, updated.ClientId);
        }

        [Fact]
        public async Task Delete_RemovesFounder()
        {
            var context = GetContextWithData();
            var controller = new FoundersController(context);

            var result = await controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Founders);
        }

        [Fact]
        public async Task CreateWithClient_CreatesLinkedFounder()
        {
            var context = GetContextWithData();
            var controller = new FoundersController(context);

            var dto = new FounderWithClientDto
            {
                INN = 333333333333,
                FullName = "Linked Founder",
                ClientId = 1
            };

            var result = await controller.CreateWithClient(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var founder = Assert.IsType<Founder>(created.Value);
            Assert.Equal(1, founder.ClientId);
        }
    }
}

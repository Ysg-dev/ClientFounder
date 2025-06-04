using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientFounder;
using ClientFounder.Data;
using ClientFounder.Models;
using ClientFounder.Models.DTOs;
using ClientFounder.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ClientFounder.Tests
{
    public class ClientsControllerTests
    {
        private ApplicationDbContext GetContextWithData()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            var founder1 = new Founder { Id = 1, INN = 111111111111, FullName = "Founder One", ClientId = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            var founder2 = new Founder { Id = 2, INN = 222222222222, FullName = "Founder Two", ClientId = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            var client = new Client
            {
                Id = 1,
                INN = 123456789012,
                Name = "Test Client",
                Type = ClientType.LegalEntity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Founders = new List<Founder> { founder2 }
            };

            context.Clients.Add(client);
            context.Founders.AddRange(founder1, founder2);
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task GetAll_ReturnsClients()
        {
            var context = GetContextWithData();
            var controller = new ClientsController(context);

            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var clients = Assert.IsAssignableFrom<IEnumerable<Client>>(okResult.Value);
            Assert.Single(clients);
        }

        [Fact]
        public async Task GetById_ExistingClient_ReturnsClient()
        {
            var context = GetContextWithData();
            var controller = new ClientsController(context);

            var result = await controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var client = Assert.IsType<Client>(okResult.Value);
            Assert.Equal(1, client.Id);
        }

        [Fact]
        public async Task GetById_NonExistingClient_ReturnsNotFound()
        {
            var context = GetContextWithData();
            var controller = new ClientsController(context);

            var result = await controller.GetById(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_WhenFreeFounderExists_CreatesClient()
        {
            var context = GetContextWithData();
            var controller = new ClientsController(context);

            var dto = new ClientCreateDto
            {
                INN = 555555555555,
                Name = "New Client",
                Type = ClientType.IndividualEntrepreneur
            };

            var result = await controller.Create(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var client = Assert.IsType<Client>(created.Value);
            Assert.Equal(dto.INN, client.INN);
        }

        [Fact]
        public async Task Create_WhenNoFreeFounder_ReturnsBadRequest()
        {
            var context = GetContextWithData();
            var founder = context.Founders.First(f => f.ClientId == null);
            context.Founders.Remove(founder);
            context.SaveChanges();

            var controller = new ClientsController(context);

            var dto = new ClientCreateDto
            {
                INN = 555555555555,
                Name = "New Client",
                Type = ClientType.IndividualEntrepreneur
            };

            var result = await controller.Create(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Нет свободных учредителей для назначения клиенту.", badRequest.Value);
        }

        [Fact]
        public async Task Update_ExistingClient_UpdatesClient()
        {
            var context = GetContextWithData();
            var controller = new ClientsController(context);

            var dto = new ClientUpdateDto
            {
                INN = 999999999999,
                Name = "Updated",
                Type = ClientType.LegalEntity
            };

            var result = await controller.Update(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            var client = Assert.IsType<Client>(ok.Value);
            Assert.Equal(dto.Name, client.Name);
        }

        [Fact]
        public async Task Delete_RemovesClientAndUnlinksFounders()
        {
            var context = GetContextWithData();
            var controller = new ClientsController(context);

            var result = await controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Clients);
            Assert.All(context.Founders, f => Assert.Null(f.ClientId));
        }

        [Fact]
        public async Task CreateWithFounder_Works_WhenFounderFree()
        {
            var context = GetContextWithData();
            var controller = new ClientsController(context);

            var dto = new ClientWithFounderDto
            {
                INN = 333333333333,
                Name = "Client With Founder",
                Type = ClientType.LegalEntity,
                IdFounder = 1 
            };

            var result = await controller.CreateWithFounder(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var client = Assert.IsType<Client>(created.Value);
            Assert.Equal(dto.INN, client.INN);
        }

        [Fact]
        public async Task CreateWithFounder_Fails_WhenFounderAlreadyHasClient()
        {
            var context = GetContextWithData();
            var controller = new ClientsController(context);

            var dto = new ClientWithFounderDto
            {
                INN = 333333333333,
                Name = "Client With Founder",
                Type = ClientType.LegalEntity,
                IdFounder = 2 
            };

            var result = await controller.CreateWithFounder(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Этот учредитель уже привязан к клиенту.", badRequest.Value);
        }
    }
}

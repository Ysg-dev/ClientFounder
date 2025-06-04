using ClientFounder.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientFounder.Data;

public static class DbInitializer
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Clients.Any() || context.Founders.Any())
            return;

        var now = DateTime.UtcNow;

        var clients = new List<Client>
        {
            new Client
            {
                INN = 123321123321,
                Name = "Камыш",
                Type = ClientType.IndividualEntrepreneur,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Client
            {
                INN = 999999999999,
                Name = "Машины",
                Type = ClientType.IndividualEntrepreneur,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        context.Clients.AddRange(clients);
        context.SaveChanges(); 

        var founders = new List<Founder>
        {
            new Founder
            {
                INN = 123321123321,
                FullName = "Александров А А",
                ClientId = clients[0].Id,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Founder
            {
                INN = 123321123321,
                FullName = "Максимов Алексей Алексеевич",
                ClientId = clients[1].Id,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        context.Founders.AddRange(founders);
        context.SaveChanges();
    }
}

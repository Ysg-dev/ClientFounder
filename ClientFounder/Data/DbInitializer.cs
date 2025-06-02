using ClientFounder.Models;

namespace ClientFounder.Data;

public static class DbInitializer
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Clients.Any()) return;

        var client1 = new Client
        {
            INN = "123456789012",
            Name = "ИП Иванов Иван",
            Type = ClientType.IndividualEntrepreneur,
            Founders = new List<Founder>()
        };

        var client2 = new Client
        {
            INN = "112233445566",
            Name = "ООО Ромашка",
            Type = ClientType.LegalEntity,
            Founders = new List<Founder>
            {
                new Founder { INN = "998877665544", FullName = "Петров Петр Петрович" },
                new Founder { INN = "776655443322", FullName = "Сидоров Сидор Сидорович" }
            }
        };

        context.Clients.AddRange(client1, client2);
        context.SaveChanges();
    }
}

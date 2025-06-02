using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClientFounder.Data;
using ClientFounder.Models;
using ClientFounder.Models.Dtos;
using ClientFounder.Models.DTOs;




namespace ClientFounder;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ClientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clients = await _context.Clients.Include(c => c.Founders).ToListAsync();
        return Ok(clients);
    }
    // метод для просмотра одного конкретного клиента ( по id)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var client = await _context.Clients
            .Include(c => c.Founders)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
            return NotFound();

        return Ok(client);
    }


    [HttpPost]
    public async Task<IActionResult> Create(ClientCreateDto dto)
    {
        var freeFounder = await _context.Founders
            .FirstOrDefaultAsync(f => f.ClientId == null);

        if (freeFounder == null)
            return BadRequest("Нет свободных учредителей для назначения клиенту.");

        var client = new Client
        {
            INN = dto.INN,
            Name = dto.Name,
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow.AddHours(3),
            UpdatedAt = DateTime.UtcNow.AddHours(3),
            Founders = new List<Founder>() // создаём связь
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync(); // получаем client.Id

        freeFounder.ClientId = client.Id;
        freeFounder.UpdatedAt = DateTime.UtcNow.AddHours(3);

        await _context.SaveChangesAsync(); // обновляем founder

        return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
    }





    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ClientUpdateDto updated)

    {
        var client = await _context.Clients
            .Include(c => c.Founders)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
            return NotFound("Клиент не найден.");

        // Обновляем поля
        client.Name = updated.Name;
        client.INN = updated.INN;

        // Если тип меняется
        if (client.Type != updated.Type)
        {
            client.Type = updated.Type;

            // Оставляем только первого учредителя, остальных освобождаем
            var keepFounder = client.Founders.FirstOrDefault();
            var removedFounders = client.Founders
                .Where(f => f != keepFounder)
                .ToList();

            foreach (var founder in removedFounders)
            {
                founder.ClientId = null;
                founder.UpdatedAt = DateTime.UtcNow.AddHours(3);
            }

            client.Founders = keepFounder != null
                ? new List<Founder> { keepFounder }
                : new List<Founder>();
        }

        // Обновляем время обновления
        client.UpdatedAt = DateTime.UtcNow.AddHours(3);

        await _context.SaveChangesAsync();
        return Ok(client);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var client = await _context.Clients
            .Include(c => c.Founders)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
            return NotFound();

        // Удаляем связь с учредителями (а не их самих)
        foreach (var founder in client.Founders)
        {
            founder.ClientId = null;
            founder.UpdatedAt = DateTime.UtcNow.AddHours(3);
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("with-founder")]
    public async Task<IActionResult> CreateWithFounder([FromBody] ClientWithFounderDto dto)
    {
        var founder = await _context.Founders
            .Include(f => f.Client)
            .FirstOrDefaultAsync(f => f.Id == dto.IdFounder);

        if (founder == null)
            return NotFound("Учредитель не найден.");

        if (founder.ClientId != 0 && founder.Client != null)
            return BadRequest("Этот учредитель уже привязан к клиенту.");

        // Создаем нового клиента
        var client = new Client
        {
            INN = dto.INN,
            Name = dto.Name,
            Type = dto.Type,
            CreatedAt = DateTime.UtcNow.AddHours(3),
            UpdatedAt = DateTime.UtcNow.AddHours(3),
            Founders = new List<Founder> { founder }
        };

        // Привязываем клиента к учредителю
        founder.Client = client;
        founder.UpdatedAt = DateTime.UtcNow.AddHours(3);

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
    }

}
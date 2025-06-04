using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClientFounder.Data;
using ClientFounder.Models;
using ClientFounder.Models.Dtos;
using ClientFounder.Models.DTOs;


namespace ClientFounder;

[ApiController]
[Route("api/[controller]")]
public class FoundersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FoundersController(ApplicationDbContext context)
    {
        _context = context;
    }

    
    [HttpPost]
    public async Task<IActionResult> Create(FounderCreateDto dto)
    {
        var founder = new Founder
        {
            INN = dto.INN,
            FullName = dto.FullName,
            CreatedAt = DateTime.UtcNow.AddHours(3),
            UpdatedAt = DateTime.UtcNow.AddHours(3)
        };

        _context.Founders.Add(founder);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = founder.Id }, founder);
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var founder = await _context.Founders
            .Include(f => f.Client)
            .FirstOrDefaultAsync(f => f.Id == id);

        return founder == null ? NotFound() : Ok(founder);
    }


    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        var founder = await _context.Founders.Include(c => c.Client).ToListAsync();
        return Ok(founder);
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] FounderUpdateDto dto)
    {
        var founder = await _context.Founders.Include(f => f.Client).FirstOrDefaultAsync(f => f.Id == id);
        if (founder == null)
            return NotFound("Учредитель не найден.");

      
        founder.INN = dto.INN;
        founder.FullName = dto.FullName;

        
        if (dto.ClientId != null && dto.ClientId != founder.ClientId)
        {
            var newClient = await _context.Clients
                .Include(c => c.Founders)
                .FirstOrDefaultAsync(c => c.Id == dto.ClientId);

            if (newClient == null)
                return BadRequest("Указанный клиент не найден.");

            
            if (newClient.Type == ClientType.IndividualEntrepreneur && newClient.Founders.Any())
            {
               
                var existingFounder = newClient.Founders.First();
                existingFounder.ClientId = null;
                existingFounder.UpdatedAt = DateTime.UtcNow.AddHours(3);
            }

          
            if (founder.ClientId != null && founder.ClientId != newClient.Id)
            {
                var oldClient = await _context.Clients.Include(c => c.Founders)
                    .FirstOrDefaultAsync(c => c.Id == founder.ClientId);

                if (oldClient != null)
                {
                    oldClient.Founders.Remove(founder);
                }
            }

            
            founder.ClientId = newClient.Id;
        }

        
        founder.UpdatedAt = DateTime.UtcNow.AddHours(3);

        await _context.SaveChangesAsync();
        return Ok(founder);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var founder = await _context.Founders
            .Include(f => f.Client)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (founder == null)
            return NotFound("Учредитель не найден.");

        
        if (founder.Client != null)
        {
            founder.Client = null;
        }

        _context.Founders.Remove(founder);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("with-client")]
    public async Task<IActionResult> CreateWithClient([FromBody] FounderWithClientDto dto)
    {
        var client = await _context.Clients
            .Include(c => c.Founders)
            .FirstOrDefaultAsync(c => c.Id == dto.ClientId);

        if (client == null)
            return NotFound("Клиент не найден.");

       
        if (client.Type == ClientType.IndividualEntrepreneur && client.Founders.Any())
        {
            return BadRequest("ИП может иметь только одного учредителя.");
        }

        var founder = new Founder
        {
            INN = dto.INN,
            FullName = dto.FullName,
            Client = client,
            CreatedAt = DateTime.UtcNow.AddHours(3),
            UpdatedAt = DateTime.UtcNow.AddHours(3)
        };

        _context.Founders.Add(founder);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = founder.Id }, founder);
    }



}

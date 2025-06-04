namespace ClientFounder.Models.DTOs;

public class ClientWithFounderDto
{
    public long INN { get; set; }
    public string Name { get; set; }
    public ClientType Type { get; set; }
    public int IdFounder { get; set; }
}

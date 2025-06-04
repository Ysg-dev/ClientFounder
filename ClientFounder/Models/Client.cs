using System.ComponentModel.DataAnnotations;

namespace ClientFounder.Models;

public enum ClientType
{
    IndividualEntrepreneur = 1,
    LegalEntity = 2
}

public class Client : BaseEntity
{
    public int Id { get; set; }

    [Required]
    public long INN { get; set; }

    [Required, StringLength(256)]
    public string Name { get; set; }

    [Required]
    public ClientType Type { get; set; }

    public List<Founder> Founders { get; set; } = new();
}
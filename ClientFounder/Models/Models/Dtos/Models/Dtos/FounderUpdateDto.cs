using System.ComponentModel.DataAnnotations;

namespace ClientFounder.Models.Dtos;

public class FounderUpdateDto
{
    [Required]
    [StringLength(12)]
    public long INN { get; set; }

    [Required]
    [StringLength(256)]
    public string FullName { get; set; }

    public int? ClientId { get; set; } // может быть null
}

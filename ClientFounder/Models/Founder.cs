using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ClientFounder.Models;

public class Founder : BaseEntity
{
    public int Id { get; set; }

    [Required, StringLength(12)]
    public long INN { get; set; }

    [Required, StringLength(256)]
    public string FullName { get; set; }

    public int? ClientId { get; set; }
    [JsonIgnore]
    public Client? Client { get; set; }


    
}
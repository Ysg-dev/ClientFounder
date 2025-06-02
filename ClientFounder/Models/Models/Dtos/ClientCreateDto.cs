using System.ComponentModel.DataAnnotations;
using ClientFounder.Models;

namespace ClientFounder.Models.Dtos
{
    public class ClientCreateDto
    {
        [Required, StringLength(12)]
        public string INN { get; set; }

        [Required, StringLength(256)]
        public string Name { get; set; }

        [Required]
        public ClientType Type { get; set; }
    }
}

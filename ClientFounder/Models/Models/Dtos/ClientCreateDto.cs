﻿using System.ComponentModel.DataAnnotations;

namespace ClientFounder.Models.Dtos
{
    public class ClientCreateDto
    {
        [Required]
        public long INN { get; set; }

        [Required, StringLength(256)]
        public string Name { get; set; }

        [Required]
        public ClientType Type { get; set; }
    }
}

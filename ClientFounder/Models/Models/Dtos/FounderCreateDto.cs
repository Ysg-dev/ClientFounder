﻿using System.ComponentModel.DataAnnotations;

namespace ClientFounder.Models.Dtos;

public class FounderCreateDto
{
    [Required]
    public long INN { get; set; }

    [Required, StringLength(256)]
    public string FullName { get; set; }
}

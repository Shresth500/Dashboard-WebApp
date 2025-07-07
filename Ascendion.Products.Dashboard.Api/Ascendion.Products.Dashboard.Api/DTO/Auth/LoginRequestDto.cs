﻿using System.ComponentModel.DataAnnotations;

namespace Ascendion.Products.Dashboard.DTO.Auth;

public class LoginRequestDto
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public required string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}

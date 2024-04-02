﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YandexTrackerApi.DbModels;

[Table("User")]
public partial class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; }

    [Required]
    [StringLength(255)]
    public string Login { get; set; }

    [Required]
    [StringLength(255)]
    public string Password { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<UsersProject> UsersProjects { get; set; } = new List<UsersProject>();
}
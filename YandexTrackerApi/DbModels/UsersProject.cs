﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YandexTrackerApi.DbModels;

[PrimaryKey("UserId", "ProjectId")]
public partial class UsersProject
{
    [Key]
    [Column("User_Id")]
    public Guid UserId { get; set; }

    [Key]
    [Column("Project_Id")]
    public Guid ProjectId { get; set; }

    public bool? Condirmed { get; set; }

    [Column("Invite_Code")]
    public int? InviteCode { get; set; }

    [ForeignKey("ProjectId")]
    [InverseProperty("UsersProjects")]
    public virtual Project Project { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UsersProjects")]
    public virtual User User { get; set; }
}
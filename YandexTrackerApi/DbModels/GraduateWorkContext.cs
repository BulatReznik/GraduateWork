﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace YandexTrackerApi.DbModels;

public partial class GraduateWorkContext : DbContext
{
    public GraduateWorkContext(DbContextOptions<GraduateWorkContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CalendarDatum> CalendarData { get; set; }

    public virtual DbSet<Diagram> Diagrams { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<TaskHandlerMapping> TaskHandlerMappings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersProject> UsersProjects { get; set; }

    public virtual DbSet<YandexTracker> YandexTrackers { get; set; }

    public virtual DbSet<YandexTrackerTask> YandexTrackerTasks { get; set; }

    public virtual DbSet<YandexTrackerUser> YandexTrackerUsers { get; set; }

    public virtual DbSet<YandexTrackerUserHoliday> YandexTrackerUserHolidays { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CalendarDatum>(entity =>
        {
            entity.HasKey(e => e.Day).HasName("PK_CalendarData_Day");
        });

        modelBuilder.Entity<Diagram>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Diagram_Id");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Project).WithMany(p => p.Diagrams).HasConstraintName("FK_Diagram_Project_Id");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Project_Id");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_User_Id");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<UsersProject>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ProjectId }).HasName("PK_User_Id_Project_Id");

            entity.HasOne(d => d.Project).WithMany(p => p.UsersProjects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsersProjects_Project_Id");

            entity.HasOne(d => d.User).WithMany(p => p.UsersProjects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsersProjects_User_Id");
        });

        modelBuilder.Entity<YandexTracker>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_YandexTracker_Id");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.YandexTracker).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<YandexTrackerTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_YandexTrackerTask_Id");

            entity.HasOne(d => d.User).WithMany(p => p.YandexTrackerTasks).HasConstraintName("FK_YandexTrackerTask");
        });

        modelBuilder.Entity<YandexTrackerUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_YandexTrackerUser_Id");

            entity.HasOne(d => d.Project).WithMany(p => p.YandexTrackerUsers).HasConstraintName("FK_YandexTrackerUser_Project_Id");
        });

        modelBuilder.Entity<YandexTrackerUserHoliday>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_YandexTrackerUserHoliday_Id");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithMany(p => p.YandexTrackerUserHolidays).HasConstraintName("FK_YandexTrackerUserHoliday");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
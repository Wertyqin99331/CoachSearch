﻿using CoachSearch.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoachSearch.Data;

public sealed class ApplicationDbContext
	: IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
{
	public ApplicationDbContext()
	{
	}
	
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) { }
	
	public DbSet<Customer> Customers { get; set; } = null!;
	public DbSet<Trainer> Trainers { get; set; } = null!;
	public DbSet<TrainingProgram> TrainingPrograms { get; set; } = null!;
	public DbSet<Review> Reviews { get; set; } = null!;
	public DbSet<Like> Likes { get; set; } = null!;
}
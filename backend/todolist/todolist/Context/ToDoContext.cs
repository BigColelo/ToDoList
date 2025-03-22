using Microsoft.EntityFrameworkCore;
using todolist.Models;
using Microsoft.Extensions.Configuration;

namespace todolist.Context;

public class ToDoContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DbSet<User> Users { get; set; }
    public DbSet<Activity> Activities { get; set; }

    public ToDoContext(DbContextOptions<ToDoContext> options, IConfiguration configuration) 
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // This is only used when the context is created without passing options
            // For example, during design-time tools like migrations
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Relazione User → Activities (1-N)
        modelBuilder.Entity<Activity>()
            .HasOne(a => a.User)
            .WithMany(u => u.Activities)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
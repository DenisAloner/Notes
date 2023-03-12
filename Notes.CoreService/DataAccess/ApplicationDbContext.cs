using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Notes.CoreService.DataAccess.Entities;

namespace Notes.CoreService.DataAccess;

public class ApplicationDbContext : DbContext
{
    public DbSet<Note> Notes => Set<Note>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(Constants.Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
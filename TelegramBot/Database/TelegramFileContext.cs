using Microsoft.EntityFrameworkCore;
using TelegramBot.Configurations;
using TelegramBot.Entities;

namespace TelegramBot.Database;

public class TelegramFileContext : DbContext
{
    public DbSet<TelegramFile> Files { get; set; }
    
    private readonly string _connectionString;

    public TelegramFileContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TelegramFileConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }
}
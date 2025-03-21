using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelegramBot.Database;
using TelegramBot.Entities;

namespace TelegramBot.Configurations;

public class TelegramFileConfiguration : IEntityTypeConfiguration<TelegramFile>
{
    public void Configure(EntityTypeBuilder<TelegramFile> builder)
    {
        builder.HasKey(e => e.Id);
    }
}
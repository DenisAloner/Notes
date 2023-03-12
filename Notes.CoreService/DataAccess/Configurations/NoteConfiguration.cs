using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notes.CoreService.DataAccess.Entities;

namespace Notes.CoreService.DataAccess.Configurations;

public class NoteConfiguration:IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder
            .HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .HasDefaultValueSql("gen_random_uuid()");

        builder
            .Property(x => x.Title)
            .HasMaxLength(256);

        builder
            .Property(x => x.Title)
            .HasMaxLength(2048);
    }
}
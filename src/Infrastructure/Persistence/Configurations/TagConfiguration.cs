using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo_App.Domain.Entities;

namespace Todo_App.Infrastructure.Persistence.Configurations;
public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.Property(t => t.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .HasMany(t => t.TodoItemTags)
            .WithOne(tt => tt.Tag)
            .HasForeignKey(tt => tt.TagId)
            .IsRequired();
    }
}

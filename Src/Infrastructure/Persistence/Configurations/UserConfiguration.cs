using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User", schema: "Authentication");

            builder.Property(e => e.Id).HasColumnName("Id");

            builder.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasQueryFilter(f => f.Active == true);
        }
    }
}

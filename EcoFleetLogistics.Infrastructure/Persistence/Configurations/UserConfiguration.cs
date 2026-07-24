using EcoFleetLogistics.Domain.User;
using EcoFleetLogistics.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoFleetLogistics.Infrastructure.Persistence.Configurations;

public class UserCongiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value))
            .IsRequired()
            .HasMaxLength(256);
        
        builder.HasIndex(u => u.Email)
            .IsUnique();
        
        builder.Property(u => u.PasswordHash)
            .IsRequired();
        
        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>();
    }
}
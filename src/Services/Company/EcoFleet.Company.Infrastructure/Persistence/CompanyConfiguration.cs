using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoFleet.Company.Infrastructure.Persistence.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Domain.Companies.Company>
    {
        public void Configure(EntityTypeBuilder<Domain.Companies.Company> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.TaxNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(c => c.TaxNumber)
                .IsUnique();
       /*     
            builder.HasMany(c => c.Users)
                .WithOne(u => u.Company)
                .HasForeignKey(u => u.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

         */   
        }
    }
}
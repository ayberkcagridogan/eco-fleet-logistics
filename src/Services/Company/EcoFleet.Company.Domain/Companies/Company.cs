

using EcoFleet.Shared.Kernel.Primitives;

namespace EcoFleet.Company.Domain.Companies
{
    public class Company : BaseEntity<Guid>
    {
        public string Name { get; private set; } = null!;
        public string TaxNumber { get; private set; } = null!;
        public string AdminEmail { get; private set; } = null!;
        public string Domain { get; private set; } = null!;
        public bool IsActive { get; private set; }
    
      //  private readonly List<User> _users = new();
      //  public IReadOnlyCollection<User> Users => _users.AsReadOnly();

        private Company() {}

        private Company(Guid id, string name, string taxNumer, string adminEmail, string domain, bool isActive)
        {
            Id = id;
            Name = name;
            TaxNumber = taxNumer;
            AdminEmail = adminEmail;
            Domain = domain;
            IsActive = isActive;
        }

        public static Company Create(string name, string taxNumber, string adminEmail, string domain)
        {
            if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Company name cannot be empty.", nameof(name));

            if (string.IsNullOrWhiteSpace(taxNumber))
                throw new ArgumentException("Tax number cannot be empty.", nameof(taxNumber));

            if (string.IsNullOrWhiteSpace(adminEmail))
                throw new ArgumentException("Admin Email cannot be empty.", nameof(adminEmail));

            if (string.IsNullOrWhiteSpace(domain))
                throw new ArgumentException("Domain cannot be empty.", nameof(domain));

            return new Company
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                TaxNumber = taxNumber.Trim(),
                AdminEmail = adminEmail,
                Domain = domain.ToLower().Trim(),
                IsActive = true,
            };
        }

        public void UpdateStatus(bool isActive)
        {
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
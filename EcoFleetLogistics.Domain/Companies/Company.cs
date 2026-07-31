using EcoFleetLogistics.Domain.Common;
using EcoFleetLogistics.Domain.Users;

namespace EcoFleetLogistics.Domain.Companies
{
    public class Company : BaseEntity
    {
        public string Name { get; private set; }
        public string TaxNumber { get; private set; }
        public bool IsActive { get; private set; }
    
        private readonly List<User> _users = new();
        public IReadOnlyCollection<User> Users => _users.AsReadOnly();

        private Company()
        {
            Name = null!;
            TaxNumber = null!;
        }

        private Company(Guid id, string name, string taxNumer, bool isActive)
        {
            Id = id;
            Name = name;
            TaxNumber = taxNumer;
            IsActive = isActive;
        }

        public static Company Create(string name, string taxNumber)
        {
            if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Company name cannot be empty.", nameof(name));

            if (string.IsNullOrWhiteSpace(taxNumber))
                throw new ArgumentException("Tax number cannot be empty.", nameof(taxNumber));

            return new Company
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                TaxNumber = taxNumber.Trim(),
                IsActive = true,
            };
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
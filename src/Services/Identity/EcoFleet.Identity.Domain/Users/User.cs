using EcoFleet.Identity.Domain.Users.Enums;
using EcoFleet.Identity.Domain.ValueObjects;
using EcoFleet.Shared.Kernel.Primitives;
using EcoFleet.Shared.Kernel.Primitives.Interfaces;

namespace EcoFleet.Identity.Domain.Users;
public class User : BaseEntity<Guid>, IMultiTenant
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string PasswordHash {get; private set;} = null!;
    public UserRole Role { get; private set; }
    public Guid TenantId { get; private set; }
   // public Company Company { get; private set; } = null!;
  

    private User() {}// Parameterless constructor for EF Core
    
    
    private User(Guid id, string firstName, string lastName, Email email, string passwordHash, UserRole role, Guid tenantId)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        TenantId = tenantId;
    }

    public static User Create(string firstName, string lastName, string email, string passwordHash, Guid tenantId, UserRole role = UserRole.User)
    {
         if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First Name cannot be empty.", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("E-mail name cannot be empty.", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password cannot be empty.", nameof(passwordHash));
            
        var emeilValueObject = Email.Create(email);
            
        return new User(Guid.NewGuid(), firstName, lastName, emeilValueObject, passwordHash, role, tenantId);
    }

    public static UserRole ResolveRole(string? role)
    {
        if(string.IsNullOrWhiteSpace(role))
           return UserRole.User;
        
        if(!Enum.TryParse<UserRole>(role, ignoreCase: true, out var parsedRole))
            throw new ArgumentException("Invalid role specified. Valid roles are: User, Admin vs.", nameof(role));
        
        return parsedRole;
    }
}
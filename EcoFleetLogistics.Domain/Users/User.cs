using EcoFleetLogistics.Domain.Companies;
using EcoFleetLogistics.Domain.Users.Enums;
using EcoFleetLogistics.Domain.ValueObjects;

namespace EcoFleetLogistics.Domain.Users;
public class User
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash {get; private set;}
    public UserRole Role { get; private set; }
    public DateTime CreateAt { get; private set; }

    public Guid CompanyId { get; private set; }
    public Company Company { get; private set; }
  

    private User() // Parameterless constructor for EF Core
    {
        FirstName = null!;
        LastName = null!;
        Email = null!;
        PasswordHash = null!;
        Company = null!;
    }
    
    private User(Guid id, string firstName, string lastName, Email email, string passwordHash, UserRole role, Guid companyId)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CompanyId = companyId;
        CreateAt = DateTime.UtcNow;
    }

    public static User Create(string firstName, string lastName, string email, string passwordHash, Guid companyId, UserRole role = UserRole.Customer)
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
            
        return new User(Guid.NewGuid(), firstName, lastName, emeilValueObject, passwordHash, role, companyId);
    }

    public static UserRole ResolveRole(string? role)
    {
        if(string.IsNullOrWhiteSpace(role))
           return UserRole.Customer;
        
        if(!Enum.TryParse<UserRole>(role, ignoreCase: true, out var parsedRole))
            throw new ArgumentException("Invalid role specified. Valid roles are: User, Admin vs.", nameof(role));
        
        return parsedRole;
    }
}
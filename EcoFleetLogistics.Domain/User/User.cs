using System.Collections;
using EcoFleetLogistics.Domain.User.Enums;
using EcoFleetLogistics.Domain.ValueObjects;

namespace EcoFleetLogistics.Domain.User;
public class User
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash {get; private set;}
    public UserRole Role { get; private set; }
    public DateTime CreateAt { get; private set; }
    private User() // Parameterless constructor for EF Core
    {
        FirstName = null!;
        LastName = null!;
        Email = null!;
        PasswordHash = null!;
    }
    
    private User(Guid id, string firstName, string lastName, Email email, string passwordHash, UserRole role)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreateAt = DateTime.UtcNow;
    }

    public static User Create(string firstName, string lastName, string email, string passwordHash, UserRole role = UserRole.User)
    {
         if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First Name cannot be empty.");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("E-mail name cannot be empty.");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password cannot be empty.");
            
        var emeilValueObject = Email.Create(email);
            
        return new User(Guid.NewGuid(), firstName, lastName, emeilValueObject, passwordHash, role);
    }

    public static UserRole ResolveRole(string? role)
    {
        if(string.IsNullOrWhiteSpace(role))
           return UserRole.User;
        
        if(!Enum.TryParse<UserRole>(role, ignoreCase: true, out var parsedRole))
            throw new ArgumentException("Invalid role specified. Valid roles are: User, Admin vs.");
        
        return parsedRole;
    }
}
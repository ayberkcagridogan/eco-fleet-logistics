using System.Collections;
using EcoFleetLogistics.Domain.User.Enums;

namespace EcoFleetLogistics.Domain.User;
public class User
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
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
    
    private User(string firstName, string lastName, string email, string passwordHash, UserRole role = UserRole.User)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreateAt = DateTime.UtcNow;
    }    
}
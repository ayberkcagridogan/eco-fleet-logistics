using System.Reflection.Metadata;

namespace EcoFleet.Identity.Application.Common.Authentication.Interfaces;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
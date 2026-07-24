using System.Reflection.Metadata;

namespace EcoFleetLogistics.Application.Common.Interfaces;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
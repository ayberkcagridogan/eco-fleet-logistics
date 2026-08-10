using EcoFleet.Identity.Domain.Users;
using EcoFleet.Shared.Kernel.Primitives;

namespace EcoFleet.Identity.Domain.Authentication;

public class RefreshToken : BaseEntity<Guid>
{
    public string Token { get; private set; }  = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken(){}
    private RefreshToken(Guid id, string token, DateTime expiresAt, DateTime createdAt, Guid userId)
    {
        Id = id;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
        UserId = userId;
    }

    public static RefreshToken Create(string token, DateTime expiresAt, Guid userId)
    {
        if(string.IsNullOrWhiteSpace(token))
              throw new ArgumentException("Token cannot be empty.", nameof(token));
        
        if(expiresAt <= DateTime.UtcNow)
           throw new ArgumentException("Expiration date must be in the future.", nameof(expiresAt));

        return new RefreshToken(
            Guid.NewGuid(),
            token,
            expiresAt,
            DateTime.UtcNow,
            userId);
    }

    public void Revoke()
    {
        if(IsRevoked) return;

        RevokedAt = DateTime.UtcNow;
    }
}
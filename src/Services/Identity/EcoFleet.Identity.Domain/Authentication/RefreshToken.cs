using EcoFleet.Identity.Domain.Users;
using EcoFleet.Shared.Kernel.Primitives;

namespace EcoFleet.Identity.Domain.Authentication;

public class RefreshToken : BaseEntity<Guid>
{
    public string Token { get; private set; }  = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsUsed => UsedAt != null;
    public bool IsActive => !IsDeleted && !IsExpired && !IsUsed;

    private RefreshToken(){}
    private RefreshToken(Guid id, string token, DateTime expiresAt, Guid userId)
    {
        Id = id;
        Token = token;
        ExpiresAt = expiresAt;
        UserId = userId;
        CreatedById = userId;
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
            userId);
    }

    public void MarkAsDeleted(Guid deletedById)
    {
        DeletedById = deletedById;
    }
    public void MarkAsUsed(Guid? updatedById = null)
    {
        if (IsUsed) return;

        UsedAt = DateTime.UtcNow;
        if (updatedById.HasValue && updatedById.Value != Guid.Empty)
        {
            UpdatedById = updatedById.Value;
        }
    }
}
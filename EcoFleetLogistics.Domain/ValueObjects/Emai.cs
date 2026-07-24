using System.Text.RegularExpressions;

namespace EcoFleetLogistics.Domain.ValueObjects;

public class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    public string Value {get;}

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException("Email address cannot be empty.");

        var formattedEmail = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(formattedEmail))
            throw new InvalidOperationException("Invalid email address format.");

        return new Email(formattedEmail);
    }

    public bool Equals(Email? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is Email other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}
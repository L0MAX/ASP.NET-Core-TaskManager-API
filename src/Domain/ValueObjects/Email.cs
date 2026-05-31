using System.Text.RegularExpressions;
using TaskManager.Domain.Common;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.ValueObjects;

public sealed partial class Email : ValueObject
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");

        var normalized = email.Trim().ToLowerInvariant();

        if (normalized.Length > 256)
            throw new DomainException("Email must not exceed 256 characters.");

        if (!EmailRegex().IsMatch(normalized))
            throw new DomainException("Email format is invalid.");

        return new Email(normalized);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
}

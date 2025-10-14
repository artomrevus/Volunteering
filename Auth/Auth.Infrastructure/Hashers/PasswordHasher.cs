using System.Security.Cryptography;
using System.Text;
using Auth.Application.Interfaces.Hashers;
using Auth.Domain.Entities;

namespace Auth.Infrastructure.Hashers;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var builder = new StringBuilder();

        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }
}
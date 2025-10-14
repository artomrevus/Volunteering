using Auth.Domain.Constants;
using Auth.Domain.Exceptions;

namespace Auth.Domain.Entities;

public class UserEntity
{
    public string Id { get; private set; }
    
    public string Username { get; private set; }
    
    public string PasswordHash { get; private set; }
    
    public string Role { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public UserEntity(
        string username,
        string passwordHash,
        string role)
    {
        if (!UserRoles.Exists(role))
        {
            throw new UserException($"User role '{role}' does not exist.");
        }
        
        Id = Guid.NewGuid().ToString();
        Username = username;
        PasswordHash = passwordHash;
        Role = UserRoles.Normalize(role);
        CreatedAt = DateTime.UtcNow;
    }
    
    public void UpdatePassword(string newPasswordHash)
    {
        if (PasswordHash == newPasswordHash)
        {
            throw new UserException("Password and new password should not be the same.");
        }
        
        PasswordHash = newPasswordHash;
    }
}
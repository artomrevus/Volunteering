namespace Auth.Application.Interfaces.Hashers;

public interface IPasswordHasher
{
    string Hash(string input);
}
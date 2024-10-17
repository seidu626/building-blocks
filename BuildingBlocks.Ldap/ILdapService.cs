using BuildingBlocks.Exceptions;
using CSharpFunctionalExtensions;

namespace BuildingBlocks.Ldap;

public interface ILdapService<TUser> where TUser : IAppUser, new()
{
    Task<Result<TUser, Error>> Login(string username, string password);

    Task<Result<TUser, Error>> Login(string username, string password, string? domain);

    Task<Result<TUser, Error>> FindUser(string username);

    Task<Result<TUser, Error>> FindUser(string username, string domain);

    Task<Result<TUser, Error>> FindUserByEmail(string email);
    Task<Result<TUser, Error>> FindUserByEmail(string email, string? domain);

    Task<Result<TUser, Error>> FindUserByPhone(string phone);
    Task<Result<TUser, Error>> FindUserByPhone(string phone, string? domain);

    Task<Result<List<string>, Error>> GetUserAttributes(string attributeName, string attributeValue, string domain);
}
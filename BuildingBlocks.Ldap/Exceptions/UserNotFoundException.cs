// Decompiled with JetBrains decompiler
// Type: Skoruba.Duende.IdentityServer.STS.Identity.Infrastructure.LdapExtension.Exceptions.UserNotFoundException
// Assembly: Skoruba.Duende.IdentityServer.STS.Identity, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 425C0317-D64B-453B-BC95-043C64DD9F8A
// Assembly location: C:\Users\420919\Repositories\STS\STS\Skoruba.Duende.IdentityServer.STS.Identity.dll

#nullable disable
namespace BuildingBlocks.Ldap.Exceptions;

internal class UserNotFoundException : Exception
{
  public UserNotFoundException(string message)
    : base(message)
  {
  }

  public UserNotFoundException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}
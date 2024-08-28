// Decompiled with JetBrains decompiler
// Type: Skoruba.Duende.IdentityServer.STS.Identity.Infrastructure.LdapExtension.DirectoryServices.ILdapConnection
// Assembly: Skoruba.Duende.IdentityServer.STS.Identity, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 425C0317-D64B-453B-BC95-043C64DD9F8A
// Assembly location: C:\Users\420919\Repositories\STS\STS\Skoruba.Duende.IdentityServer.STS.Identity.dll

#nullable disable
using Novell.Directory.Ldap;

namespace BuildingBlocks.Ldap.DirectoryServices;

internal interface ILdapConnection : Novell.Directory.Ldap.ILdapConnection, IDisposable, IDebugIdentifier
{
  new bool Bound { get; }
}
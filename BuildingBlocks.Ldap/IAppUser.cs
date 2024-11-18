using System.Security.Claims;
using Novell.Directory.Ldap;

namespace BuildingBlocks.Ldap
{
    public interface IAppUser
    {
        string? SubjectId { get; set; }
        string? Username { get; set; }
        string Fullname { get; set; }
        string? Email { get; set; }
        string Mobile { get; set; }
        string? ProviderSubjectId { get; set; }
        string? ProviderName { get; set; }
        bool IsActive { get; }
        string DisplayName { get; set; }
        ICollection<Claim> Claims { get; set; }
        string[] LdapAttributes { get; }
        void FillClaims(LdapEntry ldapEntry, LdapEntry? manager = null);
        void SetBaseDetails(LdapEntry ldapEntry,  LdapEntry? manager = null, string? providerName = "");
    }
}
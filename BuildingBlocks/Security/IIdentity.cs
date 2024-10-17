using System.Runtime.Serialization;

#nullable disable
namespace BuildingBlocks.Security
{
    public interface IIdentity
    {
        [DataMember] string Username { get; set; }

        [DataMember] string Email { get; set; }
    }
}
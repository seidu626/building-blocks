using System.Runtime.Serialization;

namespace BuildingBlocks.Data;

/// <summary>
/// Represents data provider type enumeration
/// </summary>
public enum DataProviderType
{
    /// <summary>
    /// Unknown
    /// </summary>
    [EnumMember(Value = "")] Unknown,

    /// <summary>
    /// MS SQL Server
    /// </summary>
    [EnumMember(Value = "sqlserver")] SqlServer,

    [EnumMember(Value = "inmemory")] InMemory,

    [EnumMember(Value = "sqlite")] Sqlite,

    /// <summary>
    /// MySQL
    /// </summary>
    [EnumMember(Value = "mysql")] MySql,

    /// <summary>
    /// PostgreSQL
    /// </summary>
    [EnumMember(Value = "postgresql")] PostgreSQL
}
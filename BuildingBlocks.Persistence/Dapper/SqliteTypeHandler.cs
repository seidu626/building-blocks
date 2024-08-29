using Dapper;
using System.Data;

namespace BuildingBlocks.Persistence.Dapper
{
    /// <summary>
    /// A base class for SQLite type handlers in Dapper.
    /// </summary>
    /// <typeparam name="T">The type this handler will manage.</typeparam>
    public abstract class SqliteTypeHandler<T> : SqlMapper.TypeHandler<T>
    {
        /// <summary>
        /// Sets the value of a database parameter.
        /// </summary>
        /// <param name="parameter">The database parameter.</param>
        /// <param name="value">The value to be set.</param>
        public override void SetValue(IDbDataParameter parameter, T value)
        {
            parameter.Value = (object?)value ?? DBNull.Value; // Handle null values correctly.
        }
    }
}
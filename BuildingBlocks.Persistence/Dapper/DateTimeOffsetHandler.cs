using System;
using Dapper;

namespace BuildingBlocks.Persistence.Dapper
{
    /// <summary>
    /// A custom Dapper type handler for handling DateTimeOffset with SQLite.
    /// </summary>
    public class DateTimeOffsetHandler : SqliteTypeHandler<DateTimeOffset>
    {
        /// <summary>
        /// Parses a string value from the database into a DateTimeOffset object.
        /// </summary>
        /// <param name="value">The value from the database.</param>
        /// <returns>A DateTimeOffset parsed from the string value.</returns>
        public override DateTimeOffset Parse(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Value cannot be null");
            }

            if (value is string stringValue)
            {
                return DateTimeOffset.Parse(stringValue);
            }

            throw new ArgumentException("Invalid value type. Expected a string.", nameof(value));
        }
    }
}
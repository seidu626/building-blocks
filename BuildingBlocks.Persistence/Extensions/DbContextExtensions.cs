using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence.Extensions
{
    public static class DbContextExtensions
    {
        public static string GetTableName<T>(DbContext context) where T : class
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            return entityType.GetTableName();
        }

        public static bool ColumnExists(this DbContext context, string tableName, string columnName)
        {
            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(columnName))
                return false;

            var sql = @"SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = @tableName AND column_name = @columnName";
            return context.Database.ExecuteSqlRaw(sql, new SqlParameter("@tableName", tableName), new SqlParameter("@columnName", columnName)) > 0;
        }

        public static void ColumnEnsure(this DbContext context, string tableName, string columnName, string columnDataType)
        {
            if (!context.ColumnExists(tableName, columnName))
            {
                var sql = $"ALTER TABLE {tableName} ADD {columnName} {columnDataType}";
                context.Database.ExecuteSqlRaw(sql);
            }
        }

        public static void ColumnDelete(this DbContext context, string tableName, string columnName)
        {
            if (context.ColumnExists(tableName, columnName))
            {
                var sql = $"ALTER TABLE {tableName} DROP COLUMN {columnName}";
                context.Database.ExecuteSqlRaw(sql);
            }
        }

        public static bool TableExists(this DbContext context, string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
                return false;

            var sql = @"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE table_name = @tableName";
            return context.Database.ExecuteSqlRaw(sql, new SqlParameter("@tableName", tableName)) > 0;
        }

        public static bool DatabaseExists(this DbContext context, string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName))
                return false;

            var sql = @"SELECT database_id FROM sys.databases WHERE Name = @databaseName";
            return context.Database.ExecuteSqlRaw(sql, new SqlParameter("@databaseName", databaseName)) > 0;
        }

        public static int InsertInto(this DbContext context, string sql, params object[] parameters)
        {
            sql += "; SELECT CAST(SCOPE_IDENTITY() as int);";
            return context.Database.ExecuteSqlRaw(sql, parameters);
        }

        public static int Execute(this DbContext context, string sql, params object[] parameters)
        {
            return context.Database.ExecuteSqlRaw(sql, parameters);
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void DumpAttachedEntities(this DbContext context)
        {
            context.ChangeTracker.Entries()
                .Where(entry => entry.State != (Microsoft.EntityFrameworkCore.EntityState)EntityState.Detached)
                .ToList()
                .ForEach(entry =>
                    Debug.WriteLine($"{entry.Entity.GetType().Name} {entry.State} {entry.Entity}"));
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void CreateSqlTimeout(this DbContext context)
        {
            var sql = @"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[GetTimeoutError]') AND OBJECTPROPERTY(object_id, N'IsProcedure') = 1)
                BEGIN
                    EXEC('CREATE PROCEDURE [dbo].[GetTimeoutError]
                    AS
                    BEGIN
                        WAITFOR DELAY ''00:01:00''
                        SELECT GETDATE()
                    END')
                END;";
            context.Database.ExecuteSqlRaw(sql);
        }
    }
}

using System.Data;
using System.Data.Common;
using System.Reflection;
using BuildingBlocks.Data;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Extensions;
using BuildingBlocks.Types;
using CSharpFunctionalExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.State;

public class SqlQueryHelper
{
    private readonly string _connectionString;
    private readonly ILogger<SqlQueryHelper> _logger;
    private readonly IDistributedCache _distributedCache;
    private readonly DataProviderType _dataProvider;

    public SqlQueryHelper(
        string connectionString,
        DataProviderType dataProvider,
        IDistributedCache distributedCache,
        ILogger<SqlQueryHelper> logger)
    {
        _connectionString = connectionString;
        _distributedCache = distributedCache;
        _logger = logger;
        _dataProvider = dataProvider;
    }

    public async Task<Result<IPagedList<T>, Error>> GetPagedEntitiesAsync<T>(
        CancellationToken cancellationToken,
        string query = "SpGetAllUserOrders",
        bool enableCache = false,
        Dictionary<string, string> columnToPropertyMap = null,
        DistributedCacheEntryOptions cacheEntryOptions = null,
        Action<List<T>>? anyFunction = null,
        params KeyValuePair<string, object>[] parameters) where T : new()
    {
        try
        {
            string cacheKey = BuildCacheKey(query, parameters);
            if (enableCache)
            {
                var cachedResult =
                    await _distributedCache.GetAsync<PagedList<T>>(cacheKey, _logger, cancellationToken);
                if (cachedResult != null)
                {
                    return Result.Success<IPagedList<T>, Error>(cachedResult);
                }
            }

            List<T> results = new List<T>();
            await using var connection = (DbConnection)CreateConnection();
            await connection.OpenAsync(cancellationToken);

            await using var cmd = CreateCommand(connection, query, CommandType.StoredProcedure);

            foreach (var parameter in parameters)
            {
                cmd.Parameters.Add(CreateParameter(cmd,
                    parameter.Key.StartsWith("@") ? parameter.Key : "@" + parameter.Key, parameter.Value));
            }

            var retValParam = CreateParameter(cmd, "@TotalCount", DbType.Int32, ParameterDirection.Output);


            await using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
            {
                var columnNames = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                while (await reader.ReadAsync(cancellationToken))
                {
                    var obj = new T();
                    foreach (var property in typeof(T).GetProperties())
                    {
                        string columnName = columnToPropertyMap?.TryGetValue(property.Name, out var mappedColumn) ?? false
                            ? mappedColumn
                            : property.Name;

                        if (columnNames.Contains(columnName) && reader[columnName] != DBNull.Value)
                        {
                            var value = reader[columnName];
                            if (property.PropertyType.IsGenericType &&
                                property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                property.SetValue(obj, Convert.ChangeType(value, Nullable.GetUnderlyingType(property.PropertyType)));
                            }
                            else
                            {
                                property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                            }
                        }
                    }

                    results.Add(obj);
                }
            }

            anyFunction?.Invoke(results);

            int totalCount = (int)(retValParam.Value ?? 0);
            var pageIndex = parameters.FirstOrDefault(p => p.Key == "@pageIndex" || p.Key == "pageIndex")
                .Value;
            var pageSize = parameters.FirstOrDefault(p => p.Key == "@pageSize" || p.Key == "pageSize")
                .Value;

            var pagedResults = new PagedList<T>(results, Convert.ToInt32(pageIndex),
                Convert.ToInt32(pageSize), totalCount);

            if (enableCache && pagedResults.Any())
            {
                cacheEntryOptions ??= new DistributedCacheEntryOptions
                    { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) };
                await _distributedCache.SetAsync(cacheKey, pagedResults, cacheEntryOptions,
                    cancellationToken);
            }

            return Result.Success<IPagedList<T>, Error>(pagedResults);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure<IPagedList<T>, Error>(
                new Error("general.exception", ex.Message, ex));
        }
    }

    public async Task<Result<IPagedList<T>, Error>> GetPagedEntitiesWithColumnAttribAsync<T>(
        CancellationToken cancellationToken,
        string query = "SpGetAllUserOrders",
        bool enableCache = false,
        DistributedCacheEntryOptions cacheEntryOptions = null,
        params KeyValuePair<string, object>[] parameters) where T : new()
    {
        try
        {
            string cacheKey = BuildCacheKey(query, parameters);
            if (enableCache)
            {
                var cachedResult = await _distributedCache.GetAsync<PagedList<T>>(cacheKey, _logger, cancellationToken);
                if (cachedResult != null)
                {
                    return Result.Success<IPagedList<T>, Error>(cachedResult);
                }
            }

            List<T> results = new List<T>();

            await using var connection = (DbConnection)CreateConnection();
            await connection.OpenAsync(cancellationToken);

            await using var cmd = CreateCommand(connection, query, CommandType.StoredProcedure);

            foreach (var parameter in parameters)
            {
                cmd.Parameters.Add(CreateParameter(cmd,
                    parameter.Key.StartsWith("@") ? parameter.Key : "@" + parameter.Key, parameter.Value));
            }

            var retValParam = CreateParameter(cmd, "@TotalCount", DbType.Int32, ParameterDirection.Output);

            await using (var reader = await cmd.ExecuteReaderAsync(cancellationToken))
            {
                var columnNames = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();

                while (await reader.ReadAsync(cancellationToken))
                {
                    var obj = new T();
                    foreach (var property in typeof(T).GetProperties())
                    {
                        var columnNameAttribute = property.GetCustomAttribute<ColumnNameAttribute>();
                        string columnName = columnNameAttribute?.ColumnName ?? property.Name;

                        if (columnNames.Contains(columnName) && reader[columnName] != DBNull.Value)
                        {
                            property.SetValue(obj, Convert.ChangeType(reader[columnName], property.PropertyType));
                        }
                    }

                    results.Add(obj);
                }
            }

            int totalCount = (int)(retValParam.Value ?? 0);
            var pageIndex = parameters.FirstOrDefault(p => p.Key == "@pageIndex" || p.Key == "pageIndex").Value;
            var pageSize = parameters.FirstOrDefault(p => p.Key == "@pageSize" || p.Key == "pageSize").Value;

            var pagedResults =
                new PagedList<T>(results, Convert.ToInt32(pageIndex), Convert.ToInt32(pageSize), totalCount);

            if (enableCache && pagedResults.Any())
            {
                cacheEntryOptions ??= new DistributedCacheEntryOptions
                    { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) };
                await _distributedCache.SetAsync(cacheKey, pagedResults, cacheEntryOptions, cancellationToken);
            }

            return Result.Success<IPagedList<T>, Error>(pagedResults);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Result.Failure<IPagedList<T>, Error>(new Error("general.exception", ex.Message, ex));
        }
    }

    #region Helpers

    private string BuildCacheKey(string query, KeyValuePair<string, object>[] parameters)
    {
        return query + ":" + (parameters.Any()
            ? (query.Trim() + "-" + string.Join(":", parameters.Select(p => p.Key + ":" + p.Value?.ToString())))
            .Hash()
            : query.Trim().Hash());
    }

    private DbCommand CreateCommand(DbConnection connection, string query, CommandType commandType)
    {
        var command = connection.CreateCommand();
        command.CommandText = query;
        command.CommandType = commandType;
        return command;
    }

    private IDbConnection CreateConnection()
    {
        return _dataProvider switch
        {
            DataProviderType.SqlServer => new SqlConnection(_connectionString),
            DataProviderType.PostgreSQL => new Npgsql.NpgsqlConnection(_connectionString),
            DataProviderType.Sqlite => new Microsoft.Data.Sqlite.SqliteConnection(_connectionString),
            _ => throw new NotSupportedException("Unsupported database provider")
        };
    }


    private DbParameter CreateParameter(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        return parameter;
    }

    private DbParameter CreateParameter(DbCommand command, string name, DbType dbType, ParameterDirection direction)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.DbType = dbType;
        parameter.Direction = direction;
        return parameter;
    }

    #endregion
}
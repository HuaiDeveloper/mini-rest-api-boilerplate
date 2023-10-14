using System.Data;
using Dapper;
using Npgsql;
using NpgsqlTypes;

namespace MiniApi.Persistence.EntityFrameworkCore;

public static class NpgBulkExtensions
{
    public static async Task<bool> NpgBulkInsertAsync<TModel>(
        this ApplicationDbContext dbContext,
        string tableName,
        List<TModel> models,
        ILogger? logger = null)
        where TModel : class
    {
        bool isConnectionClose = false;

        try
        {
            if (models.Count == 0) throw new Exception("models not found data");
            if (string.IsNullOrEmpty(tableName)) throw new Exception("table name is empty");
            
            isConnectionClose = (dbContext.DbConnection.State == ConnectionState.Closed);
            if (isConnectionClose) 
                dbContext.DbConnection.Open();

            var npgConnection = (NpgsqlConnection)dbContext.DbConnection;

            var modelProperties = typeof(TModel).GetProperties();
            var columnNamesString = string.Join(", ", modelProperties.Select(t => $"\"{t.Name}\"").ToArray());

            var importSql = $"COPY \"{tableName}\" ({columnNamesString}) FROM STDIN (FORMAT BINARY)";
            await using var binaryImport = await npgConnection.BeginBinaryImportAsync(importSql).ConfigureAwait(false);
            foreach (var model in models)
            {
                await binaryImport.StartRowAsync().ConfigureAwait(false);
                foreach (var modelProperty in modelProperties)
                {
                    var npgDbType = ConvertPropertyTypeToNpgsqlDbType(modelProperty.PropertyType);
                    await binaryImport.WriteAsync(modelProperty.GetValue(model), npgDbType).ConfigureAwait(false);
                }
            }
            await binaryImport.CompleteAsync().ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            if (logger != null)
                logger.LogError(ex, ex.Message);

            return false;
        }
        finally
        {
            if (isConnectionClose) 
                dbContext.DbConnection.Close();
        }
    }
    
    public static async Task<bool> NpgBulkUpsertAsync<TModel>(
        this ApplicationDbContext dbContext,
        string tableName,
        List<TModel> models,
        string[] conflictColumnNames,
        ILogger? logger = null
        )
        where TModel : class
    {
        bool isConnectionClose = false;
        
        try
        {
            if (models.Count == 0) throw new Exception("models not found data");
            if (conflictColumnNames.Length == 0) throw new Exception("conflict column names not found data");
            if (string.IsNullOrEmpty(tableName)) throw new Exception("table name is empty");
            
            isConnectionClose = (dbContext.DbConnection.State == ConnectionState.Closed);
            if (isConnectionClose) 
                dbContext.DbConnection.Open();
            
            var npgConnection = (NpgsqlConnection)dbContext.DbConnection;
        
            var modelProperties = typeof(TModel).GetProperties();
            var columnNamesString = string.Join(", ", modelProperties.Select(t => $"\"{t.Name}\"").ToArray());
            
            var sourceTempTableName = $"source_temp_{tableName}";
            
            var createTempTableSql = $"CREATE TEMP TABLE IF NOT EXISTS \"{sourceTempTableName}\" AS TABLE \"{tableName}\" WITH NO DATA;";
            await npgConnection.ExecuteAsync(createTempTableSql);
            
            var importSql = $"COPY \"{sourceTempTableName}\" ({columnNamesString}) FROM STDIN (FORMAT BINARY)";
            await using (var binaryImport = await npgConnection.BeginBinaryImportAsync(importSql).ConfigureAwait(false))
            {
                foreach (var model in models)
                {
                    await binaryImport.StartRowAsync().ConfigureAwait(false);
                    foreach (var modelProperty in modelProperties)
                    {
                        var npgDbType = ConvertPropertyTypeToNpgsqlDbType(modelProperty.PropertyType);
                        await binaryImport.WriteAsync(modelProperty.GetValue(model), npgDbType).ConfigureAwait(false);
                    }
                
                }
                await binaryImport.CompleteAsync().ConfigureAwait(false);
            }

            var onConflictString = string.Join(",", conflictColumnNames.Select(name => $"\"{name}\""));
            var updateConflictColumnString = string.Join(",", modelProperties
                .Select(t => $"\"{t.Name}\" = EXCLUDED.\"{t.Name}\"")
                .ToArray());
            
            var insertFromTempTableSql = $"INSERT INTO \"{tableName}\" (SELECT {columnNamesString} FROM \"{sourceTempTableName}\")" +
                                         $" ON CONFLICT ({onConflictString})" +
                                         $" DO UPDATE SET {updateConflictColumnString};";
            await npgConnection.ExecuteAsync(insertFromTempTableSql);
            
            var dropTempTableSql = $"DROP TABLE IF EXISTS \"{sourceTempTableName}\";";
            await npgConnection.ExecuteAsync(dropTempTableSql);
            
            return true;
        }
        catch (Exception ex)
        {
            if (logger != null)
                logger.LogError(ex, ex.Message);
            
            return false;
        }
        finally
        {
            if (isConnectionClose) 
                dbContext.DbConnection.Close();
        }
    }

    private static NpgsqlDbType ConvertPropertyTypeToNpgsqlDbType(Type propertyType)
    {
        var typeCode = Type.GetTypeCode(propertyType);
        switch (typeCode)
        {
            case TypeCode.String:
                return NpgsqlDbType.Text;
            case TypeCode.Int32:
                return NpgsqlDbType.Integer;
            case TypeCode.Int64:
                return NpgsqlDbType.Bigint;
            case TypeCode.Decimal:
                return NpgsqlDbType.Numeric;
            case TypeCode.DateTime:
                return NpgsqlDbType.TimestampTz;
            default:
                return NpgsqlDbType.Text;
        }
    }
}
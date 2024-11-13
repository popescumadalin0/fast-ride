using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using FastRide.Server.Services.Attributes;
using FastRide.Server.Services.Contracts;

namespace FastRide.Server.Services.Wrapper;

public class TableClient<TEntity> : ITableClient<TEntity> where TEntity : class, ITableEntity, new()
{
    private readonly Azure.Data.Tables.TableClient _tableClient;

    public TableClient()
    {
        var classAttribute = typeof(TEntity).GetCustomAttributes(
            typeof(TableNameAttribute), true
        ).FirstOrDefault() as TableNameAttribute;

        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var tableName = classAttribute!.Name;
        _tableClient = new TableClient(connectionString, tableName);

        _tableClient.CreateIfNotExists();
    }

    public async Task<Response<TEntity>> GetAsync(string partitionKey, string rowKey)
    {
        var result = await _tableClient.GetEntityAsync<TEntity>(partitionKey, rowKey);

        return result;
    }

    public List<TEntity> GetBy(Expression<Func<TEntity, bool>> filter)
    {
        var entities = _tableClient.Query(filter);
        return entities.ToList();
    }

    public async Task<Response> AddOrUpdateAsync(TEntity entity)
    {
        var existingEntity = _tableClient.GetEntity<TEntity>(entity.PartitionKey, entity.RowKey);
        if (existingEntity == null)
        {
            return await _tableClient.AddEntityAsync(entity);
        }
        else
        {
            return await _tableClient.UpdateEntityAsync(entity, ETag.All, TableUpdateMode.Replace);
        }
    }

    public async Task<Response> DeleteAsync(string partitionKey, string rowKey)
    {
        return await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
    }
}
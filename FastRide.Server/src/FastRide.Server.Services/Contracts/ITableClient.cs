using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;

namespace FastRide.Server.Services.Contracts;

public interface ITableClient<TEntity> where TEntity : class, ITableEntity, new()
{
    Task<Response<TEntity>> GetAsync(string partitionKey, string rowKey);
    List<TEntity> GetBy(Expression<Func<TEntity, bool>> filter);
    Task<Response> AddOrUpdateAsync(TEntity entity);
    Task<Response> DeleteAsync(string partitionKey, string rowKey);
}
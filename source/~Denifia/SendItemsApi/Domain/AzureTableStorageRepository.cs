using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Denifia.Stardew.SendItemsApi.Domain
{

    public class AzureTableStorageRepository : ITableStorageRepository
    {
        CloudStorageAccount _storageAccount;
        CloudTableClient _tableClient;
        CloudTable _table;
        AzureStorageConfig _azureTableStorageConfig;

        public AzureTableStorageRepository(IOptions<AzureStorageConfig> azureTableStorageConfig)
        {
            _azureTableStorageConfig = azureTableStorageConfig.Value;
            var storageCredentials = new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(_azureTableStorageConfig.AccountName, _azureTableStorageConfig.AccountKey);
            _storageAccount = new CloudStorageAccount(storageCredentials, _azureTableStorageConfig.EndpointSuffix, _azureTableStorageConfig.UseHttps);
            _tableClient = _storageAccount.CreateCloudTableClient();

            // Consider: Making a mail specific repo or passing the table name in during construction
            _table = _tableClient.GetTableReference(_azureTableStorageConfig.TableName);
            _table.CreateIfNotExistsAsync().Wait();
        }

        public async Task<bool> InsertOrReplace<TEntity>(TEntity entity) where TEntity : ITableEntity
        {
            try
            {
                var insert = TableOperation.InsertOrReplace(entity);
                var result = await _table.ExecuteAsync(insert);
                if (result.Result != null)
                {
                    return true;
                }
            }
            catch
            {
                
            }
            return false;
        }

        public async Task<TEntity> Retrieve<TEntity>(string partitionKey, string rowKey) where TEntity : ITableEntity
        {
            try
            {
                var retrieve = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);
                var result = await _table.ExecuteAsync(retrieve);
                if (result.Result != null)
                {
                    return (TEntity)result.Result;
                }
            }
            catch
            {

            }
            return default(TEntity);
        }

        public async Task<bool> Delete<TEntity>(TEntity entity) where TEntity : ITableEntity
        {
            try
            {
                if (entity.ETag == null || entity.ETag.Equals(string.Empty))
                {
                    entity.ETag = "*";
                }
                var delete = TableOperation.Delete(entity);
                var result = await _table.ExecuteAsync(delete);
                if (result.Result != null)
                {
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }

        public async Task<List<TEntity>> Query<TEntity>(string filter) where TEntity : ITableEntity, new()
        {
            var query = new TableQuery<TEntity>().Where(filter);
            var continuationToken = (TableContinuationToken)null;
            var list = new List<TEntity>();
            do
            {
                var segment = await _table.ExecuteQuerySegmentedAsync(query, continuationToken);
                continuationToken = segment.ContinuationToken;
                list.AddRange(segment.Results);
            }
            while (continuationToken != null);

            return list;
        }

        public async Task<int> Count<TEntity>(string filter) where TEntity : ITableEntity, new()
        {
            var columns = new List<string>() { "Id" };
            var query = new TableQuery<TEntity>().Where(filter).Select(columns);
            var continuationToken = (TableContinuationToken)null;
            var count = 0;
            do
            {
                var segment = await _table.ExecuteQuerySegmentedAsync(query, continuationToken);
                continuationToken = segment.ContinuationToken;
                count += segment.Results.Count();
            }
            while (continuationToken != null);

            return count;
        }
    }
}

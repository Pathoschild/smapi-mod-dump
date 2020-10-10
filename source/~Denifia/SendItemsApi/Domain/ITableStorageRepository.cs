/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Denifia/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Denifia.Stardew.SendItemsApi.Domain
{
    public interface ITableStorageRepository
    {
        Task<bool> InsertOrReplace<TEntity>(TEntity entity) where TEntity : ITableEntity;
        Task<TEntity> Retrieve<TEntity>(string partitionKey, string rowKey) where TEntity : ITableEntity;
        Task<bool> Delete<TEntity>(TEntity entity) where TEntity : ITableEntity;
        Task<List<TEntity>> Query<TEntity>(string filter) where TEntity : ITableEntity, new();
        Task<int> Count<TEntity>(string filter) where TEntity : ITableEntity, new();
    }
}

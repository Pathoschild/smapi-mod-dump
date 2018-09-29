using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Denifia.Stardew.SendItemsApi.Domain
{
    public class Mail : TableEntity
    {
        public static string EntityPartitionKey = "mail";

        public Mail()
        {

        }

        public Mail(string id)
        {
            RowKey = id;
            PartitionKey = EntityPartitionKey;
        }

        public string Id { get { return RowKey; } }
        public string ToFarmerId { get; set; }
        public string FromFarmerId { get; set; }
        public string Text { get; set; }
        public DateTime ClientCreatedDate { get; set; }
        public DateTime ServerCreatedDate { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Denifia.Stardew.SendItemsFunctions
{
    public static class Functions
    {
        private const string MailTableName = "MailTable";

        [FunctionName("GetMailToFarmer")]
        public static async Task<List<Mail>> GetMailToFarmer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "mail/to/{farmerId}")] 
            HttpRequest req,
            string farmerId,
            [Table(MailTableName)] CloudTable mailTable,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Getting mail for farmer {farmerId}");

                var filter = TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Mail.EntityPartitionKey),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("ToFarmerId", QueryComparisons.Equal, farmerId));

                var query = new TableQuery<Mail>().Where(filter);

                var continuationToken = (TableContinuationToken)null;
                var list = new List<Mail>();
                do
                {
                    var segment = await mailTable.ExecuteQuerySegmentedAsync(query, continuationToken);
                    continuationToken = segment.ContinuationToken;
                    list.AddRange(segment.Results);
                }
                while (continuationToken != null);
                return list;
            }
            catch
            {
                return new List<Mail>();
            }            
        }

        [FunctionName("PutMail")]
        public static async Task<bool> PutMail(
            [HttpTrigger(AuthorizationLevel.Anonymous, "PUT", Route = "mail/{mailId}")] 
            CreateMailModel model,
            string mailId,
            [Table(MailTableName)] CloudTable mailTable,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Adding mail {mailId} from {model.FromFarmerId} to {model.ToFarmerId} with messages \"{model.Text}\"");
                var mail = new Mail(mailId)
                {
                    ToFarmerId = model.ToFarmerId,
                    Text = model.Text,
                    FromFarmerId = model.FromFarmerId,
                    ClientCreatedDate = model.CreatedDate,
                    ServerCreatedDate = DateTime.Now.ToUniversalTime()
                };

                var result = await mailTable.ExecuteAsync(TableOperation.InsertOrReplace(mail));
                return result.Result != null;
            }
            catch
            {
                return false;
            }
            
        }

        [FunctionName("DeleteMail")]
        public static async Task<bool> DeleteMail(
            [HttpTrigger(AuthorizationLevel.Anonymous, "DELETE", Route = "mail/{mailId}")]
            HttpRequest req,
            string mailId,
            [Table(MailTableName)] CloudTable mailTable,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Deleting mail {mailId}");

                var mail = new Mail(mailId)
                {
                    ETag = "*"
                };
                var result = await mailTable.ExecuteAsync(TableOperation.Delete(mail));
                return result.Result != null;
            }
            catch 
            {
                return false;
            }
        }
    }
}

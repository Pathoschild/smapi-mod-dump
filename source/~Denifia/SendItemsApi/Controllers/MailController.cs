using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LiteDB;
using Denifia.Stardew.SendItemsApi.Domain;
using Denifia.Stardew.SendItemsApi.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace Denifia.Stardew.SendItemsApi.Controllers
{
    [Route("api/[controller]")]
    public class MailController : Controller
    {
        private readonly ITableStorageRepository _repository;

        public MailController(ITableStorageRepository repository)
        {
            _repository = repository;
        }

        // GET api/mail/{mailId}
        [HttpGet("{mailId}")]
        public async Task<Mail> Get(string mailId)
        {
            return await _repository.Retrieve<Mail>(Mail.EntityPartitionKey, mailId);
        }

        // GET api/mail/to/{farmerId}
        [HttpGet("to/{farmerId}")]
        public async Task<List<Mail>> GetMailToFarmer(string farmerId)
        {
            // Consider: Should this be moved in a MailService?
            var filter = TableQuery.GenerateFilterCondition("ToFarmerId", QueryComparisons.Equal, farmerId);
            filter = TableQuery.CombineFilters(GetPartitionFilter(), TableOperators.And, filter);
            return await _repository.Query<Mail>(filter);
        }

        // GET api/mail/from/{farmerId}
        [HttpGet("from/{farmerId}")]
        public async Task<List<Mail>> GetMailFromFarmer(string farmerId)
        {
            // Consider: Should this be moved in a MailService?
            var filter = TableQuery.GenerateFilterCondition("FromFarmerId", QueryComparisons.Equal, farmerId);
            filter = TableQuery.CombineFilters(GetPartitionFilter(), TableOperators.And, filter);
            return await _repository.Query<Mail>(filter);
        }

        // GET api/mail/count
        [HttpGet("count")]
        public async Task<int> GetMailCount()
        {
            var filter = GetPartitionFilter();
            return await _repository.Count<Mail>(filter);
        }

        // PUT api/mail/{mailId}
        [HttpPut("{mailId}")]
        public async Task<bool> Put(string mailId, [FromBody]CreateMailModel model)
        {
            // Consider: Should this be moved in a MailService?
            var mail = new Mail(mailId)
            {
                ToFarmerId = model.ToFarmerId,
                Text = model.Text,
                FromFarmerId = model.FromFarmerId,
                ClientCreatedDate = model.CreatedDate,
                ServerCreatedDate = DateTime.Now.ToUniversalTime()
            };
            return await _repository.InsertOrReplace(mail);
        }

        // DELETE api/mail/{mailId}
        [HttpDelete("{mailId}")]
        public async Task<bool> Delete(string mailId)
        {
            var mail = new Mail(mailId);
            return await _repository.Delete(mail);
        }

        private string GetPartitionFilter()
        {
            return TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Mail.EntityPartitionKey);
        }
    }
}

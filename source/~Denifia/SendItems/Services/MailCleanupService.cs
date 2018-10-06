using Denifia.Stardew.SendItems.Domain;
using Denifia.Stardew.SendItems.Events;
using Denifia.Stardew.SendItems.Framework;
using RestSharp;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Denifia.Stardew.SendItems.Services
{
    public interface IMailCleanupService
    {
    }

    public class MailCleanupService : IMailCleanupService
    {
        private readonly IMod _mod;
        private readonly IConfigurationService _configService;
        private readonly IFarmerService _farmerService;
        private RestClient _restClient { get; set; }

        public MailCleanupService(IMod mod, IConfigurationService configService, IFarmerService farmerService)
        {
            _mod = mod;
            _configService = configService;
            _farmerService = farmerService;
            _restClient = new RestClient(_configService.GetApiUri());

            ModEvents.OnMailCleanup += OnMailCleanup;
            ModEvents.MailDelivered += MailDelivered;
        }

        private void OnMailCleanup(object sender, EventArgs e)
        {
            DeleteFutureComposedMail();
            UnreadFutureReadMail();
            Task.Run(DeleteReadMail);

            ModEvents.RaiseOnMailDelivery(this, EventArgs.Empty);
        }

        private async Task DeleteReadMail()
        {
            var localMail = Repository.Instance.Fetch<Mail>(x =>
                x.Status == MailStatus.Read &&
                x.ToFarmerId == _farmerService.CurrentFarmer.Id
            );
            if (!localMail.Any()) return;

            var logPrefix = "[CleanRead] ";
            _mod.Monitor.Log($"{logPrefix}Clean up read cloud mail...", LogLevel.Debug);
            var currentGameDateTime = ModHelper.GetGameDayTime();
            var readMail = localMail.Where(x => x.ReadInGameDate != null && x.ReadInGameDate <= currentGameDateTime.GetNightBefore()).ToList();
            if (readMail.Any())
            {
                _mod.Monitor.Log($"{logPrefix}.clearing {readMail.Count} read mail...", LogLevel.Debug);
                foreach (var mail in readMail)
                {
                    var deleted = await DeleteRemoteMail(mail, logPrefix);
                    if (deleted)
                    {
                        try
                        {
                            var i = Repository.Instance.Delete<Mail>(x => x.Id == mail.Id);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            _mod.Monitor.Log($"{logPrefix}.done", LogLevel.Debug);
        }

        private void DeleteFutureComposedMail()
        {
            var localMail = Repository.Instance.Fetch<Mail>(x => 
                x.Status == MailStatus.Composed && 
                x.ToFarmerId == _farmerService.CurrentFarmer.Id
            );
            if (!localMail.Any()) return;
            var currentGameDateTime = ModHelper.GetGameDayTime();
            var futureMail = localMail.Where(x => x.CreatedInGameDate > currentGameDateTime.GetNightBefore()).ToList();
            foreach (var mail in futureMail)
            {
                Repository.Instance.Delete<Mail>(x => x.Id == mail.Id);
            }
        }

        private void UnreadFutureReadMail()
        {
            var localMail = Repository.Instance.Fetch<Mail>(x =>
                x.Status == MailStatus.Read &&
                x.ToFarmerId == _farmerService.CurrentFarmer.Id
            );
            if (!localMail.Any()) return;
            var currentGameDateTime = ModHelper.GetGameDayTime();
            var futureMail = localMail.Where(x => x.ReadInGameDate == null || x.ReadInGameDate > currentGameDateTime.GetNightBefore()).ToList();
            foreach (var mail in futureMail)
            {
                mail.Status = MailStatus.Delivered;
                mail.ReadInGameDate = null;
            }

            if (!futureMail.Any()) return;
            Repository.Instance.Upsert(futureMail.AsEnumerable());
        }

        private void MailDelivered(object sender, EventArgs e)
        {
            try
            {
                Task.Run(() => DeletePostedRemoteMail());
            }
            catch (Exception ex)
            {
                ModHelper.HandleError(_mod, ex, "deleting mail from server");
            }
        }

        private void DeletePostedRemoteMail()
        {
            var localMail = Repository.Instance.Fetch<Mail>(x =>
                x.Status == MailStatus.Posted &&
                x.FromFarmerId == _farmerService.CurrentFarmer.Id
            );

            if (localMail.Any()) return;

            var logPrefix = "[CleanPosted] ";
            _mod.Monitor.Log($"{logPrefix}Clean up posted mail...", LogLevel.Debug);
            foreach (var mail in localMail)
            {
                try
                {
                    var i = Repository.Instance.Delete<Mail>(x => x.Id == mail.Id);
                }
                catch
                {
                }
            }
            
            _mod.Monitor.Log($"{logPrefix}.done", LogLevel.Debug);
        }

        private async Task<bool> DeleteRemoteMail(Mail mail, string logPrefix)
        {
            var urlSegments = new Dictionary<string, string> { { "mailId", mail.Id.ToString() } };
            var request = ModHelper.FormStandardRequest("mail/{mailId}", urlSegments, Method.DELETE);
            var response = await _restClient.ExecuteTaskAsync<bool>(request);

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                _mod.Monitor.Log($"{logPrefix}{response.ErrorMessage}", LogLevel.Warn);
                return false;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _mod.Monitor.Log($"{logPrefix}..done", LogLevel.Debug);
                return true;
            }

            return false;
        }
    }
}

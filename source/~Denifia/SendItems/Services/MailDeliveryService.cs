using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Denifia.Stardew.SendItems.Domain;
using Denifia.Stardew.SendItems.Events;
using Denifia.Stardew.SendItems.Framework;
using Denifia.Stardew.SendItems.Models;
using RestSharp;
using StardewModdingAPI;
using StardewValley;

namespace Denifia.Stardew.SendItems.Services
{
    /// <summary>
    /// Handles the local and remote delivery of mail to farmers
    /// </summary>
    public class MailDeliveryService : IMailDeliveryService
    {
        private string logPrefix = "[MailDelivery] ";
        private readonly IMod _mod;
        private readonly IConfigurationService _configService;
        private readonly IFarmerService _farmerService;
        private RestClient _restClient { get; set; }

        public MailDeliveryService(IMod mod, IConfigurationService configService, IFarmerService farmerService)
        {
            _mod = mod;
            _configService = configService;
            _farmerService = farmerService;
            _restClient = new RestClient(_configService.GetApiUri());

            ModEvents.OnMailDelivery += OnMailDelivery;
        }

        private async void OnMailDelivery(object sender, EventArgs e)
        {
            try
            {
                await DeliverPostedMail();
            }
            catch (Exception ex)
            {
                ModHelper.HandleError(_mod, ex, "delivering mail on schedule");
            }
        }

        private async Task DeliverPostedMail()
        {
            _mod.Monitor.Log($"{logPrefix}Delivering mail...", LogLevel.Debug);
            DeliverLocalMail();
            if (!_configService.InLocalOnlyMode())
            {
                await DeliverLocalMailToCloud();
                await DeliverCloudMailLocally();
            }
            DeliverMailToLetterBox();
            _mod.Monitor.Log($"{logPrefix}.mail delivered, done!", LogLevel.Debug);
            ModEvents.RaiseMailDelivered(this, EventArgs.Empty);
        }

        private void DeliverMailToLetterBox()
        {
            if (_farmerService.CurrentFarmer == null) return;
            var currentFarmerId = _farmerService.CurrentFarmer.Id;

            var count = Repository.Instance.Fetch<Mail>(x => x.Status == MailStatus.Delivered && x.ToFarmerId == currentFarmerId).Count;
            if (count > 0)
            {
                while (Game1.mailbox.Any() && Game1.mailbox.First() == ModConstants.PlayerMailKey)
                {
                    Game1.mailbox.RemoveAt(0);
                }

                for (int i = 0; i < count; i++)
                {
                    Game1.mailbox.Add(ModConstants.PlayerMailKey);
                }
            }
        }

        private void DeliverLocalMail()
        {
            var localMail = GetLocallyComposedMail();
            var localFarmers = _farmerService.GetFarmers();
            var updatedLocalMail = new List<Mail>();

            foreach (var mail in localMail)
            {
                if (localFarmers.Any(x => x.Id == mail.ToFarmerId))
                {
                    mail.Status = MailStatus.Delivered;
                    updatedLocalMail.Add(mail);
                }
            }

            UpdateLocalMail(updatedLocalMail);
        }

        private async Task DeliverLocalMailToCloud()
        {
            _mod.Monitor.Log($"{logPrefix}.delivering local mail to cloud...", LogLevel.Debug);
            var localMail = GetLocallyComposedMail();
            var localFarmers = _farmerService.GetFarmers();
            var updatedLocalMail = new List<Mail>();

            // Consider: Add an api method that takes a list of MailCreateModels
            if (localMail.Any())
            {
                _mod.Monitor.Log($"{logPrefix}..uploading {localMail.Count} mail to cloud...", LogLevel.Debug);
                foreach (var mail in localMail)
                {
                    if (!localFarmers.Any(x => x.Id == mail.ToFarmerId))
                    {
                        var createMailModel = new CreateMailModel
                        {
                            ToFarmerId = mail.ToFarmerId,
                            FromFarmerId = mail.FromFarmerId,
                            Text = mail.Text,
                            CreatedDate = mail.CreatedDate
                        };

                        var urlSegments = new Dictionary<string, string> { { "mailId", mail.Id.ToString() } };
                        var request = ModHelper.FormStandardRequest("mail/{mailId}", urlSegments, Method.PUT);
                        request.AddJsonBody(createMailModel);
                        var response = await _restClient.ExecuteTaskAsync<bool>(request);

                        if (!string.IsNullOrEmpty(response.ErrorMessage))
                        {
                            _mod.Monitor.Log($"{logPrefix}{response.ErrorMessage}", LogLevel.Warn);
                            continue;
                        }

                        if (response.Data)
                        {
                            mail.Status = MailStatus.Posted;
                            updatedLocalMail.Add(mail);
                            _mod.Monitor.Log($"{logPrefix}...done", LogLevel.Debug);
                        }
                    }
                }

                Repository.Instance.Upsert(updatedLocalMail.AsEnumerable());
            }
            else
            {
                _mod.Monitor.Log($"{logPrefix}..no local mail to deliver to cloud", LogLevel.Debug);
            }
            _mod.Monitor.Log($"{logPrefix}..done", LogLevel.Debug);
        }

        private async Task DeliverCloudMailLocally()
        {
            _mod.Monitor.Log($"{logPrefix}.deliver cloud mail locally...", LogLevel.Debug);

            var remoteMail = await GetRemotelyPostedMailForCurrentFarmerAsync();
            if (!remoteMail.Any())
            {
                _mod.Monitor.Log($"{logPrefix}..no cloud mail for current farmer", LogLevel.Debug);
                _mod.Monitor.Log($"{logPrefix}..done", LogLevel.Debug);
                return;
            }

            var localFarmers = _farmerService.GetFarmers();
            if (!localFarmers.Any()) return;

            var localFarmer = localFarmers.FirstOrDefault(x => x.Id == remoteMail.First().ToFarmerId);
            if (localFarmer == null) return;

            var localMail = Repository.Instance.Fetch<Mail>(x => x.ToFarmerId == localFarmer.Id);
            var mailNotLocal = remoteMail.Where(x => !localMail.Contains(x)).ToList();

            if (mailNotLocal.Any())
            {
                _mod.Monitor.Log($"{logPrefix}..saving {mailNotLocal.Count()} mail...", LogLevel.Debug);
                foreach (var mail in mailNotLocal)
                {
                    mail.Status = MailStatus.Delivered;
                    _mod.Monitor.Log($"{logPrefix}...done.", LogLevel.Debug);
                }

                Repository.Instance.Upsert(mailNotLocal.AsEnumerable());
            }
            else
            {
                _mod.Monitor.Log($"{logPrefix}..cloud mail already delivered locally", LogLevel.Debug);
            }
            _mod.Monitor.Log($"{logPrefix}..done", LogLevel.Debug);
        }

        private async Task<List<Mail>> GetRemotelyPostedMailForCurrentFarmerAsync()
        {
            if (_farmerService.CurrentFarmer == null) return new List<Mail>();
            var currentFarmerId = _farmerService.CurrentFarmer.Id;

            _mod.Monitor.Log($"{logPrefix}..downloading cloud mail...", LogLevel.Debug);

            var urlSegments = new Dictionary<string, string> { { "farmerId", currentFarmerId } };
            var request = ModHelper.FormStandardRequest("mail/to/{farmerId}", urlSegments, Method.GET);
            var response = await _restClient.ExecuteTaskAsync<List<Mail>>(request);

            var mail = new List<Mail>();

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                _mod.Monitor.Log($"{logPrefix}{response.ErrorMessage}", LogLevel.Warn);
                return mail;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (response.Data != null && response.Data.GetType() == typeof(List<Mail>))
                {
                    mail.AddRange(response.Data);
                }
            }

            _mod.Monitor.Log($"{logPrefix}...done.", LogLevel.Debug);
            return mail;
        }

        private List<Mail> GetLocallyComposedMail()
        {
            return Repository.Instance.Fetch<Mail>(x => x.Status == MailStatus.Composed);
        }

        private void UpdateLocalMail(List<Mail> mail)
        {
            if (!mail.Any()) return;
            Repository.Instance.Update(mail.AsEnumerable());
        }
    }
}

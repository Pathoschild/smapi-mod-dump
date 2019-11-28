using System;
using System.Linq;
using Denifia.Stardew.SendItems.Domain;
using Denifia.Stardew.SendItems.Events;
using Denifia.Stardew.SendItems.Framework;
using StardewValley;
using StardewValley.Menus;

namespace Denifia.Stardew.SendItems.Services
{
    /// <summary>
    /// Handles what to do when a player uses the letter box and reads letters
    /// </summary>
    public class LetterboxService : ILetterboxService
    {
        private readonly IFarmerService _farmerService;

        public LetterboxService(
            IFarmerService farmerService)
        {
            _farmerService = farmerService;

            ModEvents.PlayerUsingLetterbox += PlayerUsingLetterbox;
            ModEvents.MailRead += MailRead;
        }

        private void PlayerUsingLetterbox(object sender, EventArgs e)
        {
            var currentFarmerId = _farmerService.CurrentFarmer.Id;
            var mail = Repository.Instance.FirstOrDefault<Mail>(x =>
                x.Status == MailStatus.Delivered &&
                x.ToFarmerId == currentFarmerId
            );
            if (mail != null && !(Game1.mailbox == null || !Game1.mailbox.Any()))
            {
                ShowLetter(mail);
            }
        }

        private void ShowLetter(Mail mail)
        {
            if (Game1.mailbox == null || !Game1.mailbox.Any()) return;

            if (Game1.mailbox.First() == ModConstants.PlayerMailKey)
            {
                Game1.activeClickableMenu = new LetterViewerMenu(mail.Text, ModConstants.PlayerMailTitle);
                if (Game1.mailbox.Any())
                {
                    Game1.mailbox.RemoveAt(0);
                }
                ModEvents.RaiseMailRead(this, new MailReadEventArgs { Id = mail.Id });
            }
        }

        private void MailRead(object sender, MailReadEventArgs e)
        {
            var currentFarmerId = _farmerService.CurrentFarmer.Id;
            var mail = Repository.Instance.FirstOrDefault<Mail>(x => x.Id == e.Id);
            if (mail != null)
            {
                mail.Status = MailStatus.Read;
                mail.ReadInGameDate = ModHelper.GetGameDayTime();
                Repository.Instance.Update(mail);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Denifia.Stardew.SendItems.Domain;
using Denifia.Stardew.SendItems.Events;
using Denifia.Stardew.SendItems.Framework;
using Denifia.Stardew.SendItems.Menus;
using StardewValley;

namespace Denifia.Stardew.SendItems.Services
{
    /// <summary>
    /// Handles what to do when a player uses the postbox and creates a letter
    /// </summary>
    public class PostboxService : IPostboxService
    {
        // TODO: Move to ModConstants
        private const string _letterPostedNotification = "Letter Posted!";
        private const string _noFriendsNotification = "You don't have anyone in your friends list to send items too.";
        private const string _leaveSelectionKeyAndValue = "(Leave)";
        private const string _messageFormat = "Hey there!^^  I thought you might like this... Take care! ^    -{0} %item object {1} {2} %%";

        private readonly IFarmerService _farmerService;
        private readonly IConfigurationService _configService;

        public PostboxService(
            IConfigurationService configService,
            IFarmerService farmerService)
        {
            _configService = configService;
            _farmerService = farmerService;

            ModEvents.PlayerUsingPostbox += PlayerUsingPostbox;
            ModEvents.MailComposed += MailComposed;
        }

        private void PlayerUsingPostbox(object sender, EventArgs e)
        {
            DisplayFriendSelector();
        }

        private void DisplayFriendSelector()
        {
            if (Game1.activeClickableMenu != null) return;
            List<Response> responseList = new List<Response>();
            var friends = _farmerService.CurrentFarmer.Friends;
            foreach (var friend in friends)
            {
                responseList.Add(new Response(friend.Id, friend.DisplayText));
            }

            if (!responseList.Any())
            {
                ModHelper.ShowInfoMessage(_noFriendsNotification);
                return;
            }

            responseList.Add(new Response(_leaveSelectionKeyAndValue, _leaveSelectionKeyAndValue));

            Game1.currentLocation.createQuestionDialogue("Select Friend:", responseList.ToArray(), FriendSelectorAnswered, (NPC)null);
            Game1.player.Halt();
        }

        private void FriendSelectorAnswered(StardewValley.Farmer farmer, string answer)
        {
            if (answer.Equals(_leaveSelectionKeyAndValue)) return;

            var items = new List<Item> { null };
            Game1.activeClickableMenu = new ComposeLetter(answer, items, 1, 1, null, HighlightOnlyGiftableItems);
            // TODO: Should I use this instead?
            //Game1.activeClickableMenu = (IClickableMenu)new ComposeLetter(answer, items, 1, 1, new ComposeLetter.behaviorOnItemChange(onLetterChange)); 
        }

        private bool HighlightOnlyGiftableItems(Item i)
        {
            return i.canBeGivenAsGift();
        }

        private void MailComposed(object sender, MailComposedEventArgs e)
        {
            var toFarmerId = e.ToFarmerId;
            var fromFarmer = _farmerService.CurrentFarmer;
            var item = e.Item;

            if (item == null) return;

            var messageText = string.Format(_messageFormat, fromFarmer.Name, item.ParentSheetIndex, item.Stack);

            // Consider: Moving this to own service
            var mail = new Mail()
            {
                Id = Guid.NewGuid(),
                ToFarmerId = toFarmerId,
                FromFarmerId = fromFarmer.Id,
                Text = messageText,
                CreatedDate = DateTime.Now.ToUniversalTime(),
                Status = MailStatus.Composed,
                CreatedInGameDate = ModHelper.GetGameDayTime()
            };
            Repository.Instance.Insert(mail);
            ModHelper.ShowInfoMessage(_letterPostedNotification);
        }
    }
}

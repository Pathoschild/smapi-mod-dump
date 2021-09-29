/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using QuestFramework.Framework;
using QuestFramework.Framework.Helpers;
using QuestFramework.Offers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.Controllers
{
    internal class ItemOfferController
    {
        private readonly QuestOfferManager _offerManager;
        private readonly QuestManager _questManager;
        private readonly IMonitor _monitor;
        private readonly string _pickedFlag = QuestFrameworkMod.Instance.ModManifest.UniqueID + "/picked";

        public ItemOfferController(QuestOfferManager offerManager, QuestManager questManager, IMonitor monitor)
        {
            this._offerManager = offerManager;
            this._questManager = questManager;
            this._monitor = monitor;
        }

        public void CheckItemOffersQuest(Item item)
        {
            if (!Context.IsWorldReady)
                return;

            var offers = this._offerManager.GetMatchedOffers<ItemOfferAttributes>("Item");

            this.OfferQuestByThisItem(offers, item);
            this.MarkItemAsPicked(item);
        }

        private void OfferQuestByThisItem(IEnumerable<QuestOffer<ItemOfferAttributes>> offers, Item item)
        {
            foreach (var offer in offers)
            {
                if (!ItemHelper.CheckItemContextTags(item, offer.OfferDetails.ItemContextTags))
                    continue;

                int id = this._questManager.ResolveGameQuestId(offer.QuestName);

                if (id == -1 || Game1.player.hasQuest(id) || this.WasItemPicked(item))
                    continue;

                this._questManager.AcceptQuest(offer.QuestName);
                this.ShowItemUp(item, Game1.player, offer.OfferDetails.FoundMessage);
                this._monitor.Log($"Offered quest `{offer.QuestName}` by item object {item.Name}");
            }
        }

        private void ShowItemUp(Item item, Farmer farmer, string message)
        {
            bool showMessage = message != null;
            farmer.completelyStopAnimatingOrDoingAction();
            farmer.faceDirection(2);
            farmer.freezePause = 4000;

            DelayedAction.playSoundAfterDelay("getNewSpecialItem", 750);
            void showMessageFunc(Farmer f) => Game1.drawObjectDialogue(message);
            farmer.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[3]
            {
                new FarmerSprite.AnimationFrame(57, 0),
                new FarmerSprite.AnimationFrame(57, 2500, secondaryArm: false, flip: false, Farmer.showHoldingItem),
                showMessage 
                    ? new FarmerSprite.AnimationFrame((short)farmer.FarmerSprite.CurrentFrame, 500, secondaryArm: false, flip: false, showMessageFunc, behaviorAtEndOfFrame: true) 
                    : new FarmerSprite.AnimationFrame((short)farmer.FarmerSprite.CurrentFrame, 500, secondaryArm: false, flip: false)
            });
            farmer.mostRecentlyGrabbedItem = item;
            farmer.canMove = false;
        }



        private void MarkItemAsPicked(Item item)
        {
            item.modData[this._pickedFlag] = "true";
        }

        private bool WasItemPicked(Item item)
        {
            return item.modData.ContainsKey(this._pickedFlag) 
                && item.modData[this._pickedFlag] == "true";
        }
    }
}

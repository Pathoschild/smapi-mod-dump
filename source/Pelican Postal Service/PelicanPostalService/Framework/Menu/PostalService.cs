using Pelican.Friendship;
using Pelican.Items;
using Pelican.Quests;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace Pelican.Menus
{
    public class PostalService : Meta
    {
        public bool UseDefaultAction { get; set; }
        private readonly ItemHandler itemHandler;
        
        public PostalService(ItemHandler itemHandler)
        {
            this.itemHandler = itemHandler;
        }

        public void Open()
        {
            bool isValidItem = ProcessItemAndReport(itemHandler.Item);
            if (isValidItem && Game1.activeClickableMenu == null && Game1.player.CurrentTool == null)
            {
                List<string> options = NpcHandler.FindKnownNPCs();
                Game1.activeClickableMenu = options != null ? new ChooseFromListMenu(options, OnSelectOption, false) : null;
            }
        }

        private void Exit()
        {
            Game1.activeClickableMenu.exitThisMenu();
        }

        private void OnSelectOption(string displayName)
        {
            string name = NpcHandler.Dictionary[displayName];
            NpcHandler npcHandler = new NpcHandler(name);

            if (npcHandler.Target != null)
            {
                if (Config.AllowQuestSubmissions)
                {
                    QuestHandler questHandler = new QuestHandler(this);
                    questHandler.FindOneAndUpdate(npcHandler, itemHandler);

                    if (UseDefaultAction)
                    {
                        ProcessGift(npcHandler);
                    }
                }
                else
                {
                    ProcessGift(npcHandler);
                }

                Exit();
            }
        }

        private void ProcessGift(NpcHandler npcHandler)
        {
            string who = npcHandler.Target.Name;
            if (NpcHandler.CanReceiveGiftToday(who))
            {
                int rating = itemHandler.GiftTasteRating(npcHandler);
                npcHandler.Update(rating, false, null);
                itemHandler.RemoveFromInventory(1);
            }
        }

        private bool ProcessItemAndReport(Object item)
        {
            if (item != null)
            {
                bool isStrictQuestItem = item.Name.Equals("Lost Axe") || item.Name.Equals("Lucky Purple Shorts") || item.Name.Equals("Berry Basket");
                bool isStrictMarriageItem = item.Name.Equals("Bouquet") || item.Name.Equals("Mermaid's Pendant") || item.Name.Equals("Wedding Ring");
                return item.canBeGivenAsGift() && !isStrictMarriageItem && !(isStrictQuestItem && !Config.AllowQuestSubmissions) ? true : false;
            }
            return false;
        }
    }
}
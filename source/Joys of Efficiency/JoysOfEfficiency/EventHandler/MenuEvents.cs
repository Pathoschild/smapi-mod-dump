using JoysOfEfficiency.Automation;
using JoysOfEfficiency.Core;
using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace JoysOfEfficiency.EventHandler
{
    internal class MenuEvents
    {
        private static Config Conf => InstanceHolder.Config;
        public void OnMenuChanged(object sender, MenuChangedEventArgs args)
        {
            if (Conf.AutoLootTreasures && args.NewMenu is ItemGrabMenu menu)
            {
                //Opened ItemGrabMenu
                InventoryAutomation.LootAllAcceptableItems(menu);
            }

            if (Conf.CollectLetterAttachmentsAndQuests && args.NewMenu is LetterViewerMenu letter)
            {
                MailAutomation.CollectMailAttachmentsAndQuests(letter);
            }
        }
    }
}

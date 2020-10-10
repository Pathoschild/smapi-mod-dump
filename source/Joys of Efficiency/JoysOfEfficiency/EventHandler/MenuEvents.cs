/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

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

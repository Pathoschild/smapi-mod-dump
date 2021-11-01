/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack-hill/stardew-valley-stash-items
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Linq;

namespace StashItems
{
    public class ModEntry : Mod
    {
        internal static ModConfig Config { get; private set; }

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
		}

        private static void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                if (e.Pressed.Contains(Config.StashHotKey))
                {
                    ItemStashing.StashItemsInNearbyChests(Config.Radius);
                }
            }
        }
	}
}

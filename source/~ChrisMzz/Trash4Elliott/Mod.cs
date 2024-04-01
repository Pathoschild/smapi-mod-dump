/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChrisMzz/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using System.Net.Http;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace Trash4Elliott
{
    /// <summary>The mod entry point.</summary>
    internal sealed class Mod : StardewModdingAPI.Mod
    {

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.OnPlayerEnterArea;
            helper.Events.World.NpcListChanged += this.OnNPCEnterArea;
        }



        /// <summary>
        ///  Raised when player enters a new location. Gives player trash if Elliott is in the new location.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e"></param>
        private void OnPlayerEnterArea(object sender, WarpedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (e.NewLocation.characters.Contains(Game1.getCharacterFromName("Elliott")))
            {
                Item trash = new StardewValley.Object(168, 1);
                Game1.player.addItemByMenuIfNecessary(trash);
            }
        }

        /// <summary>
        /// Raised when an NPC enters or leaves a location. If it's Elliott, and the player is in the location, then this gives the player trash.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e"></param>
        private void OnNPCEnterArea(object sender, NpcListChangedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet or if the location change isn't in the current location
            if ((!Context.IsWorldReady) || (!e.IsCurrentLocation))
                return;

            foreach (NPC npc in e.Added)
            {
                if (npc.Equals((Game1.getCharacterFromName("Elliott"))))
                {
                    Item trash = new StardewValley.Object(168, 1);
                    Game1.player.addItemByMenuIfNecessary(trash);
                    return;
                }
            }


        }

    }
}

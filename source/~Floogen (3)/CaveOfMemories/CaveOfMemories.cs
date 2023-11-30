/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/MysticalBuildings
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CaveOfMemories.Framework.GameLocations;
using CaveOfMemories.Framework.Managers;
using CaveOfMemories.Framework.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CaveOfMemories
{
    public class CaveOfMemories : Mod
    {
        // Shared static helpers
        internal static IMonitor monitor;
        internal static IModHelper modHelper;
        internal static ITranslationHelper i18n;

        // Managers
        internal static AssetManager assetManager;

        public override void Entry(IModHelper helper)
        {
            // Set up the monitor, helper and multiplayer
            monitor = Monitor;
            modHelper = helper;
            i18n = helper.Translation;

            // Set up the managers
            assetManager = new AssetManager(monitor, helper);

            // Hook into required events
            helper.Events.Player.Warped += OnWarped;
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation is CaveOfMemoriesLocation caveOfMemories && caveOfMemories is not null && e.OldLocation is not Farm)
            {
                e.Player.setTileLocation(new Vector2(CaveOfMemoriesLocation.MirrorTileBase.X, CaveOfMemoriesLocation.MirrorTileBase.Y));
                e.Player.FacingDirection = 0;
            }
            else if (e.OldLocation is CaveOfMemoriesLocation && e.NewLocation is not CaveOfMemoriesLocation && Game1.eventUp is false)
            {
                // Clear the music from the cave
                Game1.changeMusicTrack("none");
            }
        }

        internal static Random GenerateRandom(Farmer who = null)
        {
            if (who is not null)
            {
                return new Random((int)((long)Game1.uniqueIDForThisGame + who.DailyLuck + Game1.stats.DaysPlayed * 500 + Game1.ticks + DateTime.Now.Ticks));
            }
            return new Random((int)((long)Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed * 500 + Game1.ticks + DateTime.Now.Ticks));
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    internal class KentInjections
    {
        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;

        public static void Initialize(IMonitor monitor, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _archipelago = archipelago;
        }

        // public static void addKentIfNecessary()
        public static bool AddKentIfNecessary_ConsiderSeasonsRandomizer_Prefix()
        {
            try
            {
                if (Game1.Date.TotalDays < 112)
                {
                    return false; // don't run original logic
                }

                if (Game1.getCharacterFromName("Kent", useLocationsListOnly: true) == null)
                {
                    Game1.getLocationFromNameInLocationsList("SamHouse").addCharacter(new NPC(new AnimatedSprite("Characters\\Kent", 0, 16, 32), new Vector2(512f, 832f), "SamHouse", 2, "Kent", false, (Dictionary<int, int[]>)null, Game1.content.Load<Texture2D>("Portraits\\Kent")));
                }

                if (Game1.player.friendshipData.ContainsKey("Kent"))
                {
                    return false; // don't run original logic
                }

                Game1.player.friendshipData.Add("Kent", new Friendship());

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(AddKentIfNecessary_ConsiderSeasonsRandomizer_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}

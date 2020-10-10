/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/oranisagu/SDV-FarmAutomation
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;

namespace FarmAutomation.Common
{
    /// <summary>
    /// helper class for actions where a reference to a farmer is necessary
    /// </summary>
    public class GhostFarmer : Farmer
    {
        private GhostFarmer() : base(new FarmerSprite(null), Vector2.Zero, 1, "GhostFarmer", new List<Item>(), true)
        {
            ClearInventory();
            uniqueMultiplayerID = Game1.player.uniqueMultiplayerID;
            professions = Game1.player.professions;
            FarmerSprite.setOwner(this);
            maxItems = 24;
        }

        public void ClearInventory()
        {
            items = new List<Item>(new Item[maxItems]);
        }
        
        /// <summary>
        /// need to override the constructor as for some reason the base sets the sprites on the main player which leads to a crash.
        /// </summary>
        /// <returns></returns>
        public static GhostFarmer CreateFarmer()
        {
            var prevSprite = Game1.player.sprite;
            var who = new GhostFarmer();
            Game1.player.sprite = prevSprite;
            return who;
        }
    }
}
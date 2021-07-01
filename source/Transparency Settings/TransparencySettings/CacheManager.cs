/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/TransparencySettings
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace TransparencySettings
{
    /// <summary>A class that stores and manages alpha values and other mod data.</summary>
    public static class CacheManager
    {
        /// <summary>The cached tile position of the current local player.</summary>
        public static Vector2 CurrentPlayerTile { get; private set; } = Vector2.Zero;

        /// <summary>A dictionary of in-game objects and the alpha values assigned to them by this mod.</summary>
        private static Dictionary<object, PerScreen<float>> alphas = new Dictionary<object, PerScreen<float>>();

        public static float GetAlpha(object instance, float changeToApply)
        {
            if (alphas.TryGetValue(instance, out PerScreen<float> alpha)) //if this instance already has stored alpha values
            {
                alpha.Value = Utility.Clamp(alpha.Value + changeToApply, 0.4f, 1f); //calculate and set new alpha (min 40%, max 100%)
                return alpha.Value;
            }
            else
            {
                alpha = new PerScreen<float>(); //create new storage for this instance's alpha values
                alpha.Value = Utility.Clamp(1f + changeToApply, 0.4f, 1f); //calculate and set new alpha (min 40%, max 100%; default value 100%)
                alphas.Add(instance, alpha); //store this instance's alpha values
                return alpha.Value;
            }
        }

        /// <summary>Initializes this manager, e.g. by enabling its SMAPI events.</summary>
        public static void Initialize(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicking += UpdateTicking;
            helper.Events.GameLoop.DayEnding += DayEnding;
            helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle;
            helper.Events.Player.Warped += Warped;
        }

        private static void UpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            CurrentPlayerTile = Game1.player.getTileLocation(); //update cached tile position of the current local player
        }

        private static void DayEnding(object sender, DayEndingEventArgs e)
        {
            alphas.Clear(); //reset all alpha values
        }

        private static void ReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            alphas.Clear(); //reset all alpha values
        }

        private static void Warped(object sender, WarpedEventArgs e)
        {
            alphas.Clear(); //reset all alpha values
        }
    }
}

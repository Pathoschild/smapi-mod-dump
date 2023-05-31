/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace Omegasis.Revitalize.Framework.Utilities
{
    /// <summary>
    /// Deals with syncing objects in multiplayer.
    /// </summary>
    public static class MultiplayerUtilities
    {
        /// <summary>
        /// The multiplayer instance for the game.
        /// </summary>
        private static Multiplayer GameMultiplayer;

        /// <summary>
        /// Handles receiving mod messages.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        public static void GetModMessage(object o, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {
        }

        /// <summary>
        /// Gets the multiplayer instance for the game so that we can access multiplayer functionality calls such as broadcasting sprites for a given action.
        /// </summary>
        /// <returns></returns>
        public static Multiplayer GetMultiplayer()
        {
            if (GameMultiplayer == null)
            {
                Multiplayer multiplayer = RevitalizeModCore.ModHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer", true).GetValue();
                if (multiplayer == null) return null;
                else
                {
                    GameMultiplayer = multiplayer;
                    return GameMultiplayer;
                }
            }
            else
                return GameMultiplayer;
        }

    }
}

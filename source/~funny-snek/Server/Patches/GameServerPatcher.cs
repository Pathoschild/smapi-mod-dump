/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/funny-snek/anticheat-and-servercode
**
*************************************************/

using StardewValley;

namespace FunnySnek.AntiCheat.Server.Patches
{
    /// <summary>Harmony patch for kick.</summary>
    internal class GameServerPatcher
    {
        /*********
        ** Public methods
        *********/
        public static bool Prefix(long peerId)
        {
            if (Game1.IsServer && (!Game1.otherFarmers.ContainsKey(peerId)))
            {
                //They have been kicked off the server
                return false;
            }

            return true;
        }
    }
}

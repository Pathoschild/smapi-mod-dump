/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace RainbowTrail
{
    public partial class ModEntry
    {
        private void ResetTrail()
        {
            trailDict.Remove(Game1.player.UniqueMultiplayerID);
            rainbowTexture = SHelper.GameContent.Load<Texture2D>(rainbowTrailKey);
        }

        private static bool RainbowTrailStatus(Farmer player)
        {
            if (!Config.ModEnabled || !player.modData.TryGetValue(rainbowTrailKey, out string str))
                return false;
            return true;

        }
    }
}
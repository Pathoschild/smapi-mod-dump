/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace StardewNametags
{
    [HarmonyPatch(typeof(Farmer), "draw")]
    class DrawPlayerNames
    {
        public static void Postfix(Farmer __instance, SpriteBatch b)
        {
            if (ModEntry.DisplayNames)
                PlayerNameBox.draw(b, __instance);
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace SurfingFestival.Patches
{
    [HarmonyPatch(typeof(Character), nameof(Character.draw), typeof( SpriteBatch ))]
    public static class CharacterDrawPatch
    {
        public static void Prefix(Character __instance, SpriteBatch b)
        {
            if ( Game1.CurrentEvent?.FestivalName != Mod.festivalName || Game1.CurrentEvent?.playerControlSequenceID != "surfingRace" )
                return;

            Mod.DrawSurfboard( __instance, b );
        }

        public static void Postfix( Character __instance, SpriteBatch b )
        {
            if ( Game1.CurrentEvent?.FestivalName != Mod.festivalName || Game1.CurrentEvent?.playerControlSequenceID != "surfingRace" )
                return;

            Mod.DrawSurfingStatuses( __instance, b );
        }
    }
}

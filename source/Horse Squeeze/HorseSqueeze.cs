/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jorgamun/HorseSqueeze
**
*************************************************/

using StardewValley;
using StardewModdingAPI;
using HarmonyLib;

using StardewValley.Characters;
using Microsoft.Xna.Framework;

namespace HorseSqueeze
{
    public class HorseSqueeze : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Characters.Horse), nameof(StardewValley.Characters.Horse.GetBoundingBox)),
                prefix: new HarmonyMethod(this.GetType(), nameof(GetBoundingBox_Prefix))
            );
        }

        public static bool GetBoundingBox_Prefix(Horse __instance, ref Rectangle __result)
        {
            if (__instance.Sprite == null)
            {
                __result = Microsoft.Xna.Framework.Rectangle.Empty;
                return false;
            }

            Vector2 vector = __instance.Position;
            int width = __instance.GetSpriteWidthForPositioning() * 4 * 3 / 4;
            var box = new Microsoft.Xna.Framework.Rectangle((int)vector.X + 8, (int)vector.Y + 16, width, 32);

            box.Inflate(-36, 0);
            __result = box;

            return false;
        }
    }
}

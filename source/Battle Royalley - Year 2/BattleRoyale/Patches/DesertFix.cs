/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using System.Runtime.CompilerServices;

namespace BattleRoyale.Patches
{
    class DesertFix : Patch
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(GameLocation), "resetLocalState")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ResetBaseState(Desert instance)
        {
            return;
        }

        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Desert), "resetLocalState");

        public static bool Prefix(Desert __instance)
        {
            ResetBaseState(__instance);

            __instance.leaving = false;

            Game1.ambientLight = Color.White;

            __instance.drivingOff = false;
            __instance.drivingBack = false;

            if (Game1.isRaining)
                Game1.changeMusicTrack("none");
            else
                Game1.changeMusicTrack("wavy");

            __instance.temporarySprites.Add(new TemporaryAnimatedSprite
            {
                texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\temporary_sprites_1"),
                sourceRect = new Rectangle(0, 513, 208, 101),
                sourceRectStartingPos = new Vector2(0f, 513f),
                animationLength = 1,
                totalNumberOfLoops = 9999,
                interval = 99999f,
                scale = 4f,
                position = new Vector2(528f, 298f) * 4f,
                layerDepth = 0.1324f,
                id = 996f
            });
            if (Game1.timeOfDay >= Game1.getModeratelyDarkTime())
            {
                __instance.lightMerchantLamps();
            }

            return false;
        }
    }
}

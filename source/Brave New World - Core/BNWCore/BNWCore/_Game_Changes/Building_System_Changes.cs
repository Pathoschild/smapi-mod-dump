/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;

namespace BNWCore
{
    public class Building_System_Changes
    {
        public static void Farm_resetLocalState_Postfix(Farm __instance)
        {
            if(ModEntry.ModHelper.ModRegistry.IsLoaded("DiogoAlbano.teste"))
            {
                if (!__instance.isThereABuildingUnderConstruction() || __instance.getBuildingUnderConstruction().daysOfConstructionLeft.Value <= 0)
                    return;
                __instance.removeTemporarySpritesWithIDLocal(16846f);
                if (__instance.isThereABuildingUnderConstruction() && __instance.getBuildingUnderConstruction().daysOfConstructionLeft.Value < 2)
                {
                    Game1.changeMusicTrack("50s");
                    Building b = __instance.getBuildingUnderConstruction();
                    __instance.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(399, 262, (b.daysOfConstructionLeft.Value == 1) ? 29 : 9, 43), new Vector2(b.tileX.Value + b.tilesWide.Value / 2, b.tileY.Value + b.tilesHigh.Value / 2) * 64f + new Vector2(-16f, -144f), false, 0f, Color.White)
                    {
                        id = 16846f,
                        scale = 4f,
                        interval = 200f,
                        animationLength = 2,
                        totalNumberOfLoops = 99999,
                        layerDepth = ((b.tileY.Value + b.tilesHigh.Value / 2) * 64 + 32) / 10000f
                    });
                }
            }
        }
    }
}

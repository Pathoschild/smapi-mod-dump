/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

using StardewModdingAPI;
using HarmonyLib;

namespace FasterPathSpeed
{
    public class ModEntry : Mod
    {
        public static ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            FarmerPatches.Initialize(Monitor, Config);

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.getMovementSpeed)),
                postfix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.GetMovementSpeed_Postfix))
            );
        }

        public static float GetPathSpeedBuffByFlooringType(StardewValley.TerrainFeatures.Flooring flooring)
        {
            if (Config.IsUseCustomPathSpeedBuffValues)
            {
                switch (flooring.whichFloor.Value)
                {
                    case StardewValley.TerrainFeatures.Flooring.wood:
                        return Config.CustomPathSpeedBuffValues.Wood;
                    case StardewValley.TerrainFeatures.Flooring.stone:
                        return Config.CustomPathSpeedBuffValues.Stone;
                    case StardewValley.TerrainFeatures.Flooring.ghost:
                        return Config.CustomPathSpeedBuffValues.Ghost;
                    case StardewValley.TerrainFeatures.Flooring.iceTile:
                        return Config.CustomPathSpeedBuffValues.IceTile;
                    case StardewValley.TerrainFeatures.Flooring.straw:
                        return Config.CustomPathSpeedBuffValues.Straw;
                    case StardewValley.TerrainFeatures.Flooring.gravel:
                        return Config.CustomPathSpeedBuffValues.Gravel;
                    case StardewValley.TerrainFeatures.Flooring.boardwalk:
                        return Config.CustomPathSpeedBuffValues.Boardwalk;
                    case StardewValley.TerrainFeatures.Flooring.colored_cobblestone:
                        return Config.CustomPathSpeedBuffValues.ColoredCobblestone;
                    case StardewValley.TerrainFeatures.Flooring.cobblestone:
                        return Config.CustomPathSpeedBuffValues.Cobblestone;
                    case StardewValley.TerrainFeatures.Flooring.steppingStone:
                        return Config.CustomPathSpeedBuffValues.SteppingStone;
                    case StardewValley.TerrainFeatures.Flooring.brick:
                        return Config.CustomPathSpeedBuffValues.Brick;
                    case StardewValley.TerrainFeatures.Flooring.plankFlooring:
                        return Config.CustomPathSpeedBuffValues.PlankFlooring;
                    case StardewValley.TerrainFeatures.Flooring.townFlooring:
                        return Config.CustomPathSpeedBuffValues.TownFlooring;
                }
            }

            return Config.DefaultPathSpeedBuff;
        }
    }
}

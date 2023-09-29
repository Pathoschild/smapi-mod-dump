/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/AlternativeTextures
**
*************************************************/

using AlternativeTextures.Framework.Models;
using AlternativeTextures.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Linq;

namespace AlternativeTextures.Framework.Patches.GameLocations
{
    internal class GameLocationPatch : PatchTemplate
    {
        private readonly Type _object = typeof(GameLocation);

        internal GameLocationPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.checkAction), new[] { typeof(xTile.Dimensions.Location), typeof(xTile.Dimensions.Rectangle), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(CheckActionPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.LowPriorityLeftClick), new[] { typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(LowPriorityLeftClickPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.leftClick), new[] { typeof(int), typeof(int), typeof(Farmer) }), prefix: new HarmonyMethod(GetType(), nameof(LowPriorityLeftClickPrefix)));
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.seasonUpdate), new[] { typeof(string), typeof(bool) }), postfix: new HarmonyMethod(GetType(), nameof(SeasonUpdatePostfix)));
        }

        private static bool CheckActionPrefix(GameLocation __instance, ref bool __result, xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            if (Game1.didPlayerJustRightClick())
            {
                return true;
            }

            if (who.CurrentTool is GenericTool tool && (tool.modData.ContainsKey(AlternativeTextures.PAINT_BUCKET_FLAG) || tool.modData.ContainsKey(AlternativeTextures.PAINT_BRUSH_FLAG) || tool.modData.ContainsKey(AlternativeTextures.SPRAY_CAN_FLAG) || tool.modData.ContainsKey(AlternativeTextures.CATALOGUE_FLAG)))
            {
                Vector2 position = ((!Game1.wasMouseVisibleThisFrame) ? Game1.player.GetToolLocation() : new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y));
                tool.beginUsing(__instance, (int)position.X, (int)position.Y, who);
                __result = false;
                return false;
            }

            return true;
        }

        private static bool LowPriorityLeftClickPrefix(GameLocation __instance, ref bool __result, int x, int y, Farmer who)
        {
            if (who.CurrentTool is GenericTool tool && (tool.modData.ContainsKey(AlternativeTextures.PAINT_BUCKET_FLAG) || tool.modData.ContainsKey(AlternativeTextures.PAINT_BRUSH_FLAG) || tool.modData.ContainsKey(AlternativeTextures.SPRAY_CAN_FLAG) || tool.modData.ContainsKey(AlternativeTextures.CATALOGUE_FLAG)))
            {
                __result = false;
                return false;
            }

            return true;
        }

        internal static void SeasonUpdatePostfix(GameLocation __instance, string season, bool onLoad = false)
        {
            if (__instance is null)
            {
                return;
            }

            if (__instance.objects != null)
            {
                for (int k = __instance.objects.Count() - 1; k >= 0; k--)
                {
                    var obj = __instance.objects.Pairs.ElementAt(k).Value;
                    if (obj.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_OWNER) && obj.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME))
                    {
                        var instanceName = GetObjectName(obj);
                        if (obj is Fence fence && fence.isGate.Value)
                        {
                            instanceName = Game1.objectInformation[325].Split('/')[0];
                        }

                        var seasonalName = String.Concat(obj.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.Craftable}_{instanceName}_{season}");
                        if ((obj.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_SEASON) && !String.IsNullOrEmpty(obj.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]) && !String.Equals(obj.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON], Game1.currentSeason, StringComparison.OrdinalIgnoreCase)) || AlternativeTextures.textureManager.DoesObjectHaveAlternativeTextureById(seasonalName))
                        {
                            obj.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON] = season;
                            obj.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = seasonalName;
                        }
                    }
                }
            }

            if (__instance.characters != null)
            {
                for (int k = __instance.characters.Count() - 1; k >= 0; k--)
                {
                    var character = __instance.characters.ElementAt(k);
                    if (character.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_OWNER) && character.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME))
                    {
                        var instanceName = GetCharacterName(character);

                        var seasonalName = String.Concat(character.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.Character}_{instanceName}_{season}");
                        if ((character.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_SEASON) && !String.IsNullOrEmpty(character.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]) && !String.Equals(character.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON], Game1.currentSeason, StringComparison.OrdinalIgnoreCase)) || AlternativeTextures.textureManager.DoesObjectHaveAlternativeTextureById(seasonalName))
                        {
                            character.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON] = season;
                            character.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = seasonalName;
                        }
                    }
                }
            }

            // Check for animals, if __instance is an applicable location
            if ((__instance is Farm farm && farm.animals != null) || (__instance is AnimalHouse animalHouse && animalHouse.animals != null))
            {
                var animals = __instance is Farm ? (__instance as Farm).animals.Values : (__instance as AnimalHouse).animals.Values;
                for (int k = animals.Count() - 1; k >= 0; k--)
                {
                    var farmAnimal = animals.ElementAt(k);
                    if (farmAnimal.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_OWNER) && farmAnimal.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME))
                    {
                        var instanceName = GetCharacterName(farmAnimal);

                        var seasonalName = String.Concat(farmAnimal.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.Character}_{instanceName}_{season}");
                        if ((farmAnimal.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_SEASON) && !String.IsNullOrEmpty(farmAnimal.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]) && !String.Equals(farmAnimal.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON], Game1.currentSeason, StringComparison.OrdinalIgnoreCase)) || AlternativeTextures.textureManager.DoesObjectHaveAlternativeTextureById(seasonalName))
                        {
                            farmAnimal.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON] = season;
                            farmAnimal.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = seasonalName;
                        }
                    }
                }
            }

            if (__instance is Farm houseFarm && __instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_OWNER) is true && __instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_NAME) is true)
            {
                var buildingType = $"Farmhouse_{Game1.MasterPlayer.HouseUpgradeLevel}";
                if (!__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME].Contains(buildingType))
                {
                    return;
                }

                var instanceName = String.Concat(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.Building}_{buildingType}");
                var instanceSeasonName = $"{instanceName}_{Game1.currentSeason}";

                if (!String.Equals(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME], instanceName, StringComparison.OrdinalIgnoreCase) && !String.Equals(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME], instanceSeasonName, StringComparison.OrdinalIgnoreCase))
                {
                    __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = String.Concat(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_OWNER], ".", $"{AlternativeTextureModel.TextureType.Building}_{buildingType}");
                    if (__instance.modData.ContainsKey(ModDataKeys.ALTERNATIVE_TEXTURE_SEASON) && !String.IsNullOrEmpty(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]))
                    {
                        __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON] = Game1.currentSeason;
                        __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME] = String.Concat(__instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_NAME], "_", __instance.modData[ModDataKeys.ALTERNATIVE_TEXTURE_SEASON]);

                        houseFarm.houseSource.Value = new Rectangle(0, 144 * (((int)Game1.MasterPlayer.houseUpgradeLevel == 3) ? 2 : ((int)Game1.MasterPlayer.houseUpgradeLevel)), 160, 144);
                        houseFarm.ApplyHousePaint();
                    }
                }
            }
        }
    }
}
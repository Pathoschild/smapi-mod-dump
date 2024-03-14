/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmAnimalVarietyRedux.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="BuildableGameLocation"/> class.</summary>
    internal class BuildableGameLocationPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the <see cref="BuildableGameLocation.isBuildingConstructed(string)"/> method.</summary>
        /// <param name="name">The name of the building to check for a constructed version.</param>
        /// <param name="__instance">The <see cref="BuildableGameLocation"/> instance being patched.</param>
        /// <param name="__result">The return value of the method being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This reimplements the original method so the passed building name can be case insensitive.</remarks>
        internal static bool IsBuildingConstructedPrefix(string name, BuildableGameLocation __instance, ref bool __result)
        {
            foreach (var building in __instance.buildings)
                if (building.buildingType.Value.ToLower() == name.ToLower() && building.daysOfConstructionLeft <= 0)
                    __result = true;

            return false;
        }

        /// <summary>The prefix for the <see cref="BuildableGameLocation.isCollidingPosition(Rectangle, xTile.Dimensions.Rectangle, bool, int, bool, Character, bool, bool, bool)"/> method.</summary>
        /// <param name="__instance">The <see cref="BuildableGameLocation"/> instance being patched.</param>
        /// <param name="__result">The return value of the method being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This reimplements the original method so the custom animal homes can be used.</remarks>
        internal static bool IsCollindingPositionPrefix(Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character, bool pathfinding, bool projectile, bool ignoreCharacterRequirement, BuildableGameLocation __instance, ref bool __result)
        {
            if (!glider && __instance.buildings.Count > 0)
            {
                var playerBox = Game1.player.GetBoundingBox();
                var animal = character as FarmAnimal;
                var isJunimo = character is JunimoHarvester;
                var isNPC = character is NPC;

                foreach (var building in __instance.buildings)
                {
                    if (!building.intersects(position) || (isFarmer && building.intersects(playerBox)))
                        continue;

                    if (animal != null)
                    {
                        var door = building.getRectForAnimalDoor();
                        door.Height += 64;

                        var buildings = new List<string>();
                        if (animal.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/buildings", out var buildingsString))
                            buildings = JsonConvert.DeserializeObject<List<string>>(buildingsString);

                        if (door.Contains(position) && buildings.Any(b => b.ToLower() == building.buildingType.Value.ToLower()))
                            continue;
                    }
                    else if (isJunimo)
                    {
                        var door = building.getRectForAnimalDoor();
                        door.Height += 64;

                        if (door.Contains(position))
                            continue;
                    }
                    else if (isNPC)
                    {
                        var door = building.getRectForHumanDoor();
                        door.Height += 64;

                        if (door.Contains(position))
                            continue;
                    }

                    __result = true;
                    return false;
                }
            }

            // call the base isCollidingPosition method
            // this approach isn't ideal but when using regular reflection and invoking the MethodInfo directly, it would call this patch (instead of the base method) resulting in a stack overflow
            // https://stackoverflow.com/questions/4357729/use-reflection-to-invoke-an-overridden-base-method/14415506#14415506
            var baseMethod = typeof(GameLocation).GetMethod("isCollidingPosition", new[] { typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool) });
            var functionPointer = baseMethod.MethodHandle.GetFunctionPointer();
            var function = (Func<Rectangle, xTile.Dimensions.Rectangle, bool, int, bool, Character, bool, bool, bool, bool>)Activator.CreateInstance(typeof(Func<Rectangle, xTile.Dimensions.Rectangle, bool, int, bool, Character, bool, bool, bool, bool>), __instance, functionPointer);
            __result = function.Invoke(position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding, projectile, ignoreCharacterRequirement);
            return false;
        }
    }
}

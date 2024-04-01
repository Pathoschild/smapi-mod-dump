/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;

namespace SinZational_Science_Showcase
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            //helper.Events.Input.ButtonReleased += Input_ButtonReleased;
            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.Patch(AccessTools.Method(typeof(Hat), nameof(Hat.draw)), transpiler: new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(HarmonyPatches.Hat__Draw__Transpiler))));
        }

        /*
        private void Input_ButtonReleased(object sender, StardewModdingAPI.Events.ButtonReleasedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (Game1.activeClickableMenu != null) return;
            if (e.Button != SButton.F5) return;

            var coop = Game1.getFarm().buildings.Where(b => b.buildingType.Value == "Deluxe Coop").First();
            var coopInterior = coop.indoors.Value as AnimalHouse;
            var autoGrabber = coopInterior.objects.Values.Where(o => o.QualifiedItemId == "(BC)165").First();
            var autoGrabberStorage = autoGrabber.heldObject.Value as Chest;
            var dayCounter = 0;
            var season = Game1.season;
            do
            {
                Monitor.Log($"Day: {Game1.stats.DaysPlayed} ({season} {Game1.dayOfMonth}), Duck Feather count: {autoGrabberStorage.Items.CountId("(O)444")}, Rabbits Feet count: {autoGrabberStorage.Items.CountId("(O)446")}", LogLevel.Info);
                Game1.stats.DaysPlayed++;
                Game1.dayOfMonth++;
                if (Game1.dayOfMonth > 28)
                {
                    Game1.dayOfMonth = 1;
                    season++;
                    if (season > Season.Winter)
                    {
                        season = Season.Spring;
                    }
                }
                Game1.getFarm().tryToAddHay(20);
                foreach (var animal in coopInterior.animals.Values)
                {
                    animal.wasPet.Value = true;
                    // Eating outside
                    if (season != Season.Winter)
                    {
                        animal.friendshipTowardFarmer.Value += 8;
                    }
                }
                coopInterior.DayUpdate((int)((Game1.stats.DaysPlayed % 28) + 1));
                if (dayCounter++ > 1000)
                {
                    Monitor.Log("After 1000 simulations, did not show??", LogLevel.Warn);
                    autoGrabber.checkForAction(Game1.player);
                    break;
                }
            }
            while (autoGrabberStorage.Items.CountId("(O)444") == 0);
        }
        */

    }

    public static class HarmonyPatches
    {
        public static IEnumerable<CodeInstruction> Hat__Draw__Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var output = new List<CodeInstruction>();
            var skipCount = 0;
            foreach (var instruction in instructions)
            {
                if (skipCount-- > 0) continue;
                /*
                 * Replace the following
                 * 	IL_001f: ldsfld class StardewValley.LocalizedContentManager StardewValley.Game1::content
	             *  IL_0024: ldstr "Characters\\Farmer\\hats_animals"
	             *  IL_0029: callvirt instance !!0 StardewValley.LocalizedContentManager::Load<class [MonoGame.Framework]Microsoft.Xna.Framework.Graphics.Texture2D>(string)
	             * with
	             * ldloc_0
	             * call HarmonyPatches::Hat__Draw__GetAnimalHat
                 */
                if (instruction.opcode == OpCodes.Ldsfld)
                {
                    var load = new CodeInstruction(OpCodes.Ldloc_0)
                    {
                        labels = instruction.labels
                    };
                    output.Add(load);
                    output.Add(new CodeInstruction(OpCodes.Call, typeof(HarmonyPatches).GetMethod(nameof(Hat__Draw__GetAnimalHat))));
                    skipCount = 2;
                    continue;
                }
                output.Add(instruction);
            }
            return output;
        }

        public static Texture2D Hat__Draw__GetAnimalHat(ParsedItemData itemData)
        {
            var rawData = itemData.RawData as string[];
            ArgUtility.TryGetOptional(rawData, 8, out var textureName, out _, itemData.TextureName);
            if (textureName == "Characters\\Farmer\\hats")
            {
                textureName = "Characters\\Farmer\\hats_animals";
            }
            return Game1.content.Load<Texture2D>(textureName);
        }
    }
}

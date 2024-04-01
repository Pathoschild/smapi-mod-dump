/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace ForageFantasy
{
    using HarmonyLib;
    using Microsoft.Xna.Framework;
    using StardewValley;
    using StardewValley.TerrainFeatures;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using StardewObject = StardewValley.Object;

    internal class AutomateCompatibility
    {
        private static ForageFantasy mod;

        internal static void ApplyPatches(ForageFantasy forageFantasy, Harmony harmony)
        {
            mod = forageFantasy;

            try
            {
                mod.DebugLog("This mod patches Automate. If you notice issues with Automate, make sure it happens without this mod before reporting it to the Automate page.");

                var tapper = AccessTools.TypeByName("Pathoschild.Stardew.Automate.Framework.Machines.Objects.TapperMachine");
                var berryBush = AccessTools.TypeByName("Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine");
                var mushroomBox = AccessTools.TypeByName("Pathoschild.Stardew.Automate.Framework.MachineWrapper");

                harmony.Patch(
                   original: AccessTools.Method(tapper, "GetOutput"),
                   prefix: new HarmonyMethod(typeof(AutomateCompatibility), nameof(PatchTapperMachineOutput)));

                harmony.Patch(
                   original: AccessTools.Method(mushroomBox, "GetOutput"),
                   prefix: new HarmonyMethod(typeof(AutomateCompatibility), nameof(PatchMushroomBoxMachineOutput)));

                harmony.Patch(
                   original: AccessTools.Method(berryBush, "GetOutput"),
                   transpiler: new HarmonyMethod(typeof(AutomateCompatibility), nameof(TranspileBushMachineQuality)));
            }
            catch (Exception e)
            {
                mod.ErrorLog($"Error while trying to patch Automate. Please report this to the mod page of {mod.ModManifest.Name}, not Automate:", e);
            }
        }

        public static void PatchMushroomBoxMachineOutput(object __instance)
        {
            var machineTypeID = mod.Helper.Reflection.GetProperty<string>(__instance, "MachineTypeID").GetValue();

            if (machineTypeID != "MushroomBox")
            {
                return;
            }

            var location = mod.Helper.Reflection.GetProperty<GameLocation>(__instance, "Location").GetValue();
            var tileArea = mod.Helper.Reflection.GetProperty<Rectangle>(__instance, "TileArea").GetValue();

            StardewObject mushroomBox = location.getObjectAtTile((int)tileArea.X, (int)tileArea.Y);

            if (!mushroomBox.IsMushroomBox())
            {
                return;
            }

            // intentionally not using getFarmerMaybeOffline because that is a waste
            var who = Game1.getFarmer(mushroomBox.owner.Value) ?? Game1.MasterPlayer;

            if (mod.Config.MushroomBoxQuality)
            {
                Random r = Utility.CreateDaySaveRandom(mushroomBox.TileLocation.X, mushroomBox.TileLocation.Y * 777f);
                mushroomBox.heldObject.Value.Quality = ForageFantasy.DetermineForageQuality(who, r);
            }
        }

        public static void PatchTapperMachineOutput(object __instance)
        {
            var tapper = mod.Helper.Reflection.GetProperty<StardewObject>(__instance, "Machine").GetValue();

            // intentionally not using getFarmerMaybeOffline because that is a waste
            var who = Game1.getFarmer(tapper.owner.Value) ?? Game1.MasterPlayer;

            // if tapper quality feature is disabled
            if (mod.Config.TapperQualityOptions <= 0 || mod.Config.TapperQualityOptions > 4)
            {
                return;
            }
            var tree = mod.Helper.Reflection.GetField<Tree>(__instance, "Tree").GetValue();

            if (tree != null)
            {
                tapper.heldObject.Value.Quality = TapperAndMushroomQualityLogic.DetermineTapperQuality(mod.Config, who, tree);
            }
        }

        public static int DetermineForageQuality(Farmer farmer)
        {
            return ForageFantasy.DetermineForageQuality(farmer, Game1.random);
        }

        public static IEnumerable<CodeInstruction> TranspileBushMachineQuality(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var instructionsList = instructions.ToList();

                var getFarmer = typeof(Game1).GetProperty(nameof(Game1.player)).GetGetMethod();
                var getFixedQuality = typeof(AutomateCompatibility).GetMethod(nameof(AutomateCompatibility.DetermineForageQuality), new Type[] { typeof(Farmer) });

                int index = -1;

                for (int i = 0; i < instructionsList.Count - 4; i++)
                {
                    if (instructionsList[i].opcode == OpCodes.Brtrue_S
                        && instructionsList[i + 1].opcode == OpCodes.Ldc_I4_0
                        && instructionsList[i + 2].opcode == OpCodes.Br_S
                        && instructionsList[i + 3].opcode == OpCodes.Ldc_I4_4
                        && instructionsList[i + 4].opcode == OpCodes.Stloc_1)
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1)
                {
                    return instructions;
                }

                instructionsList[index].opcode = OpCodes.Pop;
                instructionsList[index].operand = null;

                instructionsList[index + 1].opcode = OpCodes.Call;
                instructionsList[index + 1].operand = getFarmer;

                instructionsList[index + 2].opcode = OpCodes.Call;
                instructionsList[index + 2].operand = getFixedQuality;

                instructionsList[index + 3].opcode = OpCodes.Nop;
                instructionsList[index + 3].operand = null;

                return instructionsList;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a transpiler patch", e);
                return instructions;
            }
        }
    }
}
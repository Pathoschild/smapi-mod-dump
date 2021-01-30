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
    using Harmony;
    using StardewValley;
    using StardewValley.TerrainFeatures;
    using System;
    using System.Reflection;
    using StardewObject = StardewValley.Object;

    internal class Patcher
    {
        private static ForageFantasy mod;

        public static void PatchAll(ForageFantasy forageFantasy)
        {
            mod = forageFantasy;

            var harmony = HarmonyInstance.Create(mod.ModManifest.UniqueID);

            try
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(StardewObject), "checkForAction"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchTapperAndMushroomQuality))
                );

                harmony.Patch(
                   original: AccessTools.Method(typeof(Crop), "getRandomWildCropForSeason"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchSummerWildSeedResult))
                );

                harmony.Patch(
                   original: AccessTools.Method(typeof(Bush), "shake"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(DetectHarvestableBerryBush))
                );

                harmony.Patch(
                   original: AccessTools.Method(typeof(Bush), "shake"),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(FixBerryQuality))
                );
            }
            catch (Exception e)
            {
                mod.ErrorLog("Error while trying to setup required patches:", e);
            }

            if (mod.Helper.ModRegistry.IsLoaded("Pathoschild.Automate"))
            {
                try
                {
                    mod.DebugLog("This mod patches Automate. If you notice issues with Automate, make sure it happens without this mod before reporting it to the Automate page.");

                    // this is so ugly but I can't include a reference
                    Assembly assembly = null;

                    foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (item.GetName().Name.Trim() == "Automate")
                        {
                            assembly = item;
                            break;
                        }
                    }

                    if (assembly == null)
                    {
                        mod.ErrorLog($"Error while trying to patch Automate. Please report this to the mod page of {mod.ModManifest.Name}, not Automate.");
                        return;
                    }

                    // I don't see a use in using MachineWrapper because it's also internal I need to check for the type of the machine anyway which would be way too much reflection at runtime
                    var mushroomBox = assembly.GetType("Pathoschild.Stardew.Automate.Framework.Machines.Objects.MushroomBoxMachine");
                    var tapper = assembly.GetType("Pathoschild.Stardew.Automate.Framework.Machines.Objects.TapperMachine");
                    var berryBush = assembly.GetType("Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine");

                    harmony.Patch(
                       original: AccessTools.Method(tapper, "GetOutput"),
                       prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchTapperMachineOutput))
                    );

                    harmony.Patch(
                       original: AccessTools.Method(mushroomBox, "GetOutput"),
                       prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchMushroomBoxMachineOutput))
                    );

                    harmony.Patch(
                       original: AccessTools.Method(berryBush, "GetOutput"),
                       postfix: new HarmonyMethod(typeof(Patcher), nameof(PatchPostBushMachineXP))
                    );
                }
                catch (Exception e)
                {
                    mod.ErrorLog($"Error while trying to patch Automate. Please report this to the mod page of {mod.ModManifest.Name}, not Automate:", e);
                }
            }
        }

        public static bool PatchTapperAndMushroomQuality(ref StardewObject __instance, ref Farmer who, ref bool justCheckingForActivity)
        {
            try
            {
                if (!justCheckingForActivity && __instance != null && __instance.minutesUntilReady <= 0 && __instance.heldObject != null && __instance.heldObject.Value != null)
                {
                    if (TapperAndMushroomQualityLogic.IsTapper(__instance))
                    {
                        TapperAndMushroomQualityLogic.RewardTapperExp(mod);

                        if (mod.Config.TapperQualityOptions <= 0 && mod.Config.TapperQualityOptions > 4)
                        {
                            __instance.heldObject.Value.quality.Value = 0;
                            return true;
                        }

                        TerrainFeature terrain;
                        who.currentLocation.terrainFeatures.TryGetValue(__instance.TileLocation, out terrain);

                        if (terrain != null && terrain is Tree tree)
                        {
                            __instance.heldObject.Value.quality.Value = TapperAndMushroomQualityLogic.DetermineTapperQuality(mod, who, __instance, tree);
                        }
                        else
                        {
                            __instance.heldObject.Value.quality.Value = 0;
                        }

                        return true;
                    }

                    if (TapperAndMushroomQualityLogic.IsMushroomBox(__instance))
                    {
                        TapperAndMushroomQualityLogic.RewardMushroomBoxExp(mod);

                        if (!mod.Config.MushroomBoxQuality)
                        {
                            __instance.heldObject.Value.quality.Value = 0;
                        }
                        else
                        {
                            __instance.heldObject.Value.quality.Value = ForageFantasy.DetermineForageQuality(who);
                        }

                        return true;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        public static bool PatchSummerWildSeedResult(ref int __result)
        {
            try
            {
                if (mod.Config.CommonFiddleheadFern && Game1.currentSeason == "summer")
                {
                    __result = FernAndBurgerLogic.GetWildSeedSummerForage();

                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        [HarmonyPriority(Priority.High)]
        public static bool DetectHarvestableBerryBush(ref Bush __instance, ref bool __state)
        {
            try
            {
                // if other mods also define a __state variable of type bool they will have different values (aka harmony does not make us fight over the __state variable)
                __state = BerryBushLogic.IsHarvestableBush(__instance) && __instance.tileSheetOffset == 1;

                return true;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                __state = false;
                return true;
            }
        }

        public static void FixBerryQuality(ref Bush __instance, ref bool __state)
        {
            try
            {
                // config calls are in ChangeBerryQualityAndGiveExp
                if (__state && BerryBushLogic.IsHarvestableBush(__instance) && __instance.tileSheetOffset == 0)
                {
                    var maxShake = mod.Helper.Reflection.GetField<float>(__instance, "maxShake");

                    if (maxShake.GetValue() == 0.0245436933f)
                    {
                        BerryBushLogic.ChangeBerryQualityAndGiveExp(__instance, mod);
                    }
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }

        public static bool PatchMushroomBoxMachineOutput(ref object __instance)
        {
            try
            {
                if (mod.Config.AutomateHarvestsGrantXP)
                {
                    TapperAndMushroomQualityLogic.RewardMushroomBoxExp(mod);
                }

                var mushroomBox = mod.Helper.Reflection.GetProperty<StardewObject>(__instance, "Machine").GetValue();

                if (!mod.Config.MushroomBoxQuality)
                {
                    mushroomBox.heldObject.Value.quality.Value = 0;
                }
                else
                {
                    mushroomBox.heldObject.Value.quality.Value = ForageFantasy.DetermineForageQuality(Game1.player);
                }

                return true;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        public static bool PatchTapperMachineOutput(ref object __instance)
        {
            try
            {
                if (mod.Config.AutomateHarvestsGrantXP)
                {
                    TapperAndMushroomQualityLogic.RewardTapperExp(mod);
                }

                var tapper = mod.Helper.Reflection.GetProperty<StardewObject>(__instance, "Machine").GetValue();

                var tree = mod.Helper.Reflection.GetField<Tree>(__instance, "Tree").GetValue();

                if (mod.Config.TapperQualityOptions > 0 && mod.Config.TapperQualityOptions <= 4 && tree != null && tree is Tree)
                {
                    tapper.heldObject.Value.quality.Value = TapperAndMushroomQualityLogic.DetermineTapperQuality(mod, Game1.player, tapper, tree);
                }
                else
                {
                    tapper.heldObject.Value.quality.Value = 0;
                }

                return true;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        public static void PatchPostBushMachineXP()
        {
            try
            {
                if (mod.Config.AutomateHarvestsGrantXP)
                {
                    BerryBushLogic.RewardBerryXP(mod);
                }

                // I can't give quality because I would need to get the return value of type TrackedItem, but Harmony only gives me null if I define object __result (unlike with variables)
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }
    }
}
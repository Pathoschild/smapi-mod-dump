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
    using Microsoft.Xna.Framework.Graphics;
    using Netcode;
    using StardewValley;
    using StardewValley.Menus;
    using StardewValley.TerrainFeatures;
    using System;
    using StardewObject = StardewValley.Object;

    internal class Patcher
    {
        private static ForageFantasy mod;

        public static void PatchAll(ForageFantasy forageFantasy)
        {
            mod = forageFantasy;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            try
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(StardewObject), "checkForAction"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchTapperAndMushroomQuality)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Crop), "getRandomWildCropForSeason"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchSummerWildSeedResult)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Bush), "shake"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(DetectHarvestableBerryBush)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Bush), "shake"),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(FixBerryQuality)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Tree), nameof(Tree.UpdateTapperProduct)),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(UpdateTapperProduct_Post)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Tree), "shake"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(ShakeTree)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Tree), "performSeedDestroy"),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(PerformSeedDestroy)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Billboard), nameof(Billboard.draw), new Type[] { typeof(SpriteBatch) }),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(Draw_Postfix)));
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

                    // I don't see a use in using MachineWrapper because it's also internal I need to check for the type of the machine anyway which would be way too much reflection at runtime
                    var mushroomBox = AccessTools.TypeByName("Pathoschild.Stardew.Automate.Framework.Machines.Objects.MushroomBoxMachine");
                    var tapper = AccessTools.TypeByName("Pathoschild.Stardew.Automate.Framework.Machines.Objects.TapperMachine");
                    var berryBush = AccessTools.TypeByName("Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine");

                    harmony.Patch(
                       original: AccessTools.Method(tapper, "GetOutput"),
                       prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchTapperMachineOutput)));

                    harmony.Patch(
                       original: AccessTools.Method(mushroomBox, "GetOutput"),
                       prefix: new HarmonyMethod(typeof(Patcher), nameof(PatchMushroomBoxMachineOutput)));

                    harmony.Patch(
                       original: AccessTools.Method(berryBush, "GetOutput"),
                       postfix: new HarmonyMethod(typeof(Patcher), nameof(PatchPostBushMachineXP)));
                }
                catch (Exception e)
                {
                    mod.ErrorLog($"Error while trying to patch Automate. Please report this to the mod page of {mod.ModManifest.Name}, not Automate:", e);
                }
            }

            if (mod.Helper.ModRegistry.IsLoaded("BitwiseJonMods.OneClickShedReloader"))
            {
                try
                {
                    mod.DebugLog("This mod patches OneClickShedReloader. If you notice issues with OneClickShedReloader, make sure it happens without this mod before reporting it to the OneClickShedReloader page.");

                    var handler = AccessTools.TypeByName("BitwiseJonMods.BuildingContentsHandler");
                    var entry = AccessTools.TypeByName("BitwiseJonMods.ModEntry");

                    harmony.Patch(
                       original: AccessTools.Method(handler, "TryAddItemToPlayerInventory"),
                       prefix: new HarmonyMethod(typeof(Patcher), nameof(TryAddItemToPlayerInventory_Pre)));

                    harmony.Patch(
                       original: AccessTools.Method(handler, "TryAddItemToPlayerInventory"),
                       postfix: new HarmonyMethod(typeof(Patcher), nameof(TryAddItemToPlayerInventory_Post)));

                    harmony.Patch(
                       original: AccessTools.Method(entry, "HarvestAllItemsInBuilding"),
                       postfix: new HarmonyMethod(typeof(Patcher), nameof(ReduceQualityAfterHarvest)));
                }
                catch (Exception e)
                {
                    mod.ErrorLog($"Error while trying to patch OneClickShedReloader. Please report this to the mod page of {mod.ModManifest.Name}, not OneClickShedReloader:", e);
                }
            }

            /* // in case I want this at some point
            foreach (var method in Harmony.GetAllPatchedMethods())
            {
                var patches = Harmony.GetPatchInfo(method);

                foreach (var patch in patches.Prefixes)
                {
                    if (patch.owner == mod.ModManifest.UniqueID)
                    {
                        mod.TraceLog($"method: {method.Name} prefix: {patch.PatchMethod.Name}");
                    }
                }

                foreach (var patch in patches.Postfixes)
                {
                    if (patch.owner == mod.ModManifest.UniqueID)
                    {
                        mod.TraceLog($"method: {method.Name} postfix: {patch.PatchMethod.Name}");
                    }
                }

                foreach (var patch in patches.Transpilers)
                {
                    if (patch.owner == mod.ModManifest.UniqueID)
                    {
                        mod.TraceLog($"method: {method.Name} transpiler: {patch.PatchMethod.Name}");
                    }
                }
            }
            */
        }

        private static readonly Rectangle redMushroom = new(192, 272, 16, 16);
        private static readonly Rectangle purpleMushroom = new(224, 272, 16, 16);
        private static readonly Rectangle commonMushroom = new(320, 256, 16, 16);

        public static void Draw_Postfix(Billboard __instance, SpriteBatch b, bool ___dailyQuestBoard)
        {
            try
            {
                if (!___dailyQuestBoard)
                {
                    for (int day = 2; day <= 28; day += 2)
                    {
                        if (mod.Config.MushroomTapperCalendar)
                        {
                            Rectangle toDraw = Rectangle.Empty;

                            if (Game1.currentSeason is "spring" or "summer" || (Game1.currentSeason is "winter" && mod.TappersDreamAndMushroomTreesGrowInWinter))
                            {
                                if ((day % 10) is 2)
                                {
                                    toDraw = purpleMushroom;
                                }
                                else if ((day % 10) is 4)
                                {
                                    toDraw = redMushroom;
                                }
                                else
                                {
                                    toDraw = commonMushroom;
                                }
                            }
                            else if (Game1.currentSeason is "fall")
                            {
                                if ((day % 10) is 2)
                                {
                                    toDraw = purpleMushroom;
                                }
                                else
                                {
                                    toDraw = redMushroom;
                                }
                            }

                            if (toDraw != Rectangle.Empty)
                            {
                                Utility.drawWithShadow(b, Game1.objectSpriteSheet, new Vector2((float)(__instance.calendarDays[day - 1].bounds.X + 95), (float)(__instance.calendarDays[day - 1].bounds.Y + 5) - Game1.dialogueButtonScale / 2f), toDraw, Color.White, 0f, Vector2.Zero, 2f, false, 1f, -1, -1, 0.35f);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }

        public static void PerformSeedDestroy(Tree __instance, Tool t, Vector2 tileLocation, GameLocation location, NetLong ___lastPlayerToHit)
        {
            try
            {
                if (!mod.Config.MushroomTreeSeedsDrop)
                {
                    return;
                }

                if (___lastPlayerToHit.Value == 0L && Game1.player.getEffectiveSkillLevel(2) >= 1 && __instance.treeType.Value == Tree.mushroomTree)
                {
                    // drop mushroom tree seed
                    Game1.createMultipleObjectDebris(891, (int)tileLocation.X, (int)tileLocation.Y, 1, (t == null) ? Game1.player.UniqueMultiplayerID : t.getLastFarmerToUse().UniqueMultiplayerID, location);
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }

        public static bool ShakeTree(Tree __instance, Vector2 tileLocation, bool doEvenIfStillShaking, GameLocation location, float ___maxShake)
        {
            try
            {
                if (!mod.Config.MushroomTreeSeedsDrop || __instance.treeType.Value != Tree.mushroomTree)
                {
                    return true;
                }

                if ((___maxShake == 0f || doEvenIfStillShaking) && __instance.growthStage.Value >= 5 && !__instance.stump.Value)
                {
                    if (__instance.hasSeed.Value && (Game1.IsMultiplayer || Game1.player.ForagingLevel >= 1))
                    {
                        // drop mushroom tree seed
                        Game1.createObjectDebris(891, (int)tileLocation.X, (int)tileLocation.Y - 3, ((int)tileLocation.Y + 1) * 64, 0, 1f, location);

                        __instance.hasSeed.Value = false;
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

        public static void UpdateTapperProduct_Post(Tree __instance, StardewObject tapper_instance)
        {
            try
            {
                int minutesUntilTomorrow = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay);
                int minutesUntil2Days = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, 2);
                bool isWinter = Game1.GetSeasonForLocation(__instance.currentLocation).Equals("winter");

                // just some tree menu UI compatibility
                if (isWinter && mod.TappersDreamAndMushroomTreesGrowInWinter)
                {
                    int daysUntilSpring = new WorldDate(Game1.year + 1, "spring", 1).TotalDays - Game1.Date.TotalDays;
                    int minutesUntilSpring = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, daysUntilSpring);

                    if (tapper_instance.MinutesUntilReady == minutesUntilSpring)
                    {
                        // red mushroom
                        if (tapper_instance.heldObject.Value.ParentSheetIndex is 420)
                        {
                            tapper_instance.MinutesUntilReady = minutesUntilTomorrow;
                        }
                        else if (minutesUntilSpring > minutesUntil2Days)
                        {
                            tapper_instance.MinutesUntilReady = minutesUntil2Days;
                        }
                    }
                }

                if (!mod.Config.TapperDaysNeededChangesEnabled)
                {
                    return;
                }

                float time_multiplier = 1f;

                bool isHeavyTapper = tapper_instance != null && tapper_instance.ParentSheetIndex == 264;
                if (isHeavyTapper)
                {
                    time_multiplier = 0.5f;
                }

                switch (__instance.treeType.Value)
                {
                    case 1:
                        tapper_instance.MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, (int)Math.Max(1.0, Math.Floor(mod.Config.OakDaysNeeded * time_multiplier)));
                        return;

                    case 2:
                        tapper_instance.MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, (int)Math.Max(1.0, Math.Floor(mod.Config.MapleDaysNeeded * time_multiplier)));
                        return;

                    case 3:
                        tapper_instance.MinutesUntilReady = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, (int)Math.Max(1.0, Math.Floor(mod.Config.PineDaysNeeded * time_multiplier)));
                        return;

                    case Tree.mushroomTree:

                        if (mod.Config.MushroomTreeHeavyTappersFix && isHeavyTapper && tapper_instance.MinutesUntilReady == minutesUntil2Days)
                        {
                            tapper_instance.MinutesUntilReady = minutesUntilTomorrow;
                        }

                        if (mod.Config.MushroomTreeTappersConsistencyChange)
                        {
                            if (Game1.dayOfMonth == 28)
                            {
                                tapper_instance.heldObject.Value = new StardewObject(422, 1, false, -1, 0);
                            }

                            bool isFall = Game1.GetSeasonForLocation(__instance.currentLocation).Equals("fall");
                            // brown mushroom
                            if (tapper_instance.heldObject.Value.ParentSheetIndex == 404 && isFall)
                            {
                                // red mushroom
                                tapper_instance.heldObject.Value = new StardewObject(420, 1, false, -1, 0);
                            }
                        }

                        // recalculate again to nerf moments where it would only take one day
                        if (mod.Config.MushroomTreeHeavyTappersFix && isHeavyTapper)
                        {
                            tapper_instance.MinutesUntilReady = minutesUntilTomorrow;
                        }
                        else
                        {
                            tapper_instance.MinutesUntilReady = minutesUntil2Days;
                        }
                        return;
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }

        public static bool TryAddItemToPlayerInventory_Pre(ref Farmer player, ref Item item, ref StardewObject container)
        {
            try
            {
                if (container.IsMushroomBox())
                {
                    if (mod.Config.MushroomBoxQuality)
                    {
                        (item as StardewObject).Quality = ForageFantasy.DetermineForageQuality(player);
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

        public static void TryAddItemToPlayerInventory_Post(ref StardewObject container, ref bool __result)
        {
            try
            {
                // I can't reduce the quality of a non successfully harvested box here,
                // because it doesn't get called if the method throws a inventoryfull exception
                if (__result && container.IsMushroomBox())
                {
                    if (mod.Config.AutomationHarvestsGrantXP)
                    {
                        TapperAndMushroomQualityLogic.RewardMushroomBoxExp(mod, Game1.player);
                    }
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }

        public static void ReduceQualityAfterHarvest(ref GameLocation location)
        {
            // reduce quality of non successfully harvested items and reset in general
            try
            {
                if (!mod.Config.MushroomBoxQuality)
                {
                    return;
                }

                foreach (var item in location.Objects.Values)
                {
                    if (item.IsMushroomBox())
                    {
                        if (item.heldObject.Value != null)
                        {
                            item.heldObject.Value.Quality = StardewObject.lowQuality;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }

        public static bool PatchTapperAndMushroomQuality(ref StardewObject __instance, ref Farmer who, ref bool justCheckingForActivity)
        {
            try
            {
                if (!justCheckingForActivity && __instance != null && __instance.readyForHarvest.Value && __instance.heldObject.Value != null)
                {
                    if (__instance.IsTapper())
                    {
                        TapperAndMushroomQualityLogic.RewardTapperExp(mod, who);

                        // if tapper quality feature is disabled
                        if (mod.Config.TapperQualityOptions <= 0 && mod.Config.TapperQualityOptions > 4)
                        {
                            return true;
                        }

                        who.currentLocation.terrainFeatures.TryGetValue(__instance.TileLocation, out TerrainFeature terrain);

                        if (terrain is Tree tree)
                        {
                            __instance.heldObject.Value.Quality = TapperAndMushroomQualityLogic.DetermineTapperQuality(mod, who, tree);
                        }

                        return true;
                    }

                    if (__instance.IsMushroomBox())
                    {
                        TapperAndMushroomQualityLogic.RewardMushroomBoxExp(mod, who);

                        if (mod.Config.MushroomBoxQuality)
                        {
                            __instance.heldObject.Value.Quality = ForageFantasy.DetermineForageQuality(who);
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

        public static bool PatchSummerWildSeedResult(ref int __result, ref string season)
        {
            try
            {
                if (mod.Config.CommonFiddleheadFern && season == "summer")
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

        // set to high so it hopefully catches that the tileSheetOffset before some other mod wants to harvest this bush in prepatch
        // if other mods also define a __state variable of type bool they will have different values (aka harmony does not make us fight over the __state variable)
        [HarmonyPriority(Priority.High)]
        public static bool DetectHarvestableBerryBush(ref Bush __instance, ref bool __state)
        {
            try
            {
                // tileSheetOffset == 1 means it currently has berries to harvest
                __state = BerryBushLogic.IsHarvestableBush(__instance) && __instance.tileSheetOffset.Value == 1;

                return true;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                __state = false;
                return true;
            }
        }

        // config calls are in ChangeBerryQualityAndGiveExp
        public static void FixBerryQuality(ref Bush __instance, ref bool __state, float ___maxShake)
        {
            try
            {
                // __state && tileSheetOffset == 0 means the bush was harvested between prepatch and this
                if (__state && BerryBushLogic.IsHarvestableBush(__instance) && __instance.tileSheetOffset.Value == 0)
                {
                    if (___maxShake == 0.0245436933f)
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
                var mushroomBox = mod.Helper.Reflection.GetProperty<StardewObject>(__instance, "Machine").GetValue();

                // intentionally not using getFarmerMaybeOffline because that is a waste
                var who = Game1.getFarmer(mushroomBox.owner.Value) ?? Game1.MasterPlayer;

                if (mod.Config.AutomationHarvestsGrantXP)
                {
                    TapperAndMushroomQualityLogic.RewardMushroomBoxExp(mod, who);
                }

                if (mod.Config.MushroomBoxQuality)
                {
                    mushroomBox.heldObject.Value.Quality = ForageFantasy.DetermineForageQuality(who);
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
                var tapper = mod.Helper.Reflection.GetProperty<StardewObject>(__instance, "Machine").GetValue();

                // intentionally not using getFarmerMaybeOffline because that is a waste
                var who = Game1.getFarmer(tapper.owner.Value) ?? Game1.MasterPlayer;

                if (mod.Config.AutomationHarvestsGrantXP)
                {
                    TapperAndMushroomQualityLogic.RewardTapperExp(mod, who);
                }

                // if tapper quality feature is disabled
                if (mod.Config.TapperQualityOptions <= 0 || mod.Config.TapperQualityOptions > 4)
                {
                    return true;
                }

                var tree = mod.Helper.Reflection.GetField<Tree>(__instance, "Tree").GetValue();

                if (tree != null)
                {
                    tapper.heldObject.Value.Quality = TapperAndMushroomQualityLogic.DetermineTapperQuality(mod, who, tree);
                }

                return true;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        public static void PatchPostBushMachineXP(ref object __instance)
        {
            try
            {
                if (mod.Config.AutomationHarvestsGrantXP)
                {
                    var bush = mod.Helper.Reflection.GetProperty<Bush>(__instance, "Machine").GetValue();

                    if (bush.size.Value != Bush.greenTeaBush && bush.size.Value != Bush.walnutBush)
                    {
                        BerryBushLogic.RewardBerryXP(mod);
                    }
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
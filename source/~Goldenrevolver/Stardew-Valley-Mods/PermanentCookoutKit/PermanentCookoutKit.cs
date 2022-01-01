/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace PermanentCookoutKit
{
    using HarmonyLib;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.BellsAndWhistles;
    using StardewValley.Network;
    using StardewValley.Objects;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using StardewObject = StardewValley.Object;

    public class PermanentCookoutKit : Mod, IAssetEditor
    {
        private const int WoodID = 388;
        private const int DriftwoodID = 169;
        private const int HardwoodID = 709;
        private const int CoalID = 382;
        private const int FiberID = 771;

        private static PermanentCookoutKit mod;

        private CookoutKitConfig config;

        public static bool PatchCharcoalKiln(ref object __instance)
        {
            try
            {
                var recipesProp = __instance.GetType().GetField("Recipes", BindingFlags.NonPublic | BindingFlags.Instance);
                object[] recipes = (object[])recipesProp.GetValue(__instance);

                object oldWoodRecipe = recipes[0];

                var constructor = oldWoodRecipe.GetType().GetConstructor(new Type[] { typeof(int), typeof(int), typeof(Func<Item, StardewObject>), typeof(int) });

                Func<Item, StardewObject> output = input => new StardewObject(CoalID, 1);

                var woodRecipe = constructor.Invoke(new object[] { WoodID, mod.config.CharcoalKilnWoodNeeded, output, mod.config.CharcoalKilnTimeNeeded });

                object driftwoodRecipe = null;
                object hardwoodRecipe = null;

                int count = 1;

                if (mod.config.DriftwoodMultiplier > 0)
                {
                    count++;
                    driftwoodRecipe = constructor.Invoke(new object[] { DriftwoodID, CountWithMultiplier(mod.config.CharcoalKilnWoodNeeded, mod.config.DriftwoodMultiplier), output, mod.config.CharcoalKilnTimeNeeded });
                }

                if (mod.config.HardwoodMultiplier > 0)
                {
                    count++;
                    hardwoodRecipe = constructor.Invoke(new object[] { HardwoodID, CountWithMultiplier(mod.config.CharcoalKilnWoodNeeded, mod.config.HardwoodMultiplier), output, mod.config.CharcoalKilnTimeNeeded });
                }

                Array resultArray = Array.CreateInstance(woodRecipe.GetType(), count);

                resultArray.SetValue(woodRecipe, 0);

                if (mod.config.DriftwoodMultiplier > 0)
                {
                    resultArray.SetValue(driftwoodRecipe, 1);
                }

                if (mod.config.HardwoodMultiplier > 0)
                {
                    resultArray.SetValue(hardwoodRecipe, count - 1);
                }

                recipesProp.SetValue(__instance, resultArray);

                return true;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        public static void Draw_Post(Torch __instance, SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            try
            {
                if (!Game1.eventUp || (Game1.currentLocation != null && Game1.currentLocation.currentEvent != null && Game1.currentLocation.currentEvent.showGroundObjects) || Game1.currentLocation.IsFarm)
                {
                    float draw_layer = Math.Max(0f, (((y + 1) * 64) - 24) / 10000f) + (x * 1E-05f);

                    // draw the upper half of the cookout kit even if isOn == false
                    if (__instance.ParentSheetIndex == 278 && !__instance.IsOn)
                    {
                        Rectangle r = StardewObject.getSourceRectForBigCraftable(__instance.ParentSheetIndex + 1);
                        r.Height -= 16;
                        Vector2 scaleFactor = __instance.getScale();
                        scaleFactor *= 4f;
                        Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, (y * 64) - 64 + 12));
                        var destination = new Rectangle((int)(position.X - (scaleFactor.X / 2f)) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(position.Y - (scaleFactor.Y / 2f)) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(64f + scaleFactor.X), (int)(64f + (scaleFactor.Y / 2f)));
                        spriteBatch.Draw(Game1.bigCraftableSpriteSheet, destination, new Rectangle?(r), Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, draw_layer + 0.0028f);
                    }
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }

        public static void UpdateWhenCurrentLocation_Post(Torch __instance, GameLocation environment)
        {
            try
            {
                // remove the smoke from cookout kits that are off
                if (__instance.ParentSheetIndex == 278 && !__instance.IsOn)
                {
                    // the condition for smoke to spawn in the overridden method
                    if (mod.Helper.Reflection.GetField<float>(__instance, "smokePuffTimer").GetValue() == 1000f)
                    {
                        // make sure it really is the smoke that was just spawned and then remove it
                        if (environment.temporarySprites.Any() && environment.temporarySprites.Last().initialPosition == (__instance.TileLocation * 64f) + new Vector2(32f, -32f))
                        {
                            environment.temporarySprites.RemoveAt(environment.temporarySprites.Count - 1);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
            }
        }

        public static bool UpdateCharcoalKilnInput(ref StardewObject __instance, ref bool __result, ref Item dropInItem, ref bool probe, ref Farmer who)
        {
            try
            {
                if (__instance.name.Equals("Charcoal Kiln"))
                {
                    if (__instance.isTemporarilyInvisible)
                    {
                        __result = false;
                        return false;
                    }

                    if (!(dropInItem is StardewObject))
                    {
                        __result = false;
                        return false;
                    }

                    StardewObject dropIn = dropInItem as StardewObject;

                    if (dropInItem is Wallpaper)
                    {
                        __result = false;
                        return false;
                    }

                    if (__instance.heldObject.Value != null)
                    {
                        __result = false;
                        return false;
                    }

                    if (dropIn != null && dropIn.bigCraftable.Value)
                    {
                        __result = false;
                        return false;
                    }

                    if (__instance.bigCraftable.Value && !probe && dropIn != null && __instance.heldObject.Value == null)
                    {
                        __instance.scale.X = 5f;
                    }

                    if (probe && __instance.MinutesUntilReady > 0)
                    {
                        __result = false;
                        return false;
                    }

                    int consumeCount = -1;

                    switch (dropIn.ParentSheetIndex)
                    {
                        case DriftwoodID:
                            if (mod.config.DriftwoodMultiplier > 0 && dropIn.Stack >= CountWithMultiplier(mod.config.CharcoalKilnWoodNeeded, mod.config.DriftwoodMultiplier))
                            {
                                consumeCount = CountWithMultiplier(mod.config.CharcoalKilnWoodNeeded, mod.config.DriftwoodMultiplier);
                            }

                            break;

                        case WoodID:
                            if (dropIn.Stack >= mod.config.CharcoalKilnWoodNeeded)
                            {
                                consumeCount = mod.config.CharcoalKilnWoodNeeded;
                            }

                            break;

                        case HardwoodID:
                            if (mod.config.HardwoodMultiplier > 0 && dropIn.Stack >= CountWithMultiplier(mod.config.CharcoalKilnWoodNeeded, mod.config.HardwoodMultiplier))
                            {
                                consumeCount = CountWithMultiplier(mod.config.CharcoalKilnWoodNeeded, mod.config.HardwoodMultiplier);
                            }

                            break;

                        default:
                            break;
                    }

                    if (who.IsLocalPlayer && consumeCount == -1)
                    {
                        if (!probe && who.IsLocalPlayer && StardewObject.autoLoadChest == null)
                        {
                            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12783"));
                        }

                        __result = false;
                        return false;
                    }

                    if (__instance.heldObject.Value == null && consumeCount != -1)
                    {
                        if (!probe)
                        {
                            __instance.ConsumeInventoryItem(who, dropIn, consumeCount);
                            who.currentLocation.playSound("openBox", NetAudio.SoundContext.Default);
                            DelayedAction.playSoundAfterDelay("fireball", 50, null, -1);
                            __instance.showNextIndex.Value = true;

                            var multiplayer = (Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

                            var tempSprite = new TemporaryAnimatedSprite(27, (__instance.TileLocation * 64f) + new Vector2(-16f, -128f), Color.White, 4, false, 50f, 10, 64, ((__instance.TileLocation.Y + 1f) * 64f / 10000f) + 0.0001f, -1, 0)
                            {
                                alphaFade = 0.005f
                            };

                            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[] { tempSprite });

                            __instance.heldObject.Value = new StardewObject(CoalID, 1, false, -1, 0);
                            __instance.MinutesUntilReady = mod.config.CharcoalKilnTimeNeeded;
                        }
                        else
                        {
                            __instance.heldObject.Value = new StardewObject();

                            __result = true;
                            return false;
                        }
                    }

                    __result = false;
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

        public static bool CheckForAction_Pre(Torch __instance, Farmer who, bool justCheckingForActivity)
        {
            try
            {
                if (justCheckingForActivity)
                {
                    return true;
                }

                // check for ignition
                if (__instance.ParentSheetIndex == 278 && !__instance.IsOn && who != null)
                {
                    int coalCount = mod.config.CoalNeeded;
                    int baseKindlingCount = mod.config.FiberNeeded;
                    int baseWoodCount = mod.config.WoodNeeded;

                    float driftwoodMult = mod.config.DriftwoodMultiplier;
                    float hardwoodMult = mod.config.HardwoodMultiplier;

                    bool hasCoal = who.hasItemInInventory(CoalID, coalCount);

                    bool hasKindling = false;
                    int kindlingID = -1;
                    int actualKindlingCount = -1;

                    if (hasCoal)
                    {
                        if (!hasKindling)
                        {
                            // soggy newspaper
                            hasKindling = CheckForResource(who, 172, baseKindlingCount, mod.config.NewspaperMultiplier, ref kindlingID, ref actualKindlingCount);
                        }

                        if (!hasKindling)
                        {
                            // fiber
                            hasKindling = CheckForResource(who, FiberID, baseKindlingCount, 1, ref kindlingID, ref actualKindlingCount);
                        }

                        if (!hasKindling)
                        {
                            // wool
                            hasKindling = CheckForResource(who, 440, baseKindlingCount, mod.config.WoolMultiplier, ref kindlingID, ref actualKindlingCount);
                        }

                        if (!hasKindling)
                        {
                            // cloth
                            hasKindling = CheckForResource(who, 428, baseKindlingCount, mod.config.ClothMultiplier, ref kindlingID, ref actualKindlingCount);
                        }
                    }

                    bool hasWood = false;
                    int chosenWoodID = -1;
                    int actualWoodCount = -1;

                    if (hasCoal && hasKindling)
                    {
                        if (!hasWood)
                        {
                            // driftwood
                            hasWood = CheckForResource(who, DriftwoodID, baseKindlingCount, mod.config.DriftwoodMultiplier, ref chosenWoodID, ref actualWoodCount);
                        }

                        if (!hasWood)
                        {
                            // wood
                            hasWood = CheckForResource(who, WoodID, baseKindlingCount, 1, ref chosenWoodID, ref actualWoodCount);
                        }

                        if (!hasWood)
                        {
                            // hardwood
                            hasWood = CheckForResource(who, HardwoodID, baseKindlingCount, mod.config.HardwoodMultiplier, ref chosenWoodID, ref actualWoodCount);
                        }
                    }

                    if (hasCoal && hasKindling && hasWood)
                    {
                        who.removeItemsFromInventory(CoalID, coalCount);
                        who.removeItemsFromInventory(kindlingID, actualKindlingCount);
                        who.removeItemsFromInventory(chosenWoodID, actualWoodCount);

                        __instance.IsOn = true;

                        if (__instance.bigCraftable.Value)
                        {
                            Game1.playSound("fireball");

                            __instance.initializeLightSource(__instance.TileLocation, false);
                            AmbientLocationSounds.addSound(__instance.TileLocation, 1);
                        }
                    }
                    else
                    {
                        var coal = new StardewObject(CoalID, 0);
                        var fiber = new StardewObject(FiberID, 0);
                        var wood = new StardewObject(WoodID, 0);

                        Game1.showRedMessage($"{coalCount} {coal.DisplayName}, {baseKindlingCount} {fiber.DisplayName}, {baseWoodCount} {wood.DisplayName}");
                    }

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

        public override void Entry(IModHelper helper)
        {
            mod = this;

            config = Helper.ReadConfig<CookoutKitConfig>();

            CookoutKitConfig.VerifyConfigValues(config, this);

            Helper.Events.GameLoop.GameLaunched += delegate { CookoutKitConfig.SetUpModConfigMenu(config, this); };

            Helper.Events.GameLoop.DayEnding += delegate { SaveCookingKits(); };

            var harmony = new Harmony(ModManifest.UniqueID);

            try
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(Torch), "draw", new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
                   postfix: new HarmonyMethod(typeof(PermanentCookoutKit), nameof(Draw_Post)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Torch), "updateWhenCurrentLocation"),
                   postfix: new HarmonyMethod(typeof(PermanentCookoutKit), nameof(UpdateWhenCurrentLocation_Post)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Torch), "checkForAction"),
                   prefix: new HarmonyMethod(typeof(PermanentCookoutKit), nameof(CheckForAction_Pre)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(StardewObject), "performObjectDropInAction", new[] { typeof(Item), typeof(bool), typeof(Farmer) }),
                   prefix: new HarmonyMethod(typeof(PermanentCookoutKit), nameof(UpdateCharcoalKilnInput)));
            }
            catch (Exception e)
            {
                ErrorLog("Error while trying to setup required patches:", e);
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
                    var charcoalKiln = assembly.GetType("Pathoschild.Stardew.Automate.Framework.Machines.Objects.CharcoalKilnMachine");

                    harmony.Patch(
                       original: AccessTools.Method(charcoalKiln, "SetInput"),
                       prefix: new HarmonyMethod(typeof(PermanentCookoutKit), nameof(PatchCharcoalKiln)));
                }
                catch (Exception e)
                {
                    mod.ErrorLog($"Error while trying to patch Automate. Please report this to the mod page of {mod.ModManifest.Name}, not Automate:", e);
                }
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data/CraftingRecipes");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/CraftingRecipes"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                data["Cookout Kit"] = "390 10 388 10 771 10 382 3 335 1/Field/926/false/Foraging 6/Cookout Kit";
            }
        }

        public void DebugLog(object o)
        {
            Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        public void ErrorLog(object o, Exception e = null)
        {
            string baseMessage = o == null ? "null" : o.ToString();

            string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";

            Monitor.Log(baseMessage + errorMessage, LogLevel.Error);
        }

        private static int CountWithMultiplier(int baseCount, float multiplier)
        {
            if (multiplier > 0)
            {
                return (int)Math.Ceiling(baseCount / multiplier);
            }
            else
            {
                return -1;
            }
        }

        private static void SaveCookingKits()
        {
            // all locations except repeatedly buildable farm buildings
            foreach (var location in Game1.locations)
            {
                foreach (var item in location.Objects.Values)
                {
                    if (item != null && item.ParentSheetIndex == 278)
                    {
                        // extinguishes the fire, does not truly remove the object
                        item.performRemoveAction(item.TileLocation, location);

                        item.destroyOvernight = false;
                    }
                }
            }

            // repeatedly buildable farm buildings
            if (Game1.getFarm() != null)
            {
                foreach (var building in Game1.getFarm().buildings)
                {
                    var interior = building.indoors.Value;
                    if (interior != null)
                    {
                        foreach (var item in interior.Objects.Values)
                        {
                            if (item != null && item.ParentSheetIndex == 278)
                            {
                                // extinguishes the fire, does not truly remove the object
                                item.performRemoveAction(item.TileLocation, building.indoors.Value);

                                item.destroyOvernight = false;
                            }
                        }
                    }
                }
            }

            // this differentiation is also done a lot in the base game code, e.g. in Game1.getCharacterFromName in Game1.cs
        }

        private static bool CheckForResource(Farmer who, int id, int baseCount, float multiplier, ref int idToSet, ref int kindlingToRemove)
        {
            if (multiplier > 0)
            {
                int count = CountWithMultiplier(baseCount, multiplier);

                if (who.hasItemInInventory(id, count))
                {
                    idToSet = id;
                    kindlingToRemove = count;
                    return true;
                }
            }

            return false;
        }
    }
}
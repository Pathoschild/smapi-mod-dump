/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using SObject = StardewValley.Object;
using ProducerFrameworkMod.ContentPack;
using ProducerFrameworkMod;
using StardewValley.Network;
using System.Reflection.Emit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MassProduction.VanillaOverrides;

namespace MassProduction
{
    /// <summary>
    /// Contains the methods to patch in to the game to support the new machines.
    /// </summary>
    public class ObjectOverrides
    {
        /// <summary>
        /// Checks if a recipe can be made when an item is dropped in a machine or if the item can upgrade the machine to a mass producer.
        /// Code adapted from https://github.com/Digus/StardewValleyMods/blob/master/ProducerFrameworkMod/ObjectOverrides.cs
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="dropInItem"></param>
        /// <param name="probe"></param>
        /// <param name="who"></param>
        /// <param name="__result"></param>
        /// <returns></returns>
        [HarmonyPriority(801)] //Just before ProducerFrameworkMod. Can't use HarmonyBefore attribute, that wasn't working for some reason
        internal static bool PerformObjectDropInAction(SObject __instance, Item dropInItem, bool probe, Farmer who, ref bool __result)
        {
            if (__instance.isTemporarilyInvisible || !(dropInItem is SObject)) { return false; }

            SObject input = dropInItem as SObject;
            bool failLocationCondition = false;
            bool failSeasonCondition = false;

            MPMSettings upgradeSettings = ModEntry.GetSettingsFromItem(input.name);

            if (upgradeSettings != null)
            {
                if (!probe)
                {
                    //Change the machine's mass producer settings
                    MassProductionMachineDefinition mpm = ModEntry.GetMPMMachine(__instance.name, upgradeSettings.Key);

                    if (mpm == null)
                    {
                        Game1.showRedMessage("This cannot take that upgrade.");
                    }
                    else
                    {
                        string oldProducerKey = __instance.GetMassProducerKey();

                        if (!string.IsNullOrEmpty(oldProducerKey))
                        {
                            string upgradeItemName = ModEntry.MPMSettings[oldProducerKey].UpgradeObject;
                            JsonAssets.Api jsonAssets = ModEntry.Instance.Helper.ModRegistry.GetApi("spacechase0.JsonAssets") as JsonAssets.Api;
                            int upgradeItemId = jsonAssets.GetObjectId(upgradeItemName);

                            Game1.createItemDebris(new SObject(upgradeItemId, 1), __instance.TileLocation * Game1.tileSize, 0, who.currentLocation);
                        }

                        __instance.SetMassProducerKey(upgradeSettings.Key);
                        input.Stack -= 1;
                        __result = input.Stack <= 0;

                        return false;
                    }
                }
            }
            else
            {
                //Check if this is a valid input for the machine's use
                if (string.IsNullOrEmpty(__instance.GetMassProducerKey())) { return true; }

                if (__instance.heldObject.Value != null && !__instance.name.Equals("Crystalarium") || input.bigCraftable.Value) { return true; }

                MassProductionMachineDefinition mpm = ModEntry.GetMPMMachine(__instance.name, __instance.GetMassProducerKey());

                if (mpm == null) { return true; }

                if (StaticValues.SUPPORTED_VANILLA_MACHINES.ContainsKey(__instance.name))
                {
                    IVanillaOverride vanillaOverride = VanillaOverrideList.GetFor(__instance.name);

                    if (vanillaOverride != null)
                    {
                        bool overrideResult = vanillaOverride.Manual_PerformObjectDropInAction(__instance, input, probe, who, mpm);

                        //End early if a result has been found
                        if (overrideResult)
                        {
                            __result = input.Stack <= 0;
                            return true;
                        }
                    }
                }

                ProducerConfig baseConfig = mpm.GetBaseProducerConfig();
                GameLocation location = who.currentLocation;

                if (baseConfig != null)
                {
                    //TOREVIEW: maybe have machines that can break these conditions?
                    if (!baseConfig.CheckLocationCondition(location))
                    {
                        failLocationCondition = true;
                    }
                    if (!baseConfig.CheckSeasonCondition())
                    {
                        failSeasonCondition = true;
                    }
                    if (baseConfig.NoInputStartMode != null)
                    {
                        return true;
                    }
                }

                if (ProducerController.GetProducerItem(__instance.name, input) is ProducerRule producerRule)
                {
                    if (PFMCompatability.IsInputExcluded(producerRule, mpm, input))
                    {
                        return true;
                    }

                    if (__instance.bigCraftable.Value && !probe && __instance.heldObject.Value == null)
                    {
                        __instance.scale.X = 5f;
                    }

                    try
                    {
                        if (failLocationCondition)
                        {
                            throw new RestrictionException("Machine can't be used in this location.");
                        }
                        if (failSeasonCondition)
                        {
                            throw new RestrictionException("Machine can't be used in this season.");
                        }

                        List<InputInfo> inputAndFuelInfo = InputInfo.ConvertPFMInputs(producerRule, input);
                        PFMCompatability.ValidateIfInputsLessThanRequired(producerRule, mpm.Settings, inputAndFuelInfo, who);

                        Dictionary<int, int> fuelQuantities = new Dictionary<int, int>();

                        foreach (InputInfo inputInfo in inputAndFuelInfo)
                        {
                            if (inputInfo.IsFuel)
                            {
                                fuelQuantities.Add(inputInfo.ID, mpm.Settings.CalculateInputRequired(inputInfo));
                            }
                        }

                        Func<int, int, bool> fuelSearch = (i, q) => who.hasItemInInventory(i, fuelQuantities[i]);
                        OutputConfig outputConfig = PFMCompatability.ProduceOutput(producerRule, mpm.Settings, __instance,
                            fuelSearch, who, location, baseConfig, input, mpm.Settings.CalculateInputRequired(inputAndFuelInfo.First()), probe,
                            inputInfo: inputAndFuelInfo);

                        if (outputConfig != null)
                        {
                            if (!probe)
                            {
                                foreach (InputInfo inputInfo in inputAndFuelInfo)
                                {
                                    if (inputInfo.IsFuel)
                                    {
                                        RemoveItemsFromInventory(who, inputInfo.ID, mpm.Settings.CalculateInputRequired(inputInfo));
                                    }
                                }

                                List<InputInfo> outputConfigFuels = InputInfo.ConvertPFMInputs(outputConfig);

                                foreach (InputInfo fuel in outputConfigFuels)
                                {
                                    RemoveItemsFromInventory(who, fuel.ID, mpm.Settings.CalculateInputRequired(fuel));
                                }

                                input.Stack -= mpm.Settings.CalculateInputRequired(inputAndFuelInfo.First());
                                __result = input.Stack <= 0;
                            }
                            else
                            {
                                __result = true;
                            }
                        }
                    }
                    catch (RestrictionException e)
                    {
                        __result = false;
                        if (e.Message != null && !probe && who.IsLocalPlayer)
                        {
                            Game1.showRedMessage(e.Message);
                        }
                    }
                    return false;
                }
            }

            return (!failLocationCondition && !failSeasonCondition);
        }

        /// <summary>
        /// Copied from https://github.com/Digus/StardewValleyMods/blob/master/ProducerFrameworkMod/ObjectOverrides.cs
        /// </summary>
        /// <param name="farmer"></param>
        /// <param name="index"></param>
        /// <param name="stack"></param>
        /// <returns></returns>
        private static bool RemoveItemsFromInventory(Farmer farmer, int index, int stack)
        {
            if (farmer.hasItemInInventory(index, stack, 0))
            {
                for (int index1 = 0; index1 < farmer.items.Count; ++index1)
                {
                    if (farmer.items[index1] != null && farmer.items[index1] is SObject object1 && (object1.ParentSheetIndex == index || object1.Category == index))
                    {
                        if (farmer.items[index1].Stack > stack)
                        {
                            farmer.items[index1].Stack -= stack;
                            return true;
                        }
                        stack -= farmer.items[index1].Stack;
                        farmer.items[index1] = null;
                    }
                    if (stack <= 0)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Draws the icon of a machine upgrade on the machine if it has one.
        /// Adapted from https://github.com/Videogamers0/SDV-MachineAugmentors/blob/master/MachineAugmentors/Items/Augmentor.cs
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="alpha"></param>
        public static void Draw_Postfix(SObject __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            try
            {
                if (!string.IsNullOrEmpty(__instance.GetMassProducerKey()) && ModEntry.MPMSettings[__instance.GetMassProducerKey()] != null)
                {
                    int upgradeId = ModEntry.MPMSettings[__instance.GetMassProducerKey()].UpgradeObjectID;

                    float scale = 0.6f;
                    float drawLayer = Math.Max(0f, ((y + 1) * 64 - 24) / 10000f) + x * 1E-05f;
                    float drawLayerOffset = 1E-05f; // The SpriteBatch layer depth needs to be slightly larger to avoid z-fighting

                    Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize, y * Game1.tileSize));
                    spriteBatch.Draw(Game1.mouseCursors, position, GetPrimaryIconSourceRect(2), Color.White, 0f, Vector2.Zero, 4f * scale, SpriteEffects.None, drawLayer + drawLayerOffset);

                    Vector2 bottomLeftPosition = new Vector2(position.X, position.Y + 64f * scale);

                    Texture2D spritesheet = Game1.objectSpriteSheet;
                    Tuple<int, int> spriteCoordinates = ImageHelper.GetObjectSpritesheetIndex(spritesheet.Width, spritesheet.Height, upgradeId);
                    float iconScale = 2f;
                    Rectangle sourceRectangle = new Rectangle(spriteCoordinates.Item1, spriteCoordinates.Item2, 16, 16);
                    Vector2 spriteDestination = bottomLeftPosition + new Vector2(4f * scale, -4f * scale - sourceRectangle.Height * iconScale * scale);
                    float spriteLayerDepth = drawLayer + drawLayerOffset + 1E-04f;

                    spriteBatch.Draw(spritesheet, spriteDestination, sourceRectangle, Color.White * 1f, 0f, new Vector2(0, 0), scale * iconScale, SpriteEffects.None, spriteLayerDepth);
                }
            }
            catch
            { //Ignore errors that come up when first loading a save and can't access __instance.GetMassProducerKey()
            }
        }

        /// <summary>
        /// Returns the position of the main texture used for this item.
        /// Copied from https://github.com/Videogamers0/SDV-MachineAugmentors/blob/master/MachineAugmentors/Items/Augmentor.cs
        /// </summary>
        /// <param name="padding"></param>
        /// <returns></returns>
        private static Rectangle GetPrimaryIconSourceRect(int padding = 2)
        {
            int textureWidth = 13;
            int textureHeight = 9;
            return new Rectangle(162 - padding, 324 - padding, textureWidth + padding * 2, textureHeight + padding * 2);
        }

        /// <summary>
        /// Stops the upgrade objects from being placable on the ground.
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        public static void isPlaceable_Postfix(SObject __instance, ref bool __result)
        {
            MPMSettings upgradeSettings = ModEntry.GetSettingsFromItem(__instance.name);

            if (upgradeSettings != null)
            {
                __result = false;
            }
        }

        /// <summary>
        /// Allows for upgraded inputless machines to function.
        /// Code adapted from https://github.com/Digus/StardewValleyMods/blob/master/ProducerFrameworkMod/ObjectOverrides.cs
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="who"></param>
        /// <param name="__result"></param>
        /// <returns></returns>
        [HarmonyPriority(801)] //Just before ProducerFrameworkMod. Can't use HarmonyBefore attribute, that wasn't working for some reason
        internal static bool performDropDownAction_Prefix(SObject __instance, Farmer who, bool __result)
        {
            if (string.IsNullOrEmpty(__instance.GetMassProducerKey())) { return true; }

            MassProductionMachineDefinition mpm = ModEntry.GetMPMMachine(__instance.name, __instance.GetMassProducerKey());

            if (mpm == null) { return true; }

            if (ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig)
            {
                try
                {
                    if (!producerConfig.CheckLocationCondition(who.currentLocation))
                    {
                        throw new RestrictionException(ModEntry.Instance.Helper.Translation.Get("Message.Condition.Location"));
                    }
                    if (producerConfig.NoInputStartMode != null)
                    {
                        if (producerConfig.CheckSeasonCondition() && NoInputStartMode.Placement == producerConfig.NoInputStartMode)
                        {
                            if (ProducerController.GetProducerItem(__instance.Name, null) is ProducerRule producerRule)
                            {
                                PFMCompatability.ProduceOutput(producerRule, mpm.Settings, __instance, (i, q) => who.hasItemInInventory(i, q), 
                                    who, who.currentLocation, producerConfig);
                            }
                        }
                        return __result = false;
                    }
                }
                catch (RestrictionException e)
                {
                    if (e.Message != null && who.IsLocalPlayer)
                    {
                        Game1.showRedMessage(e.Message);
                    }
                    return __result = false;
                }
            }
            else if (StaticValues.SUPPORTED_VANILLA_MACHINES.ContainsKey(__instance.name) &&
                StaticValues.SUPPORTED_VANILLA_MACHINES[__instance.name] == InputRequirement.NoInputsOnly)
            {
                IVanillaOverride vanillaOverride = VanillaOverrideList.GetFor(__instance.name);

                if (vanillaOverride != null && vanillaOverride.Manual_PerformDropDownAction(__instance, mpm))
                {
                    return true;
                }
            }

            return true;
        }

        /// <summary>
        /// Allows for inputless machines from PFM that produce every day to function.
        /// Adapted from https://github.com/Digus/StardewValleyMods/blob/master/ProducerFrameworkMod/ObjectOverrides.cs
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        [HarmonyPriority(Priority.First + 1)] //Just before ProducerFrameworkMod. Can't use HarmonyBefore attribute, that wasn't working for some reason
        public static bool DayUpdate_Prefix(SObject __instance, GameLocation location)
        {
            if (__instance == null || string.IsNullOrEmpty(__instance.GetMassProducerKey())) { return true; }

            MassProductionMachineDefinition mpm = ModEntry.GetMPMMachine(__instance.name, __instance.GetMassProducerKey());

            if (mpm == null) { return true; }

            if (__instance.bigCraftable.Value)
            {
                if (ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig)
                {
                    if (producerConfig != null)
                    {
                        if (ProducerController.GetProducerItem(__instance.Name, null) is ProducerRule producerRule)
                        {
                            if (!producerConfig.CheckSeasonCondition() || !producerConfig.CheckLocationCondition(location))
                            {
                                ProducerRuleController.ClearProduction(__instance, location);
                                return false;
                            }
                            else if (producerConfig.NoInputStartMode != null)
                            {
                                if (producerConfig.NoInputStartMode == NoInputStartMode.DayUpdate || producerConfig.NoInputStartMode == NoInputStartMode.Placement)
                                {
                                    if (__instance.heldObject.Value == null)
                                    {
                                        try
                                        {

                                            Farmer who = Game1.getFarmer((long)__instance.owner);
                                            PFMCompatability.ProduceOutput(producerRule, mpm.Settings, __instance, 
                                                (i, q) => who.hasItemInInventory(i, q), who, who.currentLocation, producerConfig);
                                        }
                                        catch (RestrictionException)
                                        {
                                            //Does not show the restriction error since the machine is auto-starting.
                                        }
                                    }
                                }
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}

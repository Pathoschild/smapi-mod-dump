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
    using StardewValley;
    using StardewValley.BellsAndWhistles;
    using StardewValley.Extensions;
    using StardewValley.ItemTypeDefinitions;
    using StardewValley.Tools;
    using System;
    using System.Linq;
    using StardewObject = StardewValley.Object;

    internal class Patcher
    {
        internal const string WoodID = "(O)388";
        internal const string DriftwoodID = "(O)169";
        internal const string HardwoodID = "(O)709";
        internal const string CoalID = "(O)382";
        internal const string FiberID = "(O)771";

        private static PermanentCookoutKit mod;

        public static void PatchAll(PermanentCookoutKit permanentCookout)
        {
            mod = permanentCookout;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            try
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(Torch), nameof(Torch.draw), new[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(Draw_Post)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(StardewObject), nameof(StardewObject.performToolAction)),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(PerformToolAction_Post)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Torch), nameof(Torch.updateWhenCurrentLocation)),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(UpdateWhenCurrentLocation_Post)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Torch), nameof(Torch.checkForAction)),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(CheckForAction_Pre)));
            }
            catch (Exception e)
            {
                mod.ErrorLog("Error while trying to setup required patches:", e);
            }
        }

        // based on 'IsOn' case of Torch.Draw(SpriteBatch, int, int, float)
        public static void Draw_Post(Torch __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            if (!Game1.eventUp || Game1.currentLocation?.currentEvent?.showGroundObjects == true || Game1.currentLocation.IsFarm)
            {
                float draw_layer = Math.Max(0f, (((y + 1) * 64) - 24) / 10000f) + (x * 1E-05f);

                // draw the upper half of the cookout kit even if isOn == false
                if (__instance.IsCookoutKit() && !__instance.IsOn)
                {
                    ParsedItemData itemData2 = ItemRegistry.GetDataOrErrorItem(__instance.QualifiedItemId);
                    Rectangle r = itemData2.GetSourceRect(1, new int?(__instance.ParentSheetIndex)).Clone();
                    r.Height -= 16;
                    Vector2 scaleFactor = __instance.getScale();
                    scaleFactor *= 4f;
                    Vector2 position = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, (y * 64) - 64 + 12));
                    var destination = new Rectangle((int)(position.X - (scaleFactor.X / 2f)) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(position.Y - (scaleFactor.Y / 2f)) + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (int)(64f + scaleFactor.X), (int)(64f + (scaleFactor.Y / 2f)));
                    spriteBatch.Draw(itemData2.GetTexture(), destination, new Rectangle?(r), Color.White * alpha, 0f, Vector2.Zero, SpriteEffects.None, draw_layer + 0.0028f);
                }
            }
        }

        public static void UpdateWhenCurrentLocation_Post(Torch __instance, float ___smokePuffTimer)
        {
            GameLocation environment = __instance.Location;

            // remove the smoke from cookout kits that are off
            if (environment != null && __instance.IsCookoutKit() && !__instance.IsOn)
            {
                // the condition for smoke to spawn in the overridden method
                if (___smokePuffTimer == 1000f)
                {
                    // make sure it really is the smoke that was just spawned and then remove it
                    if (environment.temporarySprites.Any() && environment.temporarySprites.Last().initialPosition == (__instance.TileLocation * 64f) + new Vector2(32f, -32f))
                    {
                        environment.temporarySprites.RemoveAt(environment.temporarySprites.Count - 1);
                    }
                }
            }
        }

        public static void PerformToolAction_Post(StardewObject __instance, Tool t)
        {
            GameLocation location = __instance.Location;

            if (__instance.IsCookoutKit() && location != null)
            {
                if (!location.Objects.ContainsKey(__instance.TileLocation))
                {
                    Vector2 dropPosition = __instance.TileLocation * 64f;

                    // drop 1 iron bar
                    location.debris.Add(new Debris(ItemRegistry.Create("(O)335", 1), dropPosition));

                    // drop 10 stones
                    location.debris.Add(new Debris(ItemRegistry.Create("(O)390", 10), dropPosition));
                }
                else if (t is WateringCan can && can.WaterLeft > 0)
                {
                    // extinguishes the fire, does not truly remove the object
                    __instance.performRemoveAction();
                }
            }
        }

        public static bool CheckForAction_Pre(Torch __instance, Farmer who, bool justCheckingForActivity)
        {
            if (justCheckingForActivity)
            {
                return true;
            }

            // check for ignition
            if (!__instance.IsCookoutKit() || __instance.IsOn || who == null)
            {
                return true;
            }

            int coalCount = mod.Config.CoalNeeded;
            int baseKindlingCount = mod.Config.FiberNeeded;
            int baseWoodCount = mod.Config.WoodNeeded;

            bool hasCoal = who.Items.ContainsId(CoalID, coalCount);

            bool hasKindling = false;
            string kindlingID = null;
            int actualKindlingCount = -1;

            if (hasCoal)
            {
                if (!hasKindling)
                {
                    // soggy newspaper
                    hasKindling = CheckForResource(who, "(O)172", baseKindlingCount, mod.Config.NewspaperMultiplier, ref kindlingID, ref actualKindlingCount);
                }

                if (!hasKindling)
                {
                    // fiber
                    hasKindling = CheckForResource(who, FiberID, baseKindlingCount, 1, ref kindlingID, ref actualKindlingCount);
                }

                if (!hasKindling)
                {
                    // wool
                    hasKindling = CheckForResource(who, "(O)440", baseKindlingCount, mod.Config.WoolMultiplier, ref kindlingID, ref actualKindlingCount);
                }

                if (!hasKindling)
                {
                    // cloth
                    hasKindling = CheckForResource(who, "(O)428", baseKindlingCount, mod.Config.ClothMultiplier, ref kindlingID, ref actualKindlingCount);
                }
            }

            bool hasWood = false;
            string chosenWoodID = null;
            int actualWoodCount = -1;

            if (hasCoal && hasKindling)
            {
                if (!hasWood)
                {
                    // driftwood
                    hasWood = CheckForResource(who, DriftwoodID, baseKindlingCount, mod.Config.DriftwoodMultiplier, ref chosenWoodID, ref actualWoodCount);
                }

                if (!hasWood)
                {
                    // wood
                    hasWood = CheckForResource(who, WoodID, baseKindlingCount, 1, ref chosenWoodID, ref actualWoodCount);
                }

                if (!hasWood)
                {
                    // hardwood
                    hasWood = CheckForResource(who, HardwoodID, baseKindlingCount, mod.Config.HardwoodMultiplier, ref chosenWoodID, ref actualWoodCount);
                }
            }

            if (hasCoal && hasKindling && hasWood)
            {
                who.Items.ReduceId(CoalID, coalCount);
                who.Items.ReduceId(kindlingID, actualKindlingCount);
                who.Items.ReduceId(chosenWoodID, actualWoodCount);

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
                var coal = ItemRegistry.Create(CoalID);
                coal.stack.Value = 0;
                var fiber = ItemRegistry.Create(FiberID);
                fiber.stack.Value = 0;
                var wood = ItemRegistry.Create(WoodID);
                wood.stack.Value = 0;

                Game1.showRedMessage($"{coalCount} {coal.DisplayName}, {baseKindlingCount} {fiber.DisplayName}, {baseWoodCount} {wood.DisplayName}");
            }

            return false;
        }

        internal static int CountWithMultiplier(int baseCount, float multiplier)
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

        private static bool CheckForResource(Farmer who, string id, int baseCount, float multiplier, ref string idToSet, ref int kindlingToRemove)
        {
            if (multiplier > 0)
            {
                int count = CountWithMultiplier(baseCount, multiplier);

                if (who.Items.ContainsId(id, count))
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
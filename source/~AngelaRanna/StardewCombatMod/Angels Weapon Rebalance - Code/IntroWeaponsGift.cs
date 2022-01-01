/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AngelaRanna/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace StardewCombatMod
{
    public class IntroWeaponsGift : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.dayStarted;

            // Make sure to get the monitor set up for debugging prints
            CheckEventPostfixPatch.Initialize(this.Monitor, this.Helper);
            WeaponDoDamagePostfixPatch.Initialize(this.Monitor);
            KnockbackDeRandomizerTranspilerPatch.Initialize(this.Monitor);

            Harmony harmony = new Harmony(this.ModManifest.UniqueID);

            // Patch to do the clean checkForEvents entry that doesn't interfere with other mods' events
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkForEvents)),
                postfix: new HarmonyMethod(typeof(CheckEventPostfixPatch), nameof(CheckEventPostfixPatch.checkForEvents_postfix))
                );
            // Patch to do a second pass at destroying objects in a weapons' AOE range
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Tools.MeleeWeapon), nameof(StardewValley.Tools.MeleeWeapon.DoDamage)),
                postfix: new HarmonyMethod(typeof(WeaponDoDamagePostfixPatch), nameof(WeaponDoDamagePostfixPatch.doDamage_postfix))
                );
            // Patch to remove the extra x4 multiplier on knockback decay for flying and "slippery" foes
            //harmony.Patch(
            //    original: AccessTools.Method(typeof(StardewValley.Monsters.Monster), nameof(StardewValley.Monsters.Monster.MovePosition)),
            //    transpiler: new HarmonyMethod(typeof(KnockbackMultiplierFixerTranspilerPatch), nameof(KnockbackMultiplierFixerTranspilerPatch.movePosition_transpiler))
            //    );
            // Patch to remove the randomness from weapon knockback
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Utility), nameof(StardewValley.Utility.getAwayFromPlayerTrajectory)),
                transpiler: new HarmonyMethod(typeof(KnockbackDeRandomizerTranspilerPatch), nameof(KnockbackDeRandomizerTranspilerPatch.getAwayFromPlayerTrajectory_transpiler))
                );
        }

        private void dayStarted(object sender, DayStartedEventArgs e)
        {
            // If we haven't received the mail already, and the player has been to the mines
            if (!(Game1.player.mailReceived.Contains("WeaponRebalanceIntro")) && Game1.player.deepestMineLevel > 1)
            {
                // You've got mail!
                Game1.mailbox.Add("WeaponRebalanceIntro");
            }
        }
    }

    public class CheckEventPostfixPatch
    {
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static void Initialize(IMonitor monitor, IModHelper helper)
        {
            Monitor = monitor;
            Helper = helper;
        }

        public static void checkForEvents_postfix(GameLocation __instance)
        {
            try
            {
                // If SVE is loaded
                if (Helper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP"))
                {
                    // If the player entered the guild, they've seen the mail, this event hasn't played, and the player has enough free inventory to receive the free lewt
                    if (__instance.Name == "AdventureGuild" && Game1.player.mailReceived.Contains("WeaponRebalanceIntro")
                    && !Game1.player.eventsSeen.Contains(68940001) && Game1.player.freeSpotsInInventory() >= 2 && !Game1.eventUp && __instance.currentEvent == null)
                    {
                        // Play the Gil event
                        __instance.startEvent(new StardewValley.Event(Game1.content.LoadString("Data\\Events\\WeaponRebalanceEvents:WeaponRebalanceIntroWeaponsGiftSVE"), 68940001, Game1.player));

                        // Give the player a carving knife and a femur
                        Game1.player.addItemToInventory(new StardewValley.Tools.MeleeWeapon(16));
                        Game1.player.addItemToInventory(new StardewValley.Tools.MeleeWeapon(31));
                    }
                }
                // If the player entered the mountains, they've seen the mail, this event hasn't played, and the player has enough free inventory to receive the free lewt
                else if (__instance.Name == "Mountain" && Game1.player.mailReceived.Contains("WeaponRebalanceIntro") && !Game1.player.eventsSeen.Contains(68940000)
                    && Game1.player.freeSpotsInInventory() >= 2 && !Game1.eventUp && __instance.currentEvent == null)
                {
                    // Play the Gil event
                    __instance.startEvent(new StardewValley.Event(Game1.content.LoadString("Data\\Events\\WeaponRebalanceEvents:WeaponRebalanceIntroWeaponsGift"), 68940000, Game1.player));

                    // Give the player a carving knife and a femur
                    Game1.player.addItemToInventory(new StardewValley.Tools.MeleeWeapon(16));
                    Game1.player.addItemToInventory(new StardewValley.Tools.MeleeWeapon(31));
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(checkForEvents_postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }

    public class WeaponDoDamagePostfixPatch
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static void doDamage_postfix(GameLocation location, int x, int y, int facingDirection, int power, Farmer who)
        {
            try
            {
                // Get the weapon's area of affect
                Vector2 zero1 = Vector2.Zero;
                Vector2 zero2 = Vector2.Zero;
                Rectangle areaOfEffect = (who.CurrentItem as StardewValley.Tools.MeleeWeapon).getAreaOfEffect(x, y, facingDirection, ref zero1, ref zero2, who.GetBoundingBox(), who.FarmerSprite.currentAnimationIndex);

                // For each tile in the area
                foreach (Vector2 locationToClear in getAllTilesInArea(areaOfEffect))
                {
                    // Play the toolAction on that aread and remove any objects there
                    if (location.objects.ContainsKey(locationToClear) && location.objects[locationToClear].performToolAction((Tool)who.CurrentItem, location))
                    {
                        location.objects.Remove(locationToClear);
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(doDamage_postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        private static List<Vector2> getAllTilesInArea(Rectangle rectangle)
        {
            // Use a hash set so we only add unique locations
            HashSet<Vector2> hset = new HashSet<Vector2>();

            // For each location in the area (trimmed by 64 to match game coordinates), add it to the set
            for (int i = rectangle.Left; i < rectangle.Right; i++)
            {
                for (int j = rectangle.Top; j < rectangle.Bottom; j++)
                {
                    hset.Add(new Vector2(i / 64, j / 64));
                }
            }

            // Convert back to a list before returning
            List<Vector2> ret = new List<Vector2>(hset);
            return ret;
        }
    }

    public class KnockbackDeRandomizerTranspilerPatch
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static IEnumerable<CodeInstruction> getAwayFromPlayerTrajectory_transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            int foundKnockbackRandomizer = 0;

            // Find the line where it calculates the knockback vector, which includes a couple random calls
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldc_I4_S && (sbyte)(codes[i].operand) == 50
                    && codes[i + 1].opcode == OpCodes.Ldsfld
                    && codes[i + 2].opcode == OpCodes.Ldc_I4_S && (sbyte)(codes[i + 2].operand) == -20
                    && codes[i + 3].opcode == OpCodes.Ldc_I4_S && (sbyte)(codes[i + 3].operand) == 20
                    && codes[i + 4].opcode == OpCodes.Callvirt
                    && codes[i + 5].opcode == OpCodes.Add
                    && codes[i + 6].opcode == OpCodes.Conv_R4
                    && codes[i + 7].opcode == OpCodes.Mul
                    && codes[i + 8].opcode == OpCodes.Stind_R4)
                {
                    // Set the random calls to return a random value between 10 and 10
                    codes[i + 2].operand = (sbyte)10;
                    codes[i + 3].operand = (sbyte)10;
                    foundKnockbackRandomizer++;
                }
            }

            // Throw an error if we couldn't find the code to replace
            LogLevel loglevel = LogLevel.Info;
            if (foundKnockbackRandomizer != 2) loglevel = LogLevel.Error;
            Monitor.Log($"Found {foundKnockbackRandomizer.ToString()} instances of knockback randomizer code to replace.", loglevel);
            return codes.AsEnumerable();
        }
    }
}
using System;
using System.Collections.Generic;
using Harmony;
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
            //helper.Events.Player.Warped += this.adventureGuildEntry;

            // Make sure to get the monitor set up for debugging prints
            CheckEventPostfixPatch.Initialize(this.Monitor);
            WeaponDoDamagePostfixPatch.Initialize(this.Monitor);

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

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

        // DEPRECATED -- using the Harmony postfix patcher instead. Just keeping this around in case things break in 1.5 or we get a better way to play asset load chicken.
        //private void adventureGuildEntry(object sender, WarpedEventArgs e)
        //{
        //    // If the player entered the guild, they've seen the mail, this event hasn't played, and the player has enough free inventory to receive the free lewt
        //    if (e.NewLocation.name == "AdventureGuild" && Game1.player.mailReceived.Contains("WeaponRebalanceIntro") && !Game1.player.eventsSeen.Contains(68940000) 
        //        && Game1.player.freeSpotsInInventory() >= 2 && !Game1.eventUp && e.NewLocation.currentEvent == null)
        //    {
        //        // Play the Gil event
        //        e.NewLocation.startEvent(new StardewValley.Event(Game1.content.LoadString("Data\\Events\\WeaponRebalanceEvents:WeaponRebalanceIntroWeaponsGift"), 68940000, Game1.player));

        //        // Give the player a carving knife and a femur
        //        Game1.player.addItemToInventory(new StardewValley.Tools.MeleeWeapon(16));
        //        Game1.player.addItemToInventory(new StardewValley.Tools.MeleeWeapon(31));
        //    }
        //}
    }

    public class CheckEventPostfixPatch
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static void checkForEvents_postfix(GameLocation __instance)
        {
            try
            {
                // If the player entered the guild, they've seen the mail, this event hasn't played, and the player has enough free inventory to receive the free lewt
                if (__instance.name == "AdventureGuild" && Game1.player.mailReceived.Contains("WeaponRebalanceIntro") && !Game1.player.eventsSeen.Contains(68940000)
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
                        location.objects.Remove(locationToClear);
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
                    hset.Add(new Vector2(i/64, j/64));
                }
            }

            // Convert back to a list before returning
            List<Vector2> ret = new List<Vector2>(hset);
            return ret;
        }
    }
}
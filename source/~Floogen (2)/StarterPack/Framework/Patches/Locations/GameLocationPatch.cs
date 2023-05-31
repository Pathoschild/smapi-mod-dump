/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;

namespace StarterPack.Framework.Patches.Locations
{
    internal class GameLocationPatch : PatchTemplate
    {
        private readonly System.Type _object = typeof(GameLocation);

        public GameLocationPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(GameLocation.performTouchAction), new[] { typeof(string), typeof(Vector2) }), postfix: new HarmonyMethod(GetType(), nameof(PerformTouchActionPostfix)));
        }

        private static void PerformTouchActionPostfix(GameLocation __instance, string fullActionString, Vector2 playerStandingPosition)
        {
            if (Game1.eventUp || string.IsNullOrEmpty(fullActionString))
            {
                return;
            }

            var actionName = fullActionString.Split(' ')[0];
            if (actionName.Equals("legendarySword", System.StringComparison.OrdinalIgnoreCase))
            {
                if (Game1.player.CurrentItem is null)
                {
                    return;
                }

                string weaponModelId = null;
                if (Game1.player.CurrentItem.Name == "Ring of Yoba")
                {
                    weaponModelId = "PeacefulEnd.Archery.StarterPack/Bow/Yoba's Divine Harp";
                }
                else if (Game1.player.CurrentItem.Name == "Rusty Cog")
                {
                    weaponModelId = "PeacefulEnd.Archery.StarterPack/Crossbow/Dwarven Repeating Crossbow";
                }

                if (string.IsNullOrEmpty(weaponModelId) is false)
                {
                    Game1.player.Halt();
                    Game1.player.faceDirection(2);
                    Game1.player.showCarrying();
                    Game1.player.jitterStrength = 1f;
                    Game1.pauseThenDoFunction(7000, () => GetSpecialWeapon(Game1.player.CurrentItem, weaponModelId));
                    Game1.changeMusicTrack("none", track_interruptable: false, Game1.MusicContext.Event);
                    __instance.playSound("crit");
                    Game1.screenGlowOnce(new Color(30, 0, 150), hold: true, 0.01f, 0.999f);
                    DelayedAction.playSoundAfterDelay("stardrop", 1500);
                    Game1.screenOverlayTempSprites.AddRange(Utility.sparkleWithinArea(new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), 500, Color.White, 10, 2000));
                    Game1.afterDialogues = (Game1.afterFadeFunction)Delegate.Combine(Game1.afterDialogues, (Game1.afterFadeFunction)delegate
                    {
                        Game1.stopMusicTrack(Game1.MusicContext.Event);
                    });
                }
            }
        }

        private static void GetSpecialWeapon(Item item, string weaponModelId)
        {
            var weapon = StarterPack.apiManager.GetArcheryApi().CreateWeapon(StarterPack.manifest, weaponModelId);
            if (weapon is null)
            {
                return;
            }

            Game1.flashAlpha = 1f;
            Game1.player.holdUpItemThenMessage(weapon);
            Game1.player.removeItemFromInventory(item);
            if (!Game1.player.addItemToInventoryBool(weapon))
            {
                Game1.createItemDebris(weapon, Game1.player.getStandingPosition(), 1);
            }
            Game1.player.jitterStrength = 0f;
            Game1.screenGlowHold = false;
        }
    }
}
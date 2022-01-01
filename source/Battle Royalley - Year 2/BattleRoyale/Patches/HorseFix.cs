/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using System.Linq;

namespace BattleRoyale.Patches
{
    class HorseDespawnFix : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Horse), "dismount");

        public static bool Prefix(Horse __instance)
        {
            __instance.mutex.ReleaseLock();
            __instance.rider.mount = null;
            if (__instance.currentLocation != null)
            {
                if (!__instance.currentLocation.characters.Where((NPC c) => c is Horse && (c as Horse).HorseId == __instance.HorseId).Any())
                {
                    __instance.currentLocation.characters.Add(__instance);
                }
                __instance.Position = __instance.rider.Position;
                __instance.rider.freezePause = -1;
                __instance.dismounting.Value = false;
                __instance.rider.canMove = true;
                __instance.rider.forceCanMove();
                __instance.rider.xOffset = 0f;
                __instance.rider = null;
                __instance.Halt();
                __instance.farmerPassesThrough = false;
            }

            return false;
        }
    }

    class MultipleHorseFix : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(Horse), "checkAction");

        public static bool Prefix(Horse __instance, Farmer who, GameLocation l)
        {
            if (who != null && !who.canMove)
            {
                return true;
            }
            if (__instance.rider == null)
            {
                __instance.mutex.RequestLock(delegate
                {
                    if (who.mount != null || __instance.rider != null || who.FarmerSprite.PauseForSingleAnimation)
                    {
                        __instance.mutex.ReleaseLock();
                    }
                    else if (Game1.player.horseName.Value == null || Game1.player.horseName.Value.Length == 0)
                    {
                        Game1.activeClickableMenu = new NamingMenu(__instance.nameHorse, Game1.content.LoadString("Strings\\Characters:NameYourHorse"), Game1.content.LoadString("Strings\\Characters:DefaultHorseName"));
                    }
                    else
                    {
                        __instance.rider = who;
                        __instance.rider.freezePause = 5000;
                        __instance.rider.synchronizedJump(6f);
                        __instance.rider.Halt();
                        if (__instance.rider.Position.X < __instance.Position.X)
                        {
                            __instance.rider.faceDirection(1);
                        }
                        l.playSound("dwop");
                        __instance.mounting.Value = true;
                        __instance.rider.isAnimatingMount = true;
                        __instance.rider.completelyStopAnimatingOrDoingAction();
                        __instance.rider.faceGeneralDirection(Utility.PointToVector2(__instance.GetBoundingBox().Center), 0, opposite: false, useTileCalculations: false);
                    }
                });
                return false;
            }
            return true;
        }
    }
}

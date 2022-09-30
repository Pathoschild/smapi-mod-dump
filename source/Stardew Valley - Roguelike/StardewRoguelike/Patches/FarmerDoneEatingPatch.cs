/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using HarmonyLib;
using Netcode;
using StardewValley;
using System;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(Farmer), "doneEating")]
    internal class FarmerDoneEatingPatch
    {
        public static bool Prefix(Farmer __instance)
        {
            __instance.isEating = false;
            __instance.completelyStopAnimatingOrDoingAction();
            __instance.forceCanMove();
            if (__instance.mostRecentlyGrabbedItem == null)
                return false;

            if (__instance != Game1.player)
                return true;

            StardewValley.Object consumed = __instance.itemToEat as StardewValley.Object;
            if (consumed != null && consumed.HasContextTag("ginger_item") && __instance.hasBuff(25))
            {
                Game1.buffsDisplay.removeOtherBuff(25);
            }
            string[] objectDescription = Game1.objectInformation[consumed.ParentSheetIndex].Split('/');
            if (Convert.ToInt32(objectDescription[2]) > 0)
            {
                string[] whatToBuff = ((objectDescription.Length > 7) ? objectDescription[7].Split(' ') : new string[12]
                {
                        "0", "0", "0", "0", "0", "0", "0", "0", "0", "0",
                        "0", "0"
                });
                consumed.ModifyItemBuffs(whatToBuff);
                int duration = ((objectDescription.Length > 8) ? Convert.ToInt32(objectDescription[8]) : (-1));
                if (consumed.Quality != 0)
                {
                    duration = (int)((float)duration * 1.5f);
                }
                Buff buff = new Buff(Convert.ToInt32(whatToBuff[0]), Convert.ToInt32(whatToBuff[1]), Convert.ToInt32(whatToBuff[2]), Convert.ToInt32(whatToBuff[3]), Convert.ToInt32(whatToBuff[4]), Convert.ToInt32(whatToBuff[5]), Convert.ToInt32(whatToBuff[6]), Convert.ToInt32(whatToBuff[7]), Convert.ToInt32(whatToBuff[8]), Convert.ToInt32(whatToBuff[9]), Convert.ToInt32(whatToBuff[10]), (whatToBuff.Length > 11) ? Convert.ToInt32(whatToBuff[11]) : 0, duration, objectDescription[0], objectDescription[4]);
                if (Utility.IsNormalObjectAtParentSheetIndex(consumed, 921))
                {
                    buff.which = 28;
                }
                if (objectDescription.Length > 6 && objectDescription[6].Equals("drink"))
                {
                    Game1.buffsDisplay.tryToAddDrinkBuff(buff);
                }
                else if (Convert.ToInt32(objectDescription[2]) > 0)
                {
                    Game1.buffsDisplay.tryToAddFoodBuff(buff, Math.Min(120000, (int)(Convert.ToInt32(objectDescription[2]) / 20f * 30000f)));
                }
            }
            float oldStam = __instance.Stamina;
            int oldHealth = __instance.health;

            int staminaToHeal = consumed.staminaRecoveredOnConsumption();
            __instance.Stamina = Math.Min(__instance.MaxStamina, __instance.Stamina + staminaToHeal);

            int healAmount = consumed.healthRecoveredOnConsumption();

            if (Perks.HasPerk(Perks.PerkType.FoodEnjoyer))
                healAmount = (int)(healAmount * 1.2f);

            if (healAmount > 0 && Curse.HasCurse(CurseType.HealOverTime))
                Curse.HOTHealToTick += (int)Math.Round(healAmount * 1.5f);
            else
                __instance.health = Math.Min(__instance.maxHealth, __instance.health + healAmount);

            if (oldStam < __instance.Stamina)
                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3116", (int)(__instance.Stamina - oldStam)), 4));

            if (oldHealth < __instance.health)
                Game1.addHUDMessage(new HUDMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3118", __instance.health - oldHealth), 5));

            if (consumed != null && consumed.Edibility < 0 && __instance.IsLocalPlayer)
            {
                __instance.CanMove = false;
                NetEvent0 sickAnimationEvent = (NetEvent0)__instance.GetType().GetField("sickAnimationEvent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(__instance);
                sickAnimationEvent.Fire();
            }

            return false;
        }
    }
}

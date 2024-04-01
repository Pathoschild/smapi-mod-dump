/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using StardewValley.Buffs;
using StardewValley.GameData.Buffs;
using Microsoft.Xna.Framework.Graphics;

using static BZP_Allergies.AllergenManager;

namespace BZP_Allergies.HarmonyPatches
{
    [HarmonyPatch(typeof(Farmer), nameof(Farmer.doneEating))]
    internal class PatchFarmerDoneEating : Initializable
    {

        [HarmonyPrefix]
        static bool DoneEating_Prefix(ref Farmer __instance, out int __state)
        {
            try
            {
                StardewValley.Object? itemToEat = __instance.itemToEat as StardewValley.Object;
                __state = itemToEat == null ? int.MinValue : itemToEat.Edibility;
                if (itemToEat == null || !__instance.IsLocalPlayer)
                {
                    return true;
                }

                Texture2D sprites = Game1.content.Load<Texture2D>("Mods/BarleyZP.BzpAllergies/Sprites");

                if (FarmerIsAllergic(itemToEat) && !__instance.hasBuff(Buff.squidInkRavioli))
                {
                    // is it dairy and do we have the buff?
                    if (itemToEat.HasContextTag(GetAllergenContextTag(Allergens.DAIRY)) && __instance.hasBuff(LACTASE_PILLS_BUFF))
                    {
                        HUDMessage lactaseProtectionMessage = new("Good thing you took your lactase!");
                        lactaseProtectionMessage.messageSubject = itemToEat;
                        Game1.addHUDMessage(lactaseProtectionMessage);
                        return true;
                    }

                    // change edibility
                    itemToEat.Edibility = -20;

                    // add the allergic reaction buff
                    BuffAttributesData buffAttributesData = new()
                    {
                        Speed = -2,
                        Defense = -1,
                        Attack = -1,
                    };

                    BuffEffects effects = new(buffAttributesData);

                    Buff reactionBuff = new(ALLERIC_REACTION_DEBUFF, "food", itemToEat.DisplayName,
                        120000, sprites, 2, effects,
                        true, "Allergic Reaction", "Probably shouldn't have eaten that...");
                    reactionBuff.glow = Microsoft.Xna.Framework.Color.Green;

                    __instance.applyBuff(reactionBuff);
                    
                    // randomly apply nausea
                    if (new Random().NextDouble() < 0.50)
                    {
                        __instance.applyBuff(Buff.nauseous);
                    }

                    // send mail
                    if (!__instance.mailReceived.Contains(ModEntry.MOD_ID + "_harvey_ad"))
                    {
                        Game1.addMailForTomorrow(ModEntry.MOD_ID + "_harvey_ad");
                    }
                }
                else if (itemToEat.QualifiedItemId.Equals("(O)" + ALLERGY_RELIEF_ID))
                {
                    // nausea is automatically removed. remove the reaction as well
                    __instance.buffs.Remove(ALLERIC_REACTION_DEBUFF);
                    
                }
                else if (itemToEat.QualifiedItemId.Equals("(O)" + LACTASE_PILLS_ID))
                {
                    // get that dairy immunity
                    Buff immuneBuff = new(LACTASE_PILLS_BUFF, "food", itemToEat.DisplayName,
                        120000, sprites, 3, null,
                        false, "Dairy Immunity", "Quick, eat the cheese!");

                    __instance.applyBuff(immuneBuff);
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(DoneEating_Prefix)}:\n{ex}", LogLevel.Error);
                __state = int.MinValue;  // error value
            }
            return true; // run original logic
        }

        [HarmonyPostfix]
        static void DoneEating_Postfix(ref Farmer __instance, int __state)
        {
            try
            {
                StardewValley.Object? itemToEat = __instance.itemToEat as StardewValley.Object;
                if (itemToEat != null && __state != int.MinValue)
                {
                    // change edibility back to original value
                    itemToEat.Edibility = __state;
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(DoneEating_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}

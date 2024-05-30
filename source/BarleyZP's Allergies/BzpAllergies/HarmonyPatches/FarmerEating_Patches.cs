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
    internal class FarmerEating_Patches
    {
        public static void Patch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.createQuestionDialogue), new Type[] { typeof(string), typeof(Response[]), typeof(string) }),
                prefix: new HarmonyMethod(typeof(FarmerEating_Patches), nameof(CreateQuestionDialogue_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.doneEating)),
                prefix: new HarmonyMethod(typeof(FarmerEating_Patches), nameof(DoneEating_Prefix)),
                postfix: new HarmonyMethod(typeof(FarmerEating_Patches), nameof(DoneEating_Postfix))
            );
        }

        public static void CreateQuestionDialogue_Prefix(ref string question)
        {
            try
            {
                bool randomAllergies = ModDataGet(Game1.player, "BarleyZP.BzpAllergies_Random", out string val) && val == "true";

                if (!ModEntry.Instance.Config.HintBeforeEating || Game1.player.ActiveObject == null)
                {
                    return;
                }

                // is this the "Eat {0}?" or "Drink {0}?" popup?
                IDictionary<string, string> stringsData = Game1.content.Load<Dictionary<string, string>>("Strings/StringsFromCSFiles");

                string activeObjectName = Game1.player.ActiveObject.DisplayName;
                string eatQuestion = string.Format(stringsData["Game1.cs.3160"], activeObjectName);
                string drinkQuestion = string.Format(stringsData["Game1.cs.3159"], activeObjectName);

                if (question.Equals(eatQuestion) || question.Equals(drinkQuestion))
                {
                    bool hasDiscoveredAtLeastOneAllergy = false;
                    ISet<string> allergens = GetAllergensInObject(Game1.player.ActiveObject);
                    foreach (string a in allergens)
                    {
                        if (PlayerHasDiscoveredAllergy(a))
                        {
                            hasDiscoveredAtLeastOneAllergy = true;
                            break;
                        }
                    }

                    if (FarmerIsAllergic(Game1.player.ActiveObject) && (!randomAllergies || (randomAllergies && hasDiscoveredAtLeastOneAllergy)))
                    {
                        question += ModEntry.Instance.Translation.Get("allergic-hint");
                    }
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(CreateQuestionDialogue_Prefix)}:\n{ex}", LogLevel.Error);
            }
        }

        public static void DoneEating_Prefix(ref Farmer __instance, out int __state)
        {
            try
            {
                StardewValley.Object? itemToEat = __instance.itemToEat as StardewValley.Object;
                __state = itemToEat == null ? int.MinValue : itemToEat.Edibility;
                if (itemToEat == null || !__instance.IsLocalPlayer)
                {
                    return;
                }

                Texture2D sprites = Game1.content.Load<Texture2D>("BarleyZP.BzpAllergies/Sprites");

                if (FarmerIsAllergic(itemToEat) && !__instance.hasBuff(Buff.squidInkRavioli))
                {
                    ISet<string> itemToEatAllergens = GetAllergensInObject(itemToEat);

                    // is it dairy and do we have the buff?
                    if (itemToEatAllergens.Contains("dairy") && __instance.hasBuff(Constants.LactaseBuff))
                    {
                        HUDMessage lactaseProtectionMessage = new(ModEntry.Instance.Translation.Get("lactase-save"));
                        lactaseProtectionMessage.messageSubject = itemToEat;
                        Game1.addHUDMessage(lactaseProtectionMessage);
                        Game1.playSound("jingle1");
                        return;
                    }

                    // change edibility
                    itemToEat.Edibility = -20;

                    // clear any existing allergy buffs
                    __instance.buffs.Remove(Constants.ReactionDebuff);

                    // add the allergic reaction buff
                    __instance.applyBuff(AllergenManager.GetAllergicReactionBuff(itemToEat.DisplayName, "consume", ModEntry.Instance.Config.EatingDebuffLengthSeconds));
                    
                    // randomly apply nausea
                    if (ModEntry.Instance.Config.EnableNausea && new Random().NextDouble() < 0.50)
                    {
                        __instance.applyBuff(Buff.nauseous);
                    }

                    CheckForAllergiesToDiscover(__instance, itemToEatAllergens);
                }
                else if (itemToEat.QualifiedItemId.Equals("(O)" + Constants.AllergyReliefId))
                {
                    // nausea is automatically removed. remove the reaction as well
                    __instance.buffs.Remove(Constants.ReactionDebuff);
                    
                }
                else if (itemToEat.QualifiedItemId.Equals("(O)" + Constants.LactasePillsId))
                {
                    // get that dairy immunity
                    Buff immuneBuff = new(Constants.LactaseBuff, "food", itemToEat.DisplayName,
                        120000, sprites, 3, null,
                        false, "Dairy Immunity", ModEntry.Instance.Translation.Get("dairy-immunity"));

                    __instance.applyBuff(immuneBuff);
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(DoneEating_Prefix)}:\n{ex}", LogLevel.Error);
                __state = int.MinValue;  // error value
            }
        }
        public static void DoneEating_Postfix(ref Farmer __instance, int __state)
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
                ModEntry.Instance.Monitor.Log($"Failed in {nameof(DoneEating_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using CookingSkill.Framework;
using CookingSkill.Patches;
using Spacechase.Shared.Harmony;
using SpaceCore;
using SpaceCore.Events;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace CookingSkill
{
    // This really needs organizing/splitting
    internal class Mod : StardewModdingAPI.Mod
    {
        public static Mod Instance;
        public static Skill Skill;

        public static double GetEdibilityMultiplier()
        {
            return 1 + Game1.player.GetCustomSkillLevel(Mod.Skill) * 0.03;
        }

        public static double GetNoConsumeChance()
        {
            if (Game1.player.HasCustomProfession(Skill.ProfessionConservation))
                return 0.15;
            else
                return 0;

        }

        // Modifies the item based on professions and stuff
        // Returns for whether or not we should consume the ingredients
        public static bool OnCook(CraftingRecipe recipe, Item item, List<Chest> additionalItems)
        {
            if (recipe.isCookingRecipe && item is SObject obj)
            {
                if (!Game1.player.recipesCooked.TryGetValue(obj.ParentSheetIndex, out int timesCooked))
                    timesCooked = 0;

                Random rand = new Random((int)(Game1.stats.daysPlayed + Game1.uniqueIDForThisGame + (uint)obj.ParentSheetIndex + (uint)timesCooked));

                obj.Edibility = (int)(obj.Edibility * Mod.GetEdibilityMultiplier());

                if (Game1.player.HasCustomProfession(Skill.ProfessionSellPrice))
                {
                    obj.Price = (int)(obj.Price * 1.2);
                }

                if (Game1.player.HasCustomProfession(Skill.ProfessionSilver))
                {
                    obj.Quality = 1;
                }

                ConsumedItem[] used;
                try
                {
                    CraftingRecipePatcher.ShouldConsumeItems = false;
                    recipe.consumeIngredients(additionalItems);
                    used = CraftingRecipePatcher.LastUsedItems.ToArray();
                }
                finally
                {
                    CraftingRecipePatcher.ShouldConsumeItems = true;
                }

                int total = 0;
                foreach (ConsumedItem ingr in used)
                    total += ingr.Amount;

                for (int iq = 1; iq <= SObject.bestQuality; ++iq)
                {
                    if (iq == 3) continue; // Not a real quality

                    double chance = 0;
                    foreach (ConsumedItem ingr in used)
                    {
                        if (ingr.Item.Quality >= iq)
                            chance += (1.0 / total) * ingr.Amount;
                    }

                    if (rand.NextDouble() < chance)
                        obj.Quality = iq;
                }

                return rand.NextDouble() >= Mod.GetNoConsumeChance();
            }

            return true;
        }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Mod.Instance = this;
            Log.Monitor = this.Monitor;

            SpaceEvents.OnItemEaten += this.OnItemEaten;

            Skills.RegisterSkill(Mod.Skill = new Skill());

            HarmonyPatcher.Apply(this,
                new CraftingPagePatcher(),
                new CraftingRecipePatcher()
            );
        }

        public override object GetApi()
        {
            return new Api();
        }

        private Buff LastDrink;

        private void OnItemEaten(object sender, EventArgs e)
        {
            SObject obj = Game1.player.itemToEat as SObject;
            string[] info = Game1.objectInformation[obj.ParentSheetIndex].Split('/');
            string[] buffData = ((Convert.ToInt32(info[2]) > 0 && info.Length > 7) ? info[7].Split(' ') : null);

            if (buffData != null)
            {
                bool allZero = true;
                foreach (string bd in buffData)
                {
                    allZero = (allZero && bd == "0");
                }
                if (allZero) buffData = null;
            }

            if (info[3] == "Cooking -7")
            {
                // Need to make sure this is the original buff first.
                // So it doesn't get rebuffed from eating a buff food -> non buff food -> buff food or something
                Buff oldBuff = (info[6] == "drink" ? Game1.buffsDisplay.drink : Game1.buffsDisplay.food);
                Buff thisBuff;
                if (info[6] == "drink")
                    thisBuff = buffData == null ? null : new Buff(Convert.ToInt32(buffData[0]), Convert.ToInt32(buffData[1]), Convert.ToInt32(buffData[2]), Convert.ToInt32(buffData[3]), Convert.ToInt32(buffData[4]), Convert.ToInt32(buffData[5]), Convert.ToInt32(buffData[6]), Convert.ToInt32(buffData[7]), Convert.ToInt32(buffData[8]), Convert.ToInt32(buffData[9]), Convert.ToInt32(buffData[10]), (buffData.Length > 10) ? Convert.ToInt32(buffData[10]) : 0, (info.Length > 8) ? Convert.ToInt32(info[8]) : -1, info[0], info[4]);
                else
                    thisBuff = buffData == null ? null : new Buff(Convert.ToInt32(buffData[0]), Convert.ToInt32(buffData[1]), Convert.ToInt32(buffData[2]), Convert.ToInt32(buffData[3]), Convert.ToInt32(buffData[4]), Convert.ToInt32(buffData[5]), Convert.ToInt32(buffData[6]), Convert.ToInt32(buffData[7]), Convert.ToInt32(buffData[8]), Convert.ToInt32(buffData[9]), Convert.ToInt32(buffData[10]), (buffData.Length > 11) ? Convert.ToInt32(buffData[11]) : 0, (info.Length > 8) ? Convert.ToInt32(info[8]) : -1, info[0], info[4]);
                int[] oldAttr = oldBuff?.buffAttributes;
                int[] thisAttr = thisBuff?.buffAttributes;
                Log.Trace("Ate something: " + obj + " " + Game1.objectInformation[obj.ParentSheetIndex] + " " + buffData + " " + oldBuff + " " + thisBuff + " " + oldAttr + " " + thisAttr);
                if (oldBuff != null && thisBuff != null && oldAttr.SequenceEqual(thisAttr) &&
                     ((info[6] == "drink" && oldBuff != this.LastDrink) || (info[6] != "drink" && oldBuff != this.LastDrink)))
                {
                    // Now that we know that this is the original buff, we can buff the buff.
                    Log.Trace("Buffing buff");
                    int[] newAttr = (int[])thisAttr.Clone();
                    if (Game1.player.HasCustomProfession(Skill.ProfessionBuffLevel))
                    {
                        for (int i = 0; i < thisAttr.Length; ++i)
                        {
                            if (newAttr[i] <= 0)
                                continue;

                            if (i == 7 || i == 8)
                                newAttr[i] = (int)(newAttr[i] * 1.2);
                            else
                                newAttr[i]++;
                        }
                    }

                    int newTime = (info.Length > 8) ? Convert.ToInt32(info[8]) : -1;
                    if (newTime != -1 && Game1.player.HasCustomProfession(Skill.ProfessionBuffTime))
                    {
                        newTime = (int)(newTime * 1.25);
                    }

                    Buff newBuff = new Buff(newAttr[0], newAttr[1], newAttr[2], newAttr[3], newAttr[4], newAttr[5], newAttr[6], newAttr[7], newAttr[8], newAttr[9], newAttr[10], newAttr[11], newTime, info[0], info[4])
                    {
                        millisecondsDuration = newTime / 10 * 7000
                    };
                    // ^ The vanilla code decreases the duration based on the time of day.
                    // This is fine normally, since it ends as the day ends.
                    // However if you have something like TimeSpeed it just means it won't
                    // last as long later if eaten later in the day.

                    if (info[6] == "drink")
                    {
                        Game1.buffsDisplay.drink.removeBuff();
                        Game1.buffsDisplay.drink = newBuff;
                        Game1.buffsDisplay.drink.addBuff();
                        this.LastDrink = newBuff;
                    }
                    else
                    {
                        Game1.buffsDisplay.food.removeBuff();
                        Game1.buffsDisplay.food = newBuff;
                        Game1.buffsDisplay.food.addBuff();
                    }
                    Game1.buffsDisplay.syncIcons();
                }
                else if (thisBuff == null && Game1.player.HasCustomProfession(Skill.ProfessionBuffPlain))
                {
                    Log.Trace("Buffing plain");
                    Random rand = new Random();
                    int[] newAttr = new int[12];
                    int count = 1 + Math.Min(obj.Edibility / 30, 3);
                    for (int i = 0; i < count; ++i)
                    {
                        int attr = rand.Next(10);
                        if (attr >= 3) ++attr; // 3 unused?
                        if (attr >= 6) ++attr; // 6 is crafting speed, unused?

                        int amt = 1;
                        if (attr == 7 || attr == 8)
                            amt = 25 + rand.Next(4) * 5;
                        else
                        {
                            // 36% (assuming I used this probability calculator right) chance for a buff to be level 2
                            // 4% chance for it to be 3
                            if (rand.NextDouble() < 0.2)
                                ++amt;
                            if (rand.NextDouble() < 0.2)
                                ++amt;
                        }
                        newAttr[attr] += amt;
                    }

                    int newTime = 120 + obj.Edibility / 10 * 30;

                    Buff newBuff = new Buff(newAttr[0], newAttr[1], newAttr[2], newAttr[3], newAttr[4], newAttr[5], newAttr[6], newAttr[7], newAttr[8], newAttr[9], newAttr[10], newAttr[11], newTime, info[0], info[4])
                    {
                        millisecondsDuration = newTime / 10 * 7000
                    };

                    if (info[6] == "drink")
                    {
                        Game1.buffsDisplay.drink?.removeBuff();
                        Game1.buffsDisplay.drink = newBuff;
                        Game1.buffsDisplay.drink.addBuff();
                        this.LastDrink = newBuff;
                    }
                    else
                    {
                        Game1.buffsDisplay.drink?.removeBuff();
                        Game1.buffsDisplay.drink = newBuff;
                        Game1.buffsDisplay.drink.addBuff();
                    }
                    Game1.buffsDisplay.syncIcons();
                }
            }
        }
    }
}

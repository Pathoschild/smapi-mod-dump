/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using BirbCore.Attributes;
using MoonShared.APIs;
using Netcode;
using SpaceCore;
using SpaceCore.Events;
using SpaceCore.Interface;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Extensions;
using StardewValley.GameData.Objects;
using StardewValley.Inventories;
using StardewValley.Locations;
using StardewValley.Menus;
using static BirbCore.Attributes.SMod;

namespace CookingSkill.Core
{
    [SEvent]
    public class Events
    {
        public static Skills.SkillBuff Test { get; private set; }

        [SEvent.GameLaunchedLate]
        public static void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Log.Trace("Cooking: Trying to Register skill.");
            SpaceCore.Skills.RegisterSkill(new Cooking_Skill());

            try
            {
                Log.Trace("Cooking: Do I see better crafting?");
                if (ModEntry.BCLoaded)
                {
                    Log.Trace("Cooking: I do see better crafting, Registering API.");
                    ModEntry.BetterCrafting = ModEntry.Instance.Helper.ModRegistry.GetApi<IBetterCrafting>("leclair.bettercrafting");

                    ModEntry.BetterCrafting.PerformCraft += BetterCraftingPerformCraftEvent;
                    ModEntry.BetterCrafting.PostCraft += BetterCraftingPostCraftEvent;
                }
            }
            catch
            {
                Log.Trace("Cooking: Error with trying to load better crafting API");
            }
            SpaceEvents.OnItemEaten += OnItemEat;
            SpaceEvents.AfterGiftGiven += AfterGiftGiven;
        }

        private static void AfterGiftGiven(object sender, EventArgsGiftGiven e)
        {
            if (e.Gift.modDataForSerialization.ContainsKey("moonslime.Cooking.homemade") && sender is StardewValley.Farmer farmer)
            {
                int bonusFriendship = ((int)Math.Ceiling(e.Gift.Edibility * 0.10));
                farmer.changeFriendship(bonusFriendship, e.Npc);
            }
        }

        private static void BetterCraftingPerformCraftEvent(IGlobalPerformCraftEvent @event)
        {
            if (@event.Recipe.CraftingRecipe is not null && @event.Recipe.CraftingRecipe.isCookingRecipe)
            {
                @event.Item = PreCook(@event.Recipe.CraftingRecipe, @event.Item, true);
                @event.Complete();
            }
            @event.Complete();

        }

        private static void BetterCraftingPostCraftEvent(IPostCraftEvent @event)
        {
            if (@event.Recipe.CraftingRecipe is not null && @event.Recipe.CraftingRecipe.isCookingRecipe)
            {
                //it's easier for me to use a dictionary to not override item stack sized
                Dictionary<Item, int> consumed_items_dict = new Dictionary<Item, int>();
                foreach (Item consumed in @event.ConsumedItems)
                {
                    consumed_items_dict.Add(consumed, consumed.Stack);
                }

                @event.Item = PostCook(@event.Recipe.CraftingRecipe, @event.Item, consumed_items_dict, @event.Player, true);
            }
            
        }

        [SEvent.MenuChanged]
        private void MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is not SkillLevelUpMenu levelUpMenu)
            {
                return;
            }



            string skill = ModEntry.Instance.Helper.Reflection.GetField<string>(levelUpMenu, "currentSkill").GetValue();
            if (skill != "moonslime.Cooking")
            {
                return;
            }

            int level = ModEntry.Instance.Helper.Reflection.GetField<int>(levelUpMenu, "currentLevel").GetValue();

            List<CraftingRecipe> newRecipes = [];

            int menuHeight = 0;
            foreach (KeyValuePair<string, string> recipePair in CraftingRecipe.craftingRecipes)
            {
                string conditions = ArgUtility.Get(recipePair.Value.Split('/'), 4, "");
                if (!conditions.Contains(skill) || !conditions.Contains(level.ToString()))
                {
                    continue;
                }

                CraftingRecipe recipe = new(recipePair.Key, isCookingRecipe: false);
                newRecipes.Add(recipe);
                Game1.player.craftingRecipes.TryAdd(recipePair.Key, 0);
                menuHeight += recipe.bigCraftable ? 128 : 64;
            }

            foreach (KeyValuePair<string, string> recipePair in CraftingRecipe.cookingRecipes)
            {
                string conditions = ArgUtility.Get(recipePair.Value.Split('/'), 3, "");
                if (!conditions.Contains(skill) || !conditions.Contains(level.ToString()))
                {
                    continue;
                }

                CraftingRecipe recipe = new(recipePair.Key, isCookingRecipe: true);
                newRecipes.Add(recipe);
                if (Game1.player.cookingRecipes.TryAdd(recipePair.Key, 0) &&
                    !Game1.player.hasOrWillReceiveMail("robinKitchenLetter"))
                {
                    Game1.mailbox.Add("robinKitchenLetter");
                }

                menuHeight += recipe.bigCraftable ? 128 : 64;
            }

            ModEntry.Instance.Helper.Reflection.GetField<List<CraftingRecipe>>(levelUpMenu, "newCraftingRecipes")
                .SetValue(newRecipes);

            levelUpMenu.height = menuHeight + 256 + (levelUpMenu.getExtraInfoForLevel(skill, level).Count * 64 * 3 / 4);
        }

        [SEvent.SaveLoaded]
        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            string Id = "moonslime.Cooking";
            int skillLevel = Game1.player.GetCustomSkillLevel(Id);

            if (skillLevel >= 5 && !(Game1.player.HasCustomProfession(Cooking_Skill.Cooking5a) ||
                                     Game1.player.HasCustomProfession(Cooking_Skill.Cooking5b)))
            {
                Game1.endOfNightMenus.Push(new SkillLevelUpMenu(Id, 5));
            }

            if (skillLevel >= 10 && !(Game1.player.HasCustomProfession(Cooking_Skill.Cooking10a1) ||
                                      Game1.player.HasCustomProfession(Cooking_Skill.Cooking10a2) ||
                                      Game1.player.HasCustomProfession(Cooking_Skill.Cooking10b1) ||
                                      Game1.player.HasCustomProfession(Cooking_Skill.Cooking10b2)))
            {
                Game1.endOfNightMenus.Push(new SkillLevelUpMenu(Id, 10));
            }

            foreach (KeyValuePair<string, string> recipePair in DataLoader.CraftingRecipes(Game1.content))
            {
                string conditions = ArgUtility.Get(recipePair.Value.Split('/'), 4, "");
                if (!conditions.Contains(Id))
                {
                    continue;
                }
                if (conditions.Split(" ").Length < 2)
                {
                    continue;
                }

                int level = int.Parse(conditions.Split(" ")[1]);

                if (skillLevel < level)
                {
                    continue;
                }

                Game1.player.craftingRecipes.TryAdd(recipePair.Key, 0);
            }

            foreach (KeyValuePair<string, string> recipePair in DataLoader.CookingRecipes(Game1.content))
            {
                string conditions = ArgUtility.Get(recipePair.Value.Split('/'), 3, "");
                if (!conditions.Contains(Id))
                {
                    continue;
                }
                if (conditions.Split(" ").Length < 2)
                {
                    continue;
                }

                int level = int.Parse(conditions.Split(" ")[1]);

                if (skillLevel < level)
                {
                    continue;
                }

                if (Game1.player.cookingRecipes.TryAdd(recipePair.Key, 0) &&
                    !Game1.player.hasOrWillReceiveMail("robinKitchenLetter"))
                {
                    Game1.mailbox.Add("robinKitchenLetter");
                }
            }
        }

        public static void OnItemEat(object sender, EventArgs e)
        {
            // get the farmer. If there is no farmer (like maybe they disconnected, make sure it isnt null)
            StardewValley.Farmer who = sender as StardewValley.Farmer;
            if (who == null) return;

            //Get the farmer's unique ID, and again check for an ull player (cause I am parinoid)
            // If they don't have any professions related to food buffs, return so the rest of the code does not get ran.
            var player = Game1.getFarmer(who.UniqueMultiplayerID);
            if (player == null || !player.HasCustomProfession(Cooking_Skill.Cooking5b)) return;

            //Get the food item the player is going to eat. Make sure it doesnt return as a null item.
            StardewValley.Object food = player.itemToEat as StardewValley.Object;
            if (food == null) return;

            //Get the food's ObjectData and make sure it isn't null
            if (!Game1.objectData.TryGetValue(food.ItemId, out ObjectData data) || data == null) return;

            if (data.Buffs != null)
            {
                //For each buff in the object data, let's go through it.
                foreach (var buffData in data.Buffs)
                {
                    //If there are any spaceCore buffs on the food, run this code to increase those buffs. If there are not, run the other code.
                    if (buffData.CustomFields != null && buffData.CustomFields.Any(b => b.Key.StartsWith("spacechase.SpaceCore.SkillBuff.")))
                    {
                        Buff matchingBuff = null;
                        string id = string.IsNullOrWhiteSpace(buffData.BuffId) ? (data.IsDrink ? "drink" : "food") : buffData.BuffId;
                        foreach (Buff buff in food.GetFoodOrDrinkBuffs())
                        {
                            matchingBuff = buff;
                        }
                        if (matchingBuff != null)
                        {
                            var newSkillBuff = new Skills.SkillBuff(matchingBuff, id, buffData.CustomFields);
                            if (player.hasBuff(newSkillBuff.id))
                            {
                                player.buffs.Remove(newSkillBuff.id);
                                newSkillBuff.millisecondsDuration = (int)(Utilities.GetLevelValue(player) * newSkillBuff.millisecondsDuration);
                                if (player.HasCustomProfession(Cooking_Skill.Cooking10b1))
                                {
                                    ApplyAttributeBuff(newSkillBuff.effects, 1f);
                                    newSkillBuff.SkillLevelIncreases = newSkillBuff.SkillLevelIncreases.ToDictionary(kv => kv.Key, kv => kv.Value + 1);
                                }
                                player.buffs.Apply(newSkillBuff);
                            }
                        }
                        //For Food or drink with only custom buffs, matchingBuff will always return null.
                        //So we need to make our own food buff.
                        //With blackjack, and buffs.
                        else
                        {

                            float durationMultiplier = ((food.Quality != 0) ? 1.5f : 1f);
                            matchingBuff = new(
                                id: buffData.BuffId,
                                source: food.Name,
                                displaySource: food.DisplayName,
                                iconSheetIndex: buffData.IconSpriteIndex,
                                duration: (int)((float)buffData.Duration * durationMultiplier) * Game1.realMilliSecondsPerGameMinute
                            );


                            var newSkillBuff = new Skills.SkillBuff(matchingBuff, id, buffData.CustomFields);
                            if (player.hasBuff(newSkillBuff.id))
                            {
                                player.buffs.Remove(newSkillBuff.id);
                                newSkillBuff.millisecondsDuration = (int)(Utilities.GetLevelValue(player) * newSkillBuff.millisecondsDuration);
                                if (player.HasCustomProfession(Cooking_Skill.Cooking10b1))
                                {
                                    ApplyAttributeBuff(newSkillBuff.effects, 1f);
                                    newSkillBuff.SkillLevelIncreases = newSkillBuff.SkillLevelIncreases.ToDictionary(kv => kv.Key, kv => kv.Value + 1);
                                }
                                player.buffs.Apply(newSkillBuff);
                            }
                        }
                    }
                    else
                    {
                        foreach (Buff buff in food.GetFoodOrDrinkBuffs())
                        {
                            if (player.hasBuff(buff.id))
                            {
                                player.buffs.Remove(buff.id);
                                buff.millisecondsDuration = (int)(Utilities.GetLevelValue(player) * buff.millisecondsDuration);
                                if (player.HasCustomProfession(Cooking_Skill.Cooking10b1))
                                {
                                    ApplyAttributeBuff(buff.effects, 1f);
                                }
                                player.buffs.Apply(buff);
                            }
                        }
                    }
                }
            }
            

            // If the player has the right profession, give them an extra buff
            if (player.HasCustomProfession(Cooking_Skill.Cooking10b2))
            {
                // Define constants for attribute types and max values
                const int NumAttributes = 10;
                const int MaxAttributeValue = 5;
                const int MaxStaminaMultiplier = 16;
                const int BuffDurationMultiplier = (6000 * 10);

                // Generate random attribute and level
                int attributeBuff = Game1.random.Next(1, NumAttributes + 1);
                Log.Trace("Cooking: random attibute is: " + attributeBuff.ToString());
                int attributeLevel = Game1.random.Next(1, MaxAttributeValue + 1);
                Log.Trace("Cooking: random level is: " + attributeLevel.ToString());

                // Create a BuffEffects instance
                BuffEffects randomEffect = new()
                {
                    FarmingLevel = { 0 },
                    FishingLevel = { 0 },
                    MiningLevel = { 0 },
                    LuckLevel = { 0 },
                    ForagingLevel = { 0 },
                    MaxStamina = { 0 },
                    MagneticRadius = { 0 },
                    Defense = { 0 },
                    Attack = { 0 },
                    Speed = { 0 }
                };


                // Apply the random effect based on the randomly generated attribute
                switch (attributeBuff)
                {
                    case 1: randomEffect.FarmingLevel.Value = attributeLevel; break;
                    case 2: randomEffect.FishingLevel.Value = attributeLevel; break;
                    case 3: randomEffect.MiningLevel.Value = attributeLevel; break;
                    case 4: randomEffect.LuckLevel.Value = attributeLevel; break;
                    case 5: randomEffect.ForagingLevel.Value = attributeLevel; break;
                    case 6: randomEffect.MaxStamina.Value = attributeLevel * MaxStaminaMultiplier; break;
                    case 7: randomEffect.MagneticRadius.Value = attributeLevel * MaxStaminaMultiplier; break;
                    case 8: randomEffect.Defense.Value = attributeLevel; break;
                    case 9: randomEffect.Attack.Value = attributeLevel; break;
                    case 10: randomEffect.Speed.Value = attributeLevel; break;
                }

                // Create the buff
                Buff buff = new(
                    id: "Cooking:profession:random_buff",
                    displayName: ModEntry.Instance.I18n.Get("moonslime.Cooking.Profession10b2.buff"),
                    description: null,
                    iconTexture: ModEntry.Assets.Random_Buff,
                    iconSheetIndex: 0,
                    duration: BuffDurationMultiplier * Utilities.GetLevel(player), //Buff duration based on player Cooking level, to reward them for eating cooking foods
                    effects: randomEffect
                );
                //Remove the old buff
                player.buffs.Remove(buff.id);
                //Apply the new buff
                player.applyBuff(buff);
            }

        }

        private static void ApplyAttributeBuff(BuffEffects effects, float value)
        {
            // Define an array of all attributes with their base modifier and a multiplier
            var attributes = new (NetFloat attribute, float multiplier)[]
            {
                (effects.FarmingLevel, 1f),
                (effects.FishingLevel, 1f),
                (effects.MiningLevel, 1f),
                (effects.LuckLevel, 1f),
                (effects.ForagingLevel, 1f),
                (effects.Speed, 1f),
                (effects.Defense, 1f),
                (effects.Attack, 1f),
                (effects.CombatLevel, 1f),
                (effects.Immunity, 1f),
                (effects.MaxStamina, 16f), // Special multiplier for MaxStamina
                (effects.MagneticRadius, 16f) // Special multiplier for MagneticRadius
            };

            // Apply the value modification to each attribute
            foreach (var (attribute, multiplier) in attributes)
            {
                if (attribute.Value != 0f)
                {
                    attribute.Value += value * multiplier;
                }
            }
        }

        public static Item PreCook(CraftingRecipe recipe, Item item, bool betterCrafting = false)
        {
            ModEntry.Instance.Monitor.Log($"Starting PreCook for {item.DisplayName}", LogLevel.Trace);

            //Make sure the item coming out of the cooking recipe is an object
            if (item is StardewValley.Object obj)
            {

                float levelValue = Utilities.GetLevelValue(Game1.player);

                //increase the edibility of the object based on the cooking level of the player
                obj.Edibility = (int)(obj.Edibility * levelValue);

                //If the player has the right profession, increase the selling price
                if (Game1.player.HasCustomProfession(Cooking_Skill.Cooking10a2))
                {
                    obj.Price *= ((int)(levelValue + levelValue));


                }

                //Return the object
                if (!betterCrafting)
                {
                    return item;
                }
                else
                {
                    Utilities.BetterCraftingTempItem = item;
                    ModEntry.Instance.Monitor.Log($"Successfully finished with better crafting, returning null and stashing item for postcraft.", LogLevel.Trace);
                    return item;
                }
            }
            else
            {
                ModEntry.Instance.Monitor.Log($"Not a cooking recipe - returning item from precraft with no changes", LogLevel.Trace);
            }
            //Return the object
            return item;
        }

        public static Item PostCook(CraftingRecipe recipe, Item item, Dictionary<Item, int> consumed_items, Farmer who, bool betterCrafting = false)
        {

            if (betterCrafting)
            {
                item = Utilities.BetterCraftingTempItem;
                ModEntry.Instance.Monitor.Log($"Using better crafting - retrived stashed item: {item.DisplayName}", LogLevel.Trace);
                
            }

            //Make sure the item coming out of the cooking recipe is an object
            if (item is StardewValley.Object obj)
            {
                ModEntry.Instance.Monitor.Log($"Starting PostCook for {item.DisplayName}", LogLevel.Trace);
                //Make sure I am selecting the right items for debug purposes
                if (consumed_items != null)
                {
                    string items_string = string.Join(",", consumed_items.Select(kvp => $"{kvp.Key.DisplayName} of quality {kvp.Key.Quality}: {kvp.Value}"));
                    ModEntry.Instance.Monitor.Log($"In PostCook for recipe {items_string}", LogLevel.Trace);

                }

                //Get the exp value, based off the general exp you get from cooking (Default:2)
                float exp = ModEntry.Config.ExperienceFromCooking;
                //Get the bonus exp value based off the object's edbility. (default:50% of the object's edbility)
                float bonusExp = (obj.Edibility * ModEntry.Config.ExperienceFromEdibility);

                //Find out how many times they have cooked said recipe
                who.recipesCooked.TryGetValue(item.ItemId, out int value);
                if (value <= ModEntry.Config.BonusExpLimit)
                {
                    //Then add it to the bonus value gained from the objects edibility (Default: 10% of the items edibility given as bonus exp)
                    exp += bonusExp;
                }
                else
                {
                    //Else, give a diminishing return on the bonus exp
                    float min = Math.Max(1, value - ModEntry.Config.BonusExpLimit);
                    exp += (bonusExp / min);
                }

                //Send a message to the player at the limit for the bonus exp
                if (value == ModEntry.Config.BonusExpLimit - 1)
                {
                    Game1.showGlobalMessage(ModEntry.Instance.I18n.Get("moonslime.Cooking.no_more_bonus_exp"));
                }

                //Give the player exp. Make sure to floor the value. Don't want decimels.
                ModEntry.Instance.Monitor.Log($"Adding to player {who.Name} exp of amount {exp}", LogLevel.Trace);
                Utilities.AddEXP(who, (int)(Math.Floor(exp)));

                //Add the homecooked value to the modData for the item. So we can check for it later
                obj.modDataForSerialization.TryAdd("moonslime.Cooking.homemade", "yes");

                //determining quality
                bool QI_seasoning = item.Quality == 2;

                double ingredients_quality_RMS = 0; //using RMS as mean
                double total_items = 0;
                foreach (var consumed in consumed_items)
                {
                    int q = consumed.Key.Quality ==  4 ? 3 : consumed.Key.Quality;
                    ingredients_quality_RMS += (q*q) * consumed.Value;
                    total_items += consumed.Value;
                }
                ingredients_quality_RMS = Math.Sqrt(ingredients_quality_RMS / total_items)/3.0;
                double cooking_skill_quality = who.GetCustomSkillLevel("moonslime.Cooking") / 10.0;
                who.recipesCooked.TryGetValue(item.ItemId, out int num_times_cooked);
                double recipe_experience_quality = Math.Tanh(num_times_cooked / 20.0);
                double r = Game1.random.NextDouble();
                //old formula
                //                double dish_quality = (4*ingredients_quality_RMS + 4*cooking_skill_quality + 4*recipe_experience_quality) / 12.0;
                double dish_quality = 1.0;
                dish_quality *= (3.0 + 2.0*r) / 5.0;
                dish_quality *= 0.4 + 0.45 * cooking_skill_quality + 0.15 * recipe_experience_quality;
                dish_quality *= 0.5 + 0.15 * cooking_skill_quality + 0.35 * recipe_experience_quality;
                dish_quality *= 0.6 + 0.1 * cooking_skill_quality + 0.1 * recipe_experience_quality + 0.2*ingredients_quality_RMS;

                ModEntry.Instance.Monitor.Log($"ingredients {ingredients_quality_RMS}, skill {cooking_skill_quality}, experience {recipe_experience_quality} and random {r} led to quality {dish_quality}", LogLevel.Trace);
                if (dish_quality < 0.25)
                    obj.Quality = 0;
                else if (dish_quality < 0.5)
                    obj.Quality = 1;
                else if (dish_quality < 0.75)
                    obj.Quality = 2;
                else
                    obj.Quality = 4;
                //If the player has right profession, 50% chance tp increase dish quality
                if (Game1.player.HasCustomProfession(Cooking_Skill.Cooking5a) && Game1.random.NextBool())
                    obj.Quality += 1;
                //If the player uses QI_seasoning incerase quality by 2
                if (QI_seasoning)
                    obj.Quality += 2;
                // make sure quality is equal to 4 and not 3 if iridium, and maxes out at iridium
                if (obj.Quality >= 3)
                    obj.Quality = 4;

                ModEntry.Instance.Monitor.Log($"Created item {item.DisplayName} with size {item.Stack}", LogLevel.Trace);


                //If the player has the right profession, they get an extra number of crafts from crafting the item.
                if (who.HasCustomProfession(Cooking_Skill.Cooking10a1) && who.couldInventoryAcceptThisItem(item))
                {
                    //The chance to get an extra item is double their level chance
                    //This is to encourage people cooking while having buffs that effect cooking.
                    //So at level 10, your chance for a double craft is 60%.
                    //The player would need level 17 to have a 100% chance at double crafting items
                    float doubleLevelChance = Utilities.GetLevelValue(who, true) + Utilities.GetLevelValue(who, true);
                    if (Game1.random.NextDouble() < doubleLevelChance)
                    {
                        item.Stack += recipe.numberProducedPerCraft;
                    }
                }

                //better crafting inventory logic
                if (betterCrafting)
                {
                    if (who.couldInventoryAcceptThisItem(item))
                    {
                        ModEntry.Instance.Monitor.Log($"Adding item to directly to inventory instead of to hand, adding {item.DisplayName} with size {item.Stack}", LogLevel.Trace);
                        who.addItemToInventory(item);
                        //register dish as cooked and make the necessary checks
                        who.checkForQuestComplete(null, -1, -1, item, null, 2);
                        who.cookedRecipe(item.ItemId);
                        Game1.stats.checkForCookingAchievements();
                        return null;
                    }
                    else
                    {
                        ModEntry.Instance.Monitor.Log($"Dropping item to ground, adding {item.DisplayName} with size {item.Stack}", LogLevel.Trace);
                        who.currentLocation.debris.Add(new Debris(item, who.Position));
                        //register dish as cooked and make the necessary checks
                        who.checkForQuestComplete(null, -1, -1, item, null, 2);
                        who.cookedRecipe(item.ItemId);
                        Game1.stats.checkForCookingAchievements();
                        return null;
                    }
                }

            }
            else
            {
                ModEntry.Instance.Monitor.Log($"Not a cooking recipe - returning item from postcraft with no changes", LogLevel.Trace);
            }
            //Return the object
            return item;
        }



        public static IList<Item> GetContainerContents(List<IInventory> _materialContainers)
        {
            if (_materialContainers == null)
            {
                return null;
            }

            List<Item> list = new List<Item>();
            foreach (IInventory materialContainer in _materialContainers)
            {
                list.AddRange(materialContainer);
            }

            return list;
        }


    }
}

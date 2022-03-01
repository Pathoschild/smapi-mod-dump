/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/WizardsLizards/CauldronOfChance
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CauldronOfChance
{
    public class CauldronMagic
    {
        public static string errorMessageStats = "";
        public static string errorMessageProgress = "";

        #region properties
        #region setup
        GameLocation WizardHouse { get; set; }
        NPC Wizard { get; set; }
        StardewValley.Item resultingItem { get; set; }
        Random randomGenerator { get; set; }
        double playerLuck { get => Game1.player.DailyLuck + ingredient1.cauldronLuck + ingredient2.cauldronLuck + ingredient3.cauldronLuck + this.cauldronLuck; }
        #endregion setup

        #region ingredients
        public Ingredient ingredient1 { get; set; }
        public Ingredient ingredient2 { get; set; }
        public Ingredient ingredient3 { get; set; }
        #endregion ingredients

        #region chances
        #region special
        public double butterflies { get; set; } = 0;
        public double boom { get; set; } = 0;
        public double cauldronLuck { get; set; } = 0;
        public double duration { get; set; } = 1;
        public int effectType { get; set; } = 0;
        #endregion

        public List<int> buffList { get; set; }
        public List<int> debuffList { get; set; }
        #endregion chances
        #endregion properties

        #region constructors
        public CauldronMagic(Item ingredient1, Item ingredient2, Item ingredient3)
        {
            try
            {
                errorMessageStats += "Daily Luck: " + Game1.player.DailyLuck + ", ";
                errorMessageStats += "Has special Charm: " + Game1.player.hasSpecialCharm + ", ";
                errorMessageStats += "Ingredient 1: " + ingredient1.Name + ", ";
                errorMessageStats += "Ingredient 2: " + ingredient2.Name + ", ";
                errorMessageStats += "Ingredient 3: " + ingredient3.Name + ", ";

                errorMessageProgress = "Started Algorithm, ";

                using (Cauldron Cauldron = new Cauldron(this))
                {
                    this.ingredient1 = Cauldron.getIngredient(ingredient1);
                    errorMessageProgress = "Added Ingredient1";
                    this.ingredient2 = Cauldron.getIngredient(ingredient2);
                    errorMessageProgress = "Added Ingredient2";
                    this.ingredient3 = Cauldron.getIngredient(ingredient3);
                    errorMessageProgress = "Added Ingredient3";

                    WizardHouse = Game1.locations.Where(x => x.Name.Equals("WizardHouse")).First();
                    errorMessageProgress = "Got WizardHouse";
                    Wizard = Game1.getCharacterFromName("Wizard");
                    errorMessageProgress = "Got Wizard";
                    resultingItem = new StardewValley.Object();
                    randomGenerator = new Random();

                    buffList = new List<int>();
                    foreach (int buffIndex in Enum.GetValues(typeof(Cauldron.buffs)))
                    {
                        buffList.Add(0);
                    }
                    debuffList = new List<int>();
                    foreach (int debuffIndex in Enum.GetValues(typeof(Cauldron.debuffs)))
                    {
                        debuffList.Add(0);
                    }

                    errorMessageProgress = "Before UseCauldron";
                    UseCauldron(Cauldron);
                    errorMessageProgress = "After UseCauldron";

                    errorMessageStats += "Unique MultiplayerID: " + Game1.player.UniqueMultiplayerID + ", ";
                    ModEntry.userIds.Add(Game1.player.UniqueMultiplayerID);
                }
            }
            catch (Exception ex)
            {
                ObjectPatches.IMonitor.Log($"Failed in Cauldron Magic with Error Code:\n {ex}\n; Progress:\n {errorMessageProgress}\n; Values\n {errorMessageStats}", LogLevel.Error);
            }
        }
        #endregion constructors

        #region functions
        public void UseCauldron(Cauldron Cauldron)
        {
            #region determine effects
            findCombinations(Cauldron);
            errorMessageProgress = "After findCombinations";
            #endregion determine effects

            #region determine actual effect
            determineResult(Cauldron);
            errorMessageProgress = "after determineResult";
            #endregion determine actual effect

            #region take effect
            //Drink buff
            if (effectType == 1)
            {
                //Game1.addHUDMessage(new HUDMessage("Buff"));

                //ObjectPatches.IModHelper.Reflection.GetMethod(Game1.player, "performDrinkAnimation").Invoke(new StardewValley.Object());
                Game1.player.eatObject(Utility.fuzzyItemSearch("Joja Cola") as StardewValley.Object);
                Game1.player.canMove = false;

                DelayedAction.delayedBehavior onDrink = delegate
                {
                    //Game1.addHUDMessage(new HUDMessage("OnDrinkBuff"));
                    Game1.player.canMove = true;
                    Game1.activeClickableMenu = new DialogueBox("The magic of the Cauldron flows through you...");
                };

                DelayedAction.functionAfterDelay(onDrink, 1000);
            }
            //Drink debuff
            else if (effectType == 2)
            {
                //Game1.addHUDMessage(new HUDMessage("Debuff"));

                //ObjectPatches.IModHelper.Reflection.GetMethod(Game1.player, "performDrinkAnimation").Invoke(new StardewValley.Object());
                Game1.player.eatObject(Utility.fuzzyItemSearch("Joja Cola") as StardewValley.Object);
                Game1.player.canMove = false;

                DelayedAction.delayedBehavior afterDrink = delegate
                {
                    //Game1.addHUDMessage(new HUDMessage("AfterDrinkDebuff"));
                    Game1.player.canMove = true;
                    Game1.activeClickableMenu = new DialogueBox("An aweful taste fills your mouth...");
                };

                DelayedAction.delayedBehavior onDrink = delegate
                {
                    //Game1.addHUDMessage(new HUDMessage("OnDrinkDebuff"));
                    Game1.player.performPlayerEmote("sick");
                    DelayedAction.functionAfterDelay(afterDrink, 1000);
                };

                DelayedAction.functionAfterDelay(onDrink, 2500);
            }
            //Butterflies!!!
            else if (effectType == 3)
            {
                WizardHouse.critters = new List<Critter>();
                for(int counter = 0; counter < 7; counter++)
                {
                    WizardHouse.critters.Add(new Butterfly(new Microsoft.Xna.Framework.Vector2(4, 20)));
                }
                Game1.player.changeFriendship(222, Wizard);
                Wizard.doEmote(NPC.heartEmote);

                Game1.player.canMove = false;
                DelayedAction.delayedBehavior onButterflies = delegate
                {
                    Game1.player.canMove = true;
                    Game1.activeClickableMenu = new DialogueBox("A pleasent smell fills the air...");
                };
                DelayedAction.functionAfterDelay(onButterflies, 100);
            }
            //BOOOM!!!!
            else if (effectType == 4)
            {
                WizardHouse.explode(new Microsoft.Xna.Framework.Vector2(3, 20), 3, Game1.player);
                Game1.player.changeFriendship(-222, Wizard);
                Wizard.doEmote(NPC.angryEmote);

                Game1.player.canMove = false;
                DelayedAction.delayedBehavior onExplosion = delegate
                {
                    Game1.player.canMove = true;
                    Game1.activeClickableMenu = new DialogueBox("A vile stench fills the air...");
                };
                DelayedAction.functionAfterDelay(onExplosion, 100);
            }
            //Get item
            else if (effectType == 5)
            {
                Game1.player.currentLocation.debris.Add(new Debris(resultingItem, new Microsoft.Xna.Framework.Vector2(3 * 64f, 20 * 64f)));

                Game1.player.canMove = false;
                DelayedAction.delayedBehavior onDrop = delegate
                {
                    Game1.player.canMove = true;
                    Game1.activeClickableMenu = new DialogueBox($"A {resultingItem.DisplayName} emerges from the depths of the Cauldron.");
                };
                DelayedAction.functionAfterDelay(onDrop, 100);
            }
            //Item effect same as a food
            else if (effectType == 6)
            {
                Game1.player.canMove = false;
                ObjectPatches.IModHelper.Reflection.GetMethod(Game1.player, "performDrinkAnimation").Invoke(new StardewValley.Object());

                DelayedAction.delayedBehavior onDrink = delegate
                {
                    Game1.player.canMove = true;
                    Game1.activeClickableMenu = new DialogueBox($"The taste reminds you of {resultingItem.DisplayName}...");
                };

                DelayedAction.functionAfterDelay(onDrink, 1000);
            }
            errorMessageProgress = "";
            #endregion take effect
        }

        #region helper
        #region chance values
        public void findCombinations(Cauldron Cauldron)
        {
            List<string> ingredients = new List<string>() { ingredient1.item.Name, ingredient2.item.Name, ingredient3.item.Name }.Distinct().ToList();

            foreach ((string Type, int Match2, int Match3, List<string> Items) buffCombination in Cauldron.buffCombinations)
            {
                int matches = ingredients.Intersect(buffCombination.Items).Count();

                int value = 0;

                if (matches == 2)
                {
                    value = buffCombination.Match2;
                }
                else if (matches >= 3)
                {
                    value = buffCombination.Match3;
                }

                if(value > 0)
                {
                    switch (buffCombination.Type)
                    {
                        case "butterflies":
                            butterflies += value * Cauldron.butterfliesConst;
                            break;
                        case "boom":
                            boom += value * Cauldron.boomConst;
                            break;
                        case "cauldronLuck":
                            cauldronLuck += value * Cauldron.cauldronLuckConst;
                            break;
                        case "duration":
                            duration += value * Cauldron.durationConst;
                            break;
                        default:
                            //Add combinations double cuz players got more choice in adding it?
                            Cauldron.addToCauldron(buffCombination.Type, value);
                            break;
                    }
                }
            }
        }
        #endregion chance values

        #region chance determination
        public void determineResult(Cauldron Cauldron)
        {
            double butterboomChance = randomGenerator.NextDouble();
            //Chance for butterflies
            if (butterboomChance > 1 - getButterflies() - (getButterflies() * playerLuck))
            {
                effectType = 3;
                errorMessageStats += $"Effecttype: {effectType}, ";
                return;
            }
            //Chance for boom
            if (butterboomChance < getBoom() - (getBoom() * playerLuck))
            {
                effectType = 4;
                errorMessageStats += $"Effecttype: {effectType}, ";
                return;
            }

            //Check for crafting recipes (Drop)
            double recipeChance = CheckForRecipes(Cauldron);
            double randomRecipeChance = randomGenerator.NextDouble();
            if (randomRecipeChance < recipeChance + (recipeChance * playerLuck))
            {
                effectType = 5;
                errorMessageStats += $"Effecttype: {effectType}, ";
                return;
            }

            //Check for other item drops? (Drop)
            double itemChance = CheckForItemCombinations(Cauldron);
            double randomItemChance = randomGenerator.NextDouble();
            if (randomItemChance < itemChance + (itemChance * playerLuck))
            {
                effectType = 5;
                errorMessageStats += $"Effecttype: {effectType}, ";
                return;
            }

            //Check for cooking recipes (Drink)
            double cookingChance = CheckForCookingRecipes(Cauldron);
            double randomCookingChance = randomGenerator.NextDouble();
            if (randomCookingChance < cookingChance + (cookingChance * playerLuck))
            {
                //Other text in the end (other buff building too tho...?)
                effectType = 6;
                errorMessageStats += $"Effecttype: {effectType}, ";
            }
            errorMessageStats += $"Effecttype: {effectType}, ";

            int farming = 0;
            int fishing = 0;
            int mining = 0;
            int digging = 0;
            int luck = 0;
            int foraging = 0;
            int crafting = 0;
            int maxStamina = 0;
            int magneticRadius = 0;
            int speed = 0;
            int defense = 0;
            int attack = 0;

            int defaultBuff = -1;

            int randomDuration = randomGenerator.Next(5, 20);
            int minutesDuration = (int)(randomDuration * getDuration() * 60);
            string source = "CauldronOfChance";
            string displaySource = "Cauldron";

            string description = "";

            if (effectType == 6)
            {
                errorMessageProgress = "Getting cooking stats";
                description = $"The taste of {resultingItem.DisplayName} still lingers in your mouth...";

                if (resultingItem is StardewValley.Object)
                {
                    StardewValley.Object csObject = resultingItem as StardewValley.Object;

                    if (csObject.Name.Equals("Squid Ink Ravioli"))
                    {
                        defaultBuff = 28;
                    }

                    string[] objectDescription = Game1.objectInformation[csObject.ParentSheetIndex].Split('/');

                    if (Convert.ToInt32(objectDescription[2]) > 0)
                    {
                        string[] whatToBuff = (string[])((objectDescription.Length > 7) ? ((object)objectDescription[7].Split(' ')) : ((object)new string[12]
                        {
                        "0", "0", "0", "0", "0", "0", "0", "0", "0", "0",
                        "0", "0"
                        }));

                        csObject.ModifyItemBuffs(whatToBuff);

                        errorMessageStats += $"Cooking Stats: {whatToBuff}, ";

                        farming = Convert.ToInt32(whatToBuff[0]);
                        mining = Convert.ToInt32(whatToBuff[2]);
                        fishing = Convert.ToInt32(whatToBuff[1]);
                        foraging = Convert.ToInt32(whatToBuff[5]);
                        attack = (whatToBuff.Length > 11) ? Convert.ToInt32(whatToBuff[11]) : 0;
                        defense = Convert.ToInt32(whatToBuff[10]);
                        maxStamina = Convert.ToInt32(whatToBuff[7]) / 10;
                        luck = Convert.ToInt32(whatToBuff[4]);
                        magneticRadius = Convert.ToInt32(whatToBuff[8]) / 32;
                        speed = Convert.ToInt32(whatToBuff[9]);
                    }
                }
                errorMessageProgress = "Got cooking stats";
            }
            else
            {
                errorMessageProgress = "Calculating stats";
                //Check for other buffs (Drink)
                int buffChance = (int)(getCombinedBuffChance() + (getCombinedBuffChance() * playerLuck));
                int debuffChance = (int)(getCombinedDebuffChance() - (getCombinedDebuffChance() * playerLuck));

                if (buffChance < 1)
                {
                    buffChance = 1;
                }
                if (debuffChance < 1)
                {
                    debuffChance = 1;
                }

                int buffRandom = randomGenerator.Next(1, buffChance + debuffChance);

                #region generate buff values
                string deBuff = "";
                int value = -1;
                int type = 0;

                if (buffRandom <= buffChance)
                {
                    //Buff
                    effectType = 1;

                    //if (getCombinedDebuffChance() < 3)
                    if (getCombinedBuffChance() == 0)
                    {
                        buffList[(int)Cauldron.buffs.attackBuff1] += 1;
                        //buffList[(int)Cauldron.buffs.debuffImmunity1] += 1;
                        buffList[(int)Cauldron.buffs.defenseBuff1] += 1;
                        buffList[(int)Cauldron.buffs.farmingBuff1] += 1;
                        buffList[(int)Cauldron.buffs.fishingBuff1] += 1;
                        buffList[(int)Cauldron.buffs.foragingBuff1] += 1;
                        //buffList[(int)Cauldron.buffs.garlicOil1] += 1;
                        buffList[(int)Cauldron.buffs.luckBuff1] += 1;
                        buffList[(int)Cauldron.buffs.magneticRadiusBuff1] += 1;
                        buffList[(int)Cauldron.buffs.maxEnergyBuff1] += 1;
                        buffList[(int)Cauldron.buffs.miningBuff1] += 1;
                        buffList[(int)Cauldron.buffs.speedBuff1] += 1;
                    }

                    int maxBuffs = getCombinedBuffChance();

                    int buffRandomCounter = randomGenerator.Next(1, maxBuffs);

                    List<int> buffs = getAllBuffs();

                    int finalBuffIndex = -1;

                    foreach (int buffIndex in Enum.GetValues(typeof(Cauldron.buffs)))
                    {
                        if (buffs[buffIndex] > 0)
                        {
                            buffRandomCounter -= buffs[buffIndex];

                            if (buffRandomCounter <= 0)
                            {
                                finalBuffIndex = buffIndex;
                                break;
                            }
                        }
                    }

                    if (finalBuffIndex != -1)
                    {
                        switch (finalBuffIndex)
                        {
                            case (int)Cauldron.buffs.garlicOil1:
                                description += "(Garlic Oil)";
                                deBuff = "You've started to smell like... garlic?";
                                defaultBuff = 23;
                                break;
                            case (int)Cauldron.buffs.garlicOil2:
                                description += "(Garlic Oil)";
                                deBuff = "You've started to smell like... garlic?";
                                defaultBuff = 23;
                                break;
                            case (int)Cauldron.buffs.garlicOil3:
                                description += "(Garlic Oil)";
                                deBuff = "You've started to smell like... garlic?";
                                defaultBuff = 23;
                                break;
                            case (int)Cauldron.buffs.debuffImmunity1:
                                description += "(Debuff Immunity)";
                                deBuff = "You feel like nothing can slow you down today!";
                                defaultBuff = 28;
                                break;
                            case (int)Cauldron.buffs.debuffImmunity2:
                                description += "(Debuff Immunity)";
                                deBuff = "You feel like nothing can slow you down today!";
                                defaultBuff = 28;
                                break;
                            case (int)Cauldron.buffs.debuffImmunity3:
                                description += "(Debuff Immunity)";
                                deBuff = "You feel like nothing can slow you down today!";
                                defaultBuff = 28;
                                break;
                            case (int)Cauldron.buffs.farmingBuff1:
                                description += "(Farming +1)";
                                deBuff = "farming";
                                type = 1;
                                value = 1;
                                farming = 1;
                                break;
                            case (int)Cauldron.buffs.farmingBuff2:
                                description += "(Farming +2)";
                                deBuff = "farming";
                                type = 1;
                                value = 2;
                                farming = 2;
                                break;
                            case (int)Cauldron.buffs.farmingBuff3:
                                description += "(Farming +3)";
                                deBuff = "farming";
                                type = 1;
                                value = 3;
                                farming = 3;
                                break;
                            case (int)Cauldron.buffs.miningBuff1:
                                description += "(Mining +1)";
                                deBuff = "mining";
                                type = 1;
                                value = 1;
                                mining = 1;
                                break;
                            case (int)Cauldron.buffs.miningBuff2:
                                description += "(Mining +2)";
                                deBuff = "mining";
                                type = 1;
                                value = 2;
                                mining = 2;
                                break;
                            case (int)Cauldron.buffs.miningBuff3:
                                description += "(Mining +3)";
                                deBuff = "mining";
                                type = 1;
                                value = 3;
                                mining = 3;
                                break;
                            case (int)Cauldron.buffs.fishingBuff1:
                                description += "(Fishing +1)";
                                deBuff = "fishing";
                                type = 1;
                                value = 1;
                                fishing = 1;
                                break;
                            case (int)Cauldron.buffs.fishingBuff2:
                                description += "(Fishing +2)";
                                deBuff = "fishing";
                                type = 1;
                                value = 2;
                                fishing = 2;
                                break;
                            case (int)Cauldron.buffs.fishingBuff3:
                                description += "(Fishing +3)";
                                deBuff = "fishing";
                                type = 1;
                                value = 3;
                                fishing = 3;
                                break;
                            case (int)Cauldron.buffs.foragingBuff1:
                                description += "(Foraging +1)";
                                deBuff = "foraging";
                                type = 1;
                                value = 1;
                                foraging = 1;
                                break;
                            case (int)Cauldron.buffs.foragingBuff2:
                                description += "(Foraging +2)";
                                deBuff = "foraging";
                                type = 1;
                                value = 2;
                                foraging = 2;
                                break;
                            case (int)Cauldron.buffs.foragingBuff3:
                                description += "(Foraging +3)";
                                deBuff = "foraging";
                                type = 1;
                                value = 3;
                                foraging = 3;
                                break;
                            case (int)Cauldron.buffs.attackBuff1:
                                description += "(Attack +1)";
                                deBuff = "strong";
                                type = 2;
                                value = 1;
                                attack = 1;
                                break;
                            case (int)Cauldron.buffs.attackBuff2:
                                description += "(Attack +2)";
                                deBuff = "strong";
                                type = 2;
                                value = 2;
                                attack = 2;
                                break;
                            case (int)Cauldron.buffs.attackBuff3:
                                description += "(Attack +3)";
                                deBuff = "strong";
                                type = 2;
                                value = 3;
                                attack = 3;
                                break;
                            case (int)Cauldron.buffs.defenseBuff1:
                                description += "(Defense +1)";
                                deBuff = "resilient";
                                type = 2;
                                value = 1;
                                defense = 1;
                                break;
                            case (int)Cauldron.buffs.defenseBuff2:
                                description += "(Defense +2)";
                                deBuff = "resilient";
                                type = 2;
                                value = 2;
                                defense = 2;
                                break;
                            case (int)Cauldron.buffs.defenseBuff3:
                                description += "(Defense +3)";
                                deBuff = "resilient";
                                type = 2;
                                value = 3;
                                defense = 3;
                                break;
                            case (int)Cauldron.buffs.maxEnergyBuff1:
                                description += "(Max Stamina +10)";
                                deBuff = "vigorous";
                                type = 2;
                                value = 1;
                                maxStamina = 1;
                                break;
                            case (int)Cauldron.buffs.maxEnergyBuff2:
                                description += "(Max Stamina +20)";
                                deBuff = "vigorous";
                                type = 2;
                                value = 2;
                                maxStamina = 2;
                                break;
                            case (int)Cauldron.buffs.maxEnergyBuff3:
                                description += "(Max Stamina +30)";
                                deBuff = "vigorous";
                                type = 2;
                                value = 3;
                                maxStamina = 3;
                                break;
                            case (int)Cauldron.buffs.luckBuff1:
                                description += "(Luck +1)";
                                deBuff = "lucky";
                                type = 2;
                                value = 1;
                                luck = 1;
                                break;
                            case (int)Cauldron.buffs.luckBuff2:
                                description += "(Luck +2)";
                                deBuff = "lucky";
                                type = 2;
                                value = 2;
                                luck = 2;
                                break;
                            case (int)Cauldron.buffs.luckBuff3:
                                description += "(Luck +3)";
                                deBuff = "lucky";
                                type = 2;
                                value = 3;
                                luck = 3;
                                break;
                            case (int)Cauldron.buffs.magneticRadiusBuff1:
                                description += "(Magnetic Radius +16)";
                                deBuff = "magnetic";
                                type = 2;
                                value = 1;
                                magneticRadius = 1;
                                break;
                            case (int)Cauldron.buffs.magneticRadiusBuff2:
                                description += "(Magnetic Radius +32)";
                                deBuff = "magnetic";
                                type = 2;
                                value = 2;
                                magneticRadius = 2;
                                break;
                            case (int)Cauldron.buffs.magneticRadiusBuff3:
                                description += "(Magnetic Radius + 48)";
                                deBuff = "magnetic";
                                type = 2;
                                value = 3;
                                magneticRadius = 3;
                                break;
                            case (int)Cauldron.buffs.speedBuff1:
                                description += "(Speed +1)";
                                deBuff = "fast";
                                type = 2;
                                value = 1;
                                speed = 1;
                                break;
                            case (int)Cauldron.buffs.speedBuff2:
                                description += "(Speed +2)";
                                deBuff = "fast";
                                type = 2;
                                value = 2;
                                speed = 2;
                                break;
                            case (int)Cauldron.buffs.speedBuff3:
                                description += "(Speed +3)";
                                deBuff = "fast";
                                type = 2;
                                value = 3;
                                speed = 3;
                                break;
                        }
                    }
                }
                else
                {
                    //Debuff
                    effectType = 2;

                    //if (getCombinedBuffChance() < 3)
                    if (getCombinedDebuffChance() == 0)
                    {
                        debuffList[(int)Cauldron.debuffs.attackDebuff1] += 1;
                        debuffList[(int)Cauldron.debuffs.defenseDebuff1] += 1;
                        debuffList[(int)Cauldron.debuffs.farmingDebuff1] += 1;
                        debuffList[(int)Cauldron.debuffs.fishingDebuff1] += 1;
                        debuffList[(int)Cauldron.debuffs.foragingDebuff1] += 1;
                        debuffList[(int)Cauldron.debuffs.luckDebuff1] += 1;
                        debuffList[(int)Cauldron.debuffs.maxEnergyDebuff1] += 1;
                        debuffList[(int)Cauldron.debuffs.miningDebuff1] += 1;
                        //debuffList[(int)Cauldron.debuffs.monsterMusk1] += 1;
                        debuffList[(int)Cauldron.debuffs.speedDebuff1] += 1;
                    }

                    int maxDebuffs = getCombinedDebuffChance();

                    int debuffRandomCounter = randomGenerator.Next(1, maxDebuffs);

                    List<int> debuffs = getAllDebuffs();

                    int finalDebuffIndex = -1;

                    foreach (int debuffIndex in Enum.GetValues(typeof(Cauldron.debuffs)))
                    {
                        if (debuffs[debuffIndex] > 0)
                        {
                            debuffRandomCounter -= debuffs[debuffIndex];

                            if (debuffRandomCounter <= 0)
                            {
                                finalDebuffIndex = debuffIndex;
                                break;
                            }
                        }
                    }

                    if (finalDebuffIndex != -1)
                    {
                        switch (finalDebuffIndex)
                        {
                            case (int)Cauldron.debuffs.monsterMusk1:
                                description += "(Monster Musk)";
                                deBuff = "A sweet stench emnates from you...";
                                defaultBuff = 24;
                                break;
                            case (int)Cauldron.debuffs.monsterMusk2:
                                description += "(Monster Musk)";
                                deBuff = "A sweet stench emnates from you...";
                                defaultBuff = 24;
                                break;
                            case (int)Cauldron.debuffs.monsterMusk3:
                                description += "(Monster Musk)";
                                deBuff = "A sweet stench emnates from you...";
                                defaultBuff = 24;
                                break;
                            case (int)Cauldron.debuffs.farmingDebuff1:
                                description += "(Farming -1)";
                                deBuff = "farming";
                                type = 1;
                                value = 1;
                                farming = -1;
                                break;
                            case (int)Cauldron.debuffs.farmingDebuff2:
                                description += "(Farming -2)";
                                deBuff = "farming";
                                type = 1;
                                value = 2;
                                farming = -2;
                                break;
                            case (int)Cauldron.debuffs.farmingDebuff3:
                                description += "(Farming -3)";
                                deBuff = "farming";
                                type = 1;
                                value = 3;
                                farming = -3;
                                break;
                            case (int)Cauldron.debuffs.miningDebuff1:
                                description += "(Mining -1)";
                                deBuff = "mining";
                                type = 1;
                                value = 1;
                                mining = -1;
                                break;
                            case (int)Cauldron.debuffs.miningDebuff2:
                                description += "(Mining -2)";
                                deBuff = "mining";
                                type = 1;
                                value = 2;
                                mining = -2;
                                break;
                            case (int)Cauldron.debuffs.miningDebuff3:
                                description += "(Mining -3)";
                                deBuff = "mining";
                                type = 1;
                                value = 3;
                                mining = -3;
                                break;
                            case (int)Cauldron.debuffs.fishingDebuff1:
                                description += "(Fishing -1)";
                                deBuff = "fishing";
                                type = 1;
                                value = 1;
                                fishing = -1;
                                break;
                            case (int)Cauldron.debuffs.fishingDebuff2:
                                description += "(Fishing -2)";
                                deBuff = "fishing";
                                type = 1;
                                value = 2;
                                fishing = -2;
                                break;
                            case (int)Cauldron.debuffs.fishingDebuff3:
                                description += "(Fishing -3)";
                                deBuff = "fishing";
                                type = 1;
                                value = 3;
                                fishing = -3;
                                break;
                            case (int)Cauldron.debuffs.foragingDebuff1:
                                description += "(Foraging -1)";
                                deBuff = "foraging";
                                type = 1;
                                value = 1;
                                foraging = -1;
                                break;
                            case (int)Cauldron.debuffs.foragingDebuff2:
                                description += "(Foraging -2)";
                                deBuff = "foraging";
                                type = 1;
                                value = 2;
                                foraging = -2;
                                break;
                            case (int)Cauldron.debuffs.foragingDebuff3:
                                description += "(Foraging -3)";
                                deBuff = "foraging";
                                type = 1;
                                value = 3;
                                foraging = -3;
                                break;
                            case (int)Cauldron.debuffs.attackDebuff1:
                                description += "(Attack -1)";
                                deBuff = "strong";
                                type = 2;
                                value = 1;
                                attack = -1;
                                break;
                            case (int)Cauldron.debuffs.attackDebuff2:
                                description += "(Attack -2)";
                                deBuff = "strong";
                                type = 2;
                                value = 2;
                                attack = -2;
                                break;
                            case (int)Cauldron.debuffs.attackDebuff3:
                                description += "(Attack -3)";
                                deBuff = "strong";
                                type = 2;
                                value = 3;
                                attack = -3;
                                break;
                            case (int)Cauldron.debuffs.defenseDebuff1:
                                description += "(Defense -1)";
                                deBuff = "resilient";
                                type = 2;
                                value = 1;
                                defense = -1;
                                break;
                            case (int)Cauldron.debuffs.defenseDebuff2:
                                description += "(Defense -2)";
                                deBuff = "resilient";
                                type = 2;
                                value = 2;
                                defense = -2;
                                break;
                            case (int)Cauldron.debuffs.defenseDebuff3:
                                description += "(Defense -3)";
                                deBuff = "resilient";
                                type = 2;
                                value = 3;
                                defense = -3;
                                break;
                            case (int)Cauldron.debuffs.maxEnergyDebuff1:
                                description += "(Max Stamina -10)";
                                deBuff = "vigorous";
                                type = 2;
                                value = 1;
                                maxStamina = -1 * 10;
                                break;
                            case (int)Cauldron.debuffs.maxEnergyDebuff2:
                                description += "(Max Stamina -20)";
                                deBuff = "vigorous";
                                type = 2;
                                value = 2;
                                maxStamina = -2 * 10;
                                break;
                            case (int)Cauldron.debuffs.maxEnergyDebuff3:
                                description += "(Max Stamina -30)";
                                deBuff = "vigorous";
                                type = 2;
                                value = 3;
                                maxStamina = -3 * 10;
                                break;
                            case (int)Cauldron.debuffs.luckDebuff1:
                                description += "(Luck -1)";
                                deBuff = "lucky";
                                type = 2;
                                value = 1;
                                luck = -1;
                                break;
                            case (int)Cauldron.debuffs.luckDebuff2:
                                description += "(Luck -2)";
                                deBuff = "lucky";
                                type = 2;
                                value = 2;
                                luck = -2;
                                break;
                            case (int)Cauldron.debuffs.luckDebuff3:
                                description += "(Luck -3)";
                                deBuff = "lucky";
                                type = 2;
                                value = 3;
                                luck = -3;
                                break;
                            case (int)Cauldron.debuffs.speedDebuff1:
                                description += "(Speed -1)";
                                deBuff = "fast";
                                type = 2;
                                value = 1;
                                speed = -1;
                                break;
                            case (int)Cauldron.debuffs.speedDebuff2:
                                description += "(Speed -2)";
                                deBuff = "fast";
                                type = 2;
                                value = 2;
                                speed = -2;
                                break;
                            case (int)Cauldron.debuffs.speedDebuff3:
                                description += "(Speed -3)";
                                deBuff = "fast";
                                type = 2;
                                value = 3;
                                speed = -3;
                                break;
                        }
                    }
                }

                #region template
                //Game1.buffsDisplay.addOtherBuff(
                //    new(0,
                //        0,
                //        0,
                //        0,
                //        0,
                //        0,
                //        0,
                //        0,
                //        0,
                //        0,
                //        0,
                //        0,
                //        minutesDuration: 1,
                //        source: "<internal buff name>",
                //        displaySource: ModEntry.ModHelper.Translation.Get("<what should appear in game as the source>"))
                //    {
                //        which = myBuffId,
                //        sheetIndex = < index of the buff icon >,
                //        glow = <if player should glow set to something other than Color.White >,
                //        millisecondsDuration = < here you set the actual duration >,
                //        description = ModEntry.ModHelper.Translation.Get("<should appear in game as the description>")
                //    }
                //);
                #endregion template

                string modifier = "";
                string modifierEnd = ".";
                string debuffModifier = "";

                if (effectType == 2)
                {
                    debuffModifier = "don't ";
                }

                if (type == 0)
                {
                    description = $"{deBuff}";
                }
                else if (type == 1)
                {
                    if (value == 2)
                    {
                        modifier = "quite ";
                        modifierEnd = "...";
                    }
                    else if (value == 3)
                    {
                        modifier = "really ";
                        modifierEnd = "!!";
                    }

                    //description += $"You {debuffModifier}{modifier}feel like {deBuff} today{modifierEnd}";
                    description = $"You {debuffModifier}{modifier}feel like {deBuff} today{modifierEnd}";
                }
                else if (type == 2)
                {
                    if (value == 2)
                    {
                        modifier = "quite ";
                        modifierEnd = "...";
                    }
                    else if (value == 3)
                    {
                        modifier = "very ";
                        modifierEnd = "!!";
                    }

                    //description += $"You {debuffModifier}feel {modifier}{deBuff} today{modifierEnd}";
                    description = $"You {debuffModifier}feel {modifier}{deBuff} today{modifierEnd}";
                    errorMessageStats += $"Buff description: {description}, ";
                }
                #endregion generate buff values
                errorMessageProgress = "Calculated stats";
            }

            #region create buff
            Buff buff = new Buff(farming, fishing, mining, digging, luck, foraging, crafting, maxStamina * 10, magneticRadius * 16, speed, defense, attack, minutesDuration, source, displaySource)
            {
                sheetIndex = 17,
                //millisecondsDuration = < here you set the actual duration >,
                description = description
            };

            if(defaultBuff != -1)
            {
                buff.which = defaultBuff;
            }
            

            Game1.buffsDisplay.addOtherBuff(buff);
            #endregion create buff
        }

        public double CheckForRecipes(Cauldron Cauldron)
        {
            //0-1/3 Parts: 0% Chance. 2/3 Parts: 25% Chance. 3/3 Parts: 75% Chance (Always top down. If full match always that. if no full match but multiple 25%: Same chance for everything)
            List<string> fullMatches = new List<string>();
            List<string> halfMatches = new List<string>();

            foreach((string Result, List<string> Items) recipe in Cauldron.recipes)
            {
                List<string> ingredients = new List<string>() { ingredient1.item.Name, ingredient2.item.Name, ingredient3.item.Name };
                int matches = 0;

                foreach(string Item in recipe.Items)
                {
                    if (ingredients.Contains(Item))
                    {
                        matches += 1;
                        ingredients.Remove(Item);
                    }
                }

                if(matches == 2)
                {
                    halfMatches.Add(recipe.Result);
                }
                else if (matches >= 3)
                {
                    fullMatches.Add(recipe.Result);
                }
            }

            if (fullMatches.Count > 0)
            {
                //Get item
                int index = randomGenerator.Next(0, fullMatches.Count - 1);

                errorMessageProgress = "Searching Resulting Item";
                errorMessageStats += $"Resulting Item: {fullMatches[index]}, ";
                resultingItem = Utility.fuzzyItemSearch(fullMatches[index]);
                errorMessageProgress = "Found Resulting Item";

                return 0.75;
            }
            else if (halfMatches.Count > 0)
            {
                //Get item
                int index = randomGenerator.Next(0, halfMatches.Count - 1);

                errorMessageProgress = "Searching Resulting Item";
                errorMessageStats += $"Resulting Item: {halfMatches[index]}, ";
                resultingItem = Utility.fuzzyItemSearch(halfMatches[index]);
                errorMessageProgress = "Found Resulting Item";

                return 0.25;
            }

            return 0;
        }

        public double CheckForItemCombinations(Cauldron Cauldron)
        {
            //2 Items Match: 5% Chance. 3 Items Match: 10% Chance (Does 3-Item match override 2-Item match tho? (Even a possibility?))
            // => Decide on item by chance: 10% items are added 3x to the pool, 5% items once. Once decided: can be 10% chance even for a 5% chance item, if theres a match
            double chance = 0;

            List<string> match2 = new List<string>();
            List<string> match3 = new List<string>();
            List<string> ingredients = new List<string>() { ingredient1.item.Name, ingredient2.item.Name, ingredient3.item.Name }.Distinct().ToList();

            foreach ((string Result, List<string> Items) itemCombination in Cauldron.itemCombinations)
            {
                int matches = ingredients.Intersect(itemCombination.Items).Count();

                if(matches == 2)
                {
                    match2.Add(itemCombination.Result);

                    if(chance < 0.05)
                    {
                        chance = 0.05;
                    }
                }
                else if (matches >= 3)
                {
                    match3.Add(itemCombination.Result);

                    if(chance < 0.1)
                    {
                        chance = 0.1;
                    }
                }
            }
            foreach ((List<string> Result, List<string> Items) multipleItemCombination in Cauldron.multipleItemCombinations)
            {
                int matches = ingredients.Intersect(multipleItemCombination.Items).Count();

                if(matches == 2)
                {
                    match2.AddRange(multipleItemCombination.Result);

                    if(chance < 0.05)
                    {
                        chance = 0.05;
                    }
                }
                else if (matches >= 3)
                {
                    match3.AddRange(multipleItemCombination.Result);

                    if(chance < 0.1)
                    {
                        chance = 0.1;
                    }
                }
            }

            List<string> possibilities = new List<string>();

            foreach(string match in match2)
            {
                possibilities.Add(match);
            }
            foreach(string match in match3)
            {
                possibilities.Add(match);
                possibilities.Add(match);
                possibilities.Add(match);
            }

            if(possibilities.Count() > 0)
            {
                int index = randomGenerator.Next(0, possibilities.Count() - 1);

                errorMessageProgress = "Searching Resulting Item";
                errorMessageStats += $"Resulting Item: {possibilities[index]}, ";
                resultingItem = Utility.fuzzyItemSearch(possibilities[index]);
                errorMessageProgress = "Found Resulting Item";
            }

            return 0;
        }

        public double CheckForCookingRecipes(Cauldron Cauldron)
        {
            double chance = 0;

            List<string> match1 = new List<string>();
            List<string> match2 = new List<string>();
            List<string> match3 = new List<string>();

            List<Item> ingredients = new List<Item>() { ingredient1.item, ingredient2.item, ingredient3.item }.ToList();

            foreach ((string Result, List<int> Items, List<int> Categories) cookingRecipes in Cauldron.cookingRecipes)
            {
                List<int> matchIndexes = new List<int>();

                foreach(Item ingredient in ingredients)
                {
                    for(int index = 0; index < cookingRecipes.Items.Count; index++)
                    {
                        if(ingredient.ParentSheetIndex == cookingRecipes.Items[index])
                        {
                            matchIndexes.Add(index);
                        }
                    }
                    for(int index = 0; index < cookingRecipes.Categories.Count; index++)
                    {
                        if(ingredient.Category == cookingRecipes.Categories[index])
                        {
                            matchIndexes.Add(-index);
                        }
                    }
                }

                matchIndexes = matchIndexes.Distinct().ToList();

                if (matchIndexes.Count == 1)
                {
                    match1.Add(cookingRecipes.Result);

                    if (chance < 0.05)
                    {
                        chance = 0.05;
                    }
                }
                if (matchIndexes.Count == 2)
                {
                    match2.Add(cookingRecipes.Result);

                    if (chance < 0.10)
                    {
                        chance = 0.10;
                    }
                }
                else if (matchIndexes.Count >= 3)
                {
                    match3.Add(cookingRecipes.Result);

                    if (chance < 0.15)
                    {
                        chance = 0.15;
                    }
                }
            }

            List<string> possibilities = new List<string>();

            foreach (string match in match1)
            {
                possibilities.Add(match);
            }
            foreach (string match in match2)
            {
                possibilities.Add(match);
                possibilities.Add(match);
            }
            foreach (string match in match3)
            {
                possibilities.Add(match);
                possibilities.Add(match);
                possibilities.Add(match);
            }

            if (possibilities.Count() > 0)
            {
                int index = randomGenerator.Next(0, possibilities.Count() - 1);

                errorMessageProgress = "Searching Resulting Item";
                errorMessageStats += $"Resulting Item: {possibilities[index]}, ";
                resultingItem = Utility.fuzzyItemSearch(possibilities[index]);
                errorMessageProgress = "Found Resulting Item";
            }

            return 0;
        }

        public int getCombinedBuffChance()
        {
            return ingredient1.getBuffChance() + ingredient2.getBuffChance() + ingredient3.getBuffChance() + this.getBuffChance();
        }

        public int getCombinedDebuffChance()
        {
            return ingredient1.getDebuffChance() + ingredient2.getDebuffChance() + ingredient3.getDebuffChance() + this.getDebuffChance();
        }
        public int getBuffChance()
        {
            int buffChance = 0;

            foreach (int buffIndex in Enum.GetValues(typeof(Cauldron.buffs)))
            {
                buffChance += buffList[buffIndex];
            }

            return buffChance;
        }

        public int getDebuffChance()
        {
            int debuffChance = 0;

            foreach (int debuffIndex in Enum.GetValues(typeof(Cauldron.debuffs)))
            {
                debuffChance += debuffList[debuffIndex];
            }

            return debuffChance;
        }

        public double getButterflies()
        {
            return ingredient1.butterflies + ingredient2.butterflies + ingredient3.butterflies + this.butterflies + 0.01;
        }

        public double getBoom()
        {
            return ingredient1.boom + ingredient2.boom + ingredient3.boom + this.boom + 0.01;
        }

        public double getDuration()
        {
            return ingredient1.duration * ingredient2.duration * ingredient3.duration * this.duration;
        }

        public List<int> getAllBuffs()
        {
            List<int> buffAccumulator = new List<int>();
            foreach (int buffIndex in Enum.GetValues(typeof(Cauldron.buffs)))
            {
                buffAccumulator.Add(ingredient1.buffList[buffIndex] + ingredient2.buffList[buffIndex] + ingredient3.buffList[buffIndex] + this.buffList[buffIndex]);
            }
            return buffAccumulator;
        }

        public List<int> getAllDebuffs()
        {
            List<int> debuffAccumulator = new List<int>();
            foreach (int debuffIndex in Enum.GetValues(typeof(Cauldron.debuffs)))
            {
                debuffAccumulator.Add(ingredient1.debuffList[debuffIndex] + ingredient2.debuffList[debuffIndex] + ingredient3.debuffList[debuffIndex] + this.debuffList[debuffIndex]);
            }
            return debuffAccumulator;
        }
        #endregion chance determination
        #endregion helper
        #endregion functions
    }
}
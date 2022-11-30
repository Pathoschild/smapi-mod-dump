/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using System;
using StardewValley;
using StardewModdingAPI;
using SObject = StardewValley.Object;
using StardewSurvivalProject.source.utils;
using System.IO;
using System.Collections.Generic;

namespace StardewSurvivalProject.source
{
    public class Manager
    {
        private model.Player player;
        private model.EnvTemp envTemp;
        private String displayString = "";
        private Random rand = null;

        private string RelativeDataPath => Path.Combine("data", $"{Constants.SaveFolderName}.json");

        public Manager()
        {
            player = null;
            envTemp = null;
        }

        public void init(Farmer farmer)
        {
            player = new model.Player(farmer);
            envTemp = new model.EnvTemp();
            displayString = player.getStatStringUI();
            LogHelper.Debug("Manager initialized");
            rand = new Random();
        }

        public void onSecondUpdate()
        {
            //poition effect apply here
            if (player.hunger.value >= model.Hunger.DEFAULT_VALUE * ModConfig.GetInstance().HungerWellFedEffectPercentageThreshold / 100 &&
                player.thirst.value >= model.Thirst.DEFAULT_VALUE * ModConfig.GetInstance().ThirstWellFedEffectPercentageThreshold / 100)
                effects.EffectManager.applyEffect(effects.EffectManager.wellFedEffectIndex);
            if (player.hunger.value <= (model.Hunger.DEFAULT_VALUE * ModConfig.GetInstance().HungerEffectPercentageThreshold / 100))
                effects.EffectManager.applyEffect(effects.EffectManager.hungerEffectIndex);
            if (player.thirst.value <= (model.Thirst.DEFAULT_VALUE * ModConfig.GetInstance().ThirstEffectPercentageThreshold / 100))
                effects.EffectManager.applyEffect(effects.EffectManager.thirstEffectIndex);
            if (player.hunger.value <= 0) effects.EffectManager.applyEffect(effects.EffectManager.starvationEffectIndex);
            if (player.thirst.value <= 0) effects.EffectManager.applyEffect(effects.EffectManager.dehydrationEffectIndex);
            if (player.temp.value >= model.BodyTemp.HeatstrokeThreshold) effects.EffectManager.applyEffect(effects.EffectManager.heatstrokeEffectIndex);
            if (player.temp.value <= model.BodyTemp.HypotherminaThreshold) effects.EffectManager.applyEffect(effects.EffectManager.hypothermiaEffectIndex);
            if (player.temp.value >= model.BodyTemp.BurnThreshold) effects.EffectManager.applyEffect(effects.EffectManager.burnEffectIndex);
            if (player.temp.value <= model.BodyTemp.FrostbiteThreshold) effects.EffectManager.applyEffect(effects.EffectManager.frostbiteEffectIndex);
            if (envTemp.value >= player.temp.MinComfortTemp && envTemp.value <= player.temp.MaxComfortTemp) effects.EffectManager.applyEffect(effects.EffectManager.refreshingEffectIndex);

            //the real isPause code xd
            if (!Game1.eventUp && (Game1.activeClickableMenu == null || Game1.IsMultiplayer) && !Game1.paused)
            {
                //apply some effects' result every second
                if (Game1.buffsDisplay.otherBuffs.Exists(e => e.which == effects.EffectManager.stomachacheEffectIndex))
                {
                    player.updateActiveDrain(-model.Hunger.DEFAULT_VALUE * (ModConfig.GetInstance().StomachacheHungerPercentageDrainPerSecond / 100), 0);
                }
                if (Game1.buffsDisplay.otherBuffs.Exists(e => e.which == effects.EffectManager.burnEffectIndex))
                {
                    player.bindedFarmer.health -= ModConfig.GetInstance().HealthDrainOnBurnPerSecond;
                    Game1.currentLocation.playSound("ow");
                    Game1.hitShakeTimer = 100 * ModConfig.GetInstance().HealthDrainOnBurnPerSecond;
                }
                else if (Game1.buffsDisplay.otherBuffs.Exists(e => e.which == effects.EffectManager.frostbiteEffectIndex))
                {
                    player.bindedFarmer.health -= ModConfig.GetInstance().HealthDrainOnFrostbitePerSecond;
                    Game1.currentLocation.playSound("ow");
                    Game1.hitShakeTimer = 100 * ModConfig.GetInstance().HealthDrainOnFrostbitePerSecond;
                }
                if (Game1.buffsDisplay.otherBuffs.Exists(e => e.which == effects.EffectManager.heatstrokeEffectIndex))
                {
                    player.updateActiveDrain(0, -ModConfig.GetInstance().HeatstrokeThirstDrainPerSecond);
                }

                //if (ModConfig.GetInstance().UseReworkedStaminaDrain && !player.bindedFarmer.isMoving())
                //{
                //    player.bindedFarmer.stamina += Math.Min(player.bindedFarmer.maxStamina, player.bindedFarmer.)
                //}
            }
        }

        public void onEnvUpdate(int time, string season, int weatherIconId, GameLocation location = null, int currentMineLevel = 0)
        {
            if (!ModConfig.GetInstance().UseTemperatureModule) return;
            envTemp.updateEnvTemp(time, season, weatherIconId, location, currentMineLevel);
            envTemp.updateLocalEnvTemp(player.bindedFarmer.getTileX(), player.bindedFarmer.getTileY());
        }

        public void onClockUpdate()
        {
            if (player == null) return;
            player.updateDrain();
            
            if (ModConfig.GetInstance().UseTemperatureModule)
            {
                player.updateBodyTemp(envTemp);
            }
            displayString = player.getStatStringUI();
        }

        public void onEatingFood(SObject gameObj)
        {
            if (player == null) return;

            //addition: if player is drinking a refillable container, give back the empty container item
            if (gameObj.name.Equals("Full Canteen") || gameObj.name.Equals("Dirty Canteen") || gameObj.name.Equals("Ice Water Canteen"))
            {
                int itemId = data.ItemNameCache.getIDFromCache("Canteen");
                if (itemId != -1)
                {
                    player.bindedFarmer.addItemToInventory(new SObject(itemId, 1));
                }
            }

            //check if eaten food is cooked or artisan product, if no apply chance for stomachache effect
            if (gameObj.Category != SObject.CookingCategory && gameObj.Category != SObject.artisanGoodsCategory)
            {
                if (rand.NextDouble() * 100 >= (100 - ModConfig.GetInstance().PercentageChanceGettingStomachache))
                    effects.EffectManager.applyEffect(effects.EffectManager.stomachacheEffectIndex);
            }

            //band-aid fix coming, if edibility is 1 and healing value is not 0, dont add hunger
            //TODO: document this weird anomaly
            int healingValue = data.HealingItemDictionary.getHealingValue(gameObj.name);
            if (healingValue > 0 && gameObj.Edibility == 1) return;
            if (healingValue > 0 && gameObj.Edibility < 0 && gameObj.Edibility != -300)
            {
                player.bindedFarmer.health = Math.Min(player.bindedFarmer.maxHealth, player.bindedFarmer.health + healingValue);
            }

            double addHunger = (gameObj.Edibility >= 0)? gameObj.Edibility * ModConfig.GetInstance().HungerGainMultiplierFromItemEdibility : 0;
            player.updateEating(addHunger);
                
            displayString = player.getStatStringUI();
        }

        public void setPlayerHunger(double amt)
        {
            if (player == null || amt < 0 || amt > 1000000) return;
            player.hunger.value = amt;
        }

        public void setPlayerThirst(double amt)
        {
            if (player == null || amt < 0 || amt > 1000000) return;
            player.thirst.value = amt;
        }

        public  void setPlayerBodyTemp(double v)
        {
            if (player == null || v < -274 || v > 10000) return;
            player.temp.value = v;
        }

        public string getDisplayString()
        {
            return displayString;
        }

        public void onExit()
        {
            if (player == null) return;
            else player = null;
        }

        public void onEnvDrinkingUpdate(bool isOcean, bool isWater)
        {
            if (player == null) return;
            double addThirst = isWater ? ModConfig.GetInstance().HydrationGainOnEnvironmentWaterDrinking : 0;
            addThirst = isOcean ? -ModConfig.GetInstance().HydrationGainOnEnvironmentWaterDrinking : addThirst;

            //294 is drinking animation id
            player.bindedFarmer.animateOnce(294);

            //set isEating to true to prevent constant drinking by spamming action button 
            //conflicted with spacecore's DoneEating event
            player.bindedFarmer.isEating = true;
            //Fixing by setting itemToEat to something that doesnt do anything to player HP and stamina (in this case, daffodil)
            player.bindedFarmer.itemToEat = (Item)new SObject(18, 1); 

            player.updateDrinking(addThirst);
            displayString = player.getStatStringUI();
        }

        public void onDayEnding()
        {
            //specifically remove refreshing buff to prevent permanent stamina increase
            if (Game1.buffsDisplay.otherBuffs.Exists(e => e.which == effects.EffectManager.refreshingEffectIndex))
            {
                Game1.buffsDisplay.removeOtherBuff(effects.EffectManager.refreshingEffectIndex);
            }

            //clear all buff on day ending (bug-free?) - not bug-free, funky stuff happen with max stamina buff
            Game1.buffsDisplay.clearAllBuffs();

            if (player == null) return;

            if (!player.spouseFeed && player.bindedFarmer.getSpouse() != null)
            {
                player.bindedFarmer.changeFriendship(-ModConfig.GetInstance().FriendshipPenaltyOnNotFeedingSpouse, player.bindedFarmer.getSpouse());
                player.spouseFeed = false;
            }

            if (!ModConfig.GetInstance().UseOvernightPassiveDrain || player.bindedFarmer.passedOut) return;
            //24 mean 240 minutes of sleep (from 2am to 6am)
            player.updateActiveDrain(-ModConfig.GetInstance().PassiveHungerDrainRate * 24, -ModConfig.GetInstance().PassiveThirstDrainRate * 24);

            //get current hp and + set amount of hp instead of full
            player.healthPoint = Math.Min(player.bindedFarmer.health + ModConfig.GetInstance().HealthRestoreOnSleep, player.bindedFarmer.maxHealth);

        }

        public void updateOnRunning()
        {
            if (player == null || !ModConfig.GetInstance().UseOnRunningDrain) return;

            double THIRST_DRAIN_ON_RUNNING = ModConfig.GetInstance().RunningThirstDrainRate, HUNGER_DRAIN_ON_RUNNING = ModConfig.GetInstance().RunningHungerDrainRate;
            if (player.thirst.value <= THIRST_DRAIN_ON_RUNNING || player.hunger.value <= HUNGER_DRAIN_ON_RUNNING)
            {
                player.bindedFarmer.setRunning(false, true);
                return;
            }
            player.updateRunningDrain();
        }

        internal void ResetPlayerHungerAndThirst()
        {
            if (player == null) return;

            player.resetPlayerHungerAndThirst();
            LogHelper.Debug("Reset player stats");
        }

        public void onItemDrinkingUpdate(SObject gameObj, double overrideAddThirst = 0, double coolingModifier = 1)
        {
            if (player == null) return;
            double addThirst = overrideAddThirst;
            if (addThirst == 0) addThirst = ModConfig.GetInstance().DefaultHydrationGainOnDrinkableItems;

            player.updateDrinking(addThirst, coolingModifier);
            displayString = player.getStatStringUI();
        }

        public String getPlayerHungerStat()
        {
            return $"{player.hunger.value.ToString("#.##")} / {model.Hunger.DEFAULT_VALUE}";
        }

        public double getPlayerHungerPercentage()
        {
            return player.hunger.value / model.Hunger.DEFAULT_VALUE;
        }

        public String getPlayerThirstStat()
        {
            return $"{player.thirst.value.ToString("#.##")} / {model.Thirst.DEFAULT_VALUE}";
        }

        public double getPlayerThirstPercentage()
        {
            return player.thirst.value / model.Thirst.DEFAULT_VALUE;
        }

        public double getPlayerBodyTemp()
        {
            return player.temp.value;
        }

        public double getEnvTemp()
        {
            return this.envTemp.value;
        }

        public double getMinComfyEnvTemp()
        {
            return this.player.temp.MinComfortTemp;
        }

        public double getMaxComfyEnvTemp()
        {
            return this.player.temp.MaxComfortTemp;
        }

        public string getPlayerBodyTempString()
        {
            if (ModConfig.GetInstance().TemperatureUnit.Equals("Fahrenheit")) return ((player.temp.value * 9 / 5) + 32).ToString("#.##") + "F";
            else if (ModConfig.GetInstance().TemperatureUnit.Equals("Kelvin")) return (player.temp.value + 273).ToString("#.##") + "K";
            return player.temp.value.ToString("#.##") + "C";
        }

        public string getEnvTempString()
        {
            if (ModConfig.GetInstance().TemperatureUnit.Equals("Fahrenheit")) return ((this.envTemp.value * 9 / 5) + 32).ToString("#.##") + "F";
            else if (ModConfig.GetInstance().TemperatureUnit.Equals("Kelvin")) return (this.envTemp.value + 273).ToString("#.##") + "K";
            return this.envTemp.value.ToString("#.##") + "C";
        }

        public void updateOnToolUsed(StardewValley.Tool toolHold)
        {
            bool isFever = Game1.buffsDisplay.otherBuffs.Exists(e => e.which == effects.EffectManager.feverEffectIndex);
            int power = (int)((player.bindedFarmer.toolHold + 20f) / 600f) + 1;
            //LogHelper.Debug($"Tool Power = {power}");

            if (!ModConfig.GetInstance().UseOnToolUseDrain) return;

            //yea this is terrible
            //TODO: more generic code
            if (toolHold is StardewValley.Tools.Axe)
            {
                player.updateActiveDrain(-ModConfig.GetInstance().AxeHungerDrain, -ModConfig.GetInstance().AxeThirstDrain);
                if (isFever)
                {
                    player.bindedFarmer.stamina -= ((float)(2 * power) - (float)player.bindedFarmer.ForagingLevel * 0.1f) * ((float)(ModConfig.GetInstance().AdditionalPercentageStaminaDrainOnFever / 100));
                    Game1.staminaShakeTimer += 100;
                }
            }
            else if (toolHold is StardewValley.Tools.Hoe)
            {
                player.updateActiveDrain(-ModConfig.GetInstance().HoeHungerDrain, -ModConfig.GetInstance().HoeThirstDrain);
                if (isFever)
                {
                    player.bindedFarmer.stamina -= ((float)(2 * power) - (float)player.bindedFarmer.FarmingLevel * 0.1f) * ((float)(ModConfig.GetInstance().AdditionalPercentageStaminaDrainOnFever / 100));
                    Game1.staminaShakeTimer += 100;
                }
            }
            else if (toolHold is StardewValley.Tools.Pickaxe)
            {
                player.updateActiveDrain(-ModConfig.GetInstance().PickaxeHungerDrain, -ModConfig.GetInstance().PickaxeThirstDrain);
                if (isFever)
                {
                    player.bindedFarmer.stamina -= ((float)(2 * power) - (float)player.bindedFarmer.MiningLevel * 0.1f) * ((float)(ModConfig.GetInstance().AdditionalPercentageStaminaDrainOnFever / 100));
                    Game1.staminaShakeTimer += 100;
                }
            }
            else if (toolHold is StardewValley.Tools.MeleeWeapon)
            {
                player.updateActiveDrain(-ModConfig.GetInstance().MeleeWeaponHungerDrain, -ModConfig.GetInstance().MeleeWeaponThirstDrain);
                if (isFever)
                {
                    player.bindedFarmer.stamina -= (1f - (float)player.bindedFarmer.CombatLevel * 0.08f) * ((float)(ModConfig.GetInstance().AdditionalPercentageStaminaDrainOnFever / 100));
                }
            }
            else if (toolHold is StardewValley.Tools.Slingshot)
            {
                player.updateActiveDrain(-ModConfig.GetInstance().SlingshotHungerDrain, -ModConfig.GetInstance().SlingshotThirstDrain);
                if (isFever)
                {
                    player.bindedFarmer.stamina -= (1f - (float)player.bindedFarmer.CombatLevel * 0.08f) * ((float)(ModConfig.GetInstance().AdditionalPercentageStaminaDrainOnFever / 100));
                }
            }
            else if (toolHold is StardewValley.Tools.WateringCan)
            {
                player.updateActiveDrain(-ModConfig.GetInstance().WateringCanHungerDrain, -ModConfig.GetInstance().WateringCanThirstDrain);
                if (isFever)
                {
                    player.bindedFarmer.stamina -= ((float)(2 * (power + 1)) - (float)player.bindedFarmer.FarmingLevel * 0.1f) * ((float)(ModConfig.GetInstance().AdditionalPercentageStaminaDrainOnFever / 100));
                    Game1.staminaShakeTimer += 100;
                }
            }
            else if (toolHold is StardewValley.Tools.FishingRod)
            {
                player.updateActiveDrain(-ModConfig.GetInstance().FishingPoleHungerDrain, -ModConfig.GetInstance().FishingPoleThirstDrain);
                if (isFever)
                {
                    player.bindedFarmer.stamina -= (8f - (float)player.bindedFarmer.FishingLevel * 0.1f) * ((float)(ModConfig.GetInstance().AdditionalPercentageStaminaDrainOnFever / 100));
                    Game1.staminaShakeTimer += 100;
                }
            }
            else if (toolHold is StardewValley.Tools.MilkPail)
            {
                player.updateActiveDrain(-ModConfig.GetInstance().MilkPailHungerDrain, -ModConfig.GetInstance().MilkPailThirstDrain);
                if (isFever)
                {
                    player.bindedFarmer.stamina -= (4f - (float)player.bindedFarmer.FarmingLevel * 0.1f) * ((float)(ModConfig.GetInstance().AdditionalPercentageStaminaDrainOnFever / 100));
                    Game1.staminaShakeTimer += 100;
                }
            }
            else if (toolHold is StardewValley.Tools.Shears)
            {
                player.updateActiveDrain(-ModConfig.GetInstance().ShearHungerDrain, -ModConfig.GetInstance().ShearThirstDrain);
                if (isFever)
                {
                    player.bindedFarmer.stamina -= (4f - (float)player.bindedFarmer.FarmingLevel * 0.1f) * ((float)(ModConfig.GetInstance().AdditionalPercentageStaminaDrainOnFever / 100));
                    Game1.staminaShakeTimer += 100;
                }
            }
            else
                LogHelper.Debug("Unknown tool type");

            displayString = player.getStatStringUI();
        }

        public class SaveData
        {
            public model.Hunger hunger;
            public model.Thirst thirst;
            public model.BodyTemp bodyTemp;
            public int healthPoint;
            public model.Mood mood;

            public SaveData(model.Hunger h, model.Thirst t, model.BodyTemp bt, int hp, model.Mood m)
            {
                this.hunger = h;
                this.thirst = t;
                this.bodyTemp = bt;
                this.healthPoint = hp;
                this.mood = m;
            }
        }

        public void loadData(Mod context)
        {
            SaveData saveData = context.Helper.Data.ReadJsonFile<SaveData>(this.RelativeDataPath);
            if (saveData != null)
            {
                this.player.hunger = saveData.hunger;
                this.player.thirst = saveData.thirst;
                this.player.temp = saveData.bodyTemp;
                if (saveData.healthPoint > 0) this.player.healthPoint = saveData.healthPoint;
                if (saveData.mood != null) this.player.mood = saveData.mood;
            }
        }

        public void saveData(Mod context)
        {

            SaveData savingData = new SaveData(this.player.hunger, this.player.thirst, this.player.temp, this.player.healthPoint, this.player.mood);
            context.Helper.Data.WriteJsonFile<SaveData>(this.RelativeDataPath, savingData);
        }

        internal void dayStartProcedure()
        {
            double dice_roll = rand.NextDouble() * 100;
            //base chance of 2%, increase to +10% the less stamina player have at the end of the day
            if (dice_roll >= 100 - (ModConfig.GetInstance().PercentageChanceGettingFever + ModConfig.GetInstance().PercentageChanceGettingFever * (1 - player.bindedFarmer.stamina / player.bindedFarmer.MaxStamina)))
            {
                effects.EffectManager.applyEffect(effects.EffectManager.feverEffectIndex);
            }

            player.bindedFarmer.health = player.healthPoint;
        }

        public void onActionButton()
        {

        }

        public void onUseButton()
        {

        }

        public void updateOnGiftGiven(NPC npc, SObject gift)
        {
            if (npc == player.bindedFarmer.getSpouse())
            {
                if (gift.Category == SObject.CookingCategory)
                {
                    player.spouseFeed = true;
                }
            }
        }
    }
}

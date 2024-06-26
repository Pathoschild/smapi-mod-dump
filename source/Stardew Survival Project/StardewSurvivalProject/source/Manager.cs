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
            if (envTemp.value >= player.temp.MinComfortTemp && envTemp.value <= player.temp.MaxComfortTemp) 
                effects.EffectManager.applyEffect(effects.EffectManager.refreshingEffectIndex);

            //the real isPause code xd
            if (!Game1.eventUp && (Game1.activeClickableMenu == null || Game1.IsMultiplayer) && !Game1.paused)
            {
                //apply some effects' result every second
                if (Game1.player.buffs.IsApplied("neroyuki.rlvalley/stomachache"))
                {
                    player.updateHungerThirstDrain(-model.Hunger.DEFAULT_VALUE * (ModConfig.GetInstance().StomachacheHungerPercentageDrainPerSecond / 100), 0, reduceSaturation: false);
                }
                if (Game1.player.buffs.IsApplied("neroyuki.rlvalley/burn"))
                {
                    player.bindedFarmer.health -= ModConfig.GetInstance().HealthDrainOnBurnPerSecond;
                    Game1.currentLocation.playSound("ow");
                    Game1.hitShakeTimer = 100 * ModConfig.GetInstance().HealthDrainOnBurnPerSecond;
                }
                else if (Game1.player.buffs.IsApplied("neroyuki.rlvalley/frostbite"))
                {
                    player.bindedFarmer.health -= ModConfig.GetInstance().HealthDrainOnFrostbitePerSecond;
                    Game1.currentLocation.playSound("ow");
                    Game1.hitShakeTimer = 100 * ModConfig.GetInstance().HealthDrainOnFrostbitePerSecond;
                }
                if (Game1.player.buffs.IsApplied("neroyuki.rlvalley/heatstroke"))
                {
                    player.updateHungerThirstDrain(0, -ModConfig.GetInstance().HeatstrokeThirstDrainPerSecond);
                }

                if (ModConfig.GetInstance().UseStaminaRework)
                {
                    // TODO: make this adjustable
                    var restoredStaminaPerSecond = 0f;
                    if (!player.bindedFarmer.isMoving())
                    {
                        restoredStaminaPerSecond += ModConfig.GetInstance().StaminaRegenOnNotMovingPerSecond;
                    }
                    if (player.bindedFarmer.IsSitting())
                    {
                        restoredStaminaPerSecond += ModConfig.GetInstance().StaminaExtraRegenOnSittingPerSecond;
                    }
                    if (player.bindedFarmer.isInBed.Value)
                    {
                        restoredStaminaPerSecond += ModConfig.GetInstance().StaminaExtraRegenOnNappingPerSecond;
                    }
                    player.bindedFarmer.stamina = Math.Min(player.bindedFarmer.MaxStamina, player.bindedFarmer.stamina + restoredStaminaPerSecond);
                }
            }
        }

        public void onEnvUpdate(int time, string season, int weatherIconId, GameLocation location = null, int currentMineLevel = 0)
        {
            if (!ModConfig.GetInstance().UseTemperatureModule) return;
            envTemp.updateEnvTemp(time, season, weatherIconId, location, currentMineLevel);
            envTemp.updateLocalEnvTemp((int) player.bindedFarmer.Tile.X, (int) player.bindedFarmer.Tile.Y);
        }

        public void onClockUpdate()
        {
            if (player == null) return;
            player.updateDrain();
            
            if (ModConfig.GetInstance().UseTemperatureModule)
            {
                player.updateBodyTemp(envTemp);
            }
            if (ModConfig.GetInstance().UseSanityModule)
            {
                player.mood.CheckForMentalBreak();
            }
            displayString = player.getStatStringUI();
        }

        public void onEatingFood(SObject gameObj)
        {
            if (player == null) return;

            //addition: if player is drinking a refillable container, give back the empty container item
            if (gameObj.name.Equals("Full Canteen") || gameObj.name.Equals("Dirty Canteen") || gameObj.name.Equals("Ice Water Canteen"))
            {
                string itemId = data.ItemNameCache.getIDFromCache("Canteen");
                if (itemId != "-1")
                {
                    if (player.bindedFarmer.isInventoryFull())
                    {
                        // attempt to drop the empty canteen on the ground if the inventory is full
                        Game1.createItemDebris(new SObject(itemId, 1), player.bindedFarmer.getStandingPosition(), player.bindedFarmer.FacingDirection, null);
                    }

                    player.bindedFarmer.addItemToInventory(new SObject(itemId, 1));
                }
            }

            //check if eaten food is cooked or artisan product, if not apply chance for stomachache effect
            if (gameObj.Category != SObject.CookingCategory && gameObj.Category != SObject.artisanGoodsCategory)
            {
                if (rand.NextDouble() * 100 >= (100 - ModConfig.GetInstance().PercentageChanceGettingStomachache))
                    effects.EffectManager.applyEffect(effects.EffectManager.stomachacheEffectIndex);
            }

            // handle thirst restoration (default 0, 0) 
            var isDrinkable = Game1.objectData[gameObj.ItemId].IsDrink;
            (double addThirst, double coolingModifier) = data.CustomHydrationDictionary.getHydrationAndCoolingModifierValue(gameObj.Name, isDrinkable);

            if (addThirst != 0)
            {
                onItemDrinkingUpdate(gameObj, addThirst, coolingModifier);
            }
            else if (isDrinkable)
            {
                // all drinkable should cool player down very slightly
                coolingModifier = 1;
                onItemDrinkingUpdate(gameObj, ModConfig.GetInstance().DefaultHydrationGainOnDrinkableItems, coolingModifier);
            }

            //band-aid fix coming, if edibility is 1 and healing value is not 0, dont add hunger
            //TODO: document this weird anomaly
            int healingValue = data.HealingItemDictionary.getHealingValue(gameObj.name);
            if (healingValue > 0 && gameObj.Edibility == 1) return;
            // fix for painkiller
            if (healingValue > 0 && gameObj.Edibility < 0 && gameObj.Edibility != -300)
            {
                player.bindedFarmer.health = Math.Min(player.bindedFarmer.maxHealth, player.bindedFarmer.health + healingValue);
            }

            // handle hunger restoration (default 1, 0)
            (double addHunger, double hungerCoolingModifier) = data.CustomHungerDictionary.getHungerModifierAndCoolingModifierValue(gameObj, isDrinkable);

            // if coolerModifier is non-default value, do not apply the hungerCoolingModifier further
            player.updateEating(addHunger, coolingModifier == 0 ? hungerCoolingModifier : 0);
                
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

        public void setPlayerBodyTemp(double v)
        {
            if (player == null || v < -274 || v > 10000) return;
            player.temp.value = v;
        }

        public void setPlayerMood(double v)
        {
            if (player == null || v < -40 || v > 120) return;
            player.mood.Value = v;
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
            player.bindedFarmer.itemToEat = new SObject("(O)18", 1); 

            player.updateDrinking(addThirst);
            displayString = player.getStatStringUI();
        }

        public void onDayEnding()
        {
            //specifically remove refreshing buff to prevent permanent stamina increase
            if (Game1.player.buffs.IsApplied("neroyuki.rlvalley/refreshing"))
            {
                Game1.player.buffs.Remove("neroyuki.rlvalley/refreshing");
            }

            //clear all buff on day ending (bug-free?) - not bug-free, funky stuff happen with max stamina buff
            Game1.player.buffs.Clear();

            if (player == null) return;

            if (!player.spouseFeed && player.bindedFarmer.getSpouse() != null)
            {
                player.bindedFarmer.changeFriendship(-ModConfig.GetInstance().FriendshipPenaltyOnNotFeedingSpouse, player.bindedFarmer.getSpouse());
                player.spouseFeed = false;
            }

            if (!ModConfig.GetInstance().UseOvernightPassiveDrain || player.bindedFarmer.passedOut) return;
            //24 mean 240 minutes of sleep (from 2am to 6am)
            player.updateHungerThirstDrain(-ModConfig.GetInstance().PassiveHungerDrainRate * 24, -ModConfig.GetInstance().PassiveThirstDrainRate * 24);

            //get current hp and + set amount of hp instead of full
            player.healthPoint = Math.Min(player.bindedFarmer.health + ModConfig.GetInstance().HealthRestoreOnSleep, player.bindedFarmer.maxHealth);

        }

        public void updateOnRunning(bool isSprinting = false)
        {
            if (player == null || !ModConfig.GetInstance().UseOnRunningDrain) return;

            double thirstDrainOnRunning = ModConfig.GetInstance().RunningThirstDrainRate * (270f / player.bindedFarmer.MaxStamina) ;
            double hungerDrainOnRunning = ModConfig.GetInstance().RunningHungerDrainRate * (270f / player.bindedFarmer.MaxStamina);
            if (player.thirst.value <= thirstDrainOnRunning || player.hunger.value <= hungerDrainOnRunning)
            {
                player.bindedFarmer.setRunning(false, true);
            }
            else
            {
                player.updateRunningDrain();
            }

            if (ModConfig.GetInstance().UseStaminaRework)
            {
                float staminaDrainOnRunning = isSprinting ? ModConfig.GetInstance().StaminaDrainOnSprintingPerTick : ModConfig.GetInstance().StaminaDrainOnRunningPerTick;
                if (player.bindedFarmer.stamina <= staminaDrainOnRunning)
                {
                    player.bindedFarmer.setRunning(false, true);
                }
                player.bindedFarmer.stamina -= staminaDrainOnRunning;
                if (isSprinting)
                {
                    // play sprinting sound effect
                    Game1.playSound("daggerswipe");
                    effects.EffectManager.applyEffect(effects.EffectManager.sprintingEffectIndex);
                }
            }
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
            double addThirst = overrideAddThirst * (ModConfig.GetInstance().DefaultHydrationGainOnDrinkableItems / 10);
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

        public double getPlayerHungerSaturationStat()
        {
            return player.hunger.saturation / 100;
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

        public int getPlayerMoodIndex()
        {
            // clamp level to 0-7
            int level = Math.Max(0, Math.Min(7, (int)player.mood.Level));
            return level;
        }

        public void updateOnToolUsed(StardewValley.Tool toolHold)
        {
            bool isFever = Game1.player.buffs.IsApplied("neroyuki.rlvalley/fever");
            int power = (int)((player.bindedFarmer.toolHold + 20f) / 600f) + 1;
            //LogHelper.Debug($"Tool Power = {power}");

            if (!ModConfig.GetInstance().UseOnToolUseDrain) return;

            double hungerDrainOnToolUsed = 0;
            double thirstDrainOnToolUsed = 0;
            float staminaDrainOnToolUsed = 0;

            //yea this is terrible
            if (toolHold is StardewValley.Tools.Axe)
            {
                hungerDrainOnToolUsed = ModConfig.GetInstance().AxeHungerDrain;
                thirstDrainOnToolUsed = ModConfig.GetInstance().AxeThirstDrain;
                staminaDrainOnToolUsed = ((2 * power) - player.bindedFarmer.ForagingLevel * 0.1f);
            }
            else if (toolHold is StardewValley.Tools.Hoe)
            {
                hungerDrainOnToolUsed = ModConfig.GetInstance().HoeHungerDrain;
                thirstDrainOnToolUsed = ModConfig.GetInstance().HoeThirstDrain;
                staminaDrainOnToolUsed = ((2 * power) - player.bindedFarmer.FarmingLevel * 0.1f);
            }
            else if (toolHold is StardewValley.Tools.Pickaxe)
            {
                hungerDrainOnToolUsed = ModConfig.GetInstance().PickaxeHungerDrain;
                thirstDrainOnToolUsed = ModConfig.GetInstance().PickaxeThirstDrain;
                staminaDrainOnToolUsed = ((2 * power) - player.bindedFarmer.MiningLevel * 0.1f);
            }
            else if (toolHold is StardewValley.Tools.MeleeWeapon)
            {
                hungerDrainOnToolUsed = ModConfig.GetInstance().MeleeWeaponHungerDrain;
                thirstDrainOnToolUsed = ModConfig.GetInstance().MeleeWeaponThirstDrain;
                staminaDrainOnToolUsed = (1f - player.bindedFarmer.CombatLevel * 0.08f);
            }
            else if (toolHold is StardewValley.Tools.Slingshot)
            {
                hungerDrainOnToolUsed = ModConfig.GetInstance().SlingshotHungerDrain;
                thirstDrainOnToolUsed = ModConfig.GetInstance().SlingshotThirstDrain;
                staminaDrainOnToolUsed = (1f - player.bindedFarmer.CombatLevel * 0.08f);
            }
            else if (toolHold is StardewValley.Tools.WateringCan)
            {
                hungerDrainOnToolUsed = ModConfig.GetInstance().WateringCanHungerDrain;
                staminaDrainOnToolUsed = ((2 * (power + 1)) - player.bindedFarmer.FarmingLevel * 0.1f);
            }
            else if (toolHold is StardewValley.Tools.FishingRod)
            {
                hungerDrainOnToolUsed = ModConfig.GetInstance().FishingPoleHungerDrain;
                thirstDrainOnToolUsed = ModConfig.GetInstance().FishingPoleThirstDrain;
                staminaDrainOnToolUsed = (8f - player.bindedFarmer.FishingLevel * 0.1f);
            }
            else if (toolHold is StardewValley.Tools.MilkPail)
            {
                hungerDrainOnToolUsed = ModConfig.GetInstance().MilkPailHungerDrain;
                thirstDrainOnToolUsed = ModConfig.GetInstance().MilkPailThirstDrain;
                staminaDrainOnToolUsed = (4f - player.bindedFarmer.FarmingLevel * 0.1f);
            }
            else if (toolHold is StardewValley.Tools.Shears)
            {
                hungerDrainOnToolUsed = ModConfig.GetInstance().ShearHungerDrain;
                thirstDrainOnToolUsed = ModConfig.GetInstance().ShearThirstDrain;
                staminaDrainOnToolUsed = (4f - player.bindedFarmer.FarmingLevel * 0.1f); 
            }
            else
                LogHelper.Debug("Unknown tool type");

            if (ModConfig.GetInstance().UseOnToolUseDrain)
            {
                player.updateHungerThirstDrain(-hungerDrainOnToolUsed, -thirstDrainOnToolUsed);
            }

            // stamina draining final calculation and application
            if (ModConfig.GetInstance().UseStaminaRework)
            {
                staminaDrainOnToolUsed *= (float)(ModConfig.GetInstance().AdditionalDrainOnToolUse / 100);
            }

            if (isFever)
            {
                player.bindedFarmer.stamina -= staminaDrainOnToolUsed * ((float)(ModConfig.GetInstance().AdditionalPercentageStaminaDrainOnFever / 100));
                Game1.staminaShakeTimer += 100;
            }
            else if (ModConfig.GetInstance().UseStaminaRework)
            {
                player.bindedFarmer.stamina -= staminaDrainOnToolUsed;
            }

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
                if (saveData.mood != null) this.player.mood = new model.Mood(saveData.mood, this.player.OnFarmerMentalBreak);
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
            player.hunger.saturation = 0;
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

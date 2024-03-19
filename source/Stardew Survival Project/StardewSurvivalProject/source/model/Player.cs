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
using System.Collections.Generic;
using StardewValley;
using StardewModdingAPI;
using Newtonsoft.Json;

namespace StardewSurvivalProject.source.model
{
    public class Player
    {
        public Farmer bindedFarmer { get; }
        public Hunger hunger;
        public BodyTemp temp;
        public Thirst thirst;
        public Mood mood;
        public int healthPoint { get; set; } = 100;
        public bool spouseFeed { get; set; } = false;
        private Random rand = new Random();

        public Player(Farmer farmer)
        {
            hunger = new Hunger();
            temp = new BodyTemp();
            thirst = new Thirst();
            healthPoint = farmer.maxHealth;
            mood = new Mood(OnFarmerMentalBreak);
            bindedFarmer = farmer;
        }

        public void OnFarmerMentalBreak()
        {
            events.CustomEvents.InvokeOnMentalBreak(bindedFarmer);
        }

        //update drain passively, should happen every 10 in-game minutes
        public void updateDrain()
        {
            if (ModConfig.GetInstance().UsePassiveDrain)
            {
                updateHungerThirstDrain(-ModConfig.GetInstance().PassiveHungerDrainRate, -ModConfig.GetInstance().PassiveThirstDrainRate);
            }
        }

        public void checkIsDangerValue()
        {
            if (hunger.value <= 0)
            {
                hunger.value = 0;
                int staminaPenalty = ModConfig.GetInstance().StaminaPenaltyOnStarvation;
                bindedFarmer.stamina -= staminaPenalty;
                //Game1.currentLocation.playSound("ow");
                Game1.staminaShakeTimer = 100 * staminaPenalty;
            }
            if (thirst.value <= 0)
            {
                thirst.value = 0;
                int healthPenalty = ModConfig.GetInstance().HealthPenaltyOnDehydration;
                bindedFarmer.health -= healthPenalty; 
                Game1.currentLocation.playSound("ow");
                Game1.hitShakeTimer = 100 * healthPenalty;
            }
        }

        public void updateHungerThirstDrain(double deltaHunger, double deltaThirst, bool reduceSaturation = true)
        {
            hunger.value += deltaHunger;
            thirst.value += deltaThirst;
            if (reduceSaturation)
            {
                hunger.saturation = Math.Max(hunger.saturation + deltaHunger * 3, 0);
            }
            checkIsDangerValue();
        }

        //update hunger after eating food
        public void updateEating(double addValue, double coolingModifier)
        {
            // modify addValue with saturation
            if (addValue > 0 && ModConfig.GetInstance().ScaleHungerRestoredWithTimeFromLastMeal)
            {
                addValue = addValue * Math.Max(1 - hunger.saturation / 100, 0);
                // saturation is increased by 18.95 * ln(addValue + 1), capped at 100, if saturation is > 0, attempt to get the addValue from saturation before add the current addValue
                double saturationAddValue = Math.Min(18.95 * Math.Log(addValue + 1), 100 - hunger.saturation);

                hunger.saturation += saturationAddValue;
            }

            hunger.value = Math.Min(hunger.value + addValue, Hunger.DEFAULT_VALUE);
            if (addValue == 0) return;
            Game1.addHUDMessage(new HUDMessage($"{(addValue >= 0 ? "+" : "") + Math.Round(addValue)} Hunger", (addValue >= 0 ? HUDMessage.stamina_type : HUDMessage.error_type)));

            if (addValue > 0 && coolingModifier != 0)
            {
                updateBodyTempOnConsumingItem(coolingModifier, addValue * 0.5);
            }
            checkIsDangerValue();
        }

        public void updateDrinking(double addValue, double cooling_modifier = 1)
        {
            thirst.value = Math.Min(thirst.value + addValue, Thirst.DEFAULT_VALUE);
            if (addValue == 0) return;
            Game1.addHUDMessage(new HUDMessage($"{(addValue >= 0 ? "+" : "") + addValue} Hydration", (addValue >= 0 ? HUDMessage.stamina_type : HUDMessage.error_type)));

            if (addValue > 0)
            {
                updateBodyTempOnConsumingItem(cooling_modifier, addValue);
            }
            
            checkIsDangerValue();
        }

        public void updateBodyTempOnConsumingItem(double coolingModifier, double scaleValue, bool shouldScaleValue = true)
        {
            // should not lead to divide by zero
            if (this.temp.value >= BodyTemp.DEFAULT_VALUE && coolingModifier > 0)
            {
                //cooling down player if water was drank
                this.temp.value -= (this.temp.value - (BodyTemp.DEFAULT_VALUE)) * (1 - 1 / (0.01 * coolingModifier * (shouldScaleValue ? scaleValue : 10) + 1));
            }
            else if (this.temp.value < BodyTemp.DEFAULT_VALUE && coolingModifier <= 0)
            {
                //heating up player if hot drink was drank
                this.temp.value += ((BodyTemp.DEFAULT_VALUE) - this.temp.value) * (1 - 1 / (0.02 * (-coolingModifier) * (shouldScaleValue ? scaleValue : 10) + 1));
            }
            // overheat/overchill player
            if (Math.Abs(coolingModifier) > 4)
            {
                var tempModifer = (Math.Abs(coolingModifier) - 4) * (shouldScaleValue ? scaleValue : 10) * 0.02;
                this.temp.value -= coolingModifier > 0 ? tempModifer : -tempModifer;
            }
        }

        public void updateBodyTemp(EnvTemp envTemp)
        {
            String hat_name = "", shirt_name = "", pants_name = "", boots_name = "";
            if (bindedFarmer.hat.Value != null) hat_name = bindedFarmer.hat.Value.Name;
            if (bindedFarmer.shirtItem.Value != null) shirt_name = bindedFarmer.shirtItem.Value.Name;
            if (bindedFarmer.pantsItem.Value != null) pants_name = bindedFarmer.pantsItem.Value.Name;
            if (bindedFarmer.boots.Value != null) boots_name = bindedFarmer.boots.Value.Name;
            LogHelper.Debug($"hat={hat_name} shirt={shirt_name} pants={pants_name} boots={boots_name}");
            temp.updateComfortTemp(hat_name, shirt_name, pants_name, boots_name);
            LogHelper.Debug($"temp={temp.MinComfortTemp} tempHi={temp.MaxComfortTemp}");
            temp.BodyTempCalc(envTemp, (rand.NextDouble() * 0.2) - 0.1);
        }

        public String getStatString()
        {
            return $"Hunger = {hunger.value.ToString("#.##")}; Thirst = {thirst.value.ToString("#.##")}; Body Temp. = {temp.value.ToString("#.##")}";
        }

        public String getStatStringUI()
        {
            return $"Hunger: {hunger.value.ToString("#.##")}\nThirst: {thirst.value.ToString("#.##")}\nBody Temp.: {temp.value.ToString("#.##")}";
        }

        internal void updateRunningDrain()
        {
            double THIRST_DRAIN_ON_RUNNING = ModConfig.GetInstance().RunningThirstDrainRate, HUNGER_DRAIN_ON_RUNNING = ModConfig.GetInstance().RunningHungerDrainRate;
            updateHungerThirstDrain(-HUNGER_DRAIN_ON_RUNNING, -THIRST_DRAIN_ON_RUNNING);
        }

        internal void resetPlayerHungerAndThirst()
        {
            if (ModConfig.GetInstance().HungerEffectPercentageThreshold > 0)
                hunger.value = Hunger.DEFAULT_VALUE * ModConfig.GetInstance().HungerEffectPercentageThreshold / 100;
            else
                hunger.value = Hunger.DEFAULT_VALUE / 4;
            if (ModConfig.GetInstance().ThirstEffectPercentageThreshold > 0)
                thirst.value = Thirst.DEFAULT_VALUE * ModConfig.GetInstance().ThirstEffectPercentageThreshold / 100;
            else 
                thirst.value = Thirst.DEFAULT_VALUE / 4;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace AdjustableStaminaHealing
{
    using Player = StardewValley.Farmer;
    public class ModEntry : Mod
    {
        private readonly float MaxHealing = 2.0f;
        private readonly float MinHealing = 0.0f;

        private int TicksAccumulator = 0;
        private float HPAccumlator;

        Config config = null;

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<Config>();
            GameEvents.OneSecondTick += OnGameUpdate;
            InputEvents.ButtonPressed += OnButtonPressed;
            CorrectHealingValue();
            helper.WriteConfig(config);
            Log("Finished initialization.");
        }

        public void OnGameUpdate(object sender, EventArgs args)
        {
            if(!Context.IsWorldReady)
            {
                return;
            }
            if(config.StopHealingWhileGamePaused && (Game1.paused || Game1.activeClickableMenu != null || Game1.CurrentEvent != null))
            {
                return;
            }
            Player player = Game1.player;
            double healing = config.HealingValuePerSeconds;
            if(player.isMoving() || player.UsingTool)
            {
                TicksAccumulator = 0;
                return;
            }
            TicksAccumulator = Math.Min(TicksAccumulator + 1, config.SecondsNeededToStartHealing);
            if(config.HealHealth && player.health < player.maxHealth && TicksAccumulator >= config.SecondsNeededToStartHealing)
            {
                HPAccumlator += config.HealingValuePerSeconds;
                if(HPAccumlator == Math.Floor(HPAccumlator))
                {
                    player.health += (int)Math.Min(HPAccumlator, player.maxHealth - player.health);
                    HPAccumlator = 0;
                }
            }
            if(TicksAccumulator < config.SecondsNeededToStartHealing || player.Stamina == player.MaxStamina)
            {
                return;
            }
            else
            {
                player.Stamina += (float)Math.Min(healing, player.MaxStamina - player.Stamina);
            }
        }

        public void OnButtonPressed(object sender, EventArgs args)
        {
            if(!Context.IsPlayerFree || Game1.activeClickableMenu != null)
            {
                return;
            }
            if(IsPressed(config.DecreaseKey) && IsPressed(config.IncreaseKey))
            {
                return;
            }
            else if(IsPressed(config.DecreaseKey))
            {
                AddValueToHealing(-0.1);
            }
            else if(IsPressed(config.IncreaseKey))
            {
                AddValueToHealing(0.1);
            }
        }

        private void Log(string format, params object[] args)
        {
            Monitor.Log(string.Format(format, args));
        }
        private bool IsPressed(Keys keys)
        {
            return Keyboard.GetState().IsKeyDown(keys);
        }
        private void AddValueToHealing(double val)
        {
            config.HealingValuePerSeconds = (float)Round(config.HealingValuePerSeconds + val, 3);
            if (CorrectHealingValue())
            {
                Helper.WriteConfig(config);
                Log("Healing value changed to {0}", config.HealingValuePerSeconds);
                string health = config.HealHealth ? "/health" : "";
                if (config.HealingValuePerSeconds == 0)
                {
                    ShowHUDMessage($"Stamina{health} won't be modified by ASH itself.");
                }
                else
                {
                    ShowHUDMessage($"Stamina{health} will be incleaced by {config.HealingValuePerSeconds} per sec.");
                }
            }
        }
        private bool CorrectHealingValue()
        {
            if(config.SecondsNeededToStartHealing < 0)
            {
                config.SecondsNeededToStartHealing = 0;
            }
            if(config.HealingValuePerSeconds > MaxHealing)
            {
                config.HealingValuePerSeconds = MaxHealing;
            }
            else if(config.HealingValuePerSeconds < MinHealing)
            {
                config.HealingValuePerSeconds = MinHealing;
            }
            else
            {
                return true;
            }
            config.HealingValuePerSeconds = (float)Round(config.HealingValuePerSeconds, 3);
            return false;
        }
        private double Round(double val, int exponent)
        {
            return Math.Round(val, exponent, MidpointRounding.AwayFromZero);
        }
        private string Format(string format, params object[] args)
        {
            return string.Format(format, args);
        }
        private void ShowHUDMessage(string message,int duration = 3500)
        {
            HUDMessage hudMessage = new HUDMessage(message, 3);
            hudMessage.noIcon = true;
            hudMessage.timeLeft = duration;
            Game1.addHUDMessage(hudMessage);
        }
    }
}

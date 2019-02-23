using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace AdjustableStaminaHealing
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    public class ModEntry : Mod
    {
        private readonly float MaxHealing = 2.0f;
        private readonly float MinHealing = 0.0f;

        private int TicksAccumulator;
        private float HPAccumlator;

        Config config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<Config>();
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            CorrectHealingValue();
            helper.WriteConfig(config);
        }

        /// <summary>Raised once per second after the game state is updated.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }
            if (config.StopHealingWhileGamePaused && (Game1.paused || Game1.activeClickableMenu != null || Game1.CurrentEvent != null))
            {
                return;
            }
            Farmer player = Game1.player;
            double healing = config.HealingValuePerSeconds;
            if (player.isMoving() || player.UsingTool)
            {
                TicksAccumulator = 0;
                return;
            }
            TicksAccumulator = Math.Min(TicksAccumulator + 1, config.SecondsNeededToStartHealing);
            if (config.HealHealth && player.health < player.maxHealth && TicksAccumulator >= config.SecondsNeededToStartHealing)
            {
                HPAccumlator += config.HealingValuePerSeconds;
                if (HPAccumlator == Math.Floor(HPAccumlator))
                {
                    player.health += (int)Math.Min(HPAccumlator, player.maxHealth - player.health);
                    HPAccumlator = 0;
                }
            }
            if (TicksAccumulator < config.SecondsNeededToStartHealing || player.Stamina == player.MaxStamina)
            {
                return;
            }
            else
            {
                player.Stamina += (float)Math.Min(healing, player.MaxStamina - player.Stamina);
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void OnButtonPressed(object sender, EventArgs e)
        {
            if (!Context.IsPlayerFree || Game1.activeClickableMenu != null)
            {
                return;
            }

            IInputHelper input = this.Helper.Input;
            if (input.IsDown(config.DecreaseKey) && input.IsDown(config.IncreaseKey))
            {
                return;
            }
            else if (input.IsDown(config.DecreaseKey))
            {
                AddValueToHealing(-0.1);
            }
            else if (input.IsDown(config.IncreaseKey))
            {
                AddValueToHealing(0.1);
            }
        }

        private void AddValueToHealing(double val)
        {
            config.HealingValuePerSeconds = (float)Round(config.HealingValuePerSeconds + val, 3);
            if (CorrectHealingValue())
            {
                Helper.WriteConfig(config);
                this.Monitor.Log($"Healing value changed to {config.HealingValuePerSeconds}");
                string health = config.HealHealth ? "/health" : "";
                if (config.HealingValuePerSeconds == 0)
                {
                    ShowHUDMessage($"Stamina{health} won't be modified by ASH itself.");
                }
                else
                {
                    ShowHUDMessage($"Stamina{health} will be increased by {config.HealingValuePerSeconds} per sec.");
                }
            }
        }

        private bool CorrectHealingValue()
        {
            if (config.SecondsNeededToStartHealing < 0)
            {
                config.SecondsNeededToStartHealing = 0;
            }
            if (config.HealingValuePerSeconds > MaxHealing)
            {
                config.HealingValuePerSeconds = MaxHealing;
            }
            else if (config.HealingValuePerSeconds < MinHealing)
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

        private void ShowHUDMessage(string message, int duration = 3500)
        {
            HUDMessage hudMessage = new HUDMessage(message, 3);
            hudMessage.noIcon = true;
            hudMessage.timeLeft = duration;
            Game1.addHUDMessage(hudMessage);
        }
    }
}

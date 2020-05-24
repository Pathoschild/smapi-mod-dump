using System;
using System.Collections.Generic;
using System.IO;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SprintingMod
{
    public class Sprinting : Mod
    {
        public static SprintingModConfig Config { get; set; }

        private const string ModConflicts = "MovementMod";

        private Buff SprintingBuff { get; set; }
        private int timeSinceLastDrain = 0;
        private bool ZorynsMovementModExists = false;

        public override void Entry(IModHelper helper)
        {
            FindConflicts();
            Config = helper.ReadConfig<SprintingModConfig>();
            SprintingBuff = CreateBuff(Config.SprintSpeed);
            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            Helper.Events.Input.ButtonReleased += Input_ButtonReleased;
            Helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
            Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        }

        private void FindConflicts()
        {
            var modPaths = new List<string>();
            modPaths.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "Mods"));
            modPaths.Add(Path.Combine(Constants.ExecutionPath, "Mods"));
            foreach (var path in modPaths)
            {
                if (Directory.Exists(Path.Combine(path, ModConflicts)))
                {
                    ZorynsMovementModExists = true;
                }
            }
        }

        private static Buff CreateBuff(int sprintSpeed)
        {
            var sprintBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, sprintSpeed, 0, 0, 1000, "SprintMod", "Sprint Mod")
            {
                which = Buff.speed,
                sheetIndex = Buff.speed
            };
            return sprintBuff;
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (SprintingBuff_Exists() && ZorynsMovementModExists)
            {
                SprintingBuff.addBuff();
            }
        }

        private void GameLoop_OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (SprintingBuff_Exists())
            {
                int sprintBuffIndex = Game1.buffsDisplay.otherBuffs.IndexOf(SprintingBuff);
                Game1.buffsDisplay.otherBuffs[sprintBuffIndex].millisecondsDuration = 5555;
            }

            if (Game1.player.isMoving() && SprintingBuff_Exists())
            {
                timeSinceLastDrain += 1;
                if (timeSinceLastDrain >= Config.StaminaDrainRate)
                {
                    Game1.player.Stamina -= Config.StaminaDrain;
                    timeSinceLastDrain = 0;
                }
            }
            else
            {
                timeSinceLastDrain = 0;
            }
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            SButton pressedButton = e.Button;
            if (pressedButton.Equals(Config.SprintKey) || pressedButton.Equals(Config.SprintKeyForControllers))
            {
                if (Config.HoldToSprint)
                {
                    Player_Sprint();
                }
                else
                {
                    Player_Toggle_Sprint();
                }
            }
        }

        private void Input_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            SButton releasedButton = e.Button;
            if (releasedButton.Equals(Config.SprintKey) || releasedButton.Equals(Config.SprintKeyForControllers))
            {
                if (Config.HoldToSprint)
                {
                    Player_Walk();
                }
            }
        }

        private void Player_Sprint()
        {
            if (!SprintingBuff_Exists())
            {
                Game1.buffsDisplay.addOtherBuff(SprintingBuff);
            }
        }

        private void Player_Walk()
        {
            if (SprintingBuff_Exists())
            {
                Game1.buffsDisplay.otherBuffs.Remove(SprintingBuff);
                SprintingBuff.removeBuff();
                Game1.buffsDisplay.syncIcons();
            }
        }

        private void Player_Toggle_Sprint()
        {
            if (SprintingBuff_Exists())
            {
                Game1.buffsDisplay.otherBuffs.Remove(SprintingBuff);
                SprintingBuff.removeBuff();
                Game1.buffsDisplay.syncIcons();
            }
            else
            {
                Game1.buffsDisplay.addOtherBuff(SprintingBuff);
            }
        }

        private bool SprintingBuff_Exists()
        {
            if (SprintingBuff == null || Game1.buffsDisplay == null) { return false; }

            return Game1.buffsDisplay.otherBuffs.Contains(SprintingBuff);
        }
    }

    public class SprintingModConfig
    {
        public bool HoldToSprint { get; set; }
        public int SprintSpeed { get; set; }

        /// <summary>
        /// Sprint key for keyboard.
        /// </summary>
        public SButton SprintKey { get; set; }
        public int StaminaDrain { get; set; }
        public int StaminaDrainRate { get; set; }

        public SButton SprintKeyForControllers { get; set; }

        public SprintingModConfig()
        {
            HoldToSprint = true;
            SprintSpeed = 3;
            SprintKey = SButton.LeftControl;
            SprintKeyForControllers = SButton.LeftStick;
            StaminaDrain = 1;
            StaminaDrainRate = 5;
        }
    }
}

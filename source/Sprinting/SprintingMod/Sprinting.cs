using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;

namespace SprintingMod
{
    public class Sprinting : Mod
    {
        public static SprintingModConfig Config { get; set; }

        private const string _debuggerInfo = "[SprintingMod INFO] ";
        private const string modConflicts = "MovementMod";

        private Buff SprintingBuff { get; set; }
        private int timeSinceLastDrain = 0;
        private bool ZorynsMovementModExists = false;
        private bool defaultSpeedSet = false;
        private int defaultPlayerSpeed = 0;

        public override void Entry(IModHelper helper)
        {
            FindConflicts();
            Config = helper.ReadConfig<SprintingModConfig>();
            BuffInit();
            KeyboardInput.KeyDown += KeyboardInput_KeyDown;
            KeyboardInput.KeyUp += KeyboardInput_KeyUp;
            ControlEvents.ControllerButtonPressed += ControllerButtonPressed;
            ControlEvents.ControllerButtonReleased += ControllerButtonReleased;
            GameEvents.OneSecondTick += GameEvents_OneSecondTick;
            GameEvents.UpdateTick += GameEvents_UpdateTick;
        }

        private void FindConflicts()
        {
            var modPaths = new List<string>();
            modPaths.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "Mods"));
            modPaths.Add(Path.Combine(Constants.ExecutionPath, "Mods"));
            foreach (var path in modPaths)
            {
                if (Directory.Exists(Path.Combine(path, modConflicts)))
                {
                    ZorynsMovementModExists = true;
                }
            }
        }

        private void BuffInit()
        {
            SprintingBuff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, Config.SprintSpeed, 0, 0, 1000, "SprintMod", "Sprint Mod");
            SprintingBuff.which = Buff.speed;
            SprintingBuff.sheetIndex = Buff.speed;
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (SprintingBuff_Exists() && ZorynsMovementModExists)
            {
                SprintingBuff.addBuff();
            }
        }

        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {
            if (SprintingBuff_Exists())
            {
                Game1.buffsDisplay.otherBuffs[Game1.buffsDisplay.otherBuffs.IndexOf(SprintingBuff)].millisecondsDuration = 5555;
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

        private void KeyboardInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString().Equals(Config.SprintKey))
            {
                if (Config.HoldToSprint)
                    Player_Sprint();
                else
                    Player_Toggle_Sprint();
            }
        }

        private void KeyboardInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString().Equals(Config.SprintKey))
            {
                if (Config.HoldToSprint)
                {
                    Player_Walk();
                }
            }
        }

        private void ControllerButtonPressed(object sender, EventArgsControllerButtonPressed e)
        {
            if (e.ButtonPressed.ToString().Equals(Config.SprintKeyForControllers))
            {
                if (Config.HoldToSprint)
                    Player_Sprint();
                else
                    Player_Toggle_Sprint();
            }
        }

        private void ControllerButtonReleased(object sender, EventArgsControllerButtonReleased e)
        {
            if (e.ButtonReleased.ToString().Equals(Config.SprintKeyForControllers))
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
                Game1.buffsDisplay.addOtherBuff(SprintingBuff);
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
            if (SprintingBuff == null || Game1.buffsDisplay == null)
                return false;

            return Game1.buffsDisplay.otherBuffs.Contains(SprintingBuff);
        }
    }

    public class SprintingModConfig
    {
        public bool HoldToSprint { get; set; }
        public int SprintSpeed { get; set; }
        public string SprintKey { get; set; }
        public int StaminaDrain { get; set; }
        public int StaminaDrainRate { get; set; }
        public string SprintKeyForControllers { get; set; }

        public SprintingModConfig()
        {
            HoldToSprint = true;
            SprintSpeed = 3;
            SprintKey = "17";
            SprintKeyForControllers = "LeftStick";
            StaminaDrain = 1;
            StaminaDrainRate = 5;
        }
    }
}

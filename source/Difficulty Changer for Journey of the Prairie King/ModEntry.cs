/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/shanks3042/stardewvalleyeasyprairieking
**
*************************************************/


/*

Provides a few options to make Journey of the Prairie King minigame in Stardew
Valley easier

Copyright(C) 2020  shanks3042

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<https://www.gnu.org/licenses/>.

*/


using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace StardewValleyEasyPrairieKing
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>

        private ModConfig config_;
        private List<String> commands_ = new List<String>();
        private const int INFINITE = 99;
        private Dictionary<string, bool> valuesSet = new Dictionary<string, bool>();
        //private IReflectedField<int> waveTimerCount;
        /// 

        public override void Entry(IModHelper helper)
        {
            this.config_ = helper.ReadConfig<ModConfig>();
            this.valuesSet.Add("lives", false);
            this.valuesSet.Add("coins", false);
            this.config_.waveTimer *= 1000;

            helper.Events.GameLoop.UpdateTicked += EventUpdateTicks;
            //helper.Events.Input.ButtonPressed += onButtonPressed;

            Dictionary<string, object> prairieMembers = new Dictionary<string, object>();

            foreach (PropertyInfo member in this.config_.GetType().GetProperties())
            {
                //prairieMembers.Add(member.Name, member.GetValue(this.config_));
                if(!member.Name.ToLower().Contains("item"))
                {
                    string command = String.Concat("prairie_", member.Name.ToString());
                    string helpMessage = String.Format("Sets {0} in Prairie King", member.Name.ToString());
                    helper.ConsoleCommands.Add(command, helpMessage, this.SetPrairieValue);
                    this.commands_.Add(String.Format("{0} : {1}\n", command, helpMessage));
                }

            }

            helper.ConsoleCommands.Add("prairie_save", "Saves changes to make it permanent", this.SetPrairieValue);
            helper.ConsoleCommands.Add("prairie_help", "Prints all available commands for EasyPrairieKing", this.SetPrairieValue);


        }


        /*********
        ** Private methods
        *********/
        /// executes commands from console
        /// @param command: command name (e.g. prairie_lives)
        /// @param args : value for the commands
        /// 
        private void SetPrairieValue(string command, string[] args)
        {

            if (command.ToLower().Contains("save"))
            {
                this.Helper.WriteConfig(this.config_);
                this.Monitor.Log($"saved changes to config.", LogLevel.Info);
                return;
            }
            else if (command.ToLower().Contains("help"))
            {

                foreach(string elem in this.commands_)
                {
                    this.Monitor.Log($"{elem}", LogLevel.Info);
                }

                return;
            }
            else if (args.Length < 1)
            {
                this.Monitor.Log($"Could not set value.\n Please use numbers. (0...100)\n Example: {command} 50\n", LogLevel.Error);
                return;
            }


            string member = command.Substring(command.IndexOf("_", StringComparison.CurrentCulture) + 1);
            string text = "";

            try
            {

                try
                {
                    this.config_.GetType().GetProperty(member, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).SetValue(this.config_, Int32.Parse(args[0]));
                    text = args[0];
                }
                catch
                {
                    this.config_.GetType().GetProperty(member, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).SetValue(this.config_, Boolean.Parse(args[0]));
                    text = Boolean.Parse(args[0]).ToString();
                }

                this.Monitor.Log($"Set {member} to {text}.", LogLevel.Info);

            }
            catch//(Exception e)
            {
                // this.Monitor.Log($"{e}", LogLevel.Error);
                //this.Monitor.Log($"{e.Message}", LogLevel.Error);
                this.Monitor.Log($"Could not set {member} to {args[0]}.\n Please use numbers. (0...100) or true/false for useShotgun", LogLevel.Error);
            }

        }




        private void EventUpdateTicks(object sender, EventArgs e)
        {
            if (Game1.currentMinigame == null || !"AbigailGame".Equals(Game1.currentMinigame.GetType().Name))
            {
                valuesSet["lives"] = false;
                valuesSet["coins"] = false;
                return;
            }

            Type minigameType = Game1.currentMinigame.GetType();

            if(this.config_.useWheel)
            {
                Dictionary<int, int> wheel = new Dictionary<int, int>() { { 2, 10000 } };
                minigameType.GetField("activePowerups").SetValue(Game1.currentMinigame, wheel );
            }

            if (this.config_.lives > INFINITE)
            {
                minigameType.GetField("lives").SetValue(Game1.currentMinigame, INFINITE);
            }
            else if (this.config_.lives != 0 && !valuesSet["lives"])
            {
                minigameType.GetField("lives").SetValue(Game1.currentMinigame, this.config_.lives);
                valuesSet["lives"] = true;
            }

            if (this.config_.coins > INFINITE)
            {
                minigameType.GetField("coins").SetValue(Game1.currentMinigame, INFINITE);
            }
            else if (this.config_.coins != 0 && !valuesSet["coins"])
            {
                minigameType.GetField("coins").SetValue(Game1.currentMinigame, this.config_.coins);
                valuesSet["coins"] = true;
            }

            if (this.config_.ammoLevel != 0)
            {
                minigameType.GetField("ammoLevel").SetValue(Game1.currentMinigame, this.config_.ammoLevel);
            }

            if (this.config_.bulletDamage != 0)
            {
                minigameType.GetField("bulletDamage").SetValue(Game1.currentMinigame, this.config_.bulletDamage);
            }

            if (this.config_.fireSpeedLevel != 0)
            {
                minigameType.GetField("shootingDelay").SetValue(Game1.currentMinigame, this.config_.fireSpeedLevel);
            }

            if (this.config_.runSpeedLevel != 0)
            {
                minigameType.GetField("runSpeedLevel").SetValue(Game1.currentMinigame, this.config_.runSpeedLevel);
            }

            if (this.config_.useShotgun)
            {
                minigameType.GetField("spreadPistol").SetValue(Game1.currentMinigame, this.config_.useShotgun);
            }

            if (this.config_.alwaysInvincible)
            {
                minigameType.GetField("playerInvincibleTimer").SetValue(Game1.currentMinigame, 5000);
            }

            if (this.config_.waveTimer != 0 && (Int32)minigameType.GetField("waveTimer").GetValue(Game1.currentMinigame) > this.config_.waveTimer)
            {
                minigameType.GetField("waveTimer").SetValue(Game1.currentMinigame, this.config_.waveTimer);
            }
        }



    }
}
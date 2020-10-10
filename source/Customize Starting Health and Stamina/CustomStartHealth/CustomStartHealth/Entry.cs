/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MissCoriel/Customize-Health-and-Stamina
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomStartHealth
{
    public class ModEntry : Mod
    {
        private ConfigModel Config;
        //private SaveModel Save;
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += this.GameStart; //This will make the changes
            this.Config = Helper.ReadConfig<ConfigModel>(); //Reads the config.json
            helper.ConsoleCommands.Add("setmaxHP", "Set a specific amount of HP", setmaxHPCommand);
            helper.ConsoleCommands.Add("addHP", "Adds the amount entered to MaxHP", addHPCommand);
            helper.ConsoleCommands.Add("setmaxStam", "Set a specific amount of Stamina", setmaxStamCommand);
            helper.ConsoleCommands.Add("addStam", "Adds the amount entered to Max Stamina", addStamCommand);

            
        }

        private void GameStart(object sender, SaveLoadedEventArgs e)
        {
            int HPMod = this.Config.MaxHP;
            int StamMod = this.Config.MaxST;
            var svData = this.Helper.Data.ReadSaveData<SaveModel>("AlreadyModified");
            if (svData == null || !svData.AlreadyModified)
            {
                Game1.player.maxHealth = HPMod;
                Game1.player.maxStamina.Value = StamMod;
                Game1.player.health = HPMod;
                this.Helper.Data.WriteSaveData("AlreadyModified", new SaveModel { AlreadyModified = true });
                Monitor.Log("Setting Maximum HP to " + HPMod + "!", LogLevel.Debug);
                Monitor.Log("Setting Maximum Stamina to " + StamMod + "!", LogLevel.Debug);
            }
            else return;

        }
        private void setmaxHPCommand(string command, string[] parameters)
        {
            if (parameters.Length == 0) return;
            try
            {
                int setMax = int.Parse(parameters[0]);
                Game1.player.maxHealth = setMax;
                Monitor.Log("Set Max HP to " + setMax, LogLevel.Debug);

            }
            catch (Exception) { }
        }
        private void addHPCommand(string command, string[] parameters)
        {
            if (parameters.Length == 0) return;
            try
            {
                int addHP = int.Parse(parameters[0]);
                int currentMax = Game1.player.maxHealth;
                Game1.player.maxHealth = currentMax + addHP;
                Monitor.Log($"Add {addHP} to {currentMax}!", LogLevel.Debug);

            }
            catch (Exception) { }
        }
        private void setmaxStamCommand(string command, string[] parameters)
        {
            if (parameters.Length == 0) return;
            try
            {
                int setMax = int.Parse(parameters[0]);
                Game1.player.maxStamina.Value = setMax;
                Monitor.Log("Set Max Stamina to " + setMax, LogLevel.Debug);

            }
            catch (Exception) { }
        }
        private void addStamCommand(string command, string[] parameters)
        {
            if (parameters.Length == 0) return;
            try
            {
                int addStam = int.Parse(parameters[0]);
                int currentMax = Game1.player.maxStamina.Value;
                Game1.player.maxStamina.Value = currentMax + addStam;
                Monitor.Log($"Add {addStam} to {currentMax}!", LogLevel.Debug);

            }
            catch (Exception) { }
        }


    }
}

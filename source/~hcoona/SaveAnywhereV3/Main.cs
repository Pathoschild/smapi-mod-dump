/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hcoona/StardewValleyMods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace SaveAnywhereV3
{
    public class Main : Mod
    {
        private bool ModSaved { get; set; }

        private DateTime LastSavedDateTime { get; set; }

        private SaveLoadManager SaveLoadManager { get; set; }

        public override void Entry(IModHelper helper)
        {
            base.Entry(helper);
            Global.Helper = helper;
            Global.Monitor = Monitor;
            Global.Config = Helper.ReadConfig<Config>();
            Helper.WriteConfig(Global.Config);

            Monitor.Log($"Begin dump Save/Load services");
            foreach (var s in Global.SaveLoadServices)
            {
                Monitor.Log($"{s.GetType().Name} loaded");
            }
            Monitor.Log($"Finish dump Save/Load services");

            SaveLoadManager = new SaveLoadManager(Global.SaveLoadServices);

            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            SaveEvents.AfterSave += SaveEvents_AfterSave;

            ControlEvents.KeyPressed += ControlEvents_KeyPressed;
            if (Global.Config.AutoSavingEnabled)
            {
                LastSavedDateTime = DateTime.UtcNow;
                GameEvents.SecondUpdateTick += this.GameEvents_SecondUpdateTick;
            }
        }

        private void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (e.KeyPressed == Global.Config.SaveBindingKey)
            {
                Save();
            }
            else if (e.KeyPressed == Global.Config.LoadBindingKey)
            {
                Load();
            }
        }

        private void GameEvents_SecondUpdateTick(object sender, EventArgs e)
        {
            if (DateTime.UtcNow - LastSavedDateTime > TimeSpan.FromMinutes(Global.Config.AutoSavingIntervalMinutes))
            {
                Save();
            }
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            Load();
        }

        private void SaveEvents_AfterSave(object sender, EventArgs e)
        {
            if (ModSaved)
            {
                ModSaved = false;
            }
            else // Normal game saving
            {
                try
                {
                    foreach (var s in Global.SaveLoadServices)
                    {
                        s.Clear();
                    }
                }
                catch (Exception ex)
                {
                    Monitor.Log(ex.ToString(), LogLevel.Error);
                }
            }
        }

        private void Save()
        {
            if (Game1.activeClickableMenu == null && Game1.player.canMove)
            {
                try
                {
                    SaveLoadManager.Save();
                }
                catch (Exception ex)
                {
                    Monitor.Log(ex.ToString(), LogLevel.Error);
                    Game1.showRedMessage("Error during saving, you'd better to check the log");
                }

                Game1.activeClickableMenu = new SaveGameMenu();
                LastSavedDateTime = DateTime.UtcNow;
                ModSaved = true;
            }
            else
            {
                var message = "You cannot save here";
                Monitor.Log(message);
                Game1.showRedMessage(message);
            }
        }

        private void Load()
        {
            if (System.IO.File.Exists(SaveLoadManager.SaveFilePath))
            {
                try
                {
                    SaveLoadManager.Load();
                }
                catch (Exception ex)
                {
                    Monitor.Log(ex.ToString(), LogLevel.Error);
                    Game1.showRedMessage("Error during loading, you'd better to check the log & exit game");
                }
            }
            else
            {
                var message = "Failed to load, the save file is missing";
                Monitor.Log(message, LogLevel.Error);
                Game1.showRedMessage(message);
            }
        }
    }
}

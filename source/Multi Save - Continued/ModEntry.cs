/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/recon88/Multi-Save-Continued
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using System.IO;
using static StardewValley.Menus.LoadGameMenu;

namespace MultiSave
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {

        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;

        public static ModEntry context;
        public static string folderPrefix = "MultiSave";
        public static string backupFolderKey = "MultiSave/backupFolder";

        public static SaveFileSlot currentSaveSlot;
        public static List<string> currentSaveBackupList = new();
        public static List<string[]> saveBackupList = new();
        
        public static int tempDayOfWeek = -1;
        
        public static string[] seasons = new string[] { "spring", "summer", "fall", "winter" };

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            context = this;

            SMonitor = Monitor;
            SHelper = helper;

            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;

            Helper.Events.GameLoop.DayEnding += GameLoop_DayEnding;
            Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            Helper.Events.Input.ButtonPressed += Input_ButtonPressed;

            var harmony = new Harmony(ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Constructor(typeof(LoadGameMenu), searchForStatic: true),
                postfix: new HarmonyMethod(typeof(LoadGameMenu_Patch).GetMethod(nameof(LoadGameMenu_Patch.Postfix)))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(SaveFileSlot), nameof(SaveFileSlot.Draw)),
                postfix: new HarmonyMethod(typeof(SaveFileSlot_Draw_Patch).GetMethod(nameof(SaveFileSlot_Draw_Patch.Postfix)))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(TitleMenu), nameof(TitleMenu.backButtonPressed)),
                prefix: new HarmonyMethod(typeof(TitleMenu_backButtonPressed_Patch).GetMethod(nameof(TitleMenu_backButtonPressed_Patch.Prefix)))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(LoadGameMenu), "update"),
                postfix: new HarmonyMethod(typeof(LoadGameMenu_update_Patch).GetMethod(nameof(LoadGameMenu_update_Patch.Postfix)))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(LoadGameMenu), "deleteFile"),
                prefix: new HarmonyMethod(typeof(LoadGameMenu_deleteFile_Patch).GetMethod(nameof(LoadGameMenu_deleteFile_Patch.Prefix)))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(LoadGameMenu), nameof(LoadGameMenu.receiveLeftClick)),
                prefix: new HarmonyMethod(typeof(LoadGameMenu_receiveLeftClick_Patch).GetMethod(nameof(LoadGameMenu_receiveLeftClick_Patch.Prefix)))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(LoadGameMenu), nameof(LoadGameMenu.performHoverAction)),
                prefix: new HarmonyMethod(typeof(LoadGameMenu_performHoverAction_Patch).GetMethod(nameof(LoadGameMenu_performHoverAction_Patch.Prefix)))
                );
            harmony.Patch(
                original: AccessTools.Method(typeof(SaveFileSlot), nameof(SaveFileSlot.Activate)),
                prefix: new HarmonyMethod(typeof(SaveFileSlot_Activate_Patch).GetMethod(nameof(SaveFileSlot_Activate_Patch.Prefix)))
                );
            fixStructure();
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            if (!Config.EnableMod || Constants.CurrentSavePath is null)
                return;
            if (Config.MaxDaysOldToKeep > 0)
            {
                var backups = GetBackups(Constants.SaveFolderName);
                if (backups.Length > 0)
                {
                    var farmers = GetSaveSlots(Constants.SaveFolderName, backups);
                    foreach (var farmer in farmers)
                    {
                        var daysOld = Game1.Date.TotalDays - new WorldDate(farmer.yearForSaveGame.Value, seasons[farmer.seasonForSaveGame.Value], farmer.dayOfMonthForSaveGame.Value).TotalDays;
                        if (daysOld > Config.MaxDaysOldToKeep)
                        {
                            SMonitor.Log($"Deleting backup {daysOld} days old", LogLevel.Info);
                            Directory.Delete(farmer.modData[backupFolderKey], true);
                        }
                    }
                }
            }
        }

        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (!Config.EnableMod || !Context.CanPlayerMove || Constants.CurrentSavePath is null)
                return;
            if(Config.SaveButton != SButton.None && e.Button == Config.SaveButton)
            {
                Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("saving"), 2));
                SaveBackup();
            }
        }

        private void GameLoop_DayEnding(object sender, StardewModdingAPI.Events.DayEndingEventArgs e)
        {
            if (!Config.EnableMod || Constants.CurrentSavePath is null)
                return;
            if (Config.AutoSaveDaily || (Config.AutoSaveOnDayOfWeek > 0 && Game1.dayOfMonth % 7 == Config.AutoSaveOnDayOfWeek % 7) || (Config.AutoSaveOnDayOfMonth > 0 && Game1.dayOfMonth == Config.AutoSaveOnDayOfMonth))
                SaveBackup();
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Mod Enabled",
                getValue: () => Config.EnableMod,
                setValue: value => Config.EnableMod = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Save Daily",
                getValue: () => Config.AutoSaveDaily,
                setValue: value => Config.AutoSaveDaily = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Save On Day of Week",
                tooltip: () => "0 = disabled",
                getValue: () => Config.AutoSaveOnDayOfWeek,
                setValue: value => Config.AutoSaveOnDayOfWeek = value,
                min: 0,
                max: 7
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Save On Day of Month",
                tooltip: () => "0 = disabled",
                getValue: () => Config.AutoSaveOnDayOfMonth,
                setValue: value => Config.AutoSaveOnDayOfMonth = value,
                min: 0,
                max: (int)AccessTools.Field(typeof(SDate), "DaysInSeason").GetValue(new SDate(1,"spring"))
            );

            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => "Save Button",
                getValue: () => Config.SaveButton,
                setValue: value => Config.SaveButton = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Max Days Old",
                getValue: () => Config.MaxDaysOldToKeep,
                setValue: value => Config.MaxDaysOldToKeep = value
            );

        }

        private void fixStructure()
        {
            // This method renames the folders from aedenthorn.MultiSave to just MultiSave
            var savepath = Path.Combine(Constants.SavesPath);
            var sourcedirs = new DirectoryInfo(savepath).GetDirectories();
            foreach (DirectoryInfo dir in sourcedirs)
            {
                var savedirs = dir.GetDirectories();
                foreach (DirectoryInfo dir1 in savedirs)
                {
                    if (Path.GetFileName(dir1.Name).StartsWith("aedenthorn."))
                    {
                        var newname = dir1.Name.Split("aedenthorn.")[1];
                        dir1.MoveTo(Path.Combine(dir.FullName, newname));
                        this.Monitor.Log("Renamed save folder " + dir1.Name + " to " + newname + ". This is only done once to adopt to the new folder structure of the mod.", LogLevel.Warn);
                    }
                }
            }
        }
    }
}

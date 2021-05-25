/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A SMAPI GameLaunched event that enables GMCM support if that mod is available.</summary>
        public void EnableGMCM(object sender, GameLaunchedEventArgs e)
        {
            try
            {
                GenericModConfigMenuAPI api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu"); //attempt to get GMCM's API instance

                if (api == null) //if the API is NOT available
                {
                    Monitor.Log($"Optional API not found: Generic Mod Config Menu (GMCM).", LogLevel.Trace);
                    return;
                }
                else //if the API is available
                {
                    Monitor.Log($"Optional API found: Generic Mod Config Menu (GMCM).", LogLevel.Trace);
                }

                api.RegisterModConfig(ModManifest, () => ResetConfigToDefault(), () => Helper.WriteConfig(Utility.MConfig)); //register "revert to default" and "write" methods for this mod's config
                api.SetDefaultIngameOptinValue(ModManifest, true); //allow in-game setting changes (rather than just at the main menu)

                //register an option for each of this mod's config settings
                api.RegisterSimpleOption(ModManifest, "Enable console commands", "Uncheck this box to disable FTM's console commands, e.g. for mod compatibility.\nNOTE: This will not take effect until Stardew Valley is restarted.", () => Utility.MConfig.EnableConsoleCommands, (bool val) => Utility.MConfig.EnableConsoleCommands = val);
                api.RegisterSimpleOption(ModManifest, "Enable content packs", "Uncheck this box to disable all FTM content packs.\nOnly the \"personal\" files in FarmTypeManager/data will be used.", () => Utility.MConfig.EnableContentPacks, (bool val) => Utility.MConfig.EnableContentPacks = val);
                api.RegisterSimpleOption(ModManifest, "Enable trace log messages", "Uncheck this box to disable FTM's [TRACE] message type in SMAPI's log files.\nLogs will be smaller but provide less info.", () => Utility.MConfig.EnableTraceLogMessages, (bool val) => Utility.MConfig.EnableTraceLogMessages = val);
                api.RegisterSimpleOption(ModManifest, "Enable EPU debug messages", "Check this box to enable Expanded Preconditions Utility (EPU) debug messages.\nThis can be helpful when testing preconditions.", () => EPUDebugMessages, (bool val) => EPUDebugMessages = val);
                api.RegisterSimpleOption(ModManifest, "Monster limit per location", "The maximum number of monsters FTM will spawn on a single map.\nEnter NULL for unlimited monsters.", () => MonsterLimitAsString, (string val) => MonsterLimitAsString = val);
            }
            catch (Exception ex)
            {
                Utility.Monitor.Log($"An error happened while loading FTM's GMCM options menu. That menu might be missing or fail to work. The auto-generated error message has been added to the log.", LogLevel.Warn);
                Utility.Monitor.Log($"----------", LogLevel.Trace);
                Utility.Monitor.Log($"{ex.ToString()}", LogLevel.Trace);
            }
        }

        private void ResetConfigToDefault()
        {
            Utility.MConfig = new ModConfig(); //create and use a new config with default settings
            EPUDebugMessages = Utility.MConfig.EnableEPUDebugMessages; //re-initialize EPU's API, if applicable
        }

        private bool EPUDebugMessages
        {
            get
            {
                return Utility.MConfig.EnableEPUDebugMessages; //return the current value
            }

            set
            {
                if (Utility.EPUConditionsChecker != null) //if EPU's API instance exists
                {
                    try
                    {
                        Utility.EPUConditionsChecker.Initialize(value, this.ModManifest.UniqueID); //re-initialize the conditions checker
                        Utility.MConfig.EnableEPUDebugMessages = value; //set the new value in the config
                    }
                    catch (Exception ex)
                    {
                        Utility.Monitor.Log($"An error occurred while trying to modify EPU's API debug message setting. Please report this to FTM's developer and/or manually change the EPU setting in FTM's config.json file. The auto-generated error message has been added to your log.", LogLevel.Warn);
                        Utility.Monitor.Log($"----------", LogLevel.Trace);
                        Utility.Monitor.Log($"{ex.ToString()}", LogLevel.Trace);
                    }
                }
            }
        }

        private string MonsterLimitAsString
        {
            get
            {
                if (Utility.MConfig.MonsterLimitPerLocation == null) //if there is no monster limit
                {
                    return "null"; //return the word "null"
                }
                else //if there is a monster limit
                {
                    return Utility.MConfig.MonsterLimitPerLocation.Value.ToString(); //return the numeric limit as a string
                }
            }

            set
            {
                if (value.Trim().Equals("null", StringComparison.OrdinalIgnoreCase)) //if the value is "null" (case-insensitive, ignoring whitespace)
                {
                    Utility.MConfig.MonsterLimitPerLocation = null; //set the monster limit to null
                }
                else //if the value is not "null"
                {
                    bool parsed = Int32.TryParse(value, out int intValue); //try to parse the value as an integer
                    if (parsed) //if parsing succeeded
                    {
                        Utility.MConfig.MonsterLimitPerLocation = intValue; //set the monster limit to the integer value
                    }

                    //if parsing failed, do nothing
                }
            }
        }
    }

    /// <summary>Generic Mod Config Menu's API interface. Used to recognize & interact with the mod's API when available.</summary>
    public interface GenericModConfigMenuAPI
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
        void SetDefaultIngameOptinValue(IManifest mod, bool optedIn);

        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet);
    }
}

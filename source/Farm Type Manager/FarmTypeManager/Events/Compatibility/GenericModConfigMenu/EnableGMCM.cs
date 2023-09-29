/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A SMAPI event that enables GMCM support if that mod is available.</summary>
        public void EnableGMCM(object sender, GameLaunchedEventArgs e)
        {
            try
            {
                var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu"); //attempt to get GMCM's API instance

                if (api == null) //if the API is NOT available
                {
                    Monitor.Log($"Optional API not found: Generic Mod Config Menu (GMCM).", LogLevel.Trace);
                    return;
                }
                else //if the API is available
                {
                    Monitor.Log($"Optional API found: Generic Mod Config Menu (GMCM).", LogLevel.Trace);
                }

                //create this mod's menu
                api.Register
                (
                    mod: ModManifest,
                    reset: () => ResetConfigToDefault(),
                    save: () => Helper.WriteConfig(Utility.MConfig),
                    titleScreenOnly: false
                );

                //register an option for each of this mod's config settings
                api.AddBoolOption
                (
                    mod: ModManifest,
                    getValue: () => Utility.MConfig.EnableConsoleCommands,
                    setValue: (bool val) => Utility.MConfig.EnableConsoleCommands = val,
                    name: () => Helper.Translation.Get("Config.EnableConsoleCommands.Name"),
                    tooltip: () => Helper.Translation.Get("Config.EnableConsoleCommands.Desc")
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    getValue: () => Utility.MConfig.EnableContentPacks,
                    setValue: (bool val) => Utility.MConfig.EnableContentPacks = val,
                    name: () => Helper.Translation.Get("Config.EnableContentPacks.Name"),
                    tooltip: () => Helper.Translation.Get("Config.EnableContentPacks.Desc")
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    getValue: () => Utility.MConfig.EnableTraceLogMessages,
                    setValue: (bool val) => Utility.MConfig.EnableTraceLogMessages = val,
                    name: () => Helper.Translation.Get("Config.EnableTraceLogMessages.Name"),
                    tooltip: () => Helper.Translation.Get("Config.EnableTraceLogMessages.Desc")
                );

                api.AddBoolOption
                (
                    mod: ModManifest,
                    getValue: () => EPUDebugMessages,
                    setValue: (bool val) => EPUDebugMessages = val,
                    name: () => Helper.Translation.Get("Config.EnableEPUDebugMessages.Name"),
                    tooltip: () => Helper.Translation.Get("Config.EnableEPUDebugMessages.Desc")
                );

                api.AddTextOption
                (
                    mod: ModManifest,
                    getValue: () => MonsterLimitAsString,
                    setValue: (string val) => MonsterLimitAsString = val,
                    name: () => Helper.Translation.Get("Config.MonsterLimitPerLocation.Name"),
                    tooltip: () => Helper.Translation.Get("Config.MonsterLimitPerLocation.Desc")
                );
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
}

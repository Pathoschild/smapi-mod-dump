/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/DestroyableBushes
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;

namespace DestroyableBushes
{
    public partial class ModEntry : Mod
    {
        /// <summary>A SMAPI GameLaunched event that enables GMCM support if that mod is available.</summary>
        public void EnableGMCM(object sender, GameLaunchedEventArgs e)
        {
            try
            {
                GenericModConfigMenuAPI api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu"); //attempt to get GMCM's API instance

                if (api == null) //if the API is not available
                    return;

                api.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config)); //register "revert to default" and "write" methods for this mod's config
                api.SetDefaultIngameOptinValue(ModManifest, true); //allow in-game setting changes (rather than just at the main menu)

                //register an option for each of this mod's config settings
                api.RegisterSimpleOption
                (
                    ModManifest,
                    "When bushes regrow",
                    "The amount of time before destroyed bushes will regrow.\nType a number and then a unit of time (Days, Seasons, Years).\nLeave this blank to never respawn bushes.\nExamples: \"3 days\" \"1 season\" \"1 year\"",
                    () => Config.WhenBushesRegrow ?? "null", //return the string "null" if the setting is null
                    (string val) => Config.WhenBushesRegrow = val
                );
                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Destroyable bush locations",
                    "A list of locations where bushes should be destroyable.\nIf the list is empty, all locations will be allowed.\nSeparate each name with a comma.\nExample: \"Farm, BusStop, Forest, Woods\"",
                    () => GMCMLocationList,
                    (string val) => GMCMLocationList = val
                );

                api.RegisterLabel
                (
                    ModManifest,
                    "Destroyable bush types",
                    "The types of bush that are allowed to be destroyed."
                );
                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Small bushes",
                    "Check this box to make small bushes destroyable.",
                    () => Config.DestroyableBushTypes.SmallBushes,
                    (bool val) => Config.DestroyableBushTypes.SmallBushes = val
                );
                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Medium bushes",
                    "Check this box to make medium bushes destroyable. These are the type that can produce berries.",
                    () => Config.DestroyableBushTypes.MediumBushes,
                    (bool val) => Config.DestroyableBushTypes.MediumBushes = val
                );
                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Large bushes",
                    "Check this box to make large bushes destroyable.",
                    () => Config.DestroyableBushTypes.LargeBushes,
                    (bool val) => Config.DestroyableBushTypes.LargeBushes = val
                );

                api.RegisterLabel
                (
                    ModManifest,
                    "Amount of wood dropped",
                    "The number of wood pieces dropped when each type of bush is destroyed."
                );
                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Small bushes",
                    "The number of wood pieces dropped when a small bush is destroyed.",
                    () => Config.AmountOfWoodDropped.SmallBushes,
                    (int val) => Config.AmountOfWoodDropped.SmallBushes = val
                );
                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Medium bushes",
                    "The number of wood pieces dropped when a medium bush is destroyed.",
                    () => Config.AmountOfWoodDropped.MediumBushes,
                    (int val) => Config.AmountOfWoodDropped.MediumBushes = val
                );
                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Large bushes",
                    "The number of wood pieces dropped when a large bush is destroyed.",
                    () => Config.AmountOfWoodDropped.LargeBushes,
                    (int val) => Config.AmountOfWoodDropped.LargeBushes = val
                );
                api.RegisterSimpleOption
                (
                    ModManifest,
                    "Green tea bushes",
                    "The number of wood pieces dropped when a green tea bush is destroyed.",
                    () => Config.AmountOfWoodDropped.GreenTeaBushes,
                    (int val) => Config.AmountOfWoodDropped.GreenTeaBushes = val
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"An error happened while loading this mod's GMCM options menu. Its menu might be missing or fail to work. The auto-generated error message has been added to the log.", LogLevel.Warn);
                Monitor.Log($"----------", LogLevel.Trace);
                Monitor.Log($"{ex.ToString()}", LogLevel.Trace);
            }
        }

        /// <summary>Converts <see cref="ModConfig.DestroyableBushLocations"/> to/from a string.</summary>
        private string GMCMLocationList
        {
            get
            {
                string result = "";
                int total = Config.DestroyableBushLocations?.Count ?? 0; //get the number of listed locations (or 0 if null)

                if (total > 0) //if any locations are listed
                {
                    result = Config.DestroyableBushLocations[0]; //start with the first location name

                    for (int x = 1; x < total; x++) //for each location after the first
                    {
                        result += ", " + Config.DestroyableBushLocations[x]; //add a comma, space, and the next location name
                    }
                }

                return result;
            }

            set
            {
                List<string> locationList = new List<string>(); //create an empty list

                if (!string.IsNullOrWhiteSpace(value)) //if the provided string is NOT null or otherwise blank
                {
                    string[] locations = value.Split(','); //get an array of location names that were separated by commas

                    foreach (string location in locations) //for each location name
                    {
                        locationList.Add(location.Trim()); //add the (whitespace-trimmed) location name to the list
                    }
                }

                Config.DestroyableBushLocations = locationList; //set the created list
            }
        }
    }

    /// <summary>Generic Mod Config Menu's API interface. Used to recognize & interact with the mod's API when available.</summary>
    public interface GenericModConfigMenuAPI
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
        void SetDefaultIngameOptinValue(IManifest mod, bool optedIn);

        void RegisterLabel(IManifest mod, string labelName, string labelDesc);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet);
    }
}

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
        /// <summary>A SMAPI GameLaunched event that enables EPU support if that mod is available.</summary>
        public void EnableEPU(object sender, GameLaunchedEventArgs e)
        {
            try
            {
                IConditionsChecker api = Helper.ModRegistry.GetApi<IConditionsChecker>("Cherry.ExpandedPreconditionsUtility"); //attempt to get an instance of EPU's API

                if (api == null) //if the API is NOT available
                {
                    Monitor.Log($"Optional API not found: Expanded Preconditions Utility (EPU).", LogLevel.Trace);
                    return;
                }
                else //if the API is available
                {
                    Monitor.Log($"Optional API found: Expanded Preconditions Utility (EPU).", LogLevel.Trace);
                }

                api.Initialize(Utility.MConfig.EnableEPUDebugMessages, this.ModManifest.UniqueID); //initialize the API

                Utility.EPUConditionsChecker = api; //pass the API to this mod's static utility property
            }
            catch (Exception ex)
            {
                Utility.Monitor.Log($"An error happened while loading FTM's Expanded Preconditions Utility interface. Any spawn areas with \"EPUPreconditions\" settings will be disabled. The auto-generated error message has been added to the log.", LogLevel.Warn);
                Utility.Monitor.Log($"----------", LogLevel.Trace);
                Utility.Monitor.Log($"{ex.ToString()}", LogLevel.Trace);
            }
        }
    }

    /// <summary>Expanded Preconditions Utility's API interface. Used to recognize & interact with the mod's API when available.</summary>
    /// <remarks>Comments within this API were provided within EPU's original code.</remarks>
    public interface IConditionsChecker
    {
        /// <summary>
        /// Must be called before any condition checking is done. Verbose mode will turn on logging for every step of the condition checking process
        /// </summary>
        /// <param name="verbose">Turning verbose mode true will log every step of the condition checking process. Useful for debugging but spams the debug log. It is recommended to have this false during release, or provided in a config set to a default of false.</param>
        /// <param name="uniqueId">The unique ID of your mod. Will be prepended to all logs so it is clear which mod called the condition checking</param>
        void Initialize(bool verbose, string uniqueId);

        /// <summary>
        /// Checks an array of condition strings. Each string will be evaluated as true if every single condition provided is true. All the strings together will evaluate as true if any string is true
        /// </summary>
        /// <param name="conditions">An array of condition strings.</param>
        /// <returns></returns>
        bool CheckConditions(string[] conditions);

        /// <summary>
        /// Checks a single condition string. The string will be evaluated as true if every single condition provided is true.
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        bool CheckConditions(string conditions);
    }
}

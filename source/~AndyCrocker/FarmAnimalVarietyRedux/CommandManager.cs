/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace FarmAnimalVarietyRedux
{
    /// <summary>Handles console commands.</summary>
    public static class CommandManager
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Logs the current animal state to the console.</summary>
        public static void LogSummary()
        {
            foreach (var animal in ModEntry.Instance.CustomAnimals)
            {
                ModEntry.Instance.Monitor.Log("Animal data:", LogLevel.Info);
                ModEntry.Instance.Monitor.Log(animal.ToString(), LogLevel.Info);

                ModEntry.Instance.Monitor.Log("Shop info:", LogLevel.Info);
                if (animal.AnimalShopInfo != null)
                    ModEntry.Instance.Monitor.Log(animal.AnimalShopInfo.ToString(), LogLevel.Info);

                foreach (var subtype in animal.Subtypes)
                {
                    ModEntry.Instance.Monitor.Log("Subtype data:", LogLevel.Info);
                    ModEntry.Instance.Monitor.Log(subtype.ToString(), LogLevel.Info);

                    ModEntry.Instance.Monitor.Log("Produce data:", LogLevel.Info);
                    foreach (var produce in subtype.Produce)
                        ModEntry.Instance.Monitor.Log(produce.ToString(), LogLevel.Info);
                }

                ModEntry.Instance.Monitor.Log("\n\n\n", LogLevel.Info);
            }
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/CustomNPCExclusions
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomNPCExclusions
{
    public static class DataHelper
    {
        /// <summary>The set of characters that may separate each "entry" in an NPC's exclusion data. Any number or combination of them is allowed between each asset.</summary>
        private static readonly char[] delimiters = new[] { ' ', ',', '/', '\\' };

        /// <summary>Initializes methods used to manage exclusion data.</summary>
        public static void Initialize(IModHelper helper)
        {
            helper.Events.Content.AssetRequested += Content_AssetRequested;
        }

        /// <summary>Loads the initial version of this mod's data asset.</summary>
        /// <remarks>This is a replacement for the IAssetLoader system, which was deprecated in SMAPI v3.14.</remarks>
        private static void Content_AssetRequested(object sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo(ModEntry.AssetName, false)) //if this is the exclusion data asset
            {
                e.LoadFrom(() => new Dictionary<string, string>(), StardewModdingAPI.Events.AssetLoadPriority.Medium, null); //load an empty asset
            }
        }

        /// <summary>Loads and parses the current exclusion data for all NPC names.</summary>
        /// <returns>A case-insensitive dictionary of NPC names (keys) and lists of exclusion data entries (values).</returns>
        public static Dictionary<string, List<string>> GetAllExclusions()
        {
            var rawExclusionData = ModEntry.Instance.Helper.GameContent.Load<Dictionary<string, string>>(ModEntry.AssetName); //load all exclusion data
            var parsedExclusions = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase); //create a case-insensitive dictionary of each NPC's exclusion rules

            foreach (var entry in rawExclusionData)
            {
                List<string> parsedValue = entry.Value.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList(); //get a parsed list of this NPC's data entries
                parsedExclusions.Add(entry.Key, parsedValue); //add this NPC's name (key) and parsed value to the parsed dictionary
            }

            return parsedExclusions;
        }

        /// <summary>Gets a set of NPC names with any of the given exclusion rules.</summary>
        /// <param name="exclusions">One or more exclusion rules. If an NPC's data contains any of these rules, that NPC's name will be in the returned set.</param>
        /// <returns>A set of NPC names with any of the given exclusion rules.</returns>
        public static HashSet<string> GetNPCsWithExclusions(params string[] exclusions)
        {
            var parsedExclusions = GetAllExclusions(); //load and parse all exclusion data
            var NPCsWithExclusions = new HashSet<string>(); //create a list of NPCs with the given exclusion rules

            foreach (var entry in parsedExclusions)
            {
                foreach (string activeRule in entry.Value)
                {
                    foreach (string ruleToFind in exclusions)
                    {
                        if (activeRule.Equals(ruleToFind, StringComparison.OrdinalIgnoreCase)) //if this NPC has an exclusion rule that matches one of the arguments
                        {
                            NPCsWithExclusions.Add(entry.Key); //add this NPC's name to the return list
                            break;
                        }
                    }
                }
            }

            return NPCsWithExclusions;
        }
    }
}

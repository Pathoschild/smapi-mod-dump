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
using StardewValley.GameData.Characters;
using System;
using System.Collections.Generic;

namespace CustomNPCExclusions
{
    /// <summary>A content editor that excludes designated NPCs from random town-related shop dialog.</summary>
    /// <remarks>
    /// As of Stardew v1.6, this exclusion rule is provided by the base game, and can be used without this mod.
    /// The exclusion rule is still available here for compatibility purposes, but now uses the base game's implementation.
    /// The base game's replacement feature is the "CanCommentOnPurchasedShopItems" field in the "Data/Characters" asset.
    /// </remarks>
    public static class ShopDialog
    {
        /// <summary>Initializes and enables this feature.</summary>
        public static void Enable()
        {
            ModEntry.Instance.Helper.Events.Content.AssetRequested += Content_AssetRequested;
        }

        /// <summary>Implements this feature by editing the relevant fields in Data/Characters.</summary>
        private static void Content_AssetRequested(object sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            try
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Data/Characters"))
                {
                    e.Edit(asset =>
                    {
                        var characterData = asset.AsDictionary<string, CharacterData>().Data;
                        var excludedNPCs = DataHelper.GetNPCsWithExclusions("All", "TownEvent", "ShopDialog"); //get all NPCs with the ShopDialog exclusion (or applicable categories)

                        List<string> excluded = new List<string>(); //a list of NPCs excluded here

                        foreach (string name in excludedNPCs) //for each NPC with this exclusion rule
                        {
                            if (characterData.ContainsKey(name)) //if the NPC exists in Data/Characters
                            {
                                characterData[name].CanCommentOnPurchasedShopItems = false; //exclude them from shop dialog
                                excluded.Add(name); //add their name to the excluded list
                            }
                        }

                        if (excluded.Count > 0 && ModEntry.Instance.Monitor.IsVerbose) //if any NPCs were excluded
                        {
                            string logMessage = string.Join(", ", excluded);
                            ModEntry.Instance.Monitor.Log($"Edited \"Data/Characters\" to exclude NPCs from shop dialog: {logMessage}", LogLevel.Trace);
                        }

                    }, StardewModdingAPI.Events.AssetEditPriority.Late); //apply after other mods' edits if possible
                }
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Exclusion rule \"ShopDialog\" encountered an error. Custom NPCs might not be properly excluded from talking about items in the town's shops. Full error message:\n{ex.ToString()}", LogLevel.Error);
            }
        }
    }
}
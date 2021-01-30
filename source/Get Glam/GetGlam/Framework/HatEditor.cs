/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MartyrPher/GetGlam
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;

namespace GetGlam.Framework
{
    /// <summary>
    /// Class that allows new hats to be edited so they ignore hair.
    /// </summary>
    public class HatEditor : IAssetEditor
    {
        // Instance of ModEntry
        private ModEntry Entry;

        /// <summary>
        /// HatEditor's Constructor.
        /// </summary>
        /// <param name="entry">The instance of ModEntry</param>
        public HatEditor(ModEntry entry)
        {
            Entry = entry;
        }

        /// <summary>
        /// Whether SMAPI can edit a specific asset.
        /// </summary>
        /// <typeparam name="T">The type of asset</typeparam>
        /// <param name="asset">The asset in question</param>
        /// <returns></returns>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/hats"))
                return true;

            return false;
        }

        /// <summary>
        /// When SMAPI tries to edit a specific asset.
        /// </summary>
        /// <typeparam name="T">The type of asset</typeparam>
        /// <param name="asset">The asset in question</param>
        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/hats"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                ChangeHatData(data);
            }
        }

        /// <summary>
        /// Changes second hat field to true.
        /// </summary>
        /// <param name="data">The dictionary of hat data.</param>
        private void ChangeHatData(IDictionary<int, string> data)
        {
            foreach (int item in data.Keys.ToList())
            {
                string[] fields = data[item].Split('/');
                fields[2] = "true";
                data[item] = string.Join("/", fields);
            }
        }
    }
}

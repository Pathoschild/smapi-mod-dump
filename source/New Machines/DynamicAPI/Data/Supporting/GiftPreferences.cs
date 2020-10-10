/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Data.Supporting
{
    public class GiftPreferences
    {
        #region	Constructors

        [JsonConstructor]
        public GiftPreferences() {}

        public GiftPreferences(CharacterName name)
        {
            Name = name;
        }

        #endregion

        #region	Properties

        [JsonProperty]
        public CharacterName Name { get; set; }

        [JsonProperty]
        public List<DynamicID<ItemID, CategoryID>> Loved { get; set; }

        [JsonProperty]
        public List<DynamicID<ItemID, CategoryID>> Liked { get; set; }

        [JsonProperty]
        public List<DynamicID<ItemID, CategoryID>> Neutral { get; set; }

        [JsonProperty]
        public List<DynamicID<ItemID, CategoryID>> Disliked { get; set; }

        [JsonProperty]
        public List<DynamicID<ItemID, CategoryID>> Hated { get; set; }

        #endregion

        #region	Auxiliary Methods

        public static List<DynamicID<ItemID, CategoryID>> PartToIDs(string part)
        {
            if (string.IsNullOrWhiteSpace(part)) return new List<DynamicID<ItemID, CategoryID>>();
            return part.Trim().Split(' ').Select(s => (DynamicID<ItemID, CategoryID>)int.Parse(s)).ToList();
        }

        #endregion
    }
}
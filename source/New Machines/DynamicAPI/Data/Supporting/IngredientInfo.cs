/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using Igorious.StardewValley.DynamicAPI.Constants;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Data.Supporting
{
    public sealed class IngredientInfo
    {
        #region	Properties

        [JsonProperty(Required = Required.Always)]
        public DynamicID<ItemID, CategoryID> ID { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int Count { get; set; }

        #endregion

        #region	Constructors

        [JsonConstructor]
        public IngredientInfo() {}

        public IngredientInfo(DynamicID<ItemID, CategoryID> id, int count)
        {
            ID = id;
            Count = count;
        }

        #endregion

        #region Serialization

        public override string ToString() => $"{ID} {Count}";

        #endregion
    }
}
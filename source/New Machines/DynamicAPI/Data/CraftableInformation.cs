/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.ComponentModel;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Newtonsoft.Json;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public class CraftableInformation : IRecipeInformation, IInformation
    {
        #region Properties

        [JsonProperty(Required = Required.Always)]
        public DynamicID<CraftableID> ID { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty]
        public int Price { get; set; }

        [JsonProperty, DefaultValue(Object.inedible)]
        public int Edibility { get; set; } = Object.inedible;

        [JsonProperty, DefaultValue("Crafting")]
        public string Type { get; set; } = "Crafting";

        [JsonProperty, DefaultValue(Object.BigCraftableCategory)]
        public int Category { get; set; } = Object.BigCraftableCategory;

        [JsonProperty(Required = Required.Always)]
        public string Description { get; set; }

        [JsonProperty, DefaultValue(true)]
        public bool CanSetOutdoor { get; set; } = true;

        [JsonProperty, DefaultValue(true)]
        public bool CanSetIndoor { get; set; } = true;

        [JsonProperty, DefaultValue(Object.fragility_Removable)]
        public int Fragility { get; set; } = Object.fragility_Removable;

        [JsonProperty, DefaultValue(1)]
        public int ResourceLength { get; set; } = 1;

        #endregion

        #region Serialization

        public override string ToString()
        {
            return $"{Name}/{Price}/{Edibility}/{Type} {Category}/{Description}/{CanSetOutdoor}/{CanSetIndoor}/{Fragility}";
        }

        #endregion

        #region Explicit Interface Implemetation

        int IInformation.ID => ID;

        #endregion
    }
}
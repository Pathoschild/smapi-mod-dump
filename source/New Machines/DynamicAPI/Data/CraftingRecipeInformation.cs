using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public class CraftingRecipeInformation : IRecipeInformation
    {
        #region Properties

        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<IngredientInfo> Materials { get; set; }

        [JsonProperty, DefaultValue("Home")]
        private string Area { get; set; } = "Home";

        [JsonProperty(Required = Required.Always)]
        public DynamicID<ItemID, CraftableID> ID { get; set; }

        [JsonProperty, DefaultValue(true)]
        public bool IsBig { get; set; } = true;

        [JsonProperty(Required = Required.Always)]
        public WayToGetCraftingRecipe WayToGet { get; set; }

        #endregion

        #region Serialization

        public static CraftingRecipeInformation Parse(string craftingRecipeInformation)
        {
            var parts = craftingRecipeInformation.Split('/');
            var info = new CraftingRecipeInformation();
            var materials = parts[0].Split(' ').Select(int.Parse).ToList();
            info.Materials = new List<IngredientInfo>();
            for (var i = 0; i < materials.Count; i += 2)
            {
                info.Materials.Add(new IngredientInfo(materials[i], materials[i + 1]));
            }
            info.Area = parts[1];
            info.ID = int.Parse(parts[2]);
            info.IsBig = bool.Parse(parts[3]);
            info.WayToGet = WayToGetCraftingRecipe.Parse(parts[4]);
            return info;
        }

        public override string ToString()
        {
            return $"{string.Join(" ", Materials)}/{Area}/{ID}/{IsBig.Serialize()}/{WayToGet}";
        }

        #endregion
    }
}
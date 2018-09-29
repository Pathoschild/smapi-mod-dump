using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Newtonsoft.Json;

namespace Igorious.StardewValley.NewMachinesMod.Data
{
    public sealed class WarpTotemInformation : ItemInformation
    {
        [JsonConstructor]
        public WarpTotemInformation() { }

        public WarpTotemInformation(DynamicID<ItemID> id, string name, string description) : base(id, name, description) { }

        [JsonProperty(Required = Required.Always)]
        public string WarpLocation { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Dictionary<DynamicID<ItemID>, int> Materials { get; set; }

        [JsonProperty]
        public Skill Skill { get; set; }

        [JsonProperty]
        public int? SkillLevel { get; set; }

        #region Contracts

        public static explicit operator CraftingRecipeInformation(WarpTotemInformation m)
        {
            return new CraftingRecipeInformation
            {
                ID = (int)m.ID,
                Name = m.Name,
                IsBig = false,
                Materials = m.Materials.Select(_ => new IngredientInfo((int)_.Key, _.Value)).ToList(),
                WayToGet = new WayToGetCraftingRecipe
                {
                    Skill = m.Skill,
                    SkillLevel = m.SkillLevel,
                    IsDefault = (m.SkillLevel == null),
                }
            };
        }

        #endregion
    }
}

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
using System.ComponentModel;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Newtonsoft.Json;

namespace Igorious.StardewValley.NewMachinesMod.Data
{
    public class MachineInformation : IDrawable
    {
        #region	Properties

        [JsonProperty(Required = Required.Always)]
        public DynamicID<CraftableID> ID { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Description { get; set; }

        [JsonProperty]
        public Skill Skill { get; set; }

        [JsonProperty]
        public int? SkillLevel { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Dictionary<DynamicID<ItemID>, int> Materials { get; set; }

        [JsonProperty(Required = Required.Always)]
        public MachineOutputInformation Output { get; set; }

        [JsonProperty]
        public MachineDraw Draw { get; set; }

        [JsonProperty]
        public int? ResourceIndex { get; set; }

        [JsonProperty, DefaultValue(1)]
        public int ResourceLength { get; set; } = 1;

        [JsonProperty]
        public List<DynamicID<CraftableID>> AllowedModules { get; set; }

        [JsonProperty]
        public List<DynamicID<CraftableID>> AllowedSections { get; set; }

        #endregion

        #region Contracts

        public static explicit operator CraftableInformation(MachineInformation m)
        {
            return new CraftableInformation
            {
                ID = m.ID,
                Name = m.Name,
                Description = m.Description,
                ResourceLength = m.ResourceLength,
            };
        }

        public static explicit operator CraftingRecipeInformation(MachineInformation m)
        {
            return new CraftingRecipeInformation
            {
                ID = (int)m.ID,
                Name = m.Name,
                IsBig = true,
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

        int IDrawable.TextureIndex => ID;

        int IDrawable.ResourceHeight => 1;
    }
}
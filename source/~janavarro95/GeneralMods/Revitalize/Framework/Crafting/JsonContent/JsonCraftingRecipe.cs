/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.Revitalize.Framework.Crafting.JsonContent
{
    /// <summary>
    /// A crafting recipe in json format.
    /// </summary>
    public class JsonCraftingRecipe
    {

        /// <summary>
        /// The id for the crafting recipe that is used to uniquely identify this crafting recipe when it is part of a crafting recipe book.
        /// </summary>
        public string craftingRecipeId;

        /// <summary>
        /// The name for the crafting recipe.
        /// </summary>
        public string outputName;

        /// <summary>
        /// The description for the crafting recipe.
        /// </summary>
        public string outputDescription;

        /// <summary>
        /// The number of in-game minutes it takes to craft this item.
        /// </summary>
        public int MinutesToCraft;

        public List<JsonCraftingComponent> inputs;
        public List<JsonCraftingComponent> outputs;

        /// <summary>
        /// The stats that this recipe costs. Magic, HP, stamina, gold, etc.
        /// </summary>
        public StatCost statCost;

        public JsonCraftingRecipe()
        {
            this.outputName = "";
            this.outputDescription = "";
            this.MinutesToCraft = 0;

            this.inputs = new List<JsonCraftingComponent>()
            {
                new JsonCraftingComponent()
            };
            this.outputs = new List<JsonCraftingComponent>() { new JsonCraftingComponent() };

            this.statCost = new StatCost();
        }

        public JsonCraftingRecipe(string outputName, string outputDescription, int MinutesToCraft, List<JsonCraftingComponent> inputs, List<JsonCraftingComponent> outputs, StatCost statCost)
        {
            this.outputName = outputName;
            this.outputDescription = outputDescription;
            this.MinutesToCraft = MinutesToCraft;

            this.inputs = inputs;
            this.outputs = outputs;

            this.statCost = statCost;
        }

        /// <summary>
        /// Creates a <see cref="Recipe"/> from this json crafting recipe file.
        /// </summary>
        /// <returns></returns>
        public virtual Recipe toCraftingRecipe()
        {

            return new Recipe(this);

            /*
            if (newOutputs.Count == 1 && string.IsNullOrEmpty(this.outputName) && string.IsNullOrEmpty(this.outputDescription))
            {
                return new Recipe(newInputs, newOutputs[0], this.statCost, this.MinutesToCraft);
            }

            return new Recipe(newInputs, newOutputs, this.outputName, this.outputDescription, null, this.statCost, this.MinutesToCraft);
            */
        }


    }
}

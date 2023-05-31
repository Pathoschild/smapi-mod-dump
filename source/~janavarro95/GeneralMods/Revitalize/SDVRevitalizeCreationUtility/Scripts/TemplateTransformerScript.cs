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
using Godot;

namespace SdvRevitalizeCreationUtility.Scripts
{
    public static class TemplateTransformerScript
    {
        /// <summary>
        /// Writes a display strings file to disk.
        /// </summary>
        /// <param name="OutputPath"></param>
        /// <param name="ObjectId"></param>
        /// <param name="DisplayName"></param>
        /// <param name="Description"></param>
        /// <param name="Category"></param>
        public static void WriteDisplayStringsFile(string OutputPath, string ObjectId ,string DisplayName, string Description, string Category)
        {
            string file = System.IO.File.ReadAllText(System.IO.Path.Combine(Game.GetGameDirectory(), "Templates", "DisplayStringTemplate.json"));
            file = Format(file, ObjectId, DisplayName, Description, Category);
            System.IO.File.WriteAllText(OutputPath, file);
        }

        /// <summary>
        /// Writes a crafting blueprint object file to disk.
        /// </summary>
        /// <param name="OutputPath"></param>
        /// <param name="ObjectId"></param>
        /// <param name="RecipesToLearn"></param>
        /// <param name="ItemToDraw"></param>
        public static void WriteCraftingBlueprintFile(string OutputPath, string ObjectId, string RecipesToLearn, string ItemToDraw)
        {
            string file = System.IO.File.ReadAllText(System.IO.Path.Combine(Game.GetGameDirectory(), "Templates", "CraftingBlueprintTemplate.json"));
            file = Format(file, ItemToDraw, RecipesToLearn, ObjectId);
            System.IO.File.WriteAllText(OutputPath, file);
        }

        /// <summary>
        /// Writes a recipe file to disk.
        /// </summary>
        /// <param name="OutputPath"></param>
        /// <param name="CraftingTabId"></param>
        /// <param name="CraftingRecipeId"></param>
        /// <param name="RecipeInputs"></param>
        /// <param name="RecipeOutputs"></param>
        public static void WriteRecipeFileForBlueprintObject(string OutputPath, string CraftingTabId, string CraftingRecipeId, string RecipeInputs, string RecipeOutputs)
        {
            string file = System.IO.File.ReadAllText(System.IO.Path.Combine(Game.GetGameDirectory(), "Templates", "RecipeTemplate.json"));
            file = Format(file, CraftingTabId, CraftingRecipeId, "0" ,RecipeInputs, RecipeOutputs );
            System.IO.File.WriteAllText(OutputPath, file);
        }

        /// <summary>
        /// Formats a given string read in from a template file, since for some reason string.format() always seems to crash when formatting a string read from a file.
        /// </summary>
        /// <param name="Original"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        public static string Format(string Original, params string[] Params)
        {
            string template = "";
            int counter = 0;
            string formatted = Original;

            foreach(string replacement in Params)
            {
                template = "{" + counter.ToString() + "}";
                formatted = formatted.Replace(template, replacement);
                counter++;
            }
            return formatted;
        }
    }
}

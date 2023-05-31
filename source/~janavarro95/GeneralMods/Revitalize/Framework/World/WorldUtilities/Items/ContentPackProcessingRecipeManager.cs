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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Omegasis.Revitalize.Framework.Constants.PathConstants.Data;
using Omegasis.Revitalize.Framework.ContentPacks;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.Objects.Machines.Misc;
using StardewModdingAPI.Utilities;

namespace Omegasis.Revitalize.Framework.World.WorldUtilities.Items
{
    public class ContentPackProcessingRecipeManager : ProcessingRecipeManager
    {
        public RevitalizeContentPack contentPack;

        public ContentPackProcessingRecipeManager()
        {


        }

        public ContentPackProcessingRecipeManager(RevitalizeContentPack contentPack)
        {
            this.contentPack = contentPack;
        }

        public override void loadRecipes()
        {
            //Load in general cases recipes.
            List<Dictionary<string, List<ProcessingRecipe>>> processingRecipes = this.loadProcessingRecipesFromJsonFiles();
            foreach (Dictionary<string, List<ProcessingRecipe>> objectIdToProcessingRecipesDict in processingRecipes)
            {

                foreach (KeyValuePair<string, List<ProcessingRecipe>> entry in objectIdToProcessingRecipesDict)
                {
                    foreach (ProcessingRecipe recipe in entry.Value)
                    {
                        this.addProcessingRecipe(entry.Key, recipe);
                        RevitalizeModCore.ModContentManager.objectProcessingRecipesManager.addProcessingRecipe(entry.Key, recipe);
                    }
                }
            }
        }

        public override List<ProcessingRecipe> getProcessingRecipesForObject(string Id)
        {
            if (this.processingRecipes.ContainsKey(Id))
            {
                return this.processingRecipes[Id];
            }
            else
            {
                return null;
            }
        }

        protected override List<Dictionary<string, List<ProcessingRecipe>>> loadProcessingRecipesFromJsonFiles()
        {
            if(!Directory.Exists(Path.Combine(this.contentPack.baseContentPack.DirectoryPath, ObjectsDataPaths.ProcessingRecipesPath)))
            {
                Directory.CreateDirectory(Path.Combine(this.contentPack.baseContentPack.DirectoryPath, ObjectsDataPaths.ProcessingRecipesPath));
            }

            return JsonUtilities.LoadJsonFilesFromDirectories<Dictionary<string, List<ProcessingRecipe>>>(this.contentPack, ObjectsDataPaths.ProcessingRecipesPath);
        }

    }
}

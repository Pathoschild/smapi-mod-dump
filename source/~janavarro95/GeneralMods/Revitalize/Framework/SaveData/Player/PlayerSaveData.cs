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
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.Revitalize.Framework.SaveData.Player
{
    public class PlayerSaveData:SaveDataBase
    {
        public Dictionary<string, string> unlockedCraftingRecipes;

        public PlayerSaveData()
        {
            this.unlockedCraftingRecipes = new Dictionary<string, string>();
        }

        public virtual void addUnlockedCraftingRecipe(string RecipeBookId, string RecipeId)
        {
            if (this.unlockedCraftingRecipes.ContainsKey(RecipeBookId)) return;
            this.unlockedCraftingRecipes.Add(RecipeBookId, RecipeId);
        }

        public override void save()
        {
            RevitalizeModCore.ModHelper.Data.WriteJsonFile(Path.Combine(RevitalizeModCore.SaveDataManager.getRelativeSaveDataPath(), "PlayerSaveData.json"), this);
        }

        public virtual void load()
        {
            foreach(KeyValuePair<string,string> recipe in this.unlockedCraftingRecipes)
            {
                RevitalizeModCore.ModContentManager.craftingManager.learnCraftingRecipe(recipe.Key, recipe.Value);
            }
        }

        public static PlayerSaveData LoadOrCreate()
        {
            if (File.Exists(Path.Combine(RevitalizeModCore.SaveDataManager.getFullSaveDataPath(), "PlayerSaveData.json")))
            {
                PlayerSaveData saveData = RevitalizeModCore.ModHelper.Data.ReadJsonFile<PlayerSaveData>(Path.Combine(RevitalizeModCore.SaveDataManager.getRelativeSaveDataPath(), "PlayerSaveData.json"));
                saveData.load();
                return saveData;
            }
            else
            {
                PlayerSaveData Config = new PlayerSaveData();
                RevitalizeModCore.ModHelper.Data.WriteJsonFile(Path.Combine(RevitalizeModCore.SaveDataManager.getRelativeSaveDataPath(), "PlayerSaveData.json"), Config);
                return Config;
            }
        }

    }
}

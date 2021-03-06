/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MineralCraftingRecipes
{
    class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            var craftingRecipesEditor = new CraftingRecipesEditor(this);
            Helper.Content.AssetEditors.Add(craftingRecipesEditor);
            craftingRecipesEditor.LearnRecipes();
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mrveress/SDVMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeedMachines.Framework.BigCraftables;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedMachines.Framework
{
    class DataLoader : IAssetEditor
    {
        public static IJsonAssetsAPI jsonAssetsAPI;
        public static bool isJsonAssetsLoaded;
        public static bool isSpaceCoreLoaded;

        public static IAssetData craftingRecipesAsset;
        public static IAssetData bigCraftableInformationsAsset;

        private bool craftablesTilesheetWasPatched = false;
        private bool dataAssetsWasPatched = false;

        public static Texture2D SeedMachinesSprite;

        public static Dictionary<string, object> AssetsToLoad = new Dictionary<string, object>();

        public DataLoader()
        {
            isJsonAssetsLoaded = ModEntry.modHelper.ModRegistry.IsLoaded("spacechase0.JsonAssets");
            isSpaceCoreLoaded = ModEntry.modHelper.ModRegistry.IsLoaded("spacechase0.SpaceCore");
            if (isJsonAssetsLoaded == true)
            {
                prepareJsonAssetsJSONs(ModEntry.settings.themeName);
                jsonAssetsAPI = ModEntry.modHelper.ModRegistry.GetApi<IJsonAssetsAPI>("spacechase0.JsonAssets");
                jsonAssetsAPI.LoadAssets(Path.Combine(ModEntry.modHelper.DirectoryPath, "assets", "SeedMachines" + ModEntry.settings.themeName + "JA"));
                prepareCorrectIDs();
            } else
            {
                SeedMachinesSprite = ModEntry.modHelper.Content.Load<Texture2D>("assets/SeedMachines" + ModEntry.settings.themeName + ".png");
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return 
                (
                    asset.AssetNameEquals("TileSheets\\Craftables") 
                    || asset.AssetNameEquals("Data\\BigCraftablesInformation") 
                    || asset.AssetNameEquals("Data\\CraftingRecipes")
                ) 
                && isJsonAssetsLoaded != true;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (isJsonAssetsLoaded == true) return;

            if (asset.AssetNameEquals("TileSheets\\Craftables"))
            {
                patchCraftablesTilesheetAsset(asset);
                checkAndPachDataAssets();
            }
            else if (asset.AssetNameEquals("Data\\BigCraftablesInformation"))
            {
                bigCraftableInformationsAsset = asset;
                checkAndPachDataAssets();
            }
            else if (asset.AssetNameEquals("Data\\CraftingRecipes"))
            {
                craftingRecipesAsset = asset;
                checkAndPachDataAssets();
            }
        }

        public void checkAndPachDataAssets()
        {
            if (this.craftablesTilesheetWasPatched && !this.dataAssetsWasPatched && bigCraftableInformationsAsset != null && craftingRecipesAsset != null)
            {
                IBigCraftableWrapper.addBigCraftablesInformations(bigCraftableInformationsAsset.AsDictionary<int, string>().Data);
                IBigCraftableWrapper.addCraftingRecipes(craftingRecipesAsset.AsDictionary<string, string>().Data);
                this.dataAssetsWasPatched = true;
            }
        }

        public void patchCraftablesTilesheetAsset(IAssetData craftableTilesheetAsset)
        {
            //Modifying default Craftables tilesheet of the game
            Texture2D CraftablesSheet = craftableTilesheetAsset.AsImage().Data;
            int originalWidth = CraftablesSheet.Width;
            int originalHeight = CraftablesSheet.Height;

            int startingId = (originalWidth / 16) * (originalHeight / 32);
            IBigCraftableWrapper.getWrapper("Seed Machine").itemID = startingId; //First free id for the mod injecting
            IBigCraftableWrapper.getWrapper("Seed Bandit").itemID = startingId + 6;

            //Getting original color array
            Color[] originalData = new Color[originalWidth * originalHeight];
            CraftablesSheet.GetData<Color>(originalData);

            //Getting mod's color array
            int customWidth = SeedMachinesSprite.Width;
            int customHeight = SeedMachinesSprite.Height;
            Color[] customData = new Color[customWidth * customHeight];
            SeedMachinesSprite.GetData<Color>(customData);

            //Preparing new clear tilesheet
            Texture2D newTileSheet = new Texture2D(Game1.game1.GraphicsDevice, originalWidth, originalHeight + customHeight, false, SurfaceFormat.Color);

            //Join default and custom color arrays
            var mixedData = new Color[originalData.Length + customData.Length];
            originalData.CopyTo(mixedData, 0);
            customData.CopyTo(mixedData, originalData.Length);

            //Push joined array of colors to prepared new tilesheet
            newTileSheet.SetData(0, new Rectangle(0, 0, originalWidth, originalHeight + customHeight), mixedData, 0, mixedData.Length);

            //Replace default tilesheet with joined one
            craftableTilesheetAsset.ReplaceWith(newTileSheet);

            this.craftablesTilesheetWasPatched = true;
        }

        public void prepareJsonAssetsJSONs(String themeName)
        {
            foreach(String wrapperName in IBigCraftableWrapper.getAllWrappers().Keys)
            {
                ModEntry.modHelper.Data.WriteJsonFile(
                    "assets/SeedMachines" + themeName + "JA/BigCraftables/" + wrapperName + "/big-craftable.json",
                    IBigCraftableWrapper.getWrapper(wrapperName).getJsonAssetsModel()
                );
            }
        }

        public void prepareCorrectIDs()
        {
            foreach (String wrapperName in IBigCraftableWrapper.getAllWrappers().Keys)
            {
                int bigCraftableId = jsonAssetsAPI.GetBigCraftableId(wrapperName);
                IBigCraftableWrapper.getWrapper(wrapperName).itemID = bigCraftableId;
            }
            
        }
    }
}

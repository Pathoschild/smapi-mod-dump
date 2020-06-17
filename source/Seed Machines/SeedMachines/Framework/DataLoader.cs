using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SeedMachines.Framework.BigCraftables;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeedMachines.Framework
{
    class DataLoader : IAssetEditor
    {
        public static IAssetData craftingRecipesAsset;
        public static Texture2D SeedMachinesSprite;

        public static Dictionary<string, object> AssetsToLoad = new Dictionary<string, object>();

        public DataLoader()
        {
            SeedMachinesSprite = ModEntry.modHelper.Content.Load<Texture2D>("assets/SeedMachines" + ModEntry.settings.themeName + ".png");
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return (asset.AssetNameEquals("TileSheets\\Craftables") || asset.AssetNameEquals("Data\\BigCraftablesInformation") || asset.AssetNameEquals("Data\\CraftingRecipes"));
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("TileSheets\\Craftables"))
            {
                //Modifying default Craftables tilesheet of the game

                Texture2D CraftablesSheet = asset.AsImage().Data;
                int originalWidth = CraftablesSheet.Width;
                int originalHeight = CraftablesSheet.Height;

                IBigCraftableWrapper.initialAbsoluteID = (originalWidth / 16) * (originalHeight / 32); //First free id for the mod injecting

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
                asset.ReplaceWith(newTileSheet);
            }
            else if (asset.AssetNameEquals("Data\\BigCraftablesInformation"))
            {
                IBigCraftableWrapper.addBigCraftablesInformations(asset.AsDictionary<int, string>().Data);
                IBigCraftableWrapper.addCraftingRecipes(craftingRecipesAsset.AsDictionary<string, string>().Data);
            }
            else if (asset.AssetNameEquals("Data\\CraftingRecipes"))
            {
                craftingRecipesAsset = asset;
            }
        }
    }
}

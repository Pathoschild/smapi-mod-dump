using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Framework;
using StardewModdingAPI.Events;
using xTile;
using xTile.Layers;
using xTile.Tiles;
using System.IO;
using BeyondTheValley.Framework;

namespace BeyondTheValley
{
    class ModEntry : Mod, IAssetLoader
    {
        public GameLocation Farm_Foraging = Game1.getLocationFromName("Farm_Foraging");

        public override void Entry(IModHelper helper)
        {
            /* Content Packs */
            foreach (IContentPack ContentPack in this.Helper.ContentPacks.GetOwned())
            {
                bool ContentFileExists = File.Exists(Path.Combine(ContentPack.DirectoryPath, "content.json"));

                ReplaceFileModel Pack = ContentPack.ReadJsonFile<ReplaceFileModel>("content.json");
                this.Monitor.Log("Reading: {ContentPack.Manifest.Name} {ContentPack.Manifest.Version} by {ContentPack.Manifest.Author} from {ContentPack.DirectoryPath} (ID: {ContentPack.Manifest.UniqueID})", LogLevel.Trace);

                if (!ContentFileExists)
                    this.Monitor.Log("{ContentPack.Manifest.Name}({ContentPack.Manifest.Version}) by {ContentPack.Manifest.Author} is missing a content.json file. Mod will be ignored", LogLevel.Warn);

                foreach (ReplaceFileModel replacement in Pack.ReplaceFiles)
                {
                    this.Monitor.Log($"Replacing {replacement.ReplaceFile} with {replacement.FromFile}");
                }
            }

            /* Helper Events */
            helper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Game1.player.mailReceived.Contains("ccVault"))
            {
                //----------Farm_Foraging--------------------//
                /// <summary> removes north fences on Forest Farm </summary>
                Layer Farm_Foraging_Front = Farm_Foraging.map.GetLayer("Buildings");
                TileSheet spring_outdoorsTileSheet = Farm_Foraging.map.GetTileSheet("untitled tile sheet");

                Farm_Foraging.removeTile(61, 50, "Front");
                Farm_Foraging.removeTile(62, 50, "Front");
                Farm_Foraging.removeTile(61, 51, "Buildings");
                Farm_Foraging.removeTile(62, 51, "Buildings"); 

                for (int TileY = 53; TileY < 90; TileY++)
                {
                    Farm_Foraging.removeTile(44, TileY, "Buildings");
                    Farm_Foraging.removeTile(44, TileY, "Front");
                }

                Farm_Foraging_Front.Tiles[44, 88] = new StaticTile(Farm_Foraging_Front, spring_outdoorsTileSheet, BlendMode.Alpha, tileIndex: 358);
                //-------------------------------------------//
            }
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            // Standard Farm/Farm
            if (asset.AssetNameEquals("Maps/Farm"))
                return true;

            // Forest Farm/Farm_Foraging
            else if (asset.AssetNameEquals("Maps/Farm_Foraging"))
                return true;

            // Cindersap Forest
            else
                return asset.AssetNameEquals("Maps/Forest");
        }

        public T Load<T>(IAssetInfo asset)
        {
            //Standard Farm/Farm
            if ()
                if (asset.AssetNameEquals("Maps/Farm"))
                    return this.Helper.Content.Load<T>("assets/Maps/FarmMaps/Farm.tbin");

            else
                return this.Helper.Content.Load<T>("assets/Maps/FarmMaps/Farm_Foraging.tbin");
        }
    }
}
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
using BeyondTheValley.Framework.Actions;
using StardewValley.Tools;

namespace BeyondTheValley
{
    class ModEntry : Mod, IAssetLoader
    {
        /* content pack replacement */
        /// <summary> content pack replaces Farm </summary>
        public bool replaceFarm = false;
        /// <summary> content pack replaces Farm </summary>
        public bool replaceFarm_Foraging = false;

        public GameLocation Farm_Foraging = Game1.getLocationFromName("Farm_Foraging");

        // other
        private CopperAxeDeletedTiles _copperAxeTiles;

        public override void Entry(IModHelper helper)
        {
            /* --------- Content Packs ------------ */
            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                bool contentFileExists = File.Exists(Path.Combine(contentPack.DirectoryPath, "content.json"));

                ContentPackModel cPack = contentPack.ReadJsonFile<ContentPackModel>("content.json");
                this.Monitor.Log($"Reading: {contentPack.Manifest.Name} {contentPack.Manifest.Version} by {contentPack.Manifest.Author} from {contentPack.DirectoryPath} (ID: {contentPack.Manifest.UniqueID})", LogLevel.Trace);

                if (!contentFileExists)
                    this.Monitor.Log($"{contentPack.Manifest.Name}({contentPack.Manifest.Version}) by {contentPack.Manifest.Author} is missing a content.json file. Mod will be ignored", LogLevel.Warn);

                foreach (ReplaceFileModel contentPackEdit in cPack.ReplaceFiles)
                {
                    this.Monitor.Log($"Replacing {contentPackEdit.ReplaceFile} with {contentPackEdit.FromFile}", LogLevel.Trace);

                    /* Check if content pack replaces one of the following files */
                    /// <summary> If content pack replaces Farm/Standard Farm </summary>
                    if (contentPackEdit.ReplaceFile == "assets/Maps/FarmMaps/Farm.tbin")
                        replaceFarm = true;
                    /// <summary> If content pack
                    if (contentPackEdit.ReplaceFile == "assets/Maps/FarmMaps/Farm_Foraging.tbin")
                        replaceFarm_Foraging = true;
                }
            }
            //--------------------------------------//

            /* Helper Events */
            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;
            helper.Events.Input.ButtonPressed += this.ButtonPressed;
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
            if (!replaceFarm && asset.AssetNameEquals("Maps/Farm"))
                return this.Helper.Content.Load<T>("assets/Maps/FarmMaps/Farm.tbin");

            else
                return this.Helper.Content.Load<T>("assets/Maps/FarmMaps/Farm_Foraging.tbin");
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (!replaceFarm_Foraging)
            {
                if (Game1.player.mailReceived.Contains("ccVault"))
                {
                    //--------------Farm_Foraging----------------//
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
        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            _copperAxeTiles = this.Helper.Data.ReadSaveData<CopperAxeDeletedTiles>("CopperAxe.DeletedTiles") ?? new CopperAxeDeletedTiles();

            if (_copperAxeTiles.inputArgs == null)
                return;

            foreach(string input in _copperAxeTiles.inputArgs)
            {
                string[] arg = input.Split(' ').ToArray();

                int tileX = int.Parse(arg[0]);
                int tileY = int.Parse(arg[1]);
                string strLayer = arg[2];
                string previousGameLocation = arg[3];

                Game1.getLocationFromName(previousGameLocation).removeTile(tileX, tileY, strLayer); 
            }
        }

        public void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            /********************** 
             **Custom Tile Actions 
             **********************/
            if (e.Button.IsActionButton())
            {
                // grabs player's cursor xy coords
                Vector2 tile = e.Cursor.GrabTile;

                string tileAction = Game1.player.currentLocation.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Action", "Buildings");

                if (tileAction != null)
                {
                    /* Action | CopperAxe (coordX) (coordY) (strLayer) */
                    /// <summary> If interacted with your Copper axe(+) equipped, it will remove the following tiles on that layer, separate with '/' delimiter </summary>
                    if (tileAction.StartsWith("CopperAxe "))
                    {
                        if (Game1.player.CurrentTool is Axe)
                        {
                            /* --- copper axe or better required --- */
                            if (Game1.player.CurrentTool.UpgradeLevel >= 1)
                            {
                                // skips first word (CopperAxe)
                                string arguments = String.Join(" ", tileAction.Split(' ').Skip(1));

                                foreach (string[] arg in arguments.Split('/').Select(item => item.Split(' ')))
                                {
                                    /// check if a parsing error happened
                                    bool parseError = false;

                                    int tileX = int.Parse(arg[0]); // get tile's X coordinate
                                    int tileY = int.Parse(arg[1]); // get tile's Y coordinate
                                    string strLayer = arg[2]; // get tile's layer
                                    string currentGameLocation = Game1.player.currentLocation.Name;

                                    // all possible values of {strLayer}
                                    string[] layerValues = new string[] { "Back", "Buildings", "Front", "AlwaysFront" };
                                    if (!layerValues.Contains(strLayer))
                                    {
                                        parseError = true;
                                        this.Monitor.Log($"The specified layer(\"{strLayer}\") for 'Action CopperAxe' is not valid. Eligible values: \"Back, Buildings, Front, AlwaysFront\". The TileAction will not work", LogLevel.Error);
                                    }

                                    // success state
                                    // only if no parsing error exists
                                    else if (!parseError)
                                    {
                                        Game1.player.currentLocation.removeTile(tileX, tileY, strLayer);

                                        this.Helper.Data.WriteSaveData("CopperAxe.DeletedTiles", _copperAxeTiles);
                                        _copperAxeTiles.inputArgs.Add(Convert.ToString(tileX) + " " + Convert.ToString(tileY) + " " + strLayer + " " + currentGameLocation);

                                        this.Monitor.Log($"Action CopperAxe, removed the tile on [{tileX}, {tileY}] from the {strLayer} Layer", LogLevel.Trace);
                                    }
                                }
                            }

                            // does not have copper axe or better
                            else
                                Game1.drawObjectDialogue("It seems like I'll need a better axe first");
                        }

                        // does not have axe equipped
                        else
                            Game1.drawObjectDialogue("It seems like I can interact with this if my axe is equipped");
                    }
                }
            }
        }
    }
}
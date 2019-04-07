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

        /* other */
        private bool tileRemoved;

        /// <summary> CopperAxeDeletedTiles model </summary>
        private SaveDeletedTilesModel _saveDeletedTiles;
        /// <summary> Retrieve multiplayer message of deleted tiles </summary>
        private List<string> mpInputArgs = new List<string>();

        private string[] layerValues =  { "Back", "Buildings", "Front", "AlwaysFront" };

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
            helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
            helper.Events.Multiplayer.ModMessageReceived += this.ModMessageReceived;
            helper.Events.Input.ButtonPressed += this.ButtonPressed;
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            // Standard Farm/Farm
            if (asset.AssetNameEquals(@"Maps/Farm"))
                return true;


            throw new FileNotFoundException();
        }

        public T Load<T>(IAssetInfo asset)
        {
            //Standard Farm/Farm
            if (!replaceFarm && asset.AssetNameEquals("Maps/Farm"))
                return this.Helper.Content.Load<T>("assets/Maps/FarmMaps/Farm.tbin");

            else if (!replaceFarm_Foraging && asset.AssetNameEquals("Maps/Farm_Foraging"))
                return this.Helper.Content.Load<T>("assets/Map/FarmMaps/Farm_Foraging.tbin");

            else
                return this.Helper.Content.Load<T>("assets/Maps/FarmMaps/Farm_Combat.tbin");
        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            _saveDeletedTiles = this.Helper.Data.ReadSaveData<SaveDeletedTilesModel>("CopperAxe.DeletedTiles") ?? new SaveDeletedTilesModel();

            // if there are any tiles needed to be deleted
            if (_saveDeletedTiles.inputArgs != null)
            {
                foreach (string input in _saveDeletedTiles.inputArgs)
                {
                    string[] arg = input.Split(' ').ToArray();

                    // parse all info to remove tile
                    int tileX = int.Parse(arg[0]);
                    int tileY = int.Parse(arg[1]);
                    string strLayer = arg[2];
                    string previousGameLocation = arg[3];

                    // remove tile
                    Game1.getLocationFromName(previousGameLocation).removeTile(tileX, tileY, strLayer);
                }
            }
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            /********************
            Mult*/
            if (tileRemoved == true)
            {
                foreach (string input in mpInputArgs)
                {
                    string[] arg = input.Split(' ').ToArray();

                    // parse all
                    int tileX = int.Parse(arg[0]);
                    int tileY = int.Parse(arg[1]);
                    string strLayer = arg[2];
                    string previousGameLocation = arg[3];

                    // remove tile
                    Game1.getLocationFromName(previousGameLocation).removeTile(tileX, tileY, strLayer);
                    this.Monitor.Log($"Action CopperAxe from host, removed the tile on [{tileX}, {tileY}] from the {strLayer} Layer", LogLevel.Trace);
                }

                tileRemoved = false;
            }
        }

        private void ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            // read list
            if (e.FromModID == "Jessebot.BeyondTheValley" && e.Type == "DeletedTiles")
            {
                mpInputArgs = e.ReadAs<List<string>>();
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

                                // perform deleting tiles
                                this.DeletedTilesAction(arguments);
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

                /* Action | IridiumAxe (coordX) (coordY) (strLayer) */
                /// <summary> If interacted with your Iridium axe(+) equipped, it will remove the following tiles on that layer, separate with '/' delimiter </summary>
                /// unfinished
                if (tileAction.StartsWith("IridiumAxe "))
                {
                    if (Game1.player.CurrentTool is Axe)
                    {
                        /* --- iridium axe or better required --- */
                        if (Game1.player.CurrentTool.UpgradeLevel >= 4)
                        {
                            // skips first word (IridiumAxe)
                            string arguments = String.Join(" ", tileAction.Split(' ').Skip(1));

                            // perform deleting tiles
                            this.DeletedTilesAction(arguments);
                        }

                        else
                            Game1.drawObjectDialogue("It seems like I'll need a better axe first");
                    }

                    // does not have axe equipped
                    else
                        Game1.drawObjectDialogue("It seems like I can interact with this if my axe is equipped");
                }
            }
        }

        private void DeletedTilesAction(string arguments)
        {
            foreach (string[] arg in arguments.Split('/').Select(item => item.Split(' ')))
            {
                // check if a parsing error happened
                bool parseError = false;

                if (!int.TryParse(arg[0], out int tileX)) // get tile's X coordinate
                {
                    parseError = true;
                    this.Monitor.Log("[Action CopperAxe] Error parsing first argument as an integer", LogLevel.Error);
                    continue;
                }

                if (!int.TryParse(arg[1], out int tileY)) // get tile's Y coordinate
                {
                    parseError = true;
                    this.Monitor.Log("[Action CopperAxe] Error parsing second argument as an integer", LogLevel.Error);
                    continue;
                }

                string strLayer = arg[2]; // get tile's layer

                string currentGameLocation = Game1.player.currentLocation.Name; // get current location's string

                //if specified layer does not exist
                if (!layerValues.Contains(strLayer))
                {
                    string value = string.Join(", ", layerValues);

                    parseError = true;
                    this.Monitor.Log($"The specified layer(\"{strLayer}\") for 'Action CopperAxe' is not valid. Eligible values: \"{layerValues}\". The TileAction will not work", LogLevel.Error);
                }

                // success state
                // only if no parsing error exists
                else if (!parseError)
                {
                    Game1.player.currentLocation.removeTile(tileX, tileY, strLayer);

                    // write deleted file data to save files
                    this.Helper.Data.WriteSaveData("CopperAxe.DeletedTiles", _saveDeletedTiles);
                    _saveDeletedTiles.inputArgs.Add(Convert.ToString(tileX) + " " + Convert.ToString(tileY) + " " + strLayer + " " + currentGameLocation);

                    // send multiplayer message
                    this.Helper.Multiplayer.SendMessage(_saveDeletedTiles.inputArgs, "DeletedTiles");

                    Game1.drawObjectDialogue("Success");
                    this.Monitor.Log($"Action CopperAxe, removed the tile on [{tileX}, {tileY}] from the {strLayer} Layer", LogLevel.Trace);

                    // check if tile was removed bool
                    tileRemoved = true;
                }
            }
        }
    }
}
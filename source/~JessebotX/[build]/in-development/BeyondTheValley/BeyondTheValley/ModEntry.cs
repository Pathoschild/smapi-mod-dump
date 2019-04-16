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
using StardewModdingAPI.Utilities;
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
        /*********
        ** Fields
        *********/
        public ITranslationHelper i18n;

        /* content pack replacement */
        /// <summary> content pack replaces Farm </summary>
        private bool replaceFarm = false;
        /// <summary> content pack replaces Farm </summary>
        private bool replaceFarm_Foraging = false;

        /* other */
        private bool tileRemoved;
        private bool axeNotEquipped;
        private bool axeUnderLeveled;

        /// <summary> All layer values accepted </summary>
        private string[] layerValues = { "Back", "Buildings", "Front", "AlwaysFront" };
        /// <summary> How many Content Packs are installed </summary>
        private int contentPacksInstalled;

        /// <summary> CopperAxeDeletedTiles model </summary>
        private SaveDeletedTilesModel _saveDeletedTiles;
        /// <summary> Retrieve multiplayer message of deleted tiles </summary>
        private List<string> mpInputArgs = new List<string>();

        /*********
        ** Entry
        *********/
        public override void Entry(IModHelper helper)
        {
            this.i18n = helper.Translation;

            /* other methods */
            ContentPackData();

            /* Helper Events */
            helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
            helper.Events.Multiplayer.ModMessageReceived += this.ModMessageReceived;
            helper.Events.Input.ButtonPressed += this.ButtonPressed;
        }

        /*********
        ** Content API crap
        *********/

        private void ContentPackData()
        {
            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                // bool for if content.json exists
                bool contentFileExists = File.Exists(Path.Combine(contentPack.DirectoryPath, "content.json"));

                //read content packs
                ContentPackModel pack = contentPack.ReadJsonFile<ContentPackModel>("content.json");
                this.Monitor.Log($"Reading: {contentPack.Manifest.Name} {contentPack.Manifest.Version} by {contentPack.Manifest.Author} from {contentPack.DirectoryPath} (ID: {contentPack.Manifest.UniqueID})", LogLevel.Trace);

                // if content.json does not exists
                if (!contentFileExists)
                    this.Monitor.Log($"{contentPack.Manifest.Name}({contentPack.Manifest.Version}) by {contentPack.Manifest.Author} is missing a content.json file. Mod will be ignored", LogLevel.Warn);

                // if content.json exists
                else if (contentFileExists)
                    contentPacksInstalled += 1;

                foreach (BVEEditModel edit in pack.ReplaceFiles)
                {
                    this.Monitor.Log($"Replacing {edit.ReplaceFile} with {edit.FromFile}", LogLevel.Trace);
                    /* Check if content pack replaces one of the following files */
                    /// <summary> 
                    /// If content pack replaces Farm/Standard Farm 
                    /// </summary>
                    if (edit.ReplaceFile == "assets/Maps/FarmMaps/Farm.tbin")
                    {
                        contentPack.LoadAsset<Map>(edit.FromFile);
                        replaceFarm = true;
                        continue;
                    }

                    /// <summary> 
                    /// If content pack replaces Farm_Combat/Wilderness Farm 
                    /// </summary>
                    if (edit.ReplaceFile == "assets/Maps/FarmMaps/Farm_Foraging.tbin")
                    {
                        contentPack.LoadAsset<Map>(edit.FromFile);
                        replaceFarm_Foraging = true;
                        continue;
                    }
                }
            }
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            // Standard Farm/Farm
            if (asset.AssetNameEquals("Maps/Farm"))
                return true;

            if (asset.AssetNameEquals("Maps/Farm_Combat"))
                return true;

            else
                return false;
        }

        public T Load<T>(IAssetInfo asset)
        {
            //Standard Farm/Farm
            if (!replaceFarm && asset.AssetNameEquals("Maps/Farm"))
                return this.Helper.Content.Load<T>("assets/Maps/FarmMaps/Farm.tbin");

            else
                return this.Helper.Content.Load<T>("assets/Maps/FarmMaps/Farm_Combat.tbin");
        }

        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            this.Monitor.Log($"{contentPacksInstalled} content packs installed for Beyond the Valley");

            _saveDeletedTiles = this.Helper.Data.ReadSaveData<SaveDeletedTilesModel>("DeletedTiles") ?? new SaveDeletedTilesModel();

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

            /* --- (Multiplayer) sync deleted tiles from tile actions --- */
            if (tileRemoved == true && !Context.IsMainPlayer)
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

        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // Custom \\
             // Tile \\
            // Actions \\
            if (e.Button.IsActionButton())
            {
                // grabs player's cursor xy coords
                Vector2 tile = e.Cursor.GrabTile;

                string tileAction = Game1.player.currentLocation.doesTileHaveProperty((int)tile.X, (int)tile.Y, "Action", "Buildings");

                if (tileAction != null)
                {
                    // --- General Tile Actions ---
                    // ----------------------------

                    /// <summary> 
                    /// Action | BVEMessage (strMessage)
                    /// If interacted, prints out a message from the i18n key 
                    /// </summary>
                    if (tileAction.StartsWith("BVEMessage "))
                    {
                        string[] input = tileAction.Split(' ').Skip(1).ToArray();

                        // get's strMessage
                        string strMessage = i18n.Get(input[0]);

                        //print's message out
                        Game1.drawObjectDialogue(strMessage);
                    }

                    // --- Delete Tiles Actions --- 
                    // ---------------------------- 

                    /// <summary>
                    /// Action | BVECopperAxe (coordX) (coordY) (strLayer)
                    /// If interacted with your Copper axe(+) equipped, it will remove the following tiles on that layer, separate with '/' delimiter 
                    /// </summary>
                    if (tileAction.StartsWith("BVECopperAxe "))
                    {
                        // process to get the first word in the current tile action
                        string[] fullString = tileAction.Split(' ').ToArray();
                        string currentAction = fullString[0];
                            
                        if (Game1.player.CurrentTool is Axe)
                        {
                            /* --- copper axe or better required --- */
                            if (Game1.player.CurrentTool.UpgradeLevel >= 1)
                            {
                                // skips first word (CopperAxe)
                                string arguments = string.Join(" ", tileAction.Split(' ').Skip(1));

                                // perform deleting tiles
                                DeletedTilesAction(arguments, currentAction);
                            }

                            // does not have copper axe or better
                            else
                            {
                                axeUnderLeveled = true;
                                FailedTileActionState();
                            }
                        }

                        // does not have axe equipped
                        else
                        {
                            axeNotEquipped = true;
                            FailedTileActionState();
                        }
                    }
                }

                /// <summary> 
                /// Action | BVESteel Axe (coordX) (coordY) (strLayer
                /// If interacted with your Steel axe(+) equipped, it will remove the following tiles on that layer, separate with '/' delimiter 
                /// </summary>
                if (tileAction.StartsWith("BVESteelAxe "))
                {
                    // process to get the first word in the current tile action
                    string[] fullString = tileAction.Split(' ').ToArray();
                    string currentAction = fullString[0];

                    if (Game1.player.CurrentTool is Axe)
                    {
                        /* --- steel axe or better required --- */
                        if (Game1.player.CurrentTool.UpgradeLevel >= 2)
                        {
                            // skips first word (SteelAxe)
                            string arguments = string.Join(" ", tileAction.Split(' ').Skip(1));

                            // perform deleting tiles
                            DeletedTilesAction(arguments, currentAction);
                        }

                        // does not have steel axe
                        else
                        {
                            axeUnderLeveled = true;
                            FailedTileActionState();
                        }
                    }

                    // does not have axe equipped
                    else
                    {
                        axeNotEquipped = true;
                        FailedTileActionState();
                    }
                }

                /// <summary> 
                /// Action | BVEGold Axe (coordX) (coordY) (strLayer)
                /// If interacted with your Gold axe(+) equipped, it will remove the following tiles on that layer, separate with '/' delimiter 
                /// </summary>
                if (tileAction.StartsWith("BVEGoldAxe "))
                {
                    // process to get the first word in the current tile action
                    string[] fullString = tileAction.Split(' ').ToArray();
                    string currentAction = fullString[0];

                    if (Game1.player.CurrentTool is Axe)
                    {
                        /* --- steel axe or better required --- */
                        if (Game1.player.CurrentTool.UpgradeLevel >= 2)
                        {
                            // skips first word (SteelAxe)
                            string arguments = string.Join(" ", tileAction.Split(' ').Skip(1));

                            // perform deleting tiles
                            DeletedTilesAction(arguments, currentAction);
                        }

                        // does not have steel axe
                        else
                        {
                            axeUnderLeveled = true;
                            FailedTileActionState();
                        }
                    }

                    // does not have axe equipped
                    else
                    {
                        axeNotEquipped = true;
                        FailedTileActionState();
                    }
                }

                /// <summary> 
                /// Action | BVEIridiumAxe (coordX) (coordY) (strLayer)
                /// If interacted with your Iridium axe(+) equipped, it will remove the following tiles on that layer, separate with '/' delimiter 
                /// </summary>
                if (tileAction.StartsWith("BVEIridiumAxe "))
                { 
                    // process to get the first word in the current tile action
                    string[] fullString = tileAction.Split(' ').ToArray();
                    string currentAction = fullString[0];

                    if (Game1.player.CurrentTool is Axe)
                    {
                        /* --- iridium axe or better required --- */
                        if (Game1.player.CurrentTool.UpgradeLevel >= 4)
                        {
                            // skips first word (IridiumAxe)
                            string arguments = String.Join(" ", tileAction.Split(' ').Skip(1));

                            // perform deleting tiles
                            DeletedTilesAction(arguments, currentAction);
                        }

                        // does not have iridium axe
                        else
                        {
                            axeUnderLeveled = true;
                            FailedTileActionState();
                        }
                    }

                    // does not have axe equipped
                    else
                    {
                        axeNotEquipped = true;
                        FailedTileActionState();
                    }
                }
            }
        }

        private void DeletedTilesAction(string arguments, string currentAction)
        {
            foreach (string[] arg in arguments.Split('/').Select(item => item.Split(' ')))
            {
                // check if a parsing error happened
                bool parseError = false;

                if (!int.TryParse(arg[0], out int tileX)) // get tile's X coordinate
                {
                    parseError = true;
                    this.Monitor.Log($"[Action {currentAction}]Error parsing first argument as an integer", LogLevel.Error);
                    continue;
                }

                if (!int.TryParse(arg[1], out int tileY)) // get tile's Y coordinate
                {
                    parseError = true;
                    this.Monitor.Log($"[Action {currentAction}]Error parsing second argument as an integer", LogLevel.Error);
                    continue;
                }

                string strLayer = arg[2]; // get tile's layer

                string currentGameLocation = Game1.player.currentLocation.Name; // get current location's string

                //if specified layer does not exist
                if (!layerValues.Contains(strLayer))
                {
                    string value = string.Join(", ", layerValues);

                    parseError = true;
                    this.Monitor.Log($"The specified layer(\"{strLayer}\") for a [Action {currentAction}] is not valid. Eligible values: \"{value}\". The TileAction will not work", LogLevel.Error);
                }

                // success state
                // only if no parsing error exists
                else if (!parseError)
                {
                    Game1.player.currentLocation.removeTile(tileX, tileY, strLayer);

                    // write deleted file data to save files
                    this.Helper.Data.WriteSaveData("DeletedTiles", _saveDeletedTiles);
                    _saveDeletedTiles.inputArgs.Add(Convert.ToString(tileX) + " " + Convert.ToString(tileY) + " " + strLayer + " " + currentGameLocation);

                    // send multiplayer message
                    this.Helper.Multiplayer.SendMessage(_saveDeletedTiles.inputArgs, "DeletedTiles");
                    Game1.drawObjectDialogue(i18n.Get("tileaction-success.1"));
                    this.Monitor.Log($"[Action {currentAction}] removed the tile on [{tileX}, {tileY}] from the {strLayer} Layer", LogLevel.Trace);

                    // check if tile was removed bool
                    tileRemoved = true;
                }
            }
        }

        private void FailedTileActionState()
        {
            /* --- Axe Deleted Tiles --- */
            /// <summary> Axe is not equipped </summary>
            if (axeNotEquipped)
            {
                Game1.drawObjectDialogue(i18n.Get("tileaction-axe.1"));
                axeNotEquipped = false;
            }

            /// <summary> Axe is under leveled/does not meet requirement </summary>
            if (axeUnderLeveled)
            {
                Game1.drawObjectDialogue(i18n.Get("tileaction-axe.2"));
                axeUnderLeveled = false;
            }
        } 
    }
}
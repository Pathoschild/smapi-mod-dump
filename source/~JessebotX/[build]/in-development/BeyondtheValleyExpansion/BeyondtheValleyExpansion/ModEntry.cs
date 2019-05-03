using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using StardewValley.TerrainFeatures;
using StardewModdingAPI;
using StardewModdingAPI.Framework;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using xTile;
using xTile.Layers;
using xTile.Tiles;
using BeyondTheValleyExpansion.Framework;
using BeyondTheValleyExpansion.Framework.Actions;
using BeyondTheValleyExpansion.Framework.ContentPacks;
using BeyondTheValleyExpansion.Framework.Farm;

namespace BeyondTheValleyExpansion
{
    class ModEntry : Mod, IAssetLoader, IAssetEditor
    {
        /*********
        ** Fields
        *********/
        /// <summary> instance of <see cref="TileActionFramework"/> class that contains the tile action code</summary>
        private TileActionFramework TileActionFramework;
        /// <summary> instance of <see cref="AvailableContentPackEdits"/> class that contains available assets to edit </summary>
        private AvailableContentPackEdits EditAsset = new AvailableContentPackEdits();
        /// <summary> instance of <see cref="TilesheetCompatibility"/> class that contains the tilesheet compatibility check </summary>
        private TilesheetCompatibility TilesheetCompat = new TilesheetCompatibility();

        /*********
        ** Entry
        *********/
        public override void Entry(IModHelper helper)
        {
            RefMod.ModHelper = helper;
            RefMod.ModMonitor = this.Monitor;
            RefMod.i18n = helper.Translation;

            // create instance of TileActionFramework class 
            TileActionFramework = new TileActionFramework(helper, Monitor);

            /* other methods */
            ContentPackData();

            /* Hook Events */
            helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
            helper.Events.Multiplayer.ModMessageReceived += this.ModMessageReceived;
            helper.Events.Input.ButtonPressed += this.ButtonPressed;
            helper.Events.GameLoop.DayStarted += this.DayStarted;

            /* Console Commands */
            helper.ConsoleCommands.Add("bve_purgesavedeletedtiles", "Removes the deleted tiles on a map from the Delete Tile Actions " +
                "\n\n Best used when you are changing maps mid save (and that map has the Delete Tile Actions)", this.ConsoleCommands_PurgeSaveDeletedTiles);
        }

        private void ContentPackData()
        {
            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                // bool for if content.json exists
                bool contentFileExists = File.Exists(Path.Combine(contentPack.DirectoryPath, "content.json"));

                // read content packs
                ContentPackModel pack = contentPack.ReadJsonFile<ContentPackModel>("content.json");
                this.Monitor.Log($"Reading: {contentPack.Manifest.Name} {contentPack.Manifest.Version} by {contentPack.Manifest.Author} from {contentPack.DirectoryPath} (ID: {contentPack.Manifest.UniqueID})", LogLevel.Trace);

                // if content.json does not exists
                if (!contentFileExists)
                    this.Monitor.Log($"{contentPack.Manifest.Name}({contentPack.Manifest.Version}) by {contentPack.Manifest.Author} is missing a content.json file. Mod will be ignored", LogLevel.Warn);

                // if content.json exists
                else if (contentFileExists)
                    RefMod.contentPacksInstalled += 1;

                foreach (BVEEditModel edit in pack.ReplaceFiles)
                {
                    this.Monitor.Log($"Replacing {edit.ReplaceFile} with {edit.FromFile}", LogLevel.Trace);

                    switch(edit.ReplaceFile)
                    {
                        // Standard Farm/Farm
                        case "assets/Maps/FarmMaps/Farm.tbin":
                            EditAsset.newFarm = contentPack.LoadAsset<Map>(edit.FromFile);
                            EditAsset.replaceFarm = true;
                            continue;
                        // Farm_Combat/Wilderness Farm
                        case "assets/Maps/FarmMaps/Farm_Combat.tbin":
                            EditAsset.newFarm_Combat = contentPack.LoadAsset<Map>(edit.FromFile);
                            EditAsset.replaceFarm_Combat = true;
                            continue;
                        // Farm_Foraging/Forest Farm
                        case "assets/Maps/FarmMaps/Farm_Foraging.tbin":
                            EditAsset.newFarm_Foraging = contentPack.LoadAsset<Map>(edit.FromFile);
                            EditAsset.replaceFarm_Foraging = true;
                            continue;

                        // ReplaceFile path does not exist or is not supported
                        default:
                            this.Monitor.Log(
                                $"[Content Pack:{contentPack.Manifest.Name} {contentPack.Manifest.Version}] Failed to replace \"{edit.ReplaceFile}\" because it does not exist and/or is not supported.", 
                                LogLevel.Error);
                            continue;
                    }
                }
            }
        }

        /*********
        ** Content API crap
        *********/

        /* AssetLoader */
        public bool CanLoad<T>(IAssetInfo asset)
        {
            // Standard Farm/Farm
            if (asset.AssetNameEquals("Maps/Farm"))
                return true;

            if (asset.AssetNameEquals("Maps/Farm_Foraging"))
                return true;

            if (asset.AssetNameEquals("Maps/Farm_Combat"))
                return true;

            else
                return false;
        }

        public T Load<T>(IAssetInfo asset)
        {
            // Standard Farm/Farm
            if (!EditAsset.replaceFarm && asset.AssetNameEquals("Maps/Farm"))
            {
                // apply custom tilesheet support
                Map map = this.Helper.Content.Load<Map>(RefFile.bveFarm);
                TilesheetCompat.TilesheetRecolours(map);
                return (T)(object)map;
            }
            else if (EditAsset.replaceFarm && asset.AssetNameEquals("Maps/Farm"))
            {
                TilesheetCompat.TilesheetRecolours(EditAsset.newFarm);
                return (T)(object)EditAsset.newFarm;
            }

            // Forest Farm/Farm_Foraging
            if (!EditAsset.replaceFarm_Foraging && asset.AssetNameEquals("Maps/Farm_Foraging"))
            {
                Map map = this.Helper.Content.Load<Map>(RefFile.bveFarm_Foraging);
                TilesheetCompat.TilesheetRecolours(map);
                return (T)(object)map;
            }
            else if (EditAsset.replaceFarm_Foraging && asset.AssetNameEquals("Maps/Farm_Foraging"))
            {
                TilesheetCompat.TilesheetRecolours(EditAsset.newFarm);
                return (T)(object)EditAsset.newFarm;
            }

            // Wilderness Farm/Farm_Combat
            if (!EditAsset.replaceFarm_Combat && asset.AssetNameEquals("Maps/Farm_Combat"))
            {
                Map map = this.Helper.Content.Load<Map>(RefFile.bveFarm_Combat);
                TilesheetCompat.TilesheetRecolours(map);
                return (T)(object)map;
            }
            else if (EditAsset.replaceFarm_Combat && asset.AssetNameEquals("Maps/Farm_Combat"))
            {
                TilesheetCompat.TilesheetRecolours(EditAsset.newFarm);
                return (T)(object)EditAsset.newFarm;
            }

            else
                throw new NotSupportedException($"Unexpected asset '{asset.AssetName}'.");
        }

        /* AssetEditor */
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/Crops"))
                return true;

            else
                return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/Crops"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;

                foreach (int itemID in data.Keys)
                {
                    string[] fields = data[itemID].Split('/').ToArray();
                    // TODO
                }
            }
        }

        // ---------------------------- \\

        /*********
         ** Helper Methods crap
         *********/ 
        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // content packs installed
            this.Monitor.Log($"{RefMod.contentPacksInstalled} content packs installed for Beyond the Valley");

            // Delete saved tiles if any
            TileActionFramework.SaveDeleteTilesAction();
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            /* --- (Multiplayer) sync deleted tiles from tile actions --- */
            if (TileActionFramework.tileRemoved == true && !Context.IsMainPlayer)
            {
                TileActionFramework.MultiplayerDeleteTilesAction();

                TileActionFramework.tileRemoved = false;
            }
        }

        private void ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            // read list
            if (e.FromModID == ModManifest.UniqueID && e.Type == "DeletedTiles")
            {
                TileActionFramework.mpInputArgs = e.ReadAs<List<string>>();
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
                    // --- General Tile Actions --- \\

                    // Action | BVEMessage (strMessage)
                    // If interacted, prints out a message from the i18n key 
                    if (tileAction.StartsWith("BVEMessage "))
                    {
                        string[] input = tileAction.Split(' ').Skip(1).ToArray();

                        //get i18n key
                        string strMessage = RefMod.i18n.Get(input[0]);

                        //print's message out
                        Game1.drawObjectDialogue(strMessage);
                    }

                    // Action | BVEAlchemy
                    // If interacted, stores item for magic
                    if (tileAction.Contains("BVEAlchemy"))
                    {
                        
                    }

                    // --- Delete Tiles Actions --- \\

                    /* pickaxe */
                    // Action | BVECopperPickaxe (coordX) (coordY) (strLayer)
                    // If interacted with your Copper pickaxe(+) equipped, it will remove the following tiles on that layer, separate with '/' delimiter 
                    if (tileAction.StartsWith("BVECopperPickaxe "))
                    {
                        // process to get the first word in the current tile action
                        string[] fullString = tileAction.Split(' ').ToArray();
                        string currentAction = fullString[0];
                        int toolUpgradeLevel = 1;

                        // calls PickaxeDeleteTilesAction in TileActionFramework
                        TileActionFramework.PickaxeDeleteTilesAction(tileAction, currentAction, toolUpgradeLevel);
                    }
                    // Action | BVESteelPickaxe (coordX) (coordY) (strLayer)
                    // If interacted with your Steel pickaxe(+) equipped, it will remove the following tiles on that layer, separate with '/' delimiter 
                    if (tileAction.StartsWith("BVESteelPickaxe "))
                    {
                        // process to get the first word in the current tile action
                        string[] fullString = tileAction.Split(' ').ToArray();
                        string currentAction = fullString[0];
                        int toolUpgradeLevel = 2;

                        // calls PickaxeDeleteTilesAction in TileActionFramework
                        TileActionFramework.PickaxeDeleteTilesAction(tileAction, currentAction, toolUpgradeLevel);
                    }
                    // Action | BVEGoldPickaxe (coordX) (coordY) (strLayer)
                    // If interacted with your Gold pickaxe(+) equipped, it will remove the following tiles on that layer, separate with '/' delimiter 
                    if (tileAction.StartsWith("BVEGoldPickaxe "))
                    {
                        // process to get the first word in the current tile action
                        string[] fullString = tileAction.Split(' ').ToArray();
                        string currentAction = fullString[0];
                        int toolUpgradeLevel = 3;

                        // calls PickaxeDeleteTilesAction in TileActionFramework
                        TileActionFramework.PickaxeDeleteTilesAction(tileAction, currentAction, toolUpgradeLevel);
                    }
                    // Action | BVEIridiumPickaxe (coordX) (coordY) (strLayer)
                    // If interacted with your Iridium pickaxe(+) equipped, it will remove the following tiles on that layer, separate with '/' delimiter 
                    if (tileAction.StartsWith("BVEIridiumPickaxe "))
                    {
                        // process to get the first word in the current tile action
                        string[] fullString = tileAction.Split(' ').ToArray();
                        string currentAction = fullString[0];
                        int toolUpgradeLevel = 4;

                        // Calls PickaxeDeleteTilesAction in TileActionFramework 
                        TileActionFramework.PickaxeDeleteTilesAction(tileAction, currentAction, toolUpgradeLevel);
                    }

                    // axe
                    // Action | BVECopperAxe (coordX) (coordY) (strLayer)
                    // If interacted with your Copper axe(+) equipped, it will remove the following tiles on that layer, separate with '/' delimiter 
                    if (tileAction.StartsWith("BVECopperAxe "))
                    {
                        // process to get the first word in the current tile action
                        string[] fullString = tileAction.Split(' ').ToArray();
                        string currentAction = fullString[0];
                        int toolUpgradeLevel = 1;

                        // Calls AxeDeleteTilesAction in TileActionFramework
                        TileActionFramework.AxeDeleteTilesAction(tileAction, currentAction, toolUpgradeLevel);
                    }
                    // Action | BVESteel Axe (coordX) (coordY) (strLayer
                    // If interacted with your Steel axe(+) equipped, it will remove the following tiles on that layer, separate with '/' delimiter 
                    if (tileAction.StartsWith("BVESteelAxe "))
                    {
                        // process to get the first word in the current tile action
                        string[] fullString = tileAction.Split(' ').ToArray();
                        string currentAction = fullString[0];
                        int toolUpgradeLevel = 2;

                        // Calls AxeDeleteTilesAction in TileActionFramework 
                        TileActionFramework.AxeDeleteTilesAction(tileAction, currentAction, toolUpgradeLevel);
                    }
                    // Action | BVEGold Axe (coordX) (coordY) (strLayer)
                    // If interacted with your Gold axe(+) equipped, it will remove the following tiles on that layer, separate with '/' delimiter 
                    if (tileAction.StartsWith("BVEGoldAxe "))
                    {
                        // process to get the first word in the current tile action
                        string[] fullString = tileAction.Split(' ').ToArray();
                        string currentAction = fullString[0];
                        int toolUpgradeLevel = 3;

                        // Calls AxeDeleteTilesAction in TileActionFramework
                        TileActionFramework.AxeDeleteTilesAction(tileAction, currentAction, toolUpgradeLevel);
                    }
                    // Action | BVEIridiumAxe (coordX) (coordY) (strLayer)
                    // If interacted with your Iridium axe(+) equipped, it will remove the following tiles on that layer, separate with '/' delimiter 
                    if (tileAction.StartsWith("BVEIridiumAxe "))
                    {
                        // process to get the first word in the current tile action
                        string[] fullString = tileAction.Split(' ').ToArray();
                        string currentAction = fullString[0];
                        int toolUpgradeLevel = 4;

                        // Calls AxeDeleteTilesAction in TileActionFramework
                        TileActionFramework.AxeDeleteTilesAction(tileAction, currentAction, toolUpgradeLevel);
                    }
                }
            }
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            // todo
        }

        /*********
         ** Console Commands crap
         *********/
        /// <summary> Command: 'bve_purgesavedeletedtiles' Removes the saves deleted tiles. </summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void ConsoleCommands_PurgeSaveDeletedTiles(string command, string[] args)
        {
            TileActionFramework.PurgeSaveDeletedTiles();
        }
    }
}
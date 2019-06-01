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
using BeyondTheValleyExpansion.Framework;
using BeyondTheValleyExpansion.Framework.Actions;
using BeyondTheValleyExpansion.Framework.Alchemy;
using StardewValley.Tools;
using StardewValley.Menus;

namespace BeyondTheValleyExpansion.Framework.Actions
{
    public class TileActionFramework
    {
        /*********
         ** Fields
         *********/
        /// <summary> check if a tile has been recently deleted from the custom tile actions </summary>
        public bool TileRemoved;
        /// <summary> check if axe is currently equipped </summary>
        public bool AxeNotEquipped;
        /// <summary> check if axe is under the minimum requirement </summary>
        public bool AxeUnderLeveled;
        /// <summary> check if pickaxe is currently equipped </summary>
        public bool PickaxeNotEquipped;
        /// <summary> check if pickaxe is under the minimum requirement </summary>
        public bool PickaxeUnderLeveled;

        /// <summary> references the <see cref="SaveDeletedTilesModel"/> class</summary>
        private SaveDeletedTilesModel SaveDeletedTiles;
        /// <summary> instance of <see cref="Alchemy"/> class that contains the alchemy framework </summary>
        private AlchemyFramework _Alchemy = new AlchemyFramework();

        /// <summary> Retrieve multiplayer message of deleted tiles </summary>
        public List<string> MPInputArgs = new List<string>();

        /// <summary> Entry point to Tile Action code. </summary>
        /// <param name="e"> The event data. </param>
        public void TileActions(ButtonPressedEventArgs e)
        {
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
                        this.Alchemy(tileAction);

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
                        this.PickaxeDeleteTilesAction(tileAction, currentAction, toolUpgradeLevel);
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
                        this.PickaxeDeleteTilesAction(tileAction, currentAction, toolUpgradeLevel);
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
                        this.PickaxeDeleteTilesAction(tileAction, currentAction, toolUpgradeLevel);
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
                        this.PickaxeDeleteTilesAction(tileAction, currentAction, toolUpgradeLevel);
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
                        this.AxeDeleteTilesAction(tileAction, currentAction, toolUpgradeLevel);
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
                        this.AxeDeleteTilesAction(tileAction, currentAction, toolUpgradeLevel);
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
                        this.AxeDeleteTilesAction(tileAction, currentAction, toolUpgradeLevel);
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
                        this.AxeDeleteTilesAction(tileAction, currentAction, toolUpgradeLevel);
                    }
                }
            }
        }
        /// <summary> first phase of triggering the Alchemy feature/options </summary>
        /// <param name="tileAction"> the custom tile action string </param>
        public void Alchemy(string tileAction)
        {
            RefMod.ModMonitor.Log($"{Game1.player.Name} interacted with [Action {tileAction}]", LogLevel.Trace);

            if (_Alchemy.UnlockedAlchemy && RefMod.Config.Alchemy.Enabled)
            {
                _Alchemy.AlchemyMenuOptions.Add(new Response("mix-ingredients", RefMod.i18n.Get("tileaction-alchemy.3")));
                _Alchemy.AlchemyMenuOptions.Add(new Response("remove-ingredients", RefMod.i18n.Get("tileaction-alchemy.4")));
                _Alchemy.AlchemyMenuOptions.Add(new Response("add-ingredients", RefMod.i18n.Get("tileaction-alchemy.5")));

                Game1.activeClickableMenu = new DialogueBox(RefMod.i18n.Get("tileaction-alchemy.1"), _Alchemy.AlchemyMenuOptions);
                Game1.player.currentLocation.afterQuestion = new GameLocation.afterQuestionBehavior((who, choice) => {
                    _Alchemy.Alchemy(who, choice);
                });
            }

            else
            {
                Game1.drawObjectDialogue(RefMod.i18n.Get("tileaction-alchemy.2"));
            }
        }

        /// <summary> trigger's the deleting tiles tileactions that require a pickaxe equipped </summary>
        /// <param name="tileAction"> the custom tile action string </param>
        /// <param name="currentAction"> the current action that evoked this method </param>
        /// <param name="toolUpgradeLevel"> the upgrade level of the current pickaxe </param>
        public void PickaxeDeleteTilesAction(string tileAction, string currentAction, int toolUpgradeLevel)
        {
            if (Game1.player.CurrentTool is Pickaxe)
            {
                /* --- copper pickaxe or better required --- */
                if (Game1.player.CurrentTool.UpgradeLevel >= toolUpgradeLevel)
                {
                    // skips first word
                    string arguments = string.Join(" ", tileAction.Split(' ').Skip(1));

                    // perform deleting tiles
                    DeleteTilesAction(arguments, currentAction);
                }

                // does not have copper pickaxe or better
                else
                {
                    PickaxeUnderLeveled = true;
                    FailedTileActionState();
                }
            }

            // does not have pickaxe equipped
            else
            {
                PickaxeNotEquipped = true;
                FailedTileActionState();
            }
        }

        /// <summary> trigger's the deleting tiles tileactions that require an axe equipped </summary>
        /// <param name="tileAction"> the custom tile action string </param>
        /// <param name="currentAction"> the current action that evoked this method </param>
        /// <param name="toolUpgradeLevel"> the upgrade level of the current axe </param>
        public void AxeDeleteTilesAction(string tileAction, string currentAction, int toolUpgradeLevel)
        {
            if (Game1.player.CurrentTool is Axe)
            {
                /* --- copper axe or better required --- */
                if (Game1.player.CurrentTool.UpgradeLevel >= toolUpgradeLevel)
                {
                    // skips first word
                    string arguments = string.Join(" ", tileAction.Split(' ').Skip(1));

                    // perform deleting tiles
                    DeleteTilesAction(arguments, currentAction);
                }

                // does not meet requirement
                else
                {
                    AxeUnderLeveled = true;
                    FailedTileActionState();
                }
            }

            // does not have axe equipped
            else
            {
                AxeNotEquipped = true;
                FailedTileActionState();
            }
        }

        /// <summary> performs the tile deleting and saves it in <see cref="SaveDeletedTilesModel.InputArgs"/> that gets written in save files </summary>
        /// <param name="arguments"> the arguments in the tile action string </param>
        /// <param name="currentAction"> the current action that evoked this method </param>
        private void DeleteTilesAction(string arguments, string currentAction)
        {
            foreach (string[] arg in arguments.Split('/').Select(item => item.Split(' ')))
            {
                // check if a parsing error happened
                bool parseError = false;

                if (!int.TryParse(arg[0], out int tileX)) // get tile's X coordinate
                {
                    parseError = true;
                    RefMod.ModMonitor.Log($"[Action {currentAction}]Error parsing first argument as an integer", LogLevel.Error);
                    continue;
                }

                if (!int.TryParse(arg[1], out int tileY)) // get tile's Y coordinate
                {
                    parseError = true;
                    RefMod.ModMonitor.Log($"[Action {currentAction}]Error parsing second argument as an integer", LogLevel.Error);
                    continue;
                }

                string strLayer = arg[2]; // get tile's layer

                string currentGameLocation = Game1.player.currentLocation.Name; // get current location's string

                //if specified layer does not exist
                if (!RefFarm.LayerValues.Contains(strLayer))
                {
                    string value = string.Join(", ", RefFarm.LayerValues);

                    parseError = true;
                    RefMod.ModMonitor.Log($"The specified layer(\"{strLayer}\") for a [Action {currentAction}] is not valid. Eligible values: \"{value}\". The TileAction will not work", LogLevel.Error);
                }

                // success state
                // only if no parsing error exists
                else if (!parseError)
                {
                    Game1.player.currentLocation.removeTile(tileX, tileY, strLayer);

                    // write deleted tile data to save files
                    RefMod.ModHelper.Data.WriteSaveData("DeletedTiles", SaveDeletedTiles);
                    SaveDeletedTiles.InputArgs.Add(Convert.ToString(tileX) + " " + Convert.ToString(tileY) + " " + strLayer + " " + currentGameLocation);

                    // send multiplayer message
                    RefMod.ModHelper.Multiplayer.SendMessage(SaveDeletedTiles.InputArgs, "DeletedTiles");
                    Game1.drawObjectDialogue(RefMod.i18n.Get("tileaction-success.1"));
                    RefMod.ModMonitor.Log($"[Action {currentAction}] removed the tile on [{tileX}, {tileY}] from the {strLayer} Layer", LogLevel.Trace);

                    // check if tile was removed bool
                    TileRemoved = true;
                }
            }
        }

        /// <summary> when the deleting tile's tile action that is interacted fails because it doesn't meet the requirements </summary>
        public void FailedTileActionState()
        {
            /* --- Axe Deleted Tiles --- */
            // Axe is not equipped
            if (AxeNotEquipped)
            {
                Game1.drawObjectDialogue(RefMod.i18n.Get("tileaction-axe.1"));
                AxeNotEquipped = false;
            }

            // Axe is under leveled/does not meet requirement 
            if (AxeUnderLeveled)
            {
                Game1.drawObjectDialogue(RefMod.i18n.Get("tileaction-axe.2"));
                AxeUnderLeveled = false;
            }

            // Pickaxe is not equipped
            if (PickaxeNotEquipped)
            {
                Game1.drawObjectDialogue(RefMod.i18n.Get("tileaction-pickaxe.1"));
                PickaxeNotEquipped = false;
            }

            // Pickaxe is under leveled/does not meet requirement
            if (PickaxeUnderLeveled)
            {
                Game1.drawObjectDialogue(RefMod.i18n.Get("tileaction-pickaxe.2"));
                PickaxeUnderLeveled = false;
            }
        }

        /// <summary> deletes the saved deleted tiles on the location from <see cref="SaveDeletedTilesModel.InputArgs"/></summary>
        public void SaveDeleteTilesAction()
        {
            SaveDeletedTiles = RefMod.ModHelper.Data.ReadSaveData<SaveDeletedTilesModel>("DeletedTiles") ?? new SaveDeletedTilesModel();

            // if there are any tiles needed to be deleted
            if (SaveDeletedTiles.InputArgs != null)
            {
                foreach (string input in SaveDeletedTiles.InputArgs)
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

        /// <summary> updates the deleted tiles when in multiplayer </summary>
        public void MultiplayerDeleteTilesAction()
        {
            if (!Context.IsPlayerFree)
                return;
            if (TileRemoved == true && !Context.IsMainPlayer)
            {
                foreach (string input in MPInputArgs)
                {
                    string[] arg = input.Split(' ').ToArray();

                    // parse all
                    int tileX = int.Parse(arg[0]);
                    int tileY = int.Parse(arg[1]);
                    string strLayer = arg[2];
                    string previousGameLocation = arg[3];

                    // remove tile
                    Game1.getLocationFromName(previousGameLocation).removeTile(tileX, tileY, strLayer);
                    RefMod.ModMonitor.Log($"Action CopperAxe from host, removed the tile on [{tileX}, {tileY}] from the {strLayer} Layer", LogLevel.Trace);
                }

                TileRemoved = false;
            }
        }

        /// <summary> console command 'bve_purgesavedeletedtiles' is used, clearing out <see cref="SaveDeletedTilesModel.InputArgs"/></summary>
        public void PurgeSaveDeletedTiles()
        {
            SaveDeletedTiles.InputArgs.Clear();
            RefMod.ModHelper.Data.WriteSaveData("DeletedTiles", SaveDeletedTiles);

            if (SaveDeletedTiles.InputArgs == null)
                RefMod.ModMonitor.Log("The contents in 'DeletedTiles' will be removed from your save file once you save the game. " +
                    "\n\n You will need to reload your save game after saving...", LogLevel.Debug);

            RefMod.ModHelper.Multiplayer.SendMessage(SaveDeletedTiles.InputArgs, "DeletedTiles");
            // update for multiplayer
            this.TileRemoved = true;
        }
    }
}

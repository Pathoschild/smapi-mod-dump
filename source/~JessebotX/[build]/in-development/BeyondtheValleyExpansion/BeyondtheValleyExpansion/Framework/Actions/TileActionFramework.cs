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
using StardewValley.Tools;

namespace BeyondTheValleyExpansion.Framework.Actions
{
    public class TileActionFramework
    {
        /*********
         ** Fields
         *********/
        /// <summary> provides simplified API's for writing mods </summary>
        internal IModHelper Helper;
        /// <summary> encapsulates monitoring and logging for a given module </summary>
        internal IMonitor Monitor;
        /// <summary> provides translations stored in the mods i18n folder </summary>
        internal ITranslationHelper i18n;

        /// <summary> check if a tile has been recently deleted from the custom tile actions </summary>
        public bool tileRemoved;
        /// <summary> check if axe is currently equipped </summary>
        public bool axeNotEquipped;
        /// <summary> check if axe is under the minimum requirement </summary>
        public bool axeUnderLeveled;
        /// <summary> check if pickaxe is currently equipped </summary>
        public bool pickaxeNotEquipped;
        /// <summary> check if pickaxe is under the minimum requirement </summary>
        public bool pickaxeUnderLeveled;

        /// <summary> references the <see cref="SaveDeletedTilesModel"/> class</summary>
        private SaveDeletedTilesModel _saveDeletedTiles;

        /// <summary> Retrieve multiplayer message of deleted tiles </summary>
        public List<string> mpInputArgs = new List<string>();

        /*********
         ** Constructor
         *********/
        /// <summary> constructor that allows <see cref="TileActionFramework"/> to access <seealso cref="IModHelper"/>, <seealso cref="IMonitor"/> and <seealso cref="ITranslationHelper"/> </summary>
        /// <param name="helper"> provides simplified API's for writing mods </param>
        /// <param name="monitor"> encapsulates monitoring and logging for a given module </param>
        public TileActionFramework(IModHelper helper, IMonitor monitor)
        {
            this.Helper = helper;
            this.Monitor = monitor;
            this.i18n = helper.Translation;
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
                    pickaxeUnderLeveled = true;
                    FailedTileActionState();
                }
            }

            // does not have pickaxe equipped
            else
            {
                pickaxeNotEquipped = true;
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

        /// <summary> performs the tile deleting and saves it in <see cref="SaveDeletedTilesModel.inputArgs"/> that gets written in save files </summary>
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
                if (!RefFarm.layerValues.Contains(strLayer))
                {
                    string value = string.Join(", ", RefFarm.layerValues);

                    parseError = true;
                    this.Monitor.Log($"The specified layer(\"{strLayer}\") for a [Action {currentAction}] is not valid. Eligible values: \"{value}\". The TileAction will not work", LogLevel.Error);
                }

                // success state
                // only if no parsing error exists
                else if (!parseError)
                {
                    Game1.player.currentLocation.removeTile(tileX, tileY, strLayer);

                    // write deleted tile data to save files
                    Helper.Data.WriteSaveData("DeletedTiles", _saveDeletedTiles);
                    _saveDeletedTiles.inputArgs.Add(Convert.ToString(tileX) + " " + Convert.ToString(tileY) + " " + strLayer + " " + currentGameLocation);

                    // send multiplayer message
                    Helper.Multiplayer.SendMessage(_saveDeletedTiles.inputArgs, "DeletedTiles");
                    Game1.drawObjectDialogue(i18n.Get("tileaction-success.1"));
                    this.Monitor.Log($"[Action {currentAction}] removed the tile on [{tileX}, {tileY}] from the {strLayer} Layer", LogLevel.Trace);

                    // check if tile was removed bool
                    tileRemoved = true;
                }
            }
        }

        /// <summary> when the deleting tile's tile action that is interacted fails because it doesn't meet the requirements </summary>
        public void FailedTileActionState()
        {
            /* --- Axe Deleted Tiles --- */
            // Axe is not equipped
            if (axeNotEquipped)
            {
                Game1.drawObjectDialogue(i18n.Get("tileaction-axe.1"));
                axeNotEquipped = false;
            }

            // Axe is under leveled/does not meet requirement 
            if (axeUnderLeveled)
            {
                Game1.drawObjectDialogue(i18n.Get("tileaction-axe.2"));
                axeUnderLeveled = false;
            }

            // Pickaxe is not equipped
            if (pickaxeNotEquipped)
            {
                Game1.drawObjectDialogue(i18n.Get("tileaction-pickaxe.1"));
                pickaxeNotEquipped = false;
            }

            // Pickaxe is under leveled/does not meet requirement
            if (pickaxeUnderLeveled)
            {
                Game1.drawObjectDialogue(i18n.Get("tileaction-pickaxe.2"));
                pickaxeUnderLeveled = false;
            }
        }

        /// <summary> deletes the saved deleted tiles on the location from <see cref="SaveDeletedTilesModel.inputArgs"/></summary>
        public void SaveDeleteTilesAction()
        {
            _saveDeletedTiles = Helper.Data.ReadSaveData<SaveDeletedTilesModel>("DeletedTiles") ?? new SaveDeletedTilesModel();

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

        /// <summary> updates the deleted tiles when in multiplayer </summary>
        public void MultiplayerDeleteTilesAction()
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
        }

        /// <summary> evokes when console command 'bve_purgesavedeletedtiles' is used, clearing out <see cref="SaveDeletedTilesModel.inputArgs"/></summary>
        public void PurgeSaveDeletedTiles()
        {
            _saveDeletedTiles.inputArgs.Clear();
            Helper.Data.WriteSaveData("DeletedTiles", _saveDeletedTiles);

            // update for multiplayer
            tileRemoved = true;
        }
    }
}

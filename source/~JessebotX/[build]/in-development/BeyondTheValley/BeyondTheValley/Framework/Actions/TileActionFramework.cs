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

namespace BeyondTheValley.Framework.Actions
{
    public class TileActionFramework
    {
        /*********
         ** Fields
         *********/
        internal IModHelper Helper;
        internal IMonitor Monitor;
        internal ITranslationHelper i18n;

        public bool tileRemoved;

        public bool axeNotEquipped;
        public bool axeUnderLeveled;

        public bool pickaxeNotEquipped;
        public bool pickaxeUnderLeveled;

        private SaveDeletedTilesModel _saveDeletedTiles;

        /// <summary> Retrieve multiplayer message of deleted tiles </summary>
        public List<string> mpInputArgs = new List<string>();

        /*********
         ** Constructor
         *********/
        public TileActionFramework(IModHelper helper, IMonitor monitor)
        {
            this.Helper = helper;
            this.Monitor = monitor;
            this.i18n = helper.Translation;
        }

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
                if (!Misc.layerValues.Contains(strLayer))
                {
                    string value = string.Join(", ", Misc.layerValues);

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

        public void FailedTileActionState()
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

            /// <summary> Pickaxe is not equipped</summary>
            if (pickaxeNotEquipped)
            {
                Game1.drawObjectDialogue(i18n.Get("tileaction-pickaxe.1"));
                pickaxeNotEquipped = false;
            }

            /// <summary> Pickaxe is under leveled/does not meet requirement </summary>
            if (pickaxeUnderLeveled)
            {
                Game1.drawObjectDialogue(i18n.Get("tileaction-pickaxe.2"));
                pickaxeUnderLeveled = false;
            }
        }

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

        public void PurgeSaveDeletedTiles()
        {
            _saveDeletedTiles.inputArgs.Clear();
            Helper.Data.WriteSaveData("DeletedTiles", _saveDeletedTiles);
        }
    }
}

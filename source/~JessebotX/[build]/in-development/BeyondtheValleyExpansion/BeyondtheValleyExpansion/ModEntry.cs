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
using BeyondTheValleyExpansion.Framework.Alchemy;

namespace BeyondTheValleyExpansion
{
    class ModEntry : Mod, IAssetLoader, IAssetEditor
    {
        /*********
        ** Fields
        *********/
        /// <summary> instance of <see cref="AlchemyFramework"/> class that contains the main alchemy code</summary>
        private AlchemyFramework _AlchemyFramework;
        /// <summary> instance of <see cref="AvailableEdits"/> class that contains available assets to edit </summary>
        private AvailableEdits _Edits;
        /// <summary> instance of <see cref="TileActionFramework"/> class that contains the tile action code</summary>
        private TileActionFramework _TileActionFramework;
        /// <summary> instance of <see cref="TilesheetCompatibility"/> class that contains the tilesheet compatibility check </summary>
        private TilesheetCompatibility _TilesheetCompat;

        /*********
        ** BeyondtheValleyAPI
        *********/
        public override object GetApi()
        {
            return new BeyondtheValleyAPI();
        }

        /*********
        ** Entry
        *********/
        /// <summary> The mod's entry point. </summary>
        /// <param name="helper"> Provides helper methods for mods. </param>
        public override void Entry(IModHelper helper)
        {
            RefMod.ModHelper = helper;
            RefMod.ModMonitor = this.Monitor;
            RefMod.i18n = helper.Translation;
            RefMod.Config = this.Helper.ReadConfig<ModConfig>();

            // use instances 
            _AlchemyFramework = new AlchemyFramework();
            _Edits = new AvailableEdits();
            _TileActionFramework = new TileActionFramework();
            _TilesheetCompat = new TilesheetCompatibility();

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
                "\n\n Best used when you are changing maps mid save (and that map has the Delete Tile Actions)", this.ConsoleCommands);
            helper.ConsoleCommands.Add("bve_alchemy", "Alchemy related commands \n\n" +
                "Usage: bve_alchemy <args> \n" +
                "<args>:\n" +
                "----------\n" +
                "trigger: unlock and trigger the alchemy menu \n\n" +
                "Example Usage:\n" +
                "bve_alchemy trigger", this.ConsoleCommands);
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
            if (!_Edits.ReplaceFarm && asset.AssetNameEquals("Maps/Farm"))
            {
                // apply custom tilesheet support
                Map map = this.Helper.Content.Load<Map>(RefFile.BveFarm);
                _TilesheetCompat.TilesheetRecolours(map);
                return (T)(object)map;
            }
            else if (_Edits.ReplaceFarm && asset.AssetNameEquals("Maps/Farm"))
            {
                _TilesheetCompat.TilesheetRecolours(_Edits.NewFarm);
                return (T)(object)_Edits.NewFarm;
            }

            // Forest Farm/Farm_Foraging
            if (!_Edits.ReplaceFarm_Foraging && asset.AssetNameEquals("Maps/Farm_Foraging"))
            {
                Map map = this.Helper.Content.Load<Map>(RefFile.BveFarm_Foraging);
                _TilesheetCompat.TilesheetRecolours(map);
                return (T)(object)map;
            }
            else if (_Edits.ReplaceFarm_Foraging && asset.AssetNameEquals("Maps/Farm_Foraging"))
            {
                _TilesheetCompat.TilesheetRecolours(_Edits.NewFarm);
                return (T)(object)_Edits.NewFarm;
            }

            // Wilderness Farm/Farm_Combat
            if (!_Edits.ReplaceFarm_Combat && asset.AssetNameEquals("Maps/Farm_Combat"))
            {
                Map map = this.Helper.Content.Load<Map>(RefFile.BveFarm_Combat);
                _TilesheetCompat.TilesheetRecolours(map);
                return (T)(object)map;
            }
            else if (_Edits.ReplaceFarm_Combat && asset.AssetNameEquals("Maps/Farm_Combat"))
            {
                _TilesheetCompat.TilesheetRecolours(_Edits.NewFarm);
                return (T)(object)_Edits.NewFarm;
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
        
        /// <summary> Handles content pack data. </summary>
        private void ContentPackData()
        {
            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                // bool for if content.json exists
                bool contentFileExists = File.Exists(Path.Combine(contentPack.DirectoryPath, "content.json"));

                // read content packs
                ContentPackModel pack = contentPack.ReadJsonFile<ContentPackModel>("content.json");
                this.Monitor.Log(
                    $"Reading: {contentPack.Manifest.Name} {contentPack.Manifest.Version} by {contentPack.Manifest.Author} from {contentPack.DirectoryPath} (ID: {contentPack.Manifest.UniqueID})", 
                    LogLevel.Trace
                );

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
                            _Edits.NewFarm = contentPack.LoadAsset<Map>(edit.FromFile);
                            _Edits.ReplaceFarm = true;
                            continue;
                        // Farm_Combat/Wilderness Farm
                        case "assets/Maps/FarmMaps/Farm_Combat.tbin":
                            _Edits.NewFarm_Combat = contentPack.LoadAsset<Map>(edit.FromFile);
                            _Edits.ReplaceFarm_Combat = true;
                            continue;
                        // Farm_Foraging/Forest Farm
                        case "assets/Maps/FarmMaps/Farm_Foraging.tbin":
                            _Edits.NewFarm_Foraging = contentPack.LoadAsset<Map>(edit.FromFile);
                            _Edits.ReplaceFarm_Foraging = true;
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
         ** Helper Methods crap
         *********/ 
        private void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // content packs installed
            this.Monitor.Log($"{RefMod.contentPacksInstalled} content packs installed for Beyond the Valley");

            // Delete saved tiles if any
            _TileActionFramework.SaveDeleteTilesAction();
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            _TileActionFramework.MultiplayerDeleteTilesAction();            
        }

        private void ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            // read list
            if (e.FromModID == this.ModManifest.UniqueID && e.Type == "DeletedTiles")
            {
                _TileActionFramework.MPInputArgs = e.ReadAs<List<string>>();
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            // begin tile action code
            _TileActionFramework.TileActions(e);
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            // todo
        }

        /// <summary> Beyond the Valley Expansion's console commands. </summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void ConsoleCommands(string command, string[] args)
        {
            if (command == "bve_purgesavedeletedtiles")
            {
                _TileActionFramework.PurgeSaveDeletedTiles();
            }

            if (command == "bve_alchemy")
            {
                if (args[0] == "trigger")
                {
                    _AlchemyFramework.UnlockedAlchemy = true;
                    _TileActionFramework.Alchemy("'bve_alchemy trigger' (console command)");
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using WindowsInput;
using Microsoft.Xna.Framework;
using StarAI.PathFindingCore;
using System.IO;
using StardustCore;

namespace StarAI
{
    //TODO: Clean up initial code
    //Make sure pathos doesn't update this once since it's a homework assignment. Sorry Pathos!
    //Work on dijakstra's algorithm for path finding on this one? Make sure obstacles are included.
    //Question how this is all going to work.
    public class ModCore : Mod
    {
        public static StardewModdingAPI.IMonitor CoreMonitor;
        public static StardewModdingAPI.IModHelper CoreHelper;
        public static List<Warp> warpGoals = new List<Warp>();
        public static object[] obj = new object[3];

        public override void Entry(IModHelper helper)
        {
            obj[0] = PathFindingLogic.source;
            obj[1] = PathFindingLogic.currentGoal;
            obj[2] = PathFindingLogic.queue;
            CoreHelper = helper;

            // string[] s = new string[10];

            CoreMonitor = this.Monitor;
            CoreMonitor.Log("Hello AI WORLD!", LogLevel.Info);
            Commands.initializeCommands();
            PathFindingCore.Utilities.initializeTileExceptionList();
            ExecutionCore.TaskMetaDataHeuristics.initializeToolCostDictionary();
            //throw new NotImplementedException();
            StardewModdingAPI.Events.LocationEvents.CurrentLocationChanged += LocationEvents_CurrentLocationChanged;

            StardewModdingAPI.Events.ControlEvents.KeyPressed += ControlEvents_KeyPressed;
            StardewModdingAPI.Events.SaveEvents.AfterLoad += SaveEvents_AfterLoad;
           // StardewModdingAPI.Events.GraphicsEvents.OnPreRenderEvent += PathFindingCore.Utilities.addFromPlacementListBeforeDraw;
           

            StardustCore.ModCore.SerializationManager.acceptedTypes.Add("StarAI.PathFindingCore.TileNode", new StardustCore.Serialization.SerializerDataNode(new StardustCore.Serialization.SerializerDataNode.SerializingFunction(StarAI.PathFindingCore.TileNode.Serialize), new StardustCore.Serialization.SerializerDataNode.ParsingFunction(StarAI.PathFindingCore.TileNode.ParseIntoInventory), new StardustCore.Serialization.SerializerDataNode.WorldParsingFunction(StarAI.PathFindingCore.TileNode.SerializeFromWorld), new StardustCore.Serialization.SerializerDataNode.SerializingToContainerFunction(StarAI.PathFindingCore.TileNode.Serialize)));
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            loadExceptionTiles();
        }

        public void loadExceptionTiles()
        {
            if (!Directory.Exists(Path.Combine(CoreHelper.DirectoryPath, PathFindingCore.Utilities.folderForExceptionTiles)))
            {
                Directory.CreateDirectory(Path.Combine(CoreHelper.DirectoryPath, PathFindingCore.Utilities.folderForExceptionTiles));
            }
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(Path.Combine(CoreHelper.DirectoryPath,PathFindingCore.Utilities.folderForExceptionTiles));
            foreach (string fileName in fileEntries)
            {
                try
                {
                    TileExceptionNode t = TileExceptionNode.parseJson(fileName);
                    PathFindingCore.Utilities.ignoreCheckTiles.Add(t);
                }
                catch(Exception err)
                {
                    ModCore.CoreMonitor.Log(err.ToString(), LogLevel.Error);
                }
            }
        
        }

        private void ControlEvents_KeyPressed(object sender, StardewModdingAPI.Events.EventArgsKeyPressed e)
        {
            //J key for shop
            #region
            if (e.KeyPressed == Microsoft.Xna.Framework.Input.Keys.J)
            {
                CoreMonitor.Log("OK THE J KEY WAS PRESSED!");
                List<Item> shoppingList = new List<Item>();
                StarAI.PathFindingCore.TileNode t = new StarAI.PathFindingCore.TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.ColorsList.Aqua));
                if (t == null)
                {
                    CoreMonitor.Log("WTF?????");
                }
                try
                {
                    if (t == null)
                    {
                        return;
                    }
                    shoppingList.Add((Item)t);
                    Game1.activeClickableMenu = new StardewValley.Menus.ShopMenu(shoppingList);
                }
                catch (Exception err)
                {
                    CoreMonitor.Log(Convert.ToString(err));
                }
            }
            #endregion

            //K key for placing a tile.
            #region
            if (e.KeyPressed == Microsoft.Xna.Framework.Input.Keys.K)
            {
                CoreMonitor.Log("OK THE K KEY WAS PRESSED!");

                StarAI.PathFindingCore.TileNode t = new StarAI.PathFindingCore.TileNode(1, Vector2.Zero, Path.Combine("Tiles", "GenericUncoloredTile.xnb"), Path.Combine("Tiles", "TileData.xnb"), StardustCore.IlluminateFramework.Colors.randomColor());
                if (t == null)
                {
                    CoreMonitor.Log("WTF?????");
                }
                try
                {
                    if (t == null)
                    {
                        return;
                    }
                    CoreMonitor.Log(new Vector2(Game1.player.getTileX() * Game1.tileSize, Game1.player.getTileY() * Game1.tileSize).ToString());

                    int xPos = (int)(Game1.player.getTileX()) * Game1.tileSize;
                    int yPos = (int)(Game1.player.getTileY()) * Game1.tileSize;
                    Rectangle r = new Rectangle(xPos, yPos, Game1.tileSize, Game1.tileSize);
                    Vector2 pos = new Vector2(r.X, r.Y);
                    bool ok = StarAI.PathFindingCore.TileNode.checkIfICanPlaceHere(t, pos, Game1.player.currentLocation);
                    if (ok == false) return;
                    t.placementAction(Game1.currentLocation, Game1.player.getTileX() * Game1.tileSize, Game1.player.getTileY() * Game1.tileSize);
                    //t.setAdjacentTiles(true);
                }
                catch (Exception err)
                {
                    CoreMonitor.Log(Convert.ToString(err));
                }
            }
            #endregion

            if (e.KeyPressed == Microsoft.Xna.Framework.Input.Keys.U)
            {
                ExecutionCore.TaskList.printAllTaskMetaData();
            }

                if (e.KeyPressed == Microsoft.Xna.Framework.Input.Keys.O)
            {

                foreach (var v in Game1.player.currentLocation.map.TileSheets)
                {
                    foreach (var q in Game1.player.currentLocation.map.Layers)
                    {
                        string[] s = q.ToString().Split(':');
                        string layer = s[1].Trim();
                        if (Game1.player.currentLocation.map.GetLayer(layer) == null)
                        {

                            ModCore.CoreMonitor.Log("SHITTTTTT: " + layer, LogLevel.Error);
                        }
                        int tileIndex = Game1.player.currentLocation.getTileIndexAt((int)Game1.player.getTileX() / Game1.tileSize, (int)Game1.player.getTileY() / Game1.tileSize, layer);
                        if (tileIndex == -1) continue;
                        //ModCore.CoreMonitor.Log("Position: " + (Game1.player.getTileLocation() / Game1.tileSize).ToString(), LogLevel.Warn);
                        //ModCore.CoreMonitor.Log("Layer: " + layer, LogLevel.Warn);
                        //ModCore.CoreMonitor.Log("Index: " + tileIndex.ToString(), LogLevel.Warn);
                        //ModCore.CoreMonitor.Log("Image Source: " + v.ImageSource, LogLevel.Warn);

                        if (layer == "Buildings")
                        {
                            TileExceptionNode tileException = new TileExceptionNode(v.ImageSource, tileIndex);
                            foreach(var tile in PathFindingCore.Utilities.ignoreCheckTiles)
                            {
                                if (tile.imageSource == tileException.imageSource && tile.index == tileException.index)
                                {
                                    ModCore.CoreMonitor.Log("Tile exception already initialized!");
                                    return; //tile is already initialized.
                                }
                            }
                            PathFindingCore.Utilities.ignoreCheckTiles.Add(tileException);
                            tileException.serializeJson(Path.Combine(ModCore.CoreHelper.DirectoryPath, PathFindingCore.Utilities.folderForExceptionTiles));
                            //StardustCore.ModCore.SerializationManager.
                        }
                    }
                }
            }
        }



     
        private void LocationEvents_CurrentLocationChanged(object sender, StardewModdingAPI.Events.EventArgsCurrentLocationChanged e)
        {
            CoreMonitor.Log("LOCATION CHANGED!");
            CoreMonitor.Log(Game1.player.currentLocation.name);
            foreach (var v in Game1.player.currentLocation.warps)
            {
                string s = "X: " + Convert.ToString(v.X) + " Y: " + Convert.ToString(v.Y) + " TargetX: " + Convert.ToString(v.TargetX) + " TargetY: " + Convert.ToString(v.TargetY) + " TargetLocationName: " + Convert.ToString(v.TargetName);
                CoreMonitor.Log(s);
                //warpGoals.Add(v); Disabled for now
            }
            //GameLocation loc=Game1.getLocationFromName("location name")
            //
        }
    }
}





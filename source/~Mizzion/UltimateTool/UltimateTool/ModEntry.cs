using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using UltimateTool.Framework;
using UltimateTool.Framework.Tools;
using UltimateTool.Framework.Configuration;
using MyStardewMods.Common;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;


namespace UltimateTool
{
    internal class ModEntry : Mod
    {
        private ModConfig _config;
        
        private ITool[] _tools;

        private SButton _actionKey;
       private bool _showGrid = false;

        //Player Data
        private PlayerData PlayerData;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();
            DoTools();
            UBush.helper = helper;

            //Events
            var events = helper.Events;
            events.GameLoop.SaveLoaded += OnSaveLoaded;
            events.GameLoop.Saving += OnSaving;
            events.Input.ButtonPressed += OnButtonPressed;
            events.Display.Rendered += OnRendered;
            //events.Input.MouseWheelScrolled += OnMouseScrolled;
            //events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnRendered(object sender, RenderedEventArgs e)
        {
            if(Context.IsWorldReady && Game1.activeClickableMenu == null)
            {
                HighlightRadius(Game1.spriteBatch);
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if(!Enum.TryParse<SButton>(_config.ActionKey, true, out _actionKey))
            {
                _actionKey = SButton.Z;
            }
            Game1.player.MagneticRadius = Game1.tileSize * _config.MagnetRadius;

            //Load or create PlayerData
            this.PlayerData = Helper.Data.ReadSaveData<PlayerData>("UltimateData") ??
                              new PlayerData();
        }

        private void OnSaving(object sender, SavingEventArgs e)
        {
            if (Context.IsMainPlayer && this.PlayerData != null)
                this.Helper.Data.WriteSaveData("data", this.PlayerData);
        }
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if(e.Button == _actionKey)
            {
                DoAction();
            }
            if (e.Button == SButton.G)
            {
                if(_config.ShowGrid)
                    _showGrid = !_showGrid;
            }
            if(e.Button == SButton.F5)
            {
                _config = Helper.ReadConfig<ModConfig>();
                DoTools();
            }
        }

        private void DoTools()
        {            
            ToolConfig toolConfig = _config.Tools;
            _tools = new ITool[]
            {
                new AxeTool(toolConfig.Axe),
                new FertilizerTool(toolConfig.Fertilizer),
                new GrassStarterTool(toolConfig.GrassStarter),
                new HoeTool(toolConfig.Hoe),
                new PickaxeTool(toolConfig.Pickaxe),
                new ScytheTool(toolConfig.Scythe),
                new SeedTool(toolConfig.Seeds),
                new WateringCanTool(toolConfig.WateringCan),
                new ShearTool(), 
                new MilkPailTool()

            };
        }

        private void DoMine()
        {
            SFarmer player = Game1.player;
            GameLocation location = Game1.currentLocation;
            Tool tool = player.CurrentTool;
            Item item = player.CurrentItem;
            Dictionary<Vector2, SObject> curObj = new Dictionary<Vector2, SObject>();
            Dictionary<Vector2, TerrainFeature> curTerrain = new Dictionary<Vector2, TerrainFeature>();

            ITool[] tools = _tools
                .Where(tools1 => tools1.IsEnabled(player, tool, item, location))
                .ToArray();
            if (!tools.Any())
                return;

            if (location.IsFarm || location.Name.Contains("Greenhouse") || location.Name.Contains("FarmExpan"))
                return;

            ICursorPosition c = Helper.Input.GetCursorPosition();
            Vector2[] grid = GetGrid(c.Tile, 50).ToArray();

            foreach(Vector2 tile in grid)
            {
                location.objects.TryGetValue(tile, out SObject tileObj);
                location.terrainFeatures.TryGetValue(tile, out TerrainFeature tileFeature);
                foreach (ITool tool1 in tools)
                {
                    if (tool1.Apply(tile, tileObj, tileFeature, player, tool, item, location))
                    {
                        break;
                    }
                }
            }

        }

        private void DoAction()
        {
            SFarmer player = Game1.player;
            GameLocation location = Game1.currentLocation;
            Tool tool = GetTool();//player.CurrentTool;
            Item item = player.CurrentItem;
            
            ITool[] tools = _tools
                .Where(tools1 => tools1.IsEnabled(player, tool, item, location))
                .ToArray();
            if (!tools.Any())
                return;
            ICursorPosition c = Helper.Input.GetCursorPosition();
            Vector2[] grid = GetGrid(c.Tile, _config.ToolRadius).ToArray();
            /*
            if (_config.ShowGrid)
                _showGrid = true;
            */
            foreach(Vector2 tile in grid)
            {
                location.objects.TryGetValue(tile, out SObject tileObj);
                location.terrainFeatures.TryGetValue(tile, out TerrainFeature tileFeature);
                foreach(ITool tool1 in tools)
                {
                    if(tool1.Apply(tile, tileObj, tileFeature, player, tool, item, location))
                    {
                        break;
                    }
                }                
            }            
        }

        private FarmAnimal GetAnimal(GameLocation loc, Vector2 tile)
        {
            GameLocation location = loc;

            Rectangle rectangle = new Rectangle((int)tile.X - 32, (int)tile.Y - 32, 64, 64);
            if (location is Farm farm)
            {
                foreach (FarmAnimal farmAnimal in farm.animals.Values)
                {
                    if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                    {
                        return farmAnimal;
                    }
                }
            }
            else if (location is AnimalHouse house)
            {
                foreach (FarmAnimal farmAnimal in house.animals.Values)
                {
                    if (farmAnimal.GetBoundingBox().Intersects(rectangle))
                    {
                        return farmAnimal;
                    }
                }
            }

            return null;
        }
        private Tool GetTool()
        {
            ICursorPosition c = Helper.Input.GetCursorPosition();
            Vector2[] grid = GetGrid(c.Tile, _config.ToolRadius).ToArray();
            Tool t = new Hoe();
            GameLocation location = Game1.player.currentLocation;
            ToolConfig toolConfig = _config.Tools;

            //FarmAnimal animal;
            foreach (Vector2 tile in grid)
            {
                location.objects.TryGetValue(tile, out SObject tileObj);
                location.terrainFeatures.TryGetValue(tile, out TerrainFeature tileFeature);
                //animal = GetAnimal(location, tile);

                //Check tiles to see if they're objects. Weed, Twig, Stone type stuff
                if (tileObj != null)
                {
                    if (tileObj.Name.Contains("Weed") || tileObj.Name.Contains("Twig"))
                        t = new Axe(){ UpgradeLevel = _config.ToolLevel };
                    if (tileObj.Name.Contains("Stone"))
                        t = new Pickaxe() { UpgradeLevel = _config.ToolLevel };
                }
                //Check tiles to see if they're terrainFeatures. Tree, HoeDirt type stuff
                if (tileFeature != null)
                {
                    if (tileFeature is Tree)
                        t = new Axe() { UpgradeLevel = _config.ToolLevel };
                    if(tileFeature is HoeDirt dirt && dirt.crop == null || (Game1.player.ActiveObject != null && Game1.player.CurrentItem.Category != SObject.SeedsCategory && Game1.player.CurrentItem.Category != -19))
                        t = new Pickaxe() { UpgradeLevel = _config.ToolLevel };
                    if(tileFeature is HoeDirt dirt1 && dirt1.crop != null)
                        t = new WateringCan() { UpgradeLevel = _config.ToolLevel };
                    if (tileFeature is Grass grass)
                        t = new MeleeWeapon() { Name = "Scythe", UpgradeLevel = _config.ToolLevel };
                    if (tileFeature is HoeDirt dirt3 && dirt3.crop == null && Game1.player.ActiveObject != null &&
                        (Game1.player.CurrentItem.Category == SObject.SeedsCategory ||
                         Game1.player.CurrentItem.Category == -19))
                    {
                        bool planted;
                        if (Game1.player.ActiveObject.Category == -19)
                        {
                            planted = dirt3.plant(Game1.player.ActiveObject.ParentSheetIndex, (int)tile.X, (int)tile.Y, Game1.player, true, location);
                            if(planted)
                                Game1.player.reduceActiveItemByOne();
                        }

                        if (Game1.player.ActiveObject.Category == SObject.SeedsCategory)
                        {
                            planted = dirt3.plant(Game1.player.ActiveObject.ParentSheetIndex, (int)tile.X, (int)tile.Y, Game1.player, false, location);
                            if (planted)
                                Game1.player.reduceActiveItemByOne();
                        }
                    }
                        
                }
                /*
                //See if Animal is not null
                if (animal != null)
                {
                    if (animal.toolUsedForHarvest.Value.Contains("ears"))
                        t = new Shears(){ UpgradeLevel = _config.ToolLevel };
                    else
                        t = new MilkPail(){ UpgradeLevel = _config.ToolLevel };
                }*/
                
                
            }

            return t;
        }

        private IEnumerable<Vector2>GetGrid(Vector2 origin, int to)
        {
            for(int x = -to; x <= to; x++)
            {
                for(int y = -to; y <= to; y++)
                {
                    yield return new Vector2(origin.X + x, origin.Y + y);
                }
            }
        }
        
        //Highlight Affected Area

        public void HighlightRadius(SpriteBatch spriteBatch)
        {
            ICursorPosition c = Helper.Input.GetCursorPosition();
            Vector2[] grid = GetGrid(c.Tile, _config.ToolRadius).ToArray();

            foreach (Vector2 tile in grid)
            {
                bool enabled = IsEnabled();
                Rectangle area = new Rectangle((int)(tile.X * Game1.tileSize - Game1.viewport.X), (int)(tile.Y * Game1.tileSize - Game1.viewport.Y), Game1.tileSize, Game1.tileSize);
                Color color = enabled ? Color.Green : Color.Red;
                if (_showGrid)
                {
                    spriteBatch.DrawLine(area.X, area.Y, new Vector2(area.Width, area.Height), color * 0.2f);
                    int bSize = 1;
                    Color bColor = color * 0.5f;
                    spriteBatch.DrawLine(area.X, area.Y, new Vector2(area.Width, bSize), bColor);
                    spriteBatch.DrawLine(area.X, area.Y, new Vector2(bSize, area.Height), bColor);
                    spriteBatch.DrawLine(area.X + area.Width, area.Y, new Vector2(bSize, area.Height), bColor);
                    spriteBatch.DrawLine(area.X, area.Y + area.Height, new Vector2(area.Width, bSize), bColor);
                }
                
            }
        }
        private bool IsEnabled()
        {
            if (_showGrid)
            {
                return true;
            }
            return false;
        }
        
        //Get Map Name
        private string MapName(GameLocation location)
        {
            return location.uniqueName ?? location.Name;
        }
    }
}

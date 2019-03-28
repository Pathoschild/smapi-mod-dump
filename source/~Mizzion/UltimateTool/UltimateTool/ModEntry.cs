using System;
using System.Collections.Generic;
using System.Linq;
using UltimateTool.Framework;
using UltimateTool.Framework.Tools;
using UltimateTool.Framework.Configuration;
using SFarmer = StardewValley.Farmer;
using SObject = StardewValley.Object;


namespace UltimateTool
{
    internal class ModEntry : Mod
    {
        private ModConfig Config;

        private ITool[] Tools;

        private Keys ActionKey;
        private Keys GrowKey;
        private Keys MineKey;
        private int ToolRadius;
        private bool ShowGrid = false;
        private int bID = 13379854;

        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();
            DoTools();
            UBush.helper = helper;
            
            SaveEvents.AfterLoad += AfterLoad;
            ControlEvents.KeyPressed += KeyPressed;
            GraphicsEvents.OnPostRenderEvent += OnPostRenderEvent;
            GameEvents.QuarterSecondTick += QuarterSecond;
            GameEvents.UpdateTick += UpdateTick;

        }
        private void OnPostRenderEvent(object sender, EventArgs e)
        {
            if(Context.IsWorldReady && Game1.activeClickableMenu == null)
            {
                this.HighlightRadius(Game1.spriteBatch);
            }
        }
        private void AfterLoad(object sender, EventArgs e)
        {
            if(!Enum.TryParse<Keys>(this.Config.ActionKey, true, out this.ActionKey))
            {
                this.ActionKey = Keys.Z;
            }
            if (!Enum.TryParse<Keys>(this.Config.GrowKey, true, out this.GrowKey))
            {
                this.GrowKey = Keys.X;
            }
            if (!Enum.TryParse<Keys>(this.Config.MineClearKey, true, out this.MineKey))
            {
                this.MineKey = Keys.V;
            }
            Game1.player.MagneticRadius = Game1.tileSize * this.Config.MagnetRadius;
        }
        private void KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (!Context.IsWorldReady)
                return;
            if(e.KeyPressed == this.ActionKey)
            {
                doAction();
            }
            if(e.KeyPressed == this.GrowKey)
            {

            }
            if(e.KeyPressed == this.MineKey)
            {
                doMine();
            }
            if(e.KeyPressed == Keys.G)
            {
                this.ShowGrid = this.ShowGrid ? false : true;
            }
            if(e.KeyPressed == Keys.NumPad8)
            {
                this.UpdateBuff();
            }
            if(e.KeyPressed == Keys.F5)
            {
                this.Config = this.Helper.ReadConfig<ModConfig>();
                DoTools();
            }
            if(e.KeyPressed == Keys.NumPad5)
            {
                Dictionary<string, string> dictionary1 = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
                string locationName = Game1.currentLocation.ToString();
                if (dictionary1.ContainsKey(locationName))
                {
                    string[] strArray1 = dictionary1[locationName].Split('/')[4 + Utility.getSeasonNumber(Game1.currentSeason)].Split(' ');
                    Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
                    if(strArray1.Length > 1)
                    {
                        int index = 0;
                        while(index < strArray1.Length)
                        {
                            dictionary2.Add(strArray1[index], strArray1[index + 1]);
                            index += 2;
                        }
                    }

                }
                
            }
        }
        private void QuarterSecond(object sender, EventArgs e)
        {
            SFarmer Player = Game1.player;
            // || Player.ActiveObject.Category == -74 || Player.ActiveObject.Category == -19
            if (Context.IsWorldReady)
            {
                if (Game1.activeClickableMenu == null && this.Config.ModEnabled == true && (Player.CurrentTool is Hoe || Player.CurrentTool is WateringCan || Player.CurrentTool is Axe || Player.CurrentTool is Pickaxe || (Player.CurrentTool is MeleeWeapon && Player.CurrentTool.name.ToLower().Contains("scythe"))))
                {
                    doAction();
                }
            }
        }
        private void UpdateTick(object sender, EventArgs e)
        {
            if (Context.IsPlayerFree)
                this.UpdateBuff();
        }
        private void UpdateBuff()
        {
            Buff @buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(b => b.which == this.bID);
            if (@buff == null)
            {
                    @buff = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 350, 1, 0, 0, 1, "Ultimate Tool", "Ultimate") { which = this.bID };
                    Game1.buffsDisplay.addOtherBuff(@buff);
               
            }
            @buff.millisecondsDuration = 100;
        }
        private void DoTools()
        {            
            IToolConfig toolConfig = this.Config.ITools;
            this.Tools = new ITool[]
            {
                new AxeTool(toolConfig.Axe),
                new FertilizerTool(toolConfig.Fertilizer),
                new GrassStarterTool(toolConfig.GrassStarter),
                new HoeTool(toolConfig.Hoe),
                new PickaxeTool(toolConfig.Pickaxe),
                new ScytheTool(toolConfig.Scythe),
                new SeedTool(toolConfig.Seeds),
                new WateringCanTool(toolConfig.WateringCan)

            };
            this.ToolRadius = this.Config.ToolRadius;
        }
        private void doMine()
        {
            SFarmer Player = Game1.player;
            GameLocation location = Game1.currentLocation;
            Tool tool = Player.CurrentTool;
            Item item = Player.CurrentItem;
            Dictionary<Vector2, SObject> curObj = new Dictionary<Vector2, SObject>();
            Dictionary<Vector2, TerrainFeature> curTerrain = new Dictionary<Vector2, TerrainFeature>();

            ITool[] tools = this.Tools
                .Where(tools1 => tools1.IsEnabled(Player, tool, item, location))
                .ToArray();
            if (!tools.Any())
                return;

            if (location.isFarm || location.name.Contains("Greenhouse") || location.name.Contains("FarmExpan"))
                return;
            
            
            Vector2[] grid = this.GetGrid(Player.getTileLocation(), 50).ToArray();

            foreach(Vector2 tile in grid)
            {
                location.objects.TryGetValue(tile, out SObject tileObj);
                location.terrainFeatures.TryGetValue(tile, out TerrainFeature tileFeature);
                foreach (ITool tool1 in tools)
                {
                    if (tool1.Apply(tile, tileObj, tileFeature, Player, tool, item, location))
                    {
                        break;
                    }
                }
            }

        }
        private void doAction()
        {
            SFarmer Player = Game1.player;
            GameLocation location = Game1.currentLocation;
            Tool tool = Player.CurrentTool;
            Item item = Player.CurrentItem;

            ITool[] tools = this.Tools
                .Where(tools1 => tools1.IsEnabled(Player, tool, item, location))
                .ToArray();
            if (!tools.Any())
                return;

            Vector2[] grid = this.GetGrid(Player.getTileLocation(), this.Config.ToolRadius).ToArray();


            foreach(Vector2 tile in grid)
            {
                location.objects.TryGetValue(tile, out SObject tileObj);
                location.terrainFeatures.TryGetValue(tile, out TerrainFeature tileFeature);
                foreach(ITool tool1 in tools)
                {
                    if(tool1.Apply(tile, tileObj, tileFeature, Player, tool, item, location))
                    {
                        break;
                    }
                }                
            }            
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
            foreach(Vector2 tile in this.GetGrid(Game1.player.getTileLocation(), this.ToolRadius))
            {
                bool enabled = this.IsEnabled();
                Rectangle area = new Rectangle((int)(tile.X * Game1.tileSize - Game1.viewport.X), (int)(tile.Y * Game1.tileSize - Game1.viewport.Y), Game1.tileSize, Game1.tileSize);
                Color color = enabled ? Color.Green : Color.Red;
                if (this.ShowGrid)
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
            if (this.ShowGrid)
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

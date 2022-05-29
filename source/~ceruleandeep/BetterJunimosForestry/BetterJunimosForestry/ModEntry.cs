/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using BetterJunimos;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;

namespace BetterJunimosForestry {
    public static class Modes {
        public const string Normal = "normal";
        public const string Crops = "crops";
        public const string Orchard = "orchard";
        public const string Forest = "forest";
        public const string Maze = "maze";
    }
    
    public class HutState {
        public bool ShowHUD;
        public string Mode = Modes.Normal;
    }

    public class ModeChange {
        public Guid guid;
        public readonly string mode;

        public ModeChange(Guid guid, string mode) {
            this.guid = guid;
            this.mode = mode;
        }
    }
    
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod {
        private readonly Dictionary<Rectangle, ModeChange> Rectangles = new();

        private Texture2D icons;
        private Texture2D scroll;
        
        private readonly Rectangle JunimoIcon = new(0, 0, 16, 16);
        private readonly Rectangle CropIcon = new(16, 0, 16, 16);
        private readonly Rectangle FruitTreeIcon = new(32, 0, 16, 16);
        private readonly Rectangle TreeIcon = new(48, 0, 16, 16);
        private readonly Rectangle MapIcon = new(64, 0, 16, 16);
        private readonly Rectangle LetterIcon = new(80, 0, 16, 16);
        private readonly Rectangle CheckboxesIcon = new(96, 0, 16, 16);
        
        internal static ModConfig Config;
        internal static IMonitor SMonitor;
        internal static Dictionary<Vector2, HutState> HutStates;
        internal static Dictionary<Vector2, Maze> HutMazes;

        internal static IBetterJunimosApi BJApi;
        private GenericModConfigMenuAPI GMCMAPI;

        internal static Abilities.PlantTreesAbility PlantTrees;
        internal static Abilities.PlantFruitTreesAbility PlantFruitTrees;

        private void RenderedWorld(object sender, RenderedWorldEventArgs e) {
            if (Game1.player.currentLocation is not Farm) return;
            Rectangles.Clear();
            
            foreach (var (hutPos, hutState) in HutStates)
            {
                RenderHutMenu(e, hutState, hutPos);
            }
        }

        private void RenderHutMenu(RenderedWorldEventArgs e, HutState hutState, Vector2 hutPos)
        {
            if (!hutState.ShowHUD) return;
            var hut = Util.GetHutFromPosition(hutPos);
            if (hut == null) return;
            var guid = Util.GetHutIdFromHut(hut);

            // what a palaver
            
            const int iconW = 16;
            const int paddingX = 3;
            const int offset = (iconW + 1) * Game1.pixelZoom;
            const int scrollWidth = offset * 7 + paddingX * 2;
            
            var hutXvp = hut.tileX.Value * Game1.tileSize - Game1.viewport.X; // hut x co-ord in viewport pixels
            var hutYvp = hut.tileY.Value * Game1.tileSize - Game1.viewport.Y;
            
            var scrollXvp = (int) (hutXvp + Game1.tileSize * 1.5 - scrollWidth / 2.0);
            var scrollYvp = (int) (hutYvp + Game1.tileSize * 2.25);

            var iconYvp = scrollYvp; //+ 1 * Game1.pixelZoom;

            var origin = new Vector2(scrollXvp,scrollYvp);

            var n = 0;
            Rectangle normal = new Rectangle(
                (int) origin.X + paddingX + offset * n++, 
                iconYvp, 
                iconW * Game1.pixelZoom,
                iconW * Game1.pixelZoom);
            Rectangle crops = new Rectangle(
                (int) origin.X + paddingX + offset * n++, 
                iconYvp, 
                iconW * Game1.pixelZoom,
                iconW * Game1.pixelZoom);
            Rectangle orchard = new Rectangle(
                (int) origin.X + paddingX + offset * n++, 
                iconYvp, 
                iconW * Game1.pixelZoom,
                iconW * Game1.pixelZoom);
            Rectangle forest = new Rectangle(
                (int) origin.X + paddingX + offset * n++, 
                iconYvp, 
                iconW * Game1.pixelZoom,
                iconW * Game1.pixelZoom);
            Rectangle maze = new Rectangle(
                (int) origin.X + paddingX + offset * n++, 
                iconYvp, 
                iconW * Game1.pixelZoom,
                iconW * Game1.pixelZoom);
            Rectangle quests = new Rectangle(
                (int) origin.X + paddingX + offset * n++, 
                iconYvp, 
                iconW * Game1.pixelZoom,
                iconW * Game1.pixelZoom);
            Rectangle actions = new Rectangle(
                (int) origin.X + paddingX + offset * n++, 
                iconYvp, 
                iconW * Game1.pixelZoom,
                iconW * Game1.pixelZoom);

            // var scroll = new Rectangle((int) origin.X, (int) origin.Y, scrollWidth, 18);
            //
            // Rectangles[scroll] = new ModeChange(guid, "_menu");
            Rectangles[normal] = new ModeChange(guid, "normal");
            Rectangles[crops] = new ModeChange(guid, "crops");
            Rectangles[orchard] = new ModeChange(guid, "orchard");
            Rectangles[forest] = new ModeChange(guid, "forest");
            Rectangles[maze] = new ModeChange(guid, "maze");
            Rectangles[quests] = new ModeChange(guid, "_quests");
            Rectangles[actions] = new ModeChange(guid, "_actions");

            DrawScroll(e.SpriteBatch, origin, scrollWidth);
            e.SpriteBatch.Draw(icons, normal, JunimoIcon, Color.White * (hutState.Mode == "normal" ? 1.0f : 0.25f));
            e.SpriteBatch.Draw(icons, crops, CropIcon, Color.White * (hutState.Mode == "crops" ? 1.0f : 0.25f));
            e.SpriteBatch.Draw(icons, orchard, FruitTreeIcon, Color.White * (hutState.Mode == "orchard" ? 1.0f : 0.25f));
            e.SpriteBatch.Draw(icons, forest, TreeIcon, Color.White * (hutState.Mode == "forest" ? 1.0f : 0.25f));
            e.SpriteBatch.Draw(icons, maze, MapIcon, Color.White * (hutState.Mode == "maze" ? 1.0f : 0.25f));
            e.SpriteBatch.Draw(icons, quests, LetterIcon, Color.White);
            e.SpriteBatch.Draw(icons, actions, CheckboxesIcon, Color.White);
        }

        private void DrawScroll(SpriteBatch b, Vector2 position, int scroll_width) {
            const float alpha = 1f;
            const float layerDepth = 0.88f;
            b.Draw(scroll, position + new Vector2(-12f, -3f) * 4f, new Rectangle(0, 0, 144, 24), Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 0.001f);

            // b.Draw(Game1.mouseCursors, position + new Vector2(-12f, -3f) * 4f, new Rectangle(325, 318, 12, 18), Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 0.001f);
            // b.Draw(Game1.mouseCursors, position + new Vector2(0f, -3f) * 4f, new Rectangle(337, 318, 1, 18), Color.White * alpha, 0f, Vector2.Zero, new Vector2(scroll_width, 4f), SpriteEffects.None, layerDepth - 0.001f);
            // b.Draw(Game1.mouseCursors, position + new Vector2(scroll_width, -12f), new Rectangle(338, 318, 12, 18), Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, layerDepth - 0.001f);
        }
        
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
            if (!Context.IsWorldReady) return;
            if (Game1.player.currentLocation is not Farm) return;

            if (Game1.activeClickableMenu is not null) return;
            if (!e.Button.IsUseToolButton()) return;
            if (HandleMenuClick(e)) return;

            var hut = Util.HutOnTile(e.Cursor.Tile);
            if (hut is null) return;
            
            var hutPos = Util.GetHutPositionFromHut(hut);
            if (!HutStates.ContainsKey(hutPos)) HutStates[hutPos] = new HutState();
            
            HutStates[hutPos].ShowHUD = !HutStates[hutPos].ShowHUD;

            Helper.Input.Suppress(e.Button);
        }

        private bool HandleMenuClick(ButtonPressedEventArgs e)
        {
            foreach (var (r, mc) in Rectangles)
            {
                var contains = r.Contains((int) e.Cursor.ScreenPixels.X, (int) e.Cursor.ScreenPixels.Y);
                if (!contains) continue;
                
                Helper.Input.Suppress(e.Button);
                var hut = Util.GetHutFromId(mc.guid);
                var hutPos = Util.GetHutPositionFromId(mc.guid);
                
                Game1.getFarm().playSound("junimoMeep1");

                switch (mc.mode)
                {
                    // handle specials
                    case "_quests":
                        BJApi.ShowPerfectionTracker();
                        break;
                    case "_actions":
                        BJApi.ShowConfigurationMenu();
                        BJApi.ListAvailableActions(mc.guid);
                        break;
                }

                // handle mode changes
                if (mc.mode.StartsWith("_")) continue;
                
                HutStates[hutPos].Mode = mc.mode;
                if (mc.mode == "maze")
                {
                    Maze.MakeMazeForHut(hut);
                }
                else
                {
                    Maze.ClearMazeForHut(hut);
                }

                return true;
            }

            return false;
        }

        private void OnLaunched(object sender, GameLaunchedEventArgs e) {
            HutStates = new Dictionary<Vector2, HutState>();
            HutMazes = new Dictionary<Vector2, Maze>();
            
            Config = Helper.ReadConfig<ModConfig>();
            
            GMCMAPI = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            SetupGMCM();

            BJApi = Helper.ModRegistry.GetApi<IBetterJunimosApi>("hawkfalcon.BetterJunimos");
            if (BJApi is null) {
                Monitor.Log($"Could not load Better Junimos API", LogLevel.Error);
            }
            
            icons = Helper.Content.Load<Texture2D>("assets/icons.png");
            scroll = Helper.Content.Load<Texture2D>("assets/scroll2.png");

            // BJApi.RegisterJunimoAbility(new Abilities.LayPathsAbility(Monitor));
            // Abilities now registered in OnSaveLoaded
        }

        private void SetupGMCM()
        {
            if (GMCMAPI is null) return;
            GMCMAPI.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));
            GMCMAPI.SetDefaultIngameOptinValue(ModManifest, true);

            GMCMAPI.RegisterSimpleOption(ModManifest, "Sustainable tree harvesting",
                "Only harvest wild trees when they've grown a seed", () => Config.SustainableWildTreeHarvesting,
                (val) => Config.SustainableWildTreeHarvesting = val);

            GMCMAPI.RegisterChoiceOption(ModManifest, "Wild tree pattern", "", () => Config.WildTreePattern,
                val => Config.WildTreePattern = val, Config.WildTreePatternChoices);
            GMCMAPI.RegisterChoiceOption(ModManifest, "Fruit tree pattern", "", () => Config.FruitTreePattern,
                val => Config.FruitTreePattern = val, Config.FruitTreePatternChoices);

            GMCMAPI.RegisterClampedOption(ModManifest, "Wild tree growth boost", "", () => Config.PlantWildTreesSize,
                val => Config.PlantWildTreesSize = (int) val, 0, 5, 1);
            GMCMAPI.RegisterClampedOption(ModManifest, "Fruit tree growth boost", "", () => Config.PlantFruitTreesSize,
                val => Config.PlantFruitTreesSize = (int) val, 0, 5, 1);

            GMCMAPI.RegisterSimpleOption(ModManifest, "Harvest Grass", "", () => Config.HarvestGrassEnabled,
                (val) => Config.HarvestGrassEnabled = val);
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        void OnSaveLoaded(object sender, EventArgs e) {
            // reload the config to pick up any changes made in GMCM on the title screen
            Config = Helper.ReadConfig<ModConfig>();

            
            // we use these elsewhere
            PlantTrees = new Abilities.PlantTreesAbility(Monitor);
            PlantFruitTrees = new Abilities.PlantFruitTreesAbility(Monitor);

            BJApi.RegisterJunimoAbility(new Abilities.HarvestGrassAbility());
            BJApi.RegisterJunimoAbility(new Abilities.HarvestDebrisAbility(Monitor));
            BJApi.RegisterJunimoAbility(new Abilities.CollectDroppedObjectsAbility(Monitor));
            BJApi.RegisterJunimoAbility(new Abilities.ChopTreesAbility(Monitor));
            BJApi.RegisterJunimoAbility(new Abilities.CollectSeedsAbility(Monitor));
            BJApi.RegisterJunimoAbility(new Abilities.FertilizeTreesAbility());
            BJApi.RegisterJunimoAbility(PlantTrees);
            BJApi.RegisterJunimoAbility(PlantFruitTrees);
            BJApi.RegisterJunimoAbility(new Abilities.HarvestFruitTreesAbility(Monitor));
            BJApi.RegisterJunimoAbility(new Abilities.HoeAroundTreesAbility(Monitor));
            
            if (!Context.IsMainPlayer) {
                return;
            }

            // load hut mode settings from the save file
            HutStates = Helper.Data.ReadSaveData<Dictionary<Vector2, HutState>>("ceruleandeep.BetterJunimosForestry.HutStates") ??
                        new Dictionary<Vector2, HutState>();

            // load hut maze settings from the save file
            HutMazes = Helper.Data.ReadSaveData<Dictionary<Vector2, Maze>>("ceruleandeep.BetterJunimosForestry.HutMazes") ??
                       new Dictionary<Vector2, Maze>();
        }
        
        /// <summary>Raised after a the game is saved</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        void OnSaving(object sender, SavingEventArgs e) {
            if (!Context.IsMainPlayer) return;
            Helper.Data.WriteSaveData("ceruleandeep.BetterJunimosForestry.HutStates", HutStates);
            Helper.Data.WriteSaveData("ceruleandeep.BetterJunimosForestry.HutMazes", HutMazes);
            Helper.WriteConfig(Config);
        }
        
        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        void OnDayStarted(object sender, DayStartedEventArgs e) {
            foreach (var hut in Game1.getFarm().buildings.OfType<JunimoHut>()) {
                if (Util.GetModeForHut(hut) == Modes.Maze)
                {
                    Maze.MakeMazeForHut(hut);
                }
            }

            // reset for rainy days, winter, or GMCM options change
            Helper.Content.InvalidateCache(@"Characters\Junimo");
        }
        
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        public override void Entry(IModHelper helper) {
            Helper.Events.Input.ButtonPressed += OnButtonPressed;
            Helper.Events.Display.RenderedWorld += RenderedWorld;
            Helper.Events.GameLoop.GameLaunched += OnLaunched;
            Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            Helper.Events.GameLoop.Saving += OnSaving;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;

            SMonitor = Monitor;
        }
    }
}

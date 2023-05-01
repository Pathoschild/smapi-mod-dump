/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiogoAlbano/StardewValleyMods
**
*************************************************/

using BNWCore.Automate;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Linq;
using Microsoft.Xna.Framework;
using BNWCore.Grabbers;
using System.Text;
using System;

namespace BNWCore
{
    public class ModEntry : Mod
    {
        internal static IModHelper ModHelper;
        internal static IApi IApi;
        internal static IManifest Manifest;
        internal static GameLocation SourceLocation;   
        private Assets_Editor Assets_Editor;
        private Magic_Bootle Magic_Bootle;
        private Shop_Mechanics Shop_Mechanics;
        private Menu_Config Menu_Config;
        private Magic_Net_Events Magic_Net_Events;
        private NPC_Schedules_Changes NPC_Schedules_Changes;
        private SoundLoop SoundLoop;
        public static ModConfig Config;
        public static Texture2D ObjectsTexture;
        public static Texture2D ConstructionTexture;
        public static string ModDataKey => $"{ModHelper.ModRegistry.ModID}.MagicNets";
        public static int BNWCoreMagicNetId { get; set; } = 735;
        public override void Entry(IModHelper helper)
        {
            if (Helper.ModRegistry.IsLoaded("DiogoAlbano.BNWCore") && Helper.ModRegistry.IsLoaded("Pathoschild.Automate"))
            {
                Helper.Events.GameLoop.GameLaunched += (s, e) =>
                {
                    var api = Helper.ModRegistry.GetApi<IAutomateApi>("Pathoschild.Automate");
                    if (api is not null)
                    {
                        api.AddFactory(new BNWCoreMagicNetFactory());
                    }
                };
            }
            ModHelper = helper;
            Config = Helper.ReadConfig<ModConfig>();
            Assets_Editor = new Assets_Editor();
            Magic_Bootle = new Magic_Bootle();
            Shop_Mechanics= new Shop_Mechanics();
            Menu_Config= new Menu_Config();
            Magic_Net_Events= new Magic_Net_Events();
            NPC_Schedules_Changes= new NPC_Schedules_Changes();
            SoundLoop= new SoundLoop();
            Config = Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += (s, e) => Patcher.Patch(helper);
            helper.ConsoleCommands.Add("getbnwmail", "Upgrade all tools to junimo", ConsoleCommands.SendEmails);
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.DayStarted += onDayStarted;
            helper.Events.GameLoop.DayEnding += onDayEnding;
            helper.Events.GameLoop.TimeChanged += OnTenMinuteUpdate;
            helper.Events.Display.RenderedWorld += OnRenderedWorld;
            helper.Events.World.ObjectListChanged += OnObjectListChanged;
            helper.Events.Content.AssetRequested += onAssetRequested;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.MenuChanged += Display_MenuChanged;
            helper.Events.GameLoop.OneSecondUpdateTicking += CheckMusicNeedsRestarting;
            helper.Events.Player.Warped += ResetMusicSecretWoods;
            helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
        }
        private void GameLoop_UpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            Shop_Mechanics.GameLoop_UpdateTicking(e);
        }
        public override object GetApi() => IApi ??= new Api();
        private void CheckMusicNeedsRestarting(object? sender, OneSecondUpdateTickingEventArgs e)
        {
            SoundLoop.CheckMusicNeedsRestarting(e);
        }
        private void ResetMusicSecretWoods(object? sender, WarpedEventArgs e)
        {
            SoundLoop.ResetMusicSecretWoods(e);
        }
            private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            Shop_Mechanics.Display_MenuChanged(e);
        }
        
        private void onAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            Assets_Editor.OnAssetRequested(e);
        }
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            Magic_Bootle.OnButtonPressed(e);
            Shop_Mechanics.Input_ButtonPressed(e);
        }
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            Shop_Mechanics.OnUpdateTicked(e);
        }
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Menu_Config.OnGameLaunched(e);
        }               
        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            GrabAtLocation(e.Location);
        }
        private void onDayEnding(object sender, DayEndingEventArgs e)
        {
            Magic_Net_Events.onDayEnding(e);
        }
        private void OnTenMinuteUpdate(object sender, TimeChangedEventArgs e)
        {
            if (e.NewTime % 100 == 0)
            {
                foreach (var location in Game1.locations)
                {
                    var orePanGrabber = new OrePanGrabber(this, location);
                    if (orePanGrabber.CanGrab())
                    {
                        orePanGrabber.GrabItems();
                    }
                }
            }
        }     
        private void onDayStarted(object sender, DayStartedEventArgs e)
        {
            Magic_Net_Events.onDayStarted(e);
            NPC_Schedules_Changes.GameLoop_DayStarted(e);
            var locations = Game1.locations.Concat(Game1.getFarm().buildings.Select(building => building.indoors.Value)).Where(location => location != null);
            foreach (var location in locations) GrabAtLocation(location);
        }     
        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!(Context.IsPlayerFree && !Game1.eventUp && Game1.farmEvent == null && Grabber_Config.harvestCropsRange > 0 && Config.BNWCoreharvestCrops))
                return;
            if (Config.BNWCoreharvestCrops && Grabber_Config.harvestCropsRange > 0 && Game1.player.ActiveObject != null && Game1.player.ActiveObject.bigCraftable.Value && Game1.player.ActiveObject.ParentSheetIndex == ItemIds.Autograbber)
            {
                var placementTile = Game1.GetPlacementGrabTile();
                var X = (int)placementTile.X;
                var Y = (int)placementTile.Y;
                if (Game1.IsPerformingMousePlacement())
                {
                    var range = Grabber_Config.harvestCropsRange;
                    for (int x = X - range; x <= X + range; x++)
                    {
                        for (int y = Y - range; y <= Y + range; y++)
                        {
                            if (Grabber_Config.harvestCropsRangeMode == "Walk" && Math.Abs(X - x) + Math.Abs(Y - y) > range) continue;
                            Game1.spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(new Vector2(x, y) * 64), new Rectangle(194, 388, 16, 16), Color.White, 0, Vector2.Zero, 4, SpriteEffects.None, 0.01f);
                        }
                    }
                }
            }
        }
        private bool GrabAtLocation(GameLocation location)
        {
            var grabber = new AggregateDailyGrabber(this, location);
            var previousInventory = Grabber_Config.reportYield ? grabber.GetInventory() : null;
            var grabbed = grabber.GrabItems();
            if (previousInventory != null)
            {
                var nextInventory = grabber.GetInventory();
                var sb = new StringBuilder($"Yield of autograbber(s) at {location.Name}:\n");
                var shouldPrint = false;
                foreach (var pair in nextInventory)
                {
                    var item = pair.Key;
                    var stack = pair.Value;
                    var diff = stack;
                    if (previousInventory.ContainsKey(pair.Key))
                    {
                        diff -= previousInventory[pair.Key];
                    }
                    if (diff <= 0) continue;
                    sb.AppendLine($"    {item.Name} ({item.QualityName}) x{diff}");
                    shouldPrint = true;
                }
                if (shouldPrint) Monitor.Log(sb.ToString(), LogLevel.Info);
            }
            return grabbed;
        }
    }
}

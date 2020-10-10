/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/FarmHouseRedone
**
*************************************************/

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Characters;
using Microsoft.Xna.Framework;
using Harmony;
using Netcode;

namespace FarmHouseRedone
{
    public class ModEntry : Mod
    {
        private Config config;

        public override void Entry(IModHelper helper)
        {
            Logger.monitor = Monitor;
            config = helper.ReadConfig<Config>();
            Logger.debugMode = config.debug;
            var harmony = HarmonyInstance.Create("mabelsyrup.farmhouse");

            FarmHouseStates.harmony = harmony;
            FarmHouseStates.modPath = helper.DirectoryPath;
            FarmHouseStates.spouseRooms = new Dictionary<string, int>();
            FarmHouseStates.reflector = helper.Reflection;

            Image.Renderer.testRender = Image.Renderer.render(32, 32, 8).render();

            //FarmHouse patches
            harmony.Patch(
                original: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.getFloors)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(FarmHouse_getFloors_Patch), nameof(FarmHouse_getFloors_Patch.Postfix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.getWalls)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(FarmHouse_getWalls_Patch), nameof(FarmHouse_getWalls_Patch.Postfix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.setMapForUpgradeLevel)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmHouse_setMapForUpgradeLevel_patch), nameof(FarmHouse_setMapForUpgradeLevel_patch.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.loadSpouseRoom)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmHouse_loadSpouseRoom_Patch), nameof(FarmHouse_loadSpouseRoom_Patch.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.performTenMinuteUpdate)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmHouse_performTenMinuteUpdate_patch), nameof(FarmHouse_performTenMinuteUpdate_patch.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.showSpouseRoom)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmHouse_showSpouseRoom_Patch), nameof(FarmHouse_showSpouseRoom_Patch.Prefix)))
            );


            //harmony.Patch(
            //    original: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.getBedSpot)),
            //    transpiler: new HarmonyMethod(AccessTools.Method(typeof(FarmHouse_showSpouseRoom_Patch), nameof(FarmHouse_getBedSpot_patch.Transpiler)))
            //);

            //harmony.Patch(
            //    original: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.getKitchenStandingSpot)),
            //    prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmHouse_getKitchenStandingSpot_Patch), nameof(FarmHouse_getKitchenStandingSpot_Patch.Prefix)))
            //);

            //DecoratableLocation patches
            harmony.Patch(
                original: helper.Reflection.GetMethod(new DecoratableLocation(), "doSetVisibleWallpaper").MethodInfo,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_doSetVisibleWallpaper_Patch), nameof(DecoratableLocation_doSetVisibleWallpaper_Patch.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(DecoratableLocation), nameof(DecoratableLocation.setFloor)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_setFloor_Patch), nameof(DecoratableLocation_setFloor_Patch.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(DecoratableLocation), nameof(DecoratableLocation.setWallpaper)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_setWallpaper_Patch), nameof(DecoratableLocation_setWallpaper_Patch.Prefix)))
            );
            //harmony.Patch(
            //    original: AccessTools.Method(typeof(DecoratableLocation), nameof(DecoratableLocation.resetForPlayerEntry)),
            //    prefix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_resetForPlayerEntry_Patch), nameof(DecoratableLocation_resetForPlayerEntry_Patch.Prefix)))
            //);
            harmony.Patch(
                original: AccessTools.Method(typeof(DecoratableLocation), nameof(DecoratableLocation.getFloors)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_getFloors_Patch), nameof(DecoratableLocation_getFloors_Patch.Postfix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(DecoratableLocation), nameof(DecoratableLocation.getWalls)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_getWalls_Patch), nameof(DecoratableLocation_getWalls_Patch.Postfix)))
            );
            harmony.Patch(
                original: helper.Reflection.GetMethod(new DecoratableLocation(), "doSetVisibleFloor").MethodInfo,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_doSetVisibleFloor_Patch), nameof(DecoratableLocation_doSetVisibleFloor_Patch.Prefix)))
            );

            //Game1 Patches
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.getLocationRequest)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Game1_getLocationRequest_PrePatch), nameof(Game1_getLocationRequest_PrePatch.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.warpFarmer), new Type[]
                {
                    typeof(LocationRequest),
                    typeof(int),
                    typeof(int),
                    typeof(int)
                }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Game1_warpFarmer_Patch), nameof(Game1_warpFarmer_Patch.Prefix)))
            );
            harmony.Patch(
                original: helper.Reflection.GetMethod(typeof(Game1), "_newDayAfterFade").MethodInfo,
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(Game1_newDayAfterFade_Patch), nameof(Game1_newDayAfterFade_Patch.Transpiler)))
            );

            //Farm Patches
            harmony.Patch(
                original: helper.Reflection.GetMethod(new Farm(), "resetSharedState").MethodInfo,
                postfix: new HarmonyMethod(AccessTools.Method(typeof(Farm_resetSharedState_Patch), nameof(Farm_resetSharedState_Patch.Postfix)))
            );
            harmony.Patch(
                original: helper.Reflection.GetMethod(new Farm(), "resetLocalState").MethodInfo,
                postfix: new HarmonyMethod(AccessTools.Method(typeof(Farm_resetLocalState_Patch), nameof(Farm_resetLocalState_Patch.Postfix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.showShipment)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Farm_showShipment_Patch), nameof(Farm_showShipment_Patch.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.UpdateWhenCurrentLocation)),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(Farm_UpdateWhenCurrentLocation_Patch), nameof(Farm_UpdateWhenCurrentLocation_Patch.Transpiler)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.addSpouseOutdoorArea)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Farm_addSpouseOutdoorArea_Patch), nameof(Farm_addSpouseOutdoorArea_Patch.Prefix)))
            );

            

            //Shed Patches
            harmony.Patch(
                original: AccessTools.Method(typeof(Shed), nameof(Shed.getFloors)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(Shed_getFloors_Patch), nameof(Shed_getFloors_Patch.Postfix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Shed), nameof(Shed.getWalls)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(Shed_getWalls_Patch), nameof(Shed_getWalls_Patch.Postfix)))
            );

            //GameLocation Patches
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(GameLocation_performAction_Patch), nameof(GameLocation_performAction_Patch.Prefix)))
            );
            harmony.Patch(
                original: helper.Reflection.GetMethod(new GameLocation(), "_updateAmbientLighting").MethodInfo,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(GameLocation__updateAmbientLighting_Patch), nameof(GameLocation__updateAmbientLighting_Patch.Prefix)))
            );
            //harmony.Patch(
            //    original: helper.Reflection.GetMethod(new GameLocation(), "carpenters").MethodInfo,
            //    transpiler: new HarmonyMethod(AccessTools.Method(typeof(GameLocation_carpenters_Patch), nameof(GameLocation_carpenters_Patch.Transpiler)))
            //);
            //harmony.Patch(
            //    original: helper.Reflection.GetMethod(new GameLocation(), "houseUpgradeAccept").MethodInfo,
            //    prefix: new HarmonyMethod(AccessTools.Method(typeof(GameLocation_houseUpgradeAccept_Patch), nameof(GameLocation_houseUpgradeAccept_Patch.Prefix)))
            //);
            //harmony.Patch(
            //    original: helper.Reflection.GetMethod(new GameLocation(), "houseUpgradeOffer").MethodInfo,
            //    prefix: new HarmonyMethod(AccessTools.Method(typeof(GameLocation_houseUpgradeOffer_Patch), nameof(GameLocation_houseUpgradeOffer_Patch.Prefix)))
            //);

            //NPC patches
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.marriageDuties)),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(NPC_marriageDuties_Patch), nameof(NPC_marriageDuties_Patch.Transpiler)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.setUpForOutdoorPatioActivity)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(NPC_setUpForOutdoorPatioActivity_Patch), nameof(NPC_setUpForOutdoorPatioActivity_Patch.Prefix)))
            );
            harmony.Patch(
                original: helper.Reflection.GetMethod(new NPC(), "doPlaySpousePatioAnimation").MethodInfo,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(NPC_doPlaySpousePatioAnimation_Patch), nameof(NPC_doPlaySpousePatioAnimation_Patch.Prefix)))
            );

            //Wand patches
            harmony.Patch(
                original: helper.Reflection.GetMethod(new StardewValley.Tools.Wand(), "wandWarpForReal").MethodInfo,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Wand_wandWarpForReal_Patch), nameof(Wand_wandWarpForReal_Patch.Prefix)))
            );

            //Object patches
            harmony.Patch(
                original: helper.Reflection.GetMethod(new StardewValley.Object(), "totemWarpForReal").MethodInfo,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Object_totemWarpForReal_Patch), nameof(Object_totemWarpForReal_Patch.Prefix)))
            );

            //Wallpaper patches
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Objects.Wallpaper), nameof(StardewValley.Objects.Wallpaper.placementAction)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Wallpaper_placementAction_Patch), nameof(Wallpaper_placementAction_Patch.Prefix)))
            );

            //Event patches
            harmony.Patch(
                original: helper.Reflection.GetMethod(new StardewValley.Event(), "setUpCharacters").MethodInfo,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Event_setUpCharacters_Patch), nameof(Event_setUpCharacters_Patch.Prefix))),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(Event_setUpCharacters_Patch), nameof(Event_setUpCharacters_Patch.Transpiler)))
            );

            //Pet patches
            harmony.Patch(
                original: AccessTools.Method(typeof(Pet), nameof(Pet.warpToFarmHouse)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(Pet_warpToFarmHouse_Patch), nameof(Pet_warpToFarmHouse_Patch.Postfix)))
            );

            helper.Events.GameLoop.DayStarted += newDay;
            helper.Events.World.NpcListChanged += npcListChanged;
            helper.Events.Player.Warped += fixPlayerWarps;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.SaveLoaded += saveLoaded;
            helper.Events.Input.ButtonPressed += buttonPressed;


            helper.Events.Display.RenderedWorld += Display_RenderedWorld;

            FarmHouseStates.loader = helper.Content;
            //helper.Events.Display.RenderedHud += drawStuff;
        }

        private void Display_RenderedWorld(object sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
        {
            if(FarmHouseStates.getState(StardewValley.Utility.getHomeOfFarmer(Game1.player)) != null)
            {
                //FarmHouseStates.drawRender(e.SpriteBatch);
                //e.SpriteBatch.Draw(Image.Renderer.testRender, Vector2.Zero, new Rectangle(0, 0, Image.Renderer.testRender.Width, Image.Renderer.testRender.Height), Color.White, 0.0f, Vector2.Zero, 4f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1);
            }
        }

        public void buttonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            Farmer player = Game1.MasterPlayer;
            if(e.Button == SButton.J && config.debug)
            {
                if (player.hasPet())
                {
                    player.getPet().warpToFarmHouse(player);
                }
            }
            if (e.Button == SButton.H)
            {
                Image.Renderer.testRender = Image.Renderer.render(32, 32, 8).render();
            }
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            ObjectIDHelper.init();
            //FarmHouseStates.harmony.Patch(
            //    original: Helper.Reflection.GetMethod(new FarmHouse(), "doSetVisibleFloor").MethodInfo,
            //    prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmHouse_doSetVisibleFloor_Patch), nameof(FarmHouse_doSetVisibleFloor_Patch.Prefix)))
            //);
            //Farmer Patches
            //FarmHouseStates.harmony.Patch(
            //    original: typeof(Farmer).GetConstructor(new Type[]
            //    {
            //        typeof(FarmerSprite),
            //        typeof(Vector2),
            //        typeof(int),
            //        typeof(string),
            //        typeof(List<Item>),
            //        typeof(bool)
            //    }),
            //    postfix: new HarmonyMethod(AccessTools.Method(typeof(Farmer_Ctor_Patch), nameof(Farmer_Ctor_Patch.Postfix)))
            //);

            FarmState.init();
            FarmHouseStates.init();
            OtherLocations.DecoratableStates.init();
            OtherLocations.FakeDecor.FakeDecorHandler.init();
        }

        //internal void drawStuff(object sender, StardewModdingAPI.Events.RenderedHudEventArgs e)
        //{
        //    FarmHouseStates.testCaster.draw(e.SpriteBatch);
        //}

        internal void reloadStaticClasses()
        {
            Logger.Log("Reloading static classes...");
            FarmHouseStates.init();
            FarmHouseStates.clearAll();
            FarmState.init();
            FarmState.setUpFarm(Game1.getFarm());
            OtherLocations.DecoratableStates.init();
            OtherLocations.DecoratableStates.clearAll();
            OtherLocations.FakeDecor.FakeDecorHandler.init();
            //FarmHouseStates.render = FarmHouseStates.renderFarmHouse(Utility.getHomeOfFarmer(Game1.player));
            Logger.Log("House location is " + FarmState.houseLocation.ToString());
            Logger.Log("getPorchStandingSpot() returned " + FarmState.getPorchStandingSpot().ToString());
        }

        internal void saveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            //Game1.getFarm().grandpaScore.Value = 3;
            reloadStaticClasses();
            FarmHouseStates.harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.draw)),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(Farm_draw_Patch), nameof(Farm_draw_Patch.Transpiler))),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(Farm_draw_Patch), nameof(Farm_draw_Patch.Postfix)))
            );
            FarmHouseStates.harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.leftClick)),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(Farm_leftClick_Patch), nameof(Farm_leftClick_Patch.Transpiler)))
            );
            FarmHouseStates.harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.checkAction)),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(Farm_checkAction_Patch), nameof(Farm_checkAction_Patch.Transpiler)))
            );
            FarmHouseStates.harmony.Patch(
                original: AccessTools.Method(typeof(Farm), nameof(Farm.addGrandpaCandles)),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(Farm_addGrandpaCandles_Patch), nameof(Farm_addGrandpaCandles_Patch.Transpiler)))
            );

            foreach (Farmer player in Game1.getAllFarmers())
            {
                if (player.currentLocation != null && player.currentLocation is FarmHouse)
                {
                    Logger.Log("Current location is a farmhouse; getting bed spot from this farmhouse.");
                    player.mostRecentBed = StardewValley.Utility.PointToVector2(FarmHouseStates.getBedSpot(player.currentLocation as FarmHouse)) * 64f;
                    player.position.Value = new StardewValley.Network.NetPosition(new NetVector2(player.mostRecentBed));
                    player.position.X -= 64f;
                    player.position.Y += 32f;
                }
                Logger.Log(player.Name + "'s most recent bed: " + player.mostRecentBed.ToString());
            }
        }

        internal void fixPlayerWarps(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            if (e.NewLocation is FarmHouse)
                fixPlayerHouseWarp(e);
            else if (e.NewLocation is Farm)
                fixFarmWarp(e);
        }

        internal void fixFarmWarp(StardewModdingAPI.Events.WarpedEventArgs e)
        {
            if (!(e.NewLocation is Farm))
                return;

        }

        internal void fixPlayerHouseWarp(StardewModdingAPI.Events.WarpedEventArgs e)
        {
            Farmer farmer = e.Player;
            FarmHouse house = (e.NewLocation as FarmHouse);
            //if(e.OldLocation is Farm)
            //{
            //    if (farmer.getTileLocationPoint() == house.getEntryLocation() || new Point(farmer.getTileLocationPoint().X, farmer.getTileLocationPoint().Y - 1) == house.getEntryLocation())
            //    {
            //        Logger.Log("Player warped to the farmhouse, and was in the entry location.  Setting player to modded entry (if any)...");
            //        Point entryPoint = FarmHouseStates.getEntryLocation(house);
            //        farmer.setTileLocation(new Vector2(entryPoint.X, entryPoint.Y));
            //    }
            //}
            if(e.OldLocation is Cellar)
            {
                if(farmer.getTileLocationPoint() == new Point(4, 25) || farmer.getTileLocationPoint() == new Point(5, 24))
                {
                    Logger.Log("Player warped to the farmhouse from the cellar, and was in the cellar return location.  Setting player to modded cellar return (if any)...");
                    Point cellarPoint = FarmHouseStates.getCellarLocation(house);
                    if(cellarPoint != new Point(-1, -1))
                    {
                        //farmer.setTileLocation(new Vector2(cellarPoint.X, cellarPoint.Y));
                        farmer.position.Value = new Vector2(cellarPoint.X * 64f, cellarPoint.Y * 64f);
                        Logger.Log("Set player to " + cellarPoint.X + ", " + cellarPoint.Y);
                    }
                }
                else
                {
                    Logger.Log("Player warped to the farmhouse from the cellar, but was not in the cellar return location.  Player was at " + farmer.getTileLocationPoint().X + ", " + farmer.getTileLocationPoint().Y);

                }
            }
            if (farmer.getTileLocationPoint() == house.getEntryLocation())
            {
                Logger.Log("Player warped to the farmhouse, and was in the entry location.  Setting player to modded entry (if any)...");
                Point entryPoint = FarmHouseStates.getEntryLocation(house);
                farmer.position.Value = new Vector2(entryPoint.X * 64f, entryPoint.Y * 64f);
                //farmer.setTileLocation(new Vector2());
            }
            else
            {
                Logger.Log("Player warped to the farmhouse, but was not at the entry location!  Player is at " + farmer.getTileLocationPoint().ToString() + " as a point, and " + farmer.getTileLocation().ToString() + " as a Vector2" + ", not the entry location " + house.getEntryLocation().ToString());
                Logger.Log("Player warped from " + e.OldLocation.name + ".");
                Logger.Log("House upgrade level is " + house.upgradeLevel + ".");
                if (FarmHouseStates.getState(house).entryData != null)
                    Logger.Log("House entry data is " + FarmHouseStates.getState(house).entryData);
                else
                    Logger.Log("House entry data not initialized!");
                Logger.Log("Player physical centerpoint is (" + farmer.GetBoundingBox().Center.X + ", " + farmer.GetBoundingBox().Bottom + "), with a bounding box of " + farmer.GetBoundingBox().ToString());
            }

            if(house is Cabin)
            {
                fixWarps(house as Cabin);
            }
        }

        internal void fixWarps(Cabin cabin)
        {
            List<Warp> farmWarps = new List<Warp>();
            foreach(Warp warp in cabin.warps)
            {
                if (warp.TargetName.Equals("Farm"))
                {
                    farmWarps.Add(new Warp(warp.X, warp.Y, warp.TargetName, warp.TargetX, warp.TargetY, warp.flipFarmer));
                }
            }
            cabin.warps.Clear();
            xTile.ObjectModel.PropertyValue propertyValue;
            cabin.map.Properties.TryGetValue("Warp", out propertyValue);
            if (propertyValue == null)
                return;
            string[] strArray = propertyValue.ToString().Split(' ');
            int index = 0;
            while (index < strArray.Length)
            {
                if (strArray[index + 2].Equals("Farm"))
                {
                    index += 5;
                    continue;
                }
                cabin.warps.Add(new Warp(Convert.ToInt32(strArray[index]), Convert.ToInt32(strArray[index + 1]), strArray[index + 2], Convert.ToInt32(strArray[index + 3]), Convert.ToInt32(strArray[index + 4]), false));
                index += 5;
            }
            foreach(Warp warp in farmWarps)
            {
                cabin.warps.Add(warp);
            }
        }

        internal void fixPaths(NPC npc, FarmHouse house)
        {

            if(npc.getTileLocationPoint() == house.getEntryLocation())
            {
                Logger.Log(npc.name + " entered at the entry point, repositioning at modded entry (if any)...");
                npc.setTilePosition(FarmHouseStates.getEntryLocation(house));
            }
            Logger.Log("Fixing paths for " + npc.name + "...");
            if (npc.isMarried() && npc.getSpouse() == Game1.player)
            {
                Logger.Log(npc.name + " is married to " + Game1.player.name + "...");
                if (npc.controller != null && npc.controller.endPoint != null && npc.controller.endPoint == house.getKitchenStandingSpot())
                {
                    Logger.Log(npc.name + " is pathing to the kitchen...");
                    npc.willDestroyObjectsUnderfoot = false;
                    npc.controller = new PathFindController(npc, house, FarmHouseStates.getKitchenSpot(house), 0);
                    if (npc.controller.pathToEndPoint == null)
                    {
                        npc.willDestroyObjectsUnderfoot = true;
                        npc.controller = new PathFindController(npc, house, FarmHouseStates.getKitchenSpot(house), 0);
                        npc.setNewDialogue(Game1.LoadStringByGender((int)((NetFieldBase<int, NetInt>)npc.gender), "Strings\\StringsFromCSFiles:NPC.cs.4500"), false, false);
                    }
                    else if (Game1.timeOfDay > 1300)
                    {
                        if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).ToLower().Equals("mon") || Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).ToLower().Equals("fri") && !npc.name.Equals((object)"Maru") && (!npc.name.Equals((object)"Penny") && !npc.name.Equals((object)"Harvey")))
                            npc.setNewDialogue("MarriageDialogue", "funReturn_", -1, false, true);
                        else
                            npc.setNewDialogue("MarriageDialogue", "jobReturn_", -1, false, false);
                    }
                }
                else if (npc.controller != null && npc.controller.endPoint != null && npc.controller.endPoint == house.getSpouseBedSpot(npc.name))
                {
                    Logger.Log(npc.name + " is heading to bed...");
                    npc.willDestroyObjectsUnderfoot = false;
                    npc.controller = new PathFindController(npc, house, FarmHouseStates.getBedSpot(house, true), 0);
                    if (npc.controller.pathToEndPoint == null)
                    {
                        npc.willDestroyObjectsUnderfoot = true;
                        npc.controller = new PathFindController(npc, house, FarmHouseStates.getBedSpot(house, true), 0);
                        npc.setNewDialogue(Game1.LoadStringByGender((int)((NetFieldBase<int, NetInt>)npc.gender), "Strings\\StringsFromCSFiles:NPC.cs.4500"), false, false);
                    }
                    else if (Game1.timeOfDay > 1300)
                    {
                        if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).ToLower().Equals("mon") || Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).ToLower().Equals("fri") && !npc.name.Equals((object)"Maru") && (!npc.name.Equals((object)"Penny") && !npc.name.Equals((object)"Harvey")))
                            npc.setNewDialogue("MarriageDialogue", "funReturn_", -1, false, true);
                        else
                            npc.setNewDialogue("MarriageDialogue", "jobReturn_", -1, false, false);
                    }
                }
            }
        }

        internal void npcListChanged(object sender, StardewModdingAPI.Events.NpcListChangedEventArgs e)
        {
            if (!(e.Location is FarmHouse) || e.Location is Cabin)
                return;
            FarmHouse house = (e.Location as FarmHouse);
            foreach(NPC npc in e.Added)
            {
                fixPaths(npc, house);
            }
        }

        internal void fixVoidSpouse(object sender, EventArgs e)
        {
            FarmHouse house = (Game1.getLocationFromName("FarmHouse") as FarmHouse);
            if(Game1.player.isMarried() && Game1.player.getSpouse().currentLocation == house)
            {
                NPC spouse = Game1.player.getSpouse();
                if(!house.isTileLocationOpen(new xTile.Dimensions.Location(spouse.getTileX(), spouse.getTileY())))
                {
                    spouse.setTilePosition(house.getRandomOpenPointInHouse(Game1.random));
                }
            }
        }

        internal void newDay(object sender, EventArgs e)
        {
            reloadStaticClasses();

            foreach (GameLocation location in Game1.locations)
            {
                if(location is FarmHouse)
                {
                    FarmHouse house = location as FarmHouse;
                    fixVoidObjects(house);
                    fixSpousePosition(house);
                    //setPlayerByBed(house);
                    foreach (NPC npc in house.characters)
                    {
                        if (npc.isMarried() && npc.getSpouse().Equals(Game1.player))
                            fixPaths(npc, house);
                    }
                }
            }
            //FarmState.setFarmhouseLocation(new Vector2(64, 25));
            //FarmState.setGreenhouseLocation(new Vector2(54, 27));
        }

        internal void fixVoidObjects(FarmHouse house)
        {
            FarmHouseStates.fixObjectsOnMap(house);
            //Todo: Find all loose items and put them in a gift box
            List<Item> itemsToStore = new List<Item>();
            //for(int x = 0; x < house.map.GetLayer("Back").LayerWidth; x++)
            //{
            //    for(int y = 0; y < house.map.GetLayer("Back").LayerHeight; y++)
            //    {

            //    }
            //}
            Logger.Log("Fixing void objects...");
            Dictionary<Vector2, Vector2> replacementCoordinates = new Dictionary<Vector2, Vector2>();
            List<Vector2> deadObjects = new List<Vector2>();
            foreach(KeyValuePair<Vector2, StardewValley.Object> objectPair in house.objects.Pairs)
            {
                bool canBePlaced = isObjectSpotValid(objectPair.Value, objectPair.Key, house);
                Logger.Log("Tile blocked? " + (house.isTileOnMap(objectPair.Key) && !canBePlaced).ToString());
                if(house.isTileOnMap(objectPair.Key) && !canBePlaced)
                {
                    Logger.Log("Tile " + objectPair.Key.ToString() + " was not placeable!  Moving object...");
                    Point newSpot = house.getRandomOpenPointInHouse(Game1.random);
                    int attempts = 0;
                    while((newSpot == Point.Zero || !objectPair.Value.canBePlacedHere(house, new Vector2(newSpot.X, newSpot.Y))) && attempts < 20)
                    {
                        newSpot = house.getRandomOpenPointInHouse(Game1.random);
                        attempts++;
                        Logger.Log("Failed to find empty open spot in the farmhouse for an object.  Attempt #" + attempts);
                    }
                    Logger.Log("Finished selecting spots.");
                    if(newSpot == Point.Zero)
                    {
                        Logger.Log("Could not find a suitable location for an object.  Storing it in a gift box.");
                        if(objectPair.Value is StardewValley.Objects.Chest)
                        {
                            StardewValley.Objects.Chest chest = (objectPair.Value as StardewValley.Objects.Chest);
                            Logger.Log("Object to be stored is a chest, so the contents will be stored separately.");
                            foreach(Item item in chest.items)
                            {
                                itemsToStore.Add(item);
                            }
                        }
                        itemsToStore.Add(objectPair.Value);
                        deadObjects.Add(objectPair.Key);
                    }
                    else
                    {
                        Vector2 tileLocation = new Vector2(newSpot.X, newSpot.Y);
                        replacementCoordinates[tileLocation] = objectPair.Key;
                        objectPair.Value.tileLocation.Value = tileLocation;
                        deadObjects.Add(objectPair.Key);
                        Logger.Log("Placing object at " + tileLocation.ToString());
                    }
                }
                else
                {
                    Logger.Log("Skipping " + objectPair.Key.ToString() + " because it " + (house.isTileOnMap(objectPair.Key) ? "was on the map, and " : "was not on the map, and ") + (!canBePlaced ? "was not placeable." : "was placeable."));
                }
            }
            foreach(KeyValuePair<Vector2,Vector2> replacementPair in replacementCoordinates)
            {
                if (!house.objects.ContainsKey(replacementPair.Key))
                {
                    house.objects.Add(replacementPair.Key, house.objects[replacementPair.Value]);
                }
            }
            foreach(Vector2 deadObject in deadObjects)
            {
                house.objects.Remove(deadObject);
            }
            
            List<StardewValley.Objects.Furniture> deadFurniture = new List<StardewValley.Objects.Furniture>();
            foreach (StardewValley.Objects.Furniture furniture in house.furniture)
            {
                bool canBePlaced = isFurnitureSpotValid(furniture, house);
                Logger.Log("Tile blocked? " + (!house.isTileOnMap(furniture.tileLocation) || !canBePlaced).ToString());
                fixFurniture(furniture, house, ref deadFurniture, ref itemsToStore);
            }
            foreach(StardewValley.Objects.Furniture furniture in deadFurniture)
            {
                house.furniture.Remove(furniture);
            }

            Point chestSpot = house.getRandomOpenPointInHouse(Game1.random);

            StardewValley.Objects.Chest giftBox = new StardewValley.Objects.Chest(0, itemsToStore, new Vector2(chestSpot.X, chestSpot.Y), true);
        }

        internal void fixFurniture(StardewValley.Objects.Furniture furniture, FarmHouse house, ref List<StardewValley.Objects.Furniture> deadFurniture, ref List<Item> itemsToStore, int placementAttempt = 0)
        {
            
            if (house.isTileOnMap(furniture.tileLocation) && (furniture.furniture_type == StardewValley.Objects.Furniture.window || furniture.furniture_type == StardewValley.Objects.Furniture.painting))
            {
                bool wasOnWall = false;
                foreach (Rectangle wall in house.getWalls())
                {
                    if (wall.Contains(new Point((int)furniture.tileLocation.X, (int)furniture.tileLocation.Y)))
                    {
                        wasOnWall = true;
                        break;
                    }
                }
                if (!wasOnWall)
                {
                    Logger.Log("Wall furniture was not on a wall!  Moving...");
                    List<Rectangle> walls = house.getWalls();
                    int wallToPlace = Game1.random.Next(walls.Count);
                    Vector2 placeSpot = Vector2.Zero;
                    int attempts = 0;
                    while (placeSpot == Vector2.Zero && attempts < 20)
                    {
                        int wallX = Game1.random.Next(walls[wallToPlace].Width) + walls[wallToPlace].X;
                        Vector2 testSpot = new Vector2(wallX, walls[wallToPlace].Y);
                        if (!house.isTileOccupiedForPlacement(testSpot, furniture))
                            placeSpot = testSpot;
                        attempts++;
                        Logger.Log("Suitable wall location not found!  Trying again...  Attempt #" + attempts);
                    }
                    if (placeSpot == Vector2.Zero)
                    {
                        Logger.Log("Could not find a suitable location for a piece of furniture.  Storing it in a gift box.");
                        itemsToStore.Add(furniture);
                        deadFurniture.Add(furniture);
                    }
                    else
                    {
                        furniture.tileLocation.Value = new Vector2(placeSpot.X, placeSpot.Y);
                        reclaculateFurniture(furniture);
                        Logger.Log("Placing furniture at " + furniture.tileLocation.ToString());
                    }
                }
            }
            else if (!house.isTileOnMap(furniture.tileLocation) || !isFurnitureSpotValid(furniture, house))
            {
                Logger.Log("Furniture was stuck, moving...");
                Point newSpot = house.getRandomOpenPointInHouse(Game1.random);
                int attempts = 0;
                while ((newSpot == Point.Zero || !furniture.canBePlacedHere(house, new Vector2(newSpot.X, newSpot.Y)) || !isCompletelyClear(furniture, new Vector2(newSpot.X, newSpot.Y), house) || isTileVoid(house, new Vector2(newSpot.X, newSpot.Y)) || (furniture.furniture_type == StardewValley.Objects.Furniture.window)) && attempts < 20)
                {
                    newSpot = house.getRandomOpenPointInHouse(Game1.random);
                    attempts++;
                    Logger.Log("Failed to find empty open spot in the farmhouse for a piece of furniture.  Attempt #" + attempts);
                }
                if (newSpot == Point.Zero)
                {
                    Logger.Log("Could not find a suitable location for a piece of furniture.  Storing it in a gift box.");
                    itemsToStore.Add(furniture);
                    deadFurniture.Add(furniture);
                }
                else
                {
                    furniture.tileLocation.Value = new Vector2(newSpot.X, newSpot.Y);
                    reclaculateFurniture(furniture);
                    Logger.Log("Placing furniture at " + furniture.tileLocation.ToString());
                }
                if(!isFurnitureSpotValid(furniture, house) && placementAttempt < 20)
                {
                    Logger.Log("Placement failed, spot was invalid.  Retrying...  Attempt #" + placementAttempt);
                    fixFurniture(furniture, house, ref deadFurniture, ref itemsToStore, placementAttempt + 1);
                }
            }
            else
            {
                //Logger.Log("Skipping " + furniture.tileLocation.ToString() + " because it " + (house.isTileOnMap(furniture.tileLocation) ? "was on the map, and " : "was not on the map, and ") + (!canBePlaced ? "was not placeable." : "was placeable."));
                Logger.Log("Skipping " + furniture.name + " at " + furniture.tileLocation.ToString() + ".");
            }
        }

        internal bool isFurnitureSpotValid(StardewValley.Objects.Furniture furniture, FarmHouse house)
        {
            if(furniture.furniture_type == StardewValley.Objects.Furniture.rug)
            {
                Logger.Log(furniture.name + " was rug, looking for placement location...");
                for(int x = furniture.boundingBox.X; x < furniture.boundingBox.Right; x++)
                {
                    for(int y = furniture.boundingBox.Y; y < furniture.boundingBox.Bottom; y++)
                    {
                        if (!isTilePassableForRugs(house, new xTile.Dimensions.Location(x, y), Game1.viewport))
                        {
                            Logger.Log("Tile (" + x + ", " + y + ") was not passable for rugs!");
                            return false;
                        }
                    }
                }
                Logger.Log("Rug can be placed here.");
                return true;
            }
            else
            {
                Logger.Log(furniture.name + " was not rug, was " + furniture.furniture_type);
            }
            Vector2 realLocation = furniture.tileLocation;
            furniture.tileLocation.Value = Vector2.Zero;
            reclaculateFurniture(furniture);
            bool isValid = furniture.canBePlacedHere(house, realLocation);
            bool isPartiallyStuck = !isCompletelyClear(furniture, realLocation, house);
            bool isVoid = isTileVoid(house, realLocation);
            Logger.Log("Spot valid? " + isValid.ToString() + " Partially Stuck? " + isPartiallyStuck.ToString() + " Void? " + isVoid.ToString());
            furniture.tileLocation.Value = realLocation;
            reclaculateFurniture(furniture);
            return isValid && !isPartiallyStuck && !isVoid;
        }

        internal bool isCompletelyClear(StardewValley.Objects.Furniture furniture, Vector2 point, FarmHouse house)
        {

            for (int x1 = (int)point.X; x1 < point.X + furniture.getTilesWide(); ++x1)
            {
                for (int y1 = (int)point.Y; y1 < point.Y + furniture.getTilesHigh(); ++y1)
                {
                    if (house.doesTileHaveProperty(x1, y1, "NoFurniture", "Back") != null)
                    {
                        return false;
                    }
                    if (house.getTileIndexAt(x1, y1, "Buildings") != -1)
                        return false;
                }
            }
            return true;
        }

        internal bool isObjectSpotValid(StardewValley.Object candidate, Vector2 point, FarmHouse house)
        {
            Logger.Log("Testing validity of " + candidate.name + "...");
            bool passable = isPointPassableIgnoreObjects(point, house);
            Logger.Log("Tested passability of tile " + point.ToString() + ": " + passable.ToString());
            bool placeable = house.isTilePlaceable(point);
            Logger.Log("Was " + (passable ? "" : "not ") + "passable, and " + (placeable ? "" : "not ") + "placeable.");

            return placeable && passable;
        }

        internal bool isTileVoid(FarmHouse house, Vector2 location)
        {
            Logger.Log("Checking if " + location.ToString() + " is in the void...");
            //This spot is not even on the map, so it's definitely the void
            if (!house.isTileOnMap(location))
            {
                Logger.Log("Tile is off the map.");
                return true;
            }

            Map map = house.map;
            //There's a tile here
            if(map.GetLayer("Back").Tiles[(int)location.X, (int)location.Y] != null)
            {
                Logger.Log("Tile exists...");
                int tileIndex = map.GetLayer("Back").Tiles[(int)location.X, (int)location.Y].TileIndex;
                //The void tile is on index 0 on both sheets, so we can exit out as soon as we see it's not 0
                if (tileIndex != 0)
                {
                    Logger.Log("Tile of index " + tileIndex + " was not 0, so not void.");
                    return false;
                }
                //Get the image source for the tilesheet.  This allows people to name the sheets anything they want
                string sheetSource = map.GetLayer("Back").Tiles[(int)location.X, (int)location.Y].TileSheet.ImageSource;
                //The void tiles are found in townInterior and farmhouse_tiles
                Logger.Log("Tile was on townInterior? " + sheetSource.Contains("townInterior").ToString() + " Tile was on farmhouse_tiles? " + sheetSource.Contains("farmhouse_tiles").ToString());
                return (sheetSource.Contains("townInterior") || sheetSource.Contains("farmhouse_tiles"));
            }
            else
            {
                Logger.Log("Tile was null.");
                return true;
            }
        }

        internal bool isPointPassableIgnoreObjects(Vector2 location, FarmHouse house)
        {
            if (!house.isTileOnMap(location))
                return false;
            Map map = house.map;

            if(map.GetLayer("Back").Tiles[(int)location.X, (int)location.Y] != null)
            {
                //Tile flagged as impassable on the Back layer.
                if (map.GetLayer("Back").Tiles[(int)location.X, (int)location.Y].TileIndexProperties.ContainsKey("Passable"))
                    return false;
            }
            if (map.GetLayer("Buildings").Tiles[(int)location.X, (int)location.Y] != null)
            {
                //Tile on buildings layer is not marked as Shadow or Passable.
                if (!map.GetLayer("Buildings").Tiles[(int)location.X, (int)location.Y].TileIndexProperties.ContainsKey("Shadow") && !map.GetLayer("Buildings").Tiles[(int)location.X, (int)location.Y].TileIndexProperties.ContainsKey("Passable"))
                    return false;
            }
            return !isTileVoid(house, location);
        }

        internal void reclaculateFurniture(StardewValley.Objects.Furniture furniture)
        {
            furniture.boundingBox.X = (int)furniture.tileLocation.Value.X * 64;
            furniture.boundingBox.Y = (int)furniture.tileLocation.Value.Y * 64;
            furniture.updateDrawPosition();
        }

        //Slightly modified default game code, necessary for having control over the placement rules for rugs
        internal bool isTilePassableForRugs(FarmHouse house, xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport)
        {
            xTile.ObjectModel.PropertyValue propertyValue = (xTile.ObjectModel.PropertyValue)null;
            xTile.Tiles.Tile tile1 = house.map.GetLayer("Back").PickTile(tileLocation, viewport.Size);
            if (tile1 != null)
                tile1.TileIndexProperties.TryGetValue("Passable", out propertyValue);
            xTile.Tiles.Tile tile2 = house.map.GetLayer("Buildings").PickTile(tileLocation, viewport.Size);
            if (propertyValue == null && tile2 == null)
                return tile1 != null;
            return false;
        }

        internal void fixSpousePosition(FarmHouse house)
        {
            if (!Game1.player.isMarried() || Game1.player.getSpouse().currentLocation != house)
                return;
            NPC spouse = Game1.player.getSpouse();
            if (spouse.getTileLocationPoint() == house.getKitchenStandingSpot())
            {
                //Spouse was placed at the kitchen standing spot, move them to modded one.
                Logger.Log("Spouse was in kitchen standing spot...");
                Point kitchenLocation = FarmHouseStates.getKitchenSpot(house);
                spouse.setTilePosition(kitchenLocation);
                Logger.Log(spouse.name + " was moved to " + kitchenLocation.ToString());
            }
            else if (isSpouseInSpouseRoom(spouse, house))
            {
                Logger.Log(spouse.name + " began the day in the spouse room...");
                FarmHouseState state = FarmHouseStates.getState(house);
                if (state.spouseRoomData == null)
                    FarmHouseStates.updateFromMapPath(house, house.mapPath);
                if (state.spouseRoomData != "")
                {
                    Logger.Log("Map defined spouse room location...");
                    string[] spouseRoomPoint = state.spouseRoomData.Split(' ');
                    Vector2 spouseRoomLocation = new Vector2(Convert.ToInt32(spouseRoomPoint[0]) + 3, Convert.ToInt32(spouseRoomPoint[1]) + 4);
                    spouse.setTileLocation(spouseRoomLocation);
                    Logger.Log(spouse.name + " was moved to " + spouseRoomLocation.ToString());
                }
                else
                {
                    Logger.Log("Map did not define spouse room location.");
                }
            }
            else if (!house.isTileLocationOpen(new xTile.Dimensions.Location(spouse.getTileX(), spouse.getTileY())))
            {
                Logger.Log(spouse.name + " was in the void or was stuck, relocating...");
                spouse.setTilePosition(house.getRandomOpenPointInHouse(Game1.random));
            }
        }

        internal bool isSpouseInSpouseRoom(NPC spouse, FarmHouse house)
        {
            Point tileLocation = spouse.getTileLocationPoint();
            Point spouseRoomLocation = house.upgradeLevel == 1 ? new Point(32, 5) : new Point(38, 14);
            return tileLocation.Equals(spouseRoomLocation);
        }

        internal void setPlayerByBed(FarmHouse house)
        {
            if (Game1.player.currentLocation.Equals(house))
            {
                Point bedPoint = FarmHouseStates.getBedSpot(house, false);
                Vector2 bedLocation = new Vector2(bedPoint.X, bedPoint.Y);
                Game1.player.setTileLocation(bedLocation);
                Logger.Log("Bed set to " + bedLocation.ToString());
            }
        }
    }
}


//HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
//harmony.Patch(
//   method: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.getBedSpot)),
//   postfix: AccessTools.Method(typeof(FarmHouse_getBedSpot_patch), nameof(FarmHouse_getBedSpot_patch.Postfix))
//);
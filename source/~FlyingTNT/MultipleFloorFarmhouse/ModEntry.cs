/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FlyingTNT/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using xTile;
using MultipleFloorFarmhouse;
using Common.Integrations;
using xTile.Tiles;
using StardewValley.Extensions;

namespace MultiStoryFarmhouse
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public const string BaseStairsUpName = "Maps/MultipleFloorFarmhouseBaseStairsUp";
        public const string StairsUpInlineName = "Maps/MultipleFloorFarmhouseStairsUpInline";
        public const string StairsUpName = "Maps/MultipleFloorFarmhouseStairsUp";
        public const string StairsDownName = "Maps/MultipleFloorFarmhouseStairsDown";

        public static IMonitor SMonitor;
        private static IModHelper SHelper;

        /// <summary> A dictionary of all of the Floors in all of the installed content packs, even if they aren't in use.</summary>
        private static readonly Dictionary<string, Floor> floorsDict = new();

        /// <summary> A floorNumber => Map dictionary of the floors currently in use. </summary>
        private static readonly Dictionary<int, Map> floorMaps = new();

        /// <summary> The floors enabled in the config when the game was initially loaded. </summary>
        private static string[] CachedConfigFloors;

        private static int NumberOfFloors => GetPossibleFloors().Count;

        public static ModConfig Config;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            if (!Config.EnableMod)
                return;

            SMonitor = Monitor;
            SHelper = helper;
            CachedConfigFloors = Config.FloorNames.Split(',', StringSplitOptions.TrimEntries);

            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            Helper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;
            Helper.Events.Content.AssetRequested += Content_AssetRequested;

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getWalls)),
               prefix: new HarmonyMethod(typeof(CodePatches), nameof(CodePatches.GameLocation_getWalls_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(DecoratableLocation), nameof(DecoratableLocation.getFloors)),
               prefix: new HarmonyMethod(typeof(CodePatches), nameof(CodePatches.DecorableLocation_getFloors_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(SaveGame), nameof(SaveGame.loadDataToLocations)),
               prefix: new HarmonyMethod(typeof(CodePatches), nameof(CodePatches.SaveGame_loadDataToLocations_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(FarmHouse), "resetLocalState"),
               prefix: new HarmonyMethod(typeof(CodePatches), nameof(CodePatches.FarmHouse_resetLocalState_Prefix)),
               postfix: new HarmonyMethod(typeof(CodePatches), nameof(CodePatches.FarmHouse_resetLocalState_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.CanPlaceThisFurnitureHere)),
               prefix: new HarmonyMethod(typeof(CodePatches), nameof(CodePatches.GameLocation_CanPlaceThisFurnitureHere_Prefix))
            );
        }

        public static List<string> GetPossibleFloors()
        {
            return CachedConfigFloors.Where(s => floorsDict.ContainsKey(s)).ToList();
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            foreach (IContentPack contentPack in SHelper.ContentPacks.GetOwned())
            { 
                FloorsData floorsData = contentPack.ReadJsonFile<FloorsData>("content.json");
                if (floorsData is null)
                    continue;

                foreach (Floor floor in floorsData.floors)
                {
                    try
                    {
                        for(int i = 0; i < CachedConfigFloors.Length; i++)
                        {
                            if (floor.name.ToLower() == CachedConfigFloors[i].ToLower())
                            {
                                SMonitor.Log($"Setting floor {i} map to {floor.name}.");
                                floorMaps[i] = contentPack.ModContent.Load<Map>(floor.mapPath);
                            }
                        }
                        floorsDict.Add(floor.name, floor);
                    }
                    catch(Exception ex)
                    {
                        SMonitor.Log($"Exception getting map at {floor.mapPath} for {floor.name} in content pack {contentPack.Manifest.Name}:\n{ex}", LogLevel.Error);
                    }
                }
            }
            SMonitor.Log($"Loaded {floorsDict.Count} floors.");
            SMonitor.Log($"The installed floors are: {floorsDict.Join(kvp => kvp.Key)}", LogLevel.Debug);
            SMonitor.Log($"The active floors are: {GetPossibleFloors().Join()}", LogLevel.Debug);


            var configMenu = SHelper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => SHelper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("GMCM_EnableMod"),
                getValue: () => Config.EnableMod,
                setValue: value => Config.EnableMod = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("GMCM_StairsX"),
                tooltip: () => SHelper.Translation.Get("GMCM_Stairs_Description"),
                getValue: () => Config.MainFloorStairsX,
                setValue: value => Config.MainFloorStairsX = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("GMCM_StairsY"),
                tooltip: () => SHelper.Translation.Get("GMCM_Stairs_Description"),
                getValue: () => Config.MainFloorStairsY,
                setValue: value => Config.MainFloorStairsY = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("GMCM_CombineWithCellarStairs"),
                getValue: () => Config.CombineWithCellarStairs,
                setValue: value => Config.CombineWithCellarStairs = value
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => SHelper.Translation.Get("GMCM_FloorNames"),
                tooltip: () => SHelper.Translation.Get("GMCM_FloorNames_Description", new {allOptions = floorsDict.Join(kvp => kvp.Key)}),
                getValue: () => Config.FloorNames,
                setValue: value => Config.FloorNames = value
            );

            configMenu.AddParagraph(
                mod: ModManifest,
                text: () => SHelper.Translation.Get("GMCM_ActiveFloors", new {activeFloors = GetPossibleFloors().Join()})
            );
        }

        private void GameLoop_ReturnedToTitle(object sender, ReturnedToTitleEventArgs args)
        {
            // So that the stair locations are updated on save reload.
            // We don't do this whenever the config is changed because that has the side effect of removing the cellar stairs and spouse rooms.
            SHelper.GameContent.InvalidateCache("Maps/FarmHouse2");
            SHelper.GameContent.InvalidateCache("Maps/FarmHouse2_marriage");

            // So that its warp is updated
            SHelper.GameContent.InvalidateCache("Maps/MultipleFloorsMap0");
        }

        private void Content_AssetRequested(object sender, AssetRequestedEventArgs args)
        {
            if (args.NameWithoutLocale.StartsWith("Maps/MultipleFloorsMap"))
            {
                int floorNo = int.Parse(args.NameWithoutLocale.ToString()[^1].ToString());

                SMonitor.Log($"Loading floor {floorNo} map");
                Map map = floorMaps[floorNo];

                AddStairs(map, floorNo);
                AddWarps(map, floorNo);
                args.LoadFrom(() => map, AssetLoadPriority.Medium);
            }

            else if (args.NameWithoutLocale.IsEquivalentTo("Maps/FarmHouse2") || args.NameWithoutLocale.IsEquivalentTo("Maps/FarmHouse2_marriage"))
            {
                SMonitor.Log("Editing asset" + args.Name);

                try
                {
                    args.Edit(asset =>
                    {
                        var mapData = asset.AsMap();

                        int x = Config.MainFloorStairsX;
                        int y = Config.MainFloorStairsY;

                        Map stairs = SHelper.GameContent.Load<Map>(Config.CombineWithCellarStairs ? BaseStairsUpName : StairsUpInlineName);

                        if (Game1.player?.HouseUpgradeLevel < 3 && Config.CombineWithCellarStairs)
                        {
                            try
                            {
                                // Front1 has tiles that merge the cellar and upper floor stairs
                                stairs.RemoveLayer(stairs.GetLayer("Front1"));
                            }
                            catch { }
                        }

                        try
                        {
                            mapData.PatchMap(stairs, null, new Rectangle(x, y, stairs.RequireLayer("Back").LayerWidth, stairs.RequireLayer("Back").LayerHeight), PatchMapMode.Replace);
                        }
                        catch(ArgumentOutOfRangeException ex)
                        {
                            SMonitor.Log($"There was an error adding the stairs, probably due to invalid config for the stair location: {ex}", LogLevel.Error);
                            return;
                        }

                        if (!TryGetFloor(0, out Floor floor0))
                            return;

                        Point nextFloorStairsPoint = floor0.stairsStart.ToPoint();
                        Utilities.AddWarp(mapData.Data, "MultipleFloors0", Config.MainFloorStairsX + 1, Config.MainFloorStairsY + 3, nextFloorStairsPoint.X + 1, nextFloorStairsPoint.Y + 2);
                        Utilities.AddWarp(mapData.Data, "MultipleFloors0", Config.MainFloorStairsX + 2, Config.MainFloorStairsY + 3, nextFloorStairsPoint.X + 2, nextFloorStairsPoint.Y + 2);

                        SMonitor.Log(mapData.Data.Properties["Warp"]);
                    }, AssetEditPriority.Late);
                }
                catch (Exception ex)
                {
                    SMonitor.Log($"Exception adding stair tiles.\n{ex}", LogLevel.Error);
                }
            }
            else if(args.NameWithoutLocale.IsEquivalentTo(StairsUpName))
            {
                args.LoadFrom(() => SHelper.ModContent.Load<Map>("assets/Maps/StairsUp.tmx"), AssetLoadPriority.Medium);
            }
            else if (args.NameWithoutLocale.IsEquivalentTo(StairsDownName))
            {
                args.LoadFrom(() => SHelper.ModContent.Load<Map>("assets/Maps/StairsDown.tmx"), AssetLoadPriority.Medium);
            }
            else if (args.NameWithoutLocale.IsEquivalentTo(BaseStairsUpName))
            {
                args.LoadFrom(() => SHelper.ModContent.Load<Map>("assets/Maps/BaseStairsUp.tmx"), AssetLoadPriority.Medium);
            }
            else if (args.NameWithoutLocale.IsEquivalentTo(StairsUpInlineName))
            {
                args.LoadFrom(() => SHelper.ModContent.Load<Map>("assets/Maps/StairsUpInline.tmx"), AssetLoadPriority.Medium);
            }
        }

        private static void AddStairs(Map map, int floorNumber)
        {
            Map stairsUp = SHelper.GameContent.Load<Map>(StairsUpName);
            Map stairsDown = SHelper.GameContent.Load<Map>(StairsDownName);

            if (!TryGetFloor(floorNumber, out Floor floor))
                return;

            Vector2 stairs = floor.stairsStart;
            int x = (int)stairs.X;
            int y = (int)stairs.Y;

            IAssetDataForMap helper = SHelper.ModContent.GetPatchHelper(map).AsMap();

            // The patch will remove the flooring tiles, so we cache them and then re-add them after the patch
            Tile[] floorTiles = new Tile[4];
            for(int i = 0; i < 4; i++)
            {
                floorTiles[i] = map.GetLayer("Back").Tiles[x+i, y];
            }

            helper.PatchMap(stairsDown, null, new Rectangle(x, y, 4, 4), PatchMapMode.ReplaceByLayer);

            for (int i = 0; i < 4; i++)
            {
                map.GetLayer("Back").Tiles[x + i, y] = floorTiles[i];
            }

            if (floorNumber >= NumberOfFloors - 1)
                return;

            for (int i = 0; i < 4; i++)
            {
                floorTiles[i] = map.GetLayer("Back").Tiles[x + i + 3, y];
            }

            helper.PatchMap(stairsUp, null, new Rectangle(x + 3, y, 4, 4), PatchMapMode.ReplaceByLayer);

            for (int i = 0; i < 4; i++)
            {
                map.GetLayer("Back").Tiles[x + i + 3, y] = floorTiles[i];
            }
        }

        private static void AddWarps(Map map, int floorNumber)
        {
            if (!TryGetFloor(floorNumber, out Floor floor))
                return;

            Utilities.ClearWarps(map);

            Vector2 stairs = floor.stairsStart;
            int x = (int)stairs.X;
            int y = (int)stairs.Y;

            if (floorNumber == 0)
            {
                Utilities.AddWarp(map, "FarmHouse", x + 1, y + 3, Config.MainFloorStairsX + 1, Config.MainFloorStairsY + 2);
                Utilities.AddWarp(map, "FarmHouse", x + 2, y + 3, Config.MainFloorStairsX + 2, Config.MainFloorStairsY + 2);
            }
            else if(TryGetFloor(floorNumber - 1, out Floor downFloor))
            {
                Point downStairSpot = downFloor.stairsStart.ToPoint();

                Utilities.AddWarp(map, $"MultipleFloors{floorNumber - 1}", x + 1, y + 3, downStairSpot.X + 4, downStairSpot.Y + 2);
                Utilities.AddWarp(map, $"MultipleFloors{floorNumber - 1}", x + 2, y + 3, downStairSpot.X + 5, downStairSpot.Y + 2);
            }

            if (!TryGetFloor(floorNumber + 1, out Floor upFloor))
                return;

            Point upStairSpot = upFloor.stairsStart.ToPoint();

            Utilities.AddWarp(map, $"MultipleFloors{floorNumber + 1}", x + 4, y + 3, upStairSpot.X + 1, upStairSpot.Y + 2);
            Utilities.AddWarp(map, $"MultipleFloors{floorNumber + 1}", x + 5, y + 3, upStairSpot.X + 2, upStairSpot.Y + 2);
        }

        private static bool TryGetFloor(int floorIndex, [NotNullWhen(true)] out Floor floor)
        {
            if(floorIndex < 0 || floorIndex >= NumberOfFloors)
            {
                floor = null;
                return false;
            }

            floor = floorsDict[GetPossibleFloors()[floorIndex]];
            return floor is not null;
        }

        /// <summary>
        /// Tries to get the floor with the given name. Can be in either the form "MultipleFloors{floorNum}" or "{Floor.name}"
        /// </summary>
        public static bool TryGetFloor(string name, [NotNullWhen(true)] out Floor floor)
        {
            // Try to get it in the form Floor.name
            if(floorsDict.TryGetValue(name, out Floor floor1))
            {
                floor = floor1;
                return floor is not null;
            }

            // Try to get it in the form MultipleFloors{floorNum}
            if (!int.TryParse(name[^1].ToString(), System.Globalization.NumberStyles.None, null, out int floorNo))
            {
                floor = null;
                return false;
            }

            if(!TryGetFloor(floorNo, out Floor floor2))
            {
                floor = null;
                return false;
            }

            floor = floor2;
            return floor is not null;
        }
    }
}
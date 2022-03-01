/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Drawing;
using System.Linq;
using xTile;
using PlatoTK.Reflection;
using xTile.Tiles;
using xTile.Layers;
using HarmonyLib;

namespace MapTK.SpouseRooms
{
    internal class SpouseRoomHandler
    {
        private static IModHelper Helper;
        internal const string VanillaSpouseRoomMap = @"Maps\spouseRooms";
        internal static readonly string[] VanillaLayers = new string[] { "Back", "Buildings", "Front", "AlwaysFront" };


        public SpouseRoomHandler(IModHelper helper)
        {
            Helper = helper;
            SpouseRoomTokenX.Helper = helper;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            var api = Helper.ModRegistry.GetApi<PlatoTK.APIs.IContentPatcher>("Pathoschild.ContentPatcher");
            api.RegisterToken(Helper.ModRegistry.Get(Helper.ModRegistry.ModID).Manifest, "SpouseRoomX", new SpouseRoomTokenX());
            api.RegisterToken(Helper.ModRegistry.Get(Helper.ModRegistry.ModID).Manifest, "SpouseRoomY", new SpouseRoomTokenY());
            Harmony harmony = new Harmony("Platonymous.MapTK.SpouseRooms");

            harmony.Patch(
                AccessTools.Method(typeof(FarmHouse), "loadSpouseRoom"),
                new HarmonyMethod(typeof(SpouseRoomHandler), nameof(loadSpouseRoom))
                );
        }

        internal static bool loadSpouseRoom(FarmHouse __instance)
        {
            string spouse = __instance.owner.getSpouse()?.name.Value ?? __instance.owner.spouse;
            if (string.IsNullOrEmpty(spouse))
                return true;

            Microsoft.Xna.Framework.Rectangle rectangle = __instance.upgradeLevel == 1 ? new Microsoft.Xna.Framework.Rectangle(29, 1, 6, 9) : new Microsoft.Xna.Framework.Rectangle(35, 10, 6, 9);
            bool setSpot = false;
            if (__instance.Map.Properties.ContainsKey("@SpouseRoom") && __instance.map.Properties["@SpouseRoom"].ToString().Split(' ') is string[] parts
                && parts.Length == 2
                && parts.Select(s => int.Parse(s)).ToArray() is int[] pos)
            {
                setSpot = true;
                rectangle = new Microsoft.Xna.Framework.Rectangle(pos[0], pos[1], 6, 9);
            }

            Map spouseRoomMap = Helper.Content.Load<Map>(VanillaSpouseRoomMap, ContentSource.GameContent);

            int spot = GetIndexForSpouse(spouse);
            bool isVanilla = spot == -1;

            if (!setSpot && isVanilla)
                return true;

            Point point = new Point(spot % (isVanilla ? 5 : SpouseRoomTokenX.MaxSpouseRoomSpotsPerRow) * 6, spot / (isVanilla ? 5 : SpouseRoomTokenX.MaxSpouseRoomSpotsPerRow) * 9);
            __instance.map.Properties.Remove("DayTiles");
            __instance.map.Properties.Remove("NightTiles");
            for (int x = 0; x < rectangle.Width; ++x)
                for (int y = 0; y < rectangle.Height; ++y)
                    foreach (Layer layer in spouseRoomMap.Layers.Where(l => VanillaLayers.Contains(l.Id) || l.Properties.ContainsKey("@DrawBefore") || l.Properties.ContainsKey("@DrawAfter")))
                        if (layer.Id != "Front" || (y < rectangle.Height - 1))
                        {
                            CopyTile(layer.Tiles[point.X + x, point.Y + y], rectangle.X + x, rectangle.Y + y, __instance.map, layer.Id);

                            if (layer.Id == "Buildings" && layer.Tiles[point.X + x, point.Y + y] != null)
                                __instance.CallAction("adjustMapLightPropertiesForLamp", spouseRoomMap.GetLayer("Buildings").Tiles[point.X + x, point.Y + y].TileIndex, rectangle.X + x, rectangle.Y + y, "Buildings");

                            if (layer.Id == "Front" && layer.Tiles[point.X + x, point.Y + y] != null)
                                __instance.CallAction("adjustMapLightPropertiesForLamp", spouseRoomMap.GetLayer("Front").Tiles[point.X + x, point.Y + y].TileIndex, rectangle.X + x, rectangle.Y + y, "Front");
                        }

            __instance.map.GetLayer("Back").Tiles[rectangle.X + 4, rectangle.Y + 4].Properties["NoFurniture"] = "T";
            
            return false;
        }

        internal static int GetIndexForSpouse(string name)
        {
            if (SpouseRoomTokenX.SpouseRooms.FirstOrDefault(s => s.Name == name) is SpouseRoomPlacement placement)
                return placement.Spot;
            return -1;
        }

        internal static xTile.Dimensions.Size GetMaxLayerSize(Map targetMap)
        {
            xTile.Dimensions.Size maxSize = new xTile.Dimensions.Size(0, 0);

            targetMap.Layers.Select(l => l.LayerSize).ToList().ForEach(s => {
                maxSize.Width = (int)Math.Max(maxSize.Width, s.Width);
                maxSize.Height = (int)Math.Max(maxSize.Height, s.Height);
            });

            return maxSize;
        }

        internal static void CopyTile(Tile originTile, int x, int y, Map targetMap, string layer = "")
        {
            if (layer == "")
                layer = originTile?.Layer.Id;

            xTile.Dimensions.Size maxSize = GetMaxLayerSize(targetMap);

            Layer targetLayer;

            if (targetMap.Layers.FirstOrDefault(l => l.Id == layer) is Layer tLayer)
                targetLayer = tLayer;
            else if (originTile != null)
            {
                targetLayer = new Layer(layer, targetMap, maxSize, originTile.Layer.TileSize);
                foreach (var property in originTile.Layer.Properties)
                    targetLayer.Properties.Add(property.Key, property.Value);
                targetMap.AddLayer(targetLayer);
            }
            else
                return;

            if (originTile == null)
                targetLayer.Tiles[x, y] = null;
            else
            if (originTile is AnimatedTile animatedTile)
            {
                foreach (TileSheet ts in animatedTile.TileFrames.Select(frame => frame.TileSheet))
                    if (!targetMap.TileSheets.Any(t => t.Id == ts.Id))
                        targetMap.AddTileSheet(new TileSheet(ts.Id, targetMap, ts.ImageSource, ts.SheetSize, ts.TileSize));

                var newTile =
                    new AnimatedTile(targetLayer,
                    animatedTile.TileFrames
                    .Select(frame => new StaticTile(targetLayer, targetMap.TileSheets.First(t => t.Id == frame.TileSheet.Id), frame.BlendMode, frame.TileIndex))
                    .ToArray()
                    , animatedTile.FrameInterval);

                foreach (var property in originTile.Properties)
                    newTile.Properties.Add(property.Key, property.Value);

                targetLayer.Tiles[x, y] = newTile;
            }
            else if (originTile is StaticTile staticTile)
            {
                if (!targetMap.TileSheets.Any(t => t.Id == staticTile.TileSheet.Id))
                    targetMap.AddTileSheet(new TileSheet(staticTile.TileSheet.Id, targetMap, staticTile.TileSheet.ImageSource, staticTile.TileSheet.SheetSize, staticTile.TileSheet.TileSize));
                var newTile = new StaticTile(targetLayer, targetMap.TileSheets.First(t => t.Id == staticTile.TileSheet.Id), staticTile.BlendMode, staticTile.TileIndex);
                
                foreach (var property in originTile.Properties)
                    newTile.Properties.Add(property.Key, property.Value);
                
                targetLayer.Tiles[x, y] = newTile;
            }

        }
    }
}

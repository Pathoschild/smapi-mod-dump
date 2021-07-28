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
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using xTile.Layers;
using xTile.ObjectModel;
using Harmony;
using PlatoTK;
using TMXTile;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using xTile.Display;

namespace MapTK.MapExtras
{
    internal class MapExtrasHandler
    {
        internal static readonly Dictionary<Layer, List<Layer>> DrawBeforeCache = new Dictionary<Layer, List<Layer>>();
        internal static readonly Dictionary<Layer, List<Layer>> DrawAfterCache = new Dictionary<Layer, List<Layer>>();
        internal static readonly Dictionary<Layer, Vector2> ScrollModifier = new Dictionary<Layer, Vector2>();
        internal static MapTKDisplayDevice MapDisplayDevice;
        internal static Point LastViewport;

        internal const string MapMergeDirectory = @"MapTK/Merges";
        internal const string UseProperty = "@As";
        internal const string UseConditionProperty = "@As_Conditions";
        internal static IPlatoHelper Plato { get; set; }
        public MapExtrasHandler(IModHelper helper)
        {
            Plato = helper.GetPlatoHelper();
            helper.Events.GameLoop.SaveLoaded += InitializeExtraLayers;
            helper.Events.GameLoop.SaveCreated += InitializeExtraLayers;
            helper.Events.Player.Warped += SetExtraLayersOnWarp;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.DayStarted += InvalidateMerges;
            helper.Events.GameLoop.SaveLoaded += InvalidateMerges;
            helper.Events.GameLoop.SaveCreated += InvalidateMerges;
            var harmony = HarmonyInstance.Create("Platonymous.MapTK.ExtraLayers");
            harmony.Patch(AccessTools.Method(typeof(GameLocation), "reloadMap"),postfix:new HarmonyMethod(typeof(MapExtrasHandler),nameof(AfterMapReload)));
        }

        private void InvalidateMerges(object sender, EventArgs e)
        {
            Plato.ModHelper.Content.InvalidateCache(MapMergeDirectory);
            MapMergerIAssetEditor.MapMergeDataSet = Plato.ModHelper.Content.Load<Dictionary<string, MapMergeData>>(MapMergeDirectory, ContentSource.GameContent);
            var mm = Plato.ModHelper.Content.Load<Dictionary<string, MapMergeData>>(MapMergeDirectory, ContentSource.GameContent);
            mm.Select(s => s.Value).ToList().ForEach(m => Plato.ModHelper.Content.InvalidateCache(m.Target));
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Plato.ModHelper.Content.AssetLoaders.Add(new MapMergerIAssetEditor(Plato.ModHelper));
            Plato.ModHelper.Content.AssetEditors.Add(new MapMergerIAssetEditor(Plato.ModHelper));
            LastViewport = Game1.viewportCenter;
            if (xTile.Format.FormatManager.Instance.GetMapFormatByExtension("tmx") is TMXFormat tmxf)
                tmxf.DrawImageLayer = DrawImageLayer;

            var api = Plato.ModHelper.ModRegistry.GetApi<PlatoTK.APIs.IContentPatcher>("Pathoschild.ContentPatcher");
            api.RegisterToken(Plato.ModHelper.ModRegistry.Get(Plato.ModHelper.ModRegistry.ModID).Manifest, "Merge", new MapMergeToken());

            SetMapDisplayDevice();
        }

        private void SetMapDisplayDevice()
        {
            HarmonyInstance instance = HarmonyInstance.Create("MatpTK.MapTKDisplayDevice");

            instance.Patch(
                   original: AccessTools.Method(Type.GetType("StardewModdingAPI.Framework.SCore, StardewModdingAPI"), "GetMapDisplayDevice"),
                   postfix: new HarmonyMethod(typeof(PlatoTKMod), nameof(PatchMapDisplayDevice))
               );

            if (MapDisplayDevice == null)
                MapDisplayDevice = new MapTKDisplayDevice(Game1.content, Game1.graphics.GraphicsDevice, MapTKMod.CompatOptions.Contains("SpriteMaster"));

            Game1.mapDisplayDevice = MapDisplayDevice;
        }

        internal static void PatchMapDisplayDevice(ref IDisplayDevice __result)
        {
            if (MapDisplayDevice == null)
                MapDisplayDevice = new MapTKDisplayDevice(Game1.content, Game1.graphics.GraphicsDevice, MapTKMod.CompatOptions.Contains("SpriteMaster"));

            __result = MapDisplayDevice;
        }

       

        internal static void AfterMapReload(GameLocation __instance)
        {
            __instance?.Map.Layers
                .ToList()
                .ForEach(AttachLayerDrawHandlers);
        }

        private void SetExtraLayersOnWarp(object sender, WarpedEventArgs e)
        {
            DrawBeforeCache.Clear();
            DrawAfterCache.Clear();
            ScrollModifier.Clear();

            e.NewLocation?.Map.Layers
                .ToList()
                .ForEach((l) =>
                {
                    ScrollModifier.Add(l, Vector2.Zero);
                    AttachLayerDrawHandlers(l);
                });

            LastViewport = Game1.viewportCenter;
        }

        private void InitializeExtraLayers(object sender, EventArgs e)
        {
            DrawBeforeCache.Clear();
            DrawAfterCache.Clear();

            Game1.locations
                .Select(l => l.Map)
                .SelectMany(m => m.Layers)
                .ToList()
                .ForEach(AttachLayerDrawHandlers);
        }

        internal static void AttachLayerDrawHandlers(Layer layer)
        {
            layer.AfterDraw -= Layer_AfterDraw;
            layer.BeforeDraw -= Layer_BeforeDraw;
            layer.AfterDraw += Layer_AfterDraw;
            layer.BeforeDraw += Layer_BeforeDraw;
        }

        private static void Layer_BeforeDraw(object sender, LayerEventArgs layerEventArgs)
        {
            Layer_Draw(layerEventArgs, "DrawBefore", DrawBeforeCache);
        }

        private static void Layer_AfterDraw(object sender, LayerEventArgs layerEventArgs)
        {
            Layer_Draw(layerEventArgs, "DrawAfter", DrawAfterCache);
        }

        private static void Layer_Draw(LayerEventArgs layerEventArgs, string drawProperty, Dictionary<Layer, List<Layer>> cache)
        {
            if (cache.TryGetValue(layerEventArgs.Layer, out List<Layer> extraLayers))
            {
                extraLayers.ForEach((extralayer) => DrawExtraLayer(extralayer, layerEventArgs));
                return;
            }

            cache.Add(layerEventArgs.Layer, new List<Layer>());

            layerEventArgs.Layer.Map.Layers
                .Where(l => l.Properties.TryGetValue(UseProperty, out PropertyValue value) 
                            && value.ToString().Split(' ') is string[] p &&
                            p.Length >= 2
                            && p[0] == drawProperty
                            && p[1] == layerEventArgs.Layer.Id
                            && (!l.Properties.TryGetValue(UseConditionProperty, out PropertyValue conditions) || Plato.CheckConditions(conditions.ToString(), l))
                            )
                .ToList()
                .ForEach((l) => {
                    if (!cache[layerEventArgs.Layer].Contains(l))
                        cache[layerEventArgs.Layer].Add(l);
                });

            Layer_Draw(layerEventArgs, drawProperty, cache);
        }

        private static void DrawExtraLayer(Layer layer, LayerEventArgs layerEventArgs)
        {
            if(layer.Id != "Front")
                layer.Draw(Game1.mapDisplayDevice, layerEventArgs.Viewport, new xTile.Dimensions.Location(0, 0), false, Game1.pixelZoom);
        }

        internal static void DrawImageLayer(Layer layer, xTile.Dimensions.Rectangle viewport)
        {
            if (Game1.mapDisplayDevice is MapTKDisplayDevice device
                && layer.GetTileSheetForImageLayer() is xTile.Tiles.TileSheet tilesheet
                && device.GetTexture(tilesheet) is Texture2D texture)
            {
                var offset = layer.GetOffset();
                Point size = new Point(texture.Width, texture.Height);
                var bounds = new Rectangle(offset.X, offset.Y, size.X, size.Y);

                foreach(Vector2 scrollModifier in GetScrollModifier(layer, bounds))
                {
                    Vector2 global = new Vector2(offset.X, offset.Y) + scrollModifier;
                    bounds = new Rectangle((int)global.X, (int)global.Y, size.X, size.Y);
                    Vector2 parallaxModifier = GetParallaxModifier(layer);
                    Vector2 local = Game1.GlobalToLocal(global + parallaxModifier);

                    Point position = Utility.Vector2ToPoint(local);

                    Color color = Color.White;
                    if (layer.GetColor() is TMXColor c)
                        color = new Color(c.R, c.G, c.B, c.A);

                    Rectangle dest = new Rectangle(position.X, position.Y, texture.Width * Game1.pixelZoom, texture.Height * Game1.pixelZoom);
                    Game1.spriteBatch.Draw(texture, dest, color * layer.GetOpacity());
                }
            }
        }

        private static List<Vector2> GetScrollModifier(Layer layer, Rectangle bounds)
        {
            var modifier = new List<Vector2>() { Vector2.Zero };

            if (layer.Properties.TryGetValue("@AutoScroll", out PropertyValue value)
                && ScrollModifier.ContainsKey(layer)
                && value.ToString().Split(' ') is string[] values
                && values.Length > 1
                && float.TryParse(values[0], out float x)
                && float.TryParse(values[1], out float y))
            {
                modifier.Clear();
                ScrollModifier[layer] += new Vector2(x, y);
                var pos = new Vector2(bounds.X + ScrollModifier[layer].X, bounds.Y + ScrollModifier[layer].Y);
                List<Vector2> vectors = new List<Vector2>();
                vectors.Add(ScrollModifier[layer]);

                if (x != 0)
                {
                    float px = ScrollModifier[layer].X;

                    while ((px + bounds.X) < 0)
                    {
                        if ((px + bounds.X) < 0)
                            px += layer.DisplayWidth;

                        vectors.Add(new Vector2(px, ScrollModifier[layer].Y));
                    }

                    px = ScrollModifier[layer].X;

                        while((px + bounds.X + bounds.Width) > layer.DisplayHeight)
                    {
                            px -= layer.DisplayHeight;
                        vectors.Add(new Vector2(px, ScrollModifier[layer].Y));
                    }
                }

                if (y != 0)
                {
                    float py = ScrollModifier[layer].Y;

                    while ((py + bounds.Y) < 0)
                    {
                        py += layer.DisplayHeight;
                        vectors.Add(new Vector2(ScrollModifier[layer].X, py));
                    }

                    py = ScrollModifier[layer].Y;

                    if ((py + bounds.Y + bounds.Height) > layer.DisplayHeight)
                        while ((py + bounds.Y + bounds.Height) > layer.DisplayHeight)
                        {
                            py -= layer.DisplayHeight;
                            vectors.Add(new Vector2(ScrollModifier[layer].X, py));
                        }
                }

                modifier.Clear();

                int c = vectors.Count();

                modifier.AddRange( c > 5 ? vectors.Skip(c-5): vectors);
            }
            return modifier;
        }

            private static Vector2 GetParallaxModifier(Layer layer)
        {
            if (layer.Properties.TryGetValue("@Parallax", out PropertyValue value))
            {
                return Vector2.Zero;
            }

            return Vector2.Zero;
        }
    }
}

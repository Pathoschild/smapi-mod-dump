/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/yuri-moens/LadderLocator
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Object = StardewValley.Object;

namespace LadderLocator
{
    internal class ModEntry : Mod
    {
        private static List<int> _selectedNodeTypeIndices;
        private static Dictionary<Vector2, Stone> _nodeStones;
        private static ModConfig _config;
        private static Texture2D _pixelTexture;
        private static Texture2D _imageTexture;
        private static List<LadderStone> _ladderStones;
        private static bool _nextIsShaft;

        public override void Entry(IModHelper helper)
        {
            _config = Helper.ReadConfig<ModConfig>();
            _pixelTexture = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            var colorArray = Enumerable.Range(0, 1).Select(i => Color.White).ToArray();
            _pixelTexture.SetData(colorArray);
            _imageTexture = helper.Content.Load<Texture2D>(_config.HighlightImageFilename, ContentSource.ModFolder);
            _ladderStones = new List<LadderStone>();
            _selectedNodeTypeIndices = RadarNode.All.Where(radarNode => _config.NodeTypes.Contains(radarNode.type))
                .OrderBy(radarNode => radarNode.value).Select(radarNode => radarNode.spriteIndex).ToList();
            _nodeStones = new Dictionary<Vector2, Stone>();

            Helper.Events.World.ObjectListChanged += OnObjectListChanged;
            if (_config.HighlightTypes.Count > 0) Helper.Events.Display.RenderedWorld += OnRenderedWorld;
            if (_config.NodeRadar) Helper.Events.Display.RenderingHud += OnRenderingHud;
            Helper.Events.Input.ButtonPressed += OnButtonPressed;
            Helper.Events.Player.Warped += OnWarped;
        }

        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            if (!e.IsCurrentLocation) return;
            if (e.Location is StardewValley.Locations.MineShaft mine)
            {
                var ladderHasSpawned = Helper.Reflection.GetField<bool>(mine, "ladderHasSpawned").GetValue();
                if (ladderHasSpawned) _ladderStones.Clear();
                else if (_ladderStones.Count <= 0) FindLadders(mine);
            }
            if (LocationHasNodes(e.Location))
                foreach (var pair in e.Removed)
                    _nodeStones.Remove(pair.Key);
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (!e.IsLocalPlayer) return;

            _ladderStones.Clear();
            _nodeStones.Clear();
            _nextIsShaft = false;

            if (LocationHasNodes(e.NewLocation)) FindNodes(e.NewLocation);
            if (!(e.NewLocation is StardewValley.Locations.MineShaft mine && FindLadders(mine)))
            {
                Helper.Events.GameLoop.OneSecondUpdateTicked += OnUpdateTicked;
            }
        }

        private void OnUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (Game1.mine == null) return;
            if (_ladderStones.Count > 0 || FindLadders(Game1.mine))
                Helper.Events.GameLoop.OneSecondUpdateTicked -= OnUpdateTicked;
        }

        private bool FindLadders(StardewValley.Locations.MineShaft mine)
        {
            var ladderHasSpawned = Helper.Reflection.GetField<bool>(mine, "ladderHasSpawned").GetValue();
            if (ladderHasSpawned || mine.mustKillAllMonstersToAdvance() || !mine.shouldCreateLadderOnThisLevel()) return true;
            var netStonesLeftOnThisLevel = Helper.Reflection
                .GetField<NetIntDelta>(mine, "netStonesLeftOnThisLevel").GetValue().Value;
            var chance = 0.02 + 1.0 / Math.Max(1, netStonesLeftOnThisLevel) + Game1.player.LuckLevel / 100.0 +
                         Game1.player.DailyLuck / 5.0;
            if (mine.EnemyCount == 0) chance += 0.04;
            foreach (var pair in mine.Objects.Pairs.Where(pair => pair.Value.Name.Equals("Stone")))
            {
                var obj = pair.Value;
                // ladder chance calculation taken from checkStoneForItems function in MineShaft class
                var r = new Random((int)pair.Key.X * 1000 + (int)pair.Key.Y + mine.mineLevel + (int)Game1.uniqueIDForThisGame / 2);
                r.NextDouble();
                var next = r.NextDouble();
                if ((netStonesLeftOnThisLevel == 0 || next < chance) && obj.ParentSheetIndex / 24 < Game1.objectSpriteSheet.Height) _ladderStones.Add(new LadderStone(obj));
            }
            if (_config.ForceShafts && mine.getMineArea() == StardewValley.Locations.MineShaft.desertArea
                && !_nextIsShaft && _ladderStones.Count > 0) ForceShaft(mine);
            return _ladderStones.Count > 0;
        }

        private void ForceShaft(StardewValley.Locations.MineShaft mine)
        {
            var mineRandom = Helper.Reflection.GetField<Random>(mine, "mineRandom").GetValue();
            var r = Cloner.Clone(mineRandom);
            var next = r.NextDouble();
            while (next >= 0.2)
            {
                next = r.NextDouble();
                mineRandom.NextDouble();
            }
            _nextIsShaft = true;
        }

        private bool LocationHasNodes(GameLocation loc)
        {
            return loc is StardewValley.Locations.MineShaft || loc is StardewValley.Locations.VolcanoDungeon || loc is StardewValley.Locations.Mountain;
        }

        private void FindNodes(GameLocation loc)
        {
            foreach (var pair in loc.objects.Pairs.Where(pair => pair.Value.Name.Equals("Stone") && _selectedNodeTypeIndices.Contains(pair.Value.ParentSheetIndex)))
                _nodeStones.Add(pair.Key, new Stone(pair.Value));
        }

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (_ladderStones.Count > 0)
            {
                foreach (var obj in _ladderStones)
                {
                    var rect = obj.BoundingBox;
                    rect.Offset(-Game1.viewport.X, -Game1.viewport.Y);
                    var rectColor = (_config.HighlightUsesStoneTint ? obj.Tint : _config.HighlightRectangleRGBA) * Convert.ToSingle(_config.HighlightAlpha);
                    if (_config.HighlightTypes.Contains(HighlightType.Rectangle)) DrawRectangle(rect, rectColor);
                    var imageColor = (_config.HighlightUsesStoneTint ? obj.Tint : Color.Black) * Convert.ToSingle(_config.HighlightAlpha);
                    if (_config.HighlightTypes.Contains(HighlightType.Image)) DrawImage(rect, imageColor, obj.SpriteIndex, obj.Flipped);
                    if (_config.HighlightTypes.Contains(HighlightType.Sprite)) DrawSprite(rect, imageColor, obj.SpriteIndex, obj.Flipped);
                }
            }
        }

        private void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            
            if (!Game1.eventUp && _nodeStones.Count > 0)
            {
                int i = 0;
                foreach (var nodeType in _selectedNodeTypeIndices.Where(nodeType => _nodeStones.Values.Select(node => node.SpriteIndex).Contains(nodeType)))
                {
                    Game1.spriteBatch.Draw(Game1.objectSpriteSheet,
                        new Vector2(Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right - (Game1.showingHealth ? 120 : 64), Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 16 - Convert.ToInt32(16 * Convert.ToSingle(_config.RadarScale) * i)),
                        new Rectangle((nodeType % 24) * 16, (int)(nodeType / 24) * 16, 16, 16), Color.White, 0.0f, new Vector2(16, 16), Convert.ToSingle(_config.RadarScale), SpriteEffects.None, 1.0f);   
                    ++i;
                }
            }
        }

        private static void DrawRectangle(Rectangle rect, Color color)
        {
            Game1.InUIMode(() =>
            {
                Game1.spriteBatch.Draw(_pixelTexture, new Rectangle(rect.Left + 3, rect.Top, rect.Width - 6, 3), color);
                Game1.spriteBatch.Draw(_pixelTexture, new Rectangle(rect.Left + 3, rect.Bottom - 3, rect.Width - 6, 3), color);
                Game1.spriteBatch.Draw(_pixelTexture, new Rectangle(rect.Left, rect.Top + 3, 3, rect.Height - 6), color);
                Game1.spriteBatch.Draw(_pixelTexture, new Rectangle(rect.Right - 3, rect.Top + 3, 3, rect.Height - 6), color);
            });
        }
        private static void DrawSprite(Rectangle rect, Color color, int spriteIndex, bool flipped)
        {
            Game1.InUIMode(() =>
            {
                Game1.spriteBatch.Draw(Game1.objectSpriteSheet, rect, new Rectangle((spriteIndex % 24) * 16, (int)(spriteIndex / 24) * 16, 16, 16), color, 0.0f, Vector2.Zero, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0.0f);
            });
        }

        private static void DrawImage(Rectangle rect, Color color, int spriteIndex, bool flipped)
        {
            Game1.InUIMode(() =>
            {
                Game1.spriteBatch.Draw(_imageTexture, rect, null, color, 0.0f, Vector2.Zero, flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0.0f);
            });
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (_config.ToggleNodeRadar.JustPressed())
            {
                _config.NodeRadar = !_config.NodeRadar;
                Game1.addHUDMessage(new HUDMessage($"Node Radar toggled {(_config.NodeRadar ? "on" : "off")}", 2));
                Helper.WriteConfig(_config);

                Helper.Events.Display.RenderingHud -= OnRenderingHud;
                if (_config.NodeRadar) Helper.Events.Display.RenderingHud += OnRenderingHud;
            }
            else if (_config.CycleAlpha.JustPressed())
            {
                _config.HighlightAlpha = Math.Round((_config.HighlightAlpha + 0.15M) % 1.0M, 2);
                Game1.addHUDMessage(new HUDMessage($"Highlight alpha now {_config.HighlightAlpha}", 2));
                Helper.WriteConfig(_config);
            }
            else if (_config.ToggleTint.JustPressed())
            {
                _config.HighlightUsesStoneTint = !_config.HighlightUsesStoneTint;
                Game1.addHUDMessage(new HUDMessage("Highlight using stone tint toggled " + (_config.HighlightUsesStoneTint ? "on" : "off"), 2));
                Helper.WriteConfig(_config);
            }
            else if (_config.ToggleHighlightTypeKey.JustPressed())
            {
                if (_config.HighlightTypes.Contains(HighlightType.Rectangle))
                {
                    if (_config.HighlightTypes.Contains(HighlightType.Image))
                        ToggleHighlightType(HighlightType.Sprite);
                    ToggleHighlightType(HighlightType.Image);
                }
                ToggleHighlightType(HighlightType.Rectangle);
                Game1.addHUDMessage(_config.HighlightTypes.Count > 0
                    ? new HUDMessage("Ladder highlight: " + string.Join(" + ", _config.HighlightTypes), 2)
                    : new HUDMessage("Ladder highlight disabled", 2));
                Helper.WriteConfig(_config);

                Helper.Events.Display.RenderedWorld -= OnRenderedWorld;
                if (_config.HighlightTypes.Count > 0) Helper.Events.Display.RenderedWorld += OnRenderedWorld;
            }
            else if (_config.ToggleShaftsKey.JustPressed())
            {
                _config.ForceShafts = !_config.ForceShafts;
                Game1.addHUDMessage(_config.ForceShafts
                    ? new HUDMessage("Force shafts toggled on", 2)
                    : new HUDMessage("Force shafts toggled off", 2));
                Helper.WriteConfig(_config);
            }
        }

        private static void ToggleHighlightType(HighlightType type)
        {
            if (_config.HighlightTypes.Contains(type)) _config.HighlightTypes.Remove(type);
            else _config.HighlightTypes.Add(type);
        }

        class Stone
        {
            public Stone(Object obj)
            {
                SpriteIndex = obj.ParentSheetIndex;
                this.obj = obj;
            }

            public Object obj { get; }
            public int SpriteIndex { get; }
        }

        class LadderStone : Stone
        {
            public LadderStone(Object obj) : base(obj)
            {
                BoundingBox = obj.getBoundingBox(obj.TileLocation);
                Flipped = obj.Flipped;
                Tint = GetObjectSpriteAverageColor(SpriteIndex);
            }

            public Rectangle BoundingBox { get; }
            public bool Flipped { get; }
            public Color Tint { get; }
        }

        /// <summary>
        /// Gets the average color of a particular stone from the center 8x8 square of its pixels.
        /// </summary>
        /// <param name="spriteIndex">Index of sprite to get color of from SDV object sprite sheet</param>
        /// <returns>average color of given sprite</returns>
        private static Color GetObjectSpriteAverageColor(int spriteIndex)
        {
            Color[] colors = new Color[8 * 8];
            Game1.objectSpriteSheet.GetData(0, new Rectangle((spriteIndex % 24) * 16 + 4, (int)(spriteIndex / 24) * 16 + 4, 8, 8), colors, 0, 8 * 8);
            var average = new Color(Convert.ToByte(colors.Sum(c => c.R) / colors.Count()), Convert.ToByte(colors.Sum(c => c.G) / colors.Count()), Convert.ToByte(colors.Sum(c => c.B) / colors.Count()));
            return average;
        }
    }
}
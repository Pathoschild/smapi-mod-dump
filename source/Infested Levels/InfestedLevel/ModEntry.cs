/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/benrpatterson/InfestedLevels
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InfestedLevel
{
    public class ModEntry : Mod
    {
        private static readonly List<Tuple<int, string>> _infestedLevels = new List<Tuple<int, string>>();
        private ClickableTextureComponent _icon = null;
        private Color _color = new Color(Color.White.ToVector4());
        private bool _foundMushroomMod = false;
        public string Text { get; set; }

        public override void Entry(IModHelper helper)
        {
            CheckForMushroomMod();
            
            Helper.Events.GameLoop.DayStarted += TimeEvents_AfterDayStarted;
            Helper.Events.GameLoop.ReturnedToTitle += SaveEvents_AfterReturnToTitle;
            Helper.Events.GameLoop.SaveLoaded += SaveEvents_AfterLoad;
            Helper.Events.Display.RenderingHud += GraphicsEvents_OnPreRenderHudEvent;
            Helper.Events.Display.RenderedHud += GraphicsEvents_OnPostRenderHudEvent;
            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            _icon = new ClickableTextureComponent("", new Rectangle(GetWidthInPlayArea() - 134, (_foundMushroomMod ? 340 : 290), 40, 40), "", "", Game1.objectSpriteSheet, new Rectangle(351, 496, 18, 14), 3f, false);
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            _infestedLevels.Clear();
        }

        private void SaveEvents_AfterReturnToTitle(object sender, EventArgs e)
        {
            _infestedLevels.Clear();
        }

        /// <summary>
        /// Find infested levels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            _infestedLevels.Clear();
            //We only search to the deepest level the player has gone to
            var level = Game1.player.deepestMineLevel;
            //clear active mines we need to start fresh
            MineShaft.activeMines.Clear();
            //only search up to 120 because we can find infested levels in the desert cave
            //but the level number switches as you jump down holes.
            if (level > 120)
            {
                level = 120;
            }
                        
            LocateInfestedLevels(level);
            
            if (_infestedLevels.Any())
            {
                Text = $"{(string.Join(Environment.NewLine, _infestedLevels.Select(x => (x.Item1 < 120 ? x.Item1.ToString() : "D" + (x.Item1-120).ToString()) + (string.IsNullOrEmpty(x.Item2)? string.Empty : " - " + x.Item2) ).ToArray()))}";
            }
            else
            {
                Text = "None";
            }
        }

        /// <summary>
        /// render icon on HUD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphicsEvents_OnPostRenderHudEvent(object sender, EventArgs e)
        {
            if (Game1.eventUp || _icon == null)
            {
                return;
            }

            var newIconPosition = new Point(GetWidthInPlayArea() - 50, Game1.options.zoomButtons ? (_foundMushroomMod ? 340 : 290) : (_foundMushroomMod ? 300 : 260));
            _icon.bounds.X = newIconPosition.X;
            _icon.bounds.Y = newIconPosition.Y;
            _icon.draw(Game1.spriteBatch, this._color, 1f);
        }

        /// <summary>
        /// Render Text when mouse over HUD icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphicsEvents_OnPreRenderHudEvent(object sender, EventArgs e)
        {
            if (_icon == null || !_icon.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
            {
                return;
            }
            IClickableMenu.drawHoverText(Game1.spriteBatch, Text, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null);
        }

        private void CheckForMushroomMod()
        {
            _foundMushroomMod = this.Helper.ModRegistry.IsLoaded("BenPattherson.MushroomLevels") || this.Helper.ModRegistry.IsLoaded("Eireon.MushroomLevels");
        }

        /// <summary>
        /// Loops through levels 80-<paramref name="maxLevel"/> looking for infested levels
        /// Side Effect: Due to the nature of the search checked levels are counted as explored by the game
        /// </summary>
        /// <param name="maxLevel">the max level to search to</param>
        private void LocateInfestedLevels(int maxLevel)
        {
            for (var level = 1; level <= maxLevel; level++)
            {
                //if this level is divisable by 5 then the level will always result in a false so no need to check them
                //if we already have this level marked as a infested level we don't need to check it again
                if (level % 5 == 0)
                {
                    continue;
                }
                
                var mine = MineShaft.GetMine($"UndergroundMine{level}");

                //if we have a infested level then record it so we can keep track of it 
                //without having to re-check it each time
                var levelType = IsInfestedLevel(mine);
                
                if (levelType > 0)
                {
                    if (levelType == 1)
                    {
                        _infestedLevels.Add(new Tuple<int, string>(level, "Slime"));
                    }
                    else if (levelType == 2)
                    {
                        _infestedLevels.Add(new Tuple<int, string>(level, "Monster"));
                    }
                    else
                    {
                        _infestedLevels.Add(new Tuple<int, string>(level, string.Empty));
                    }

                }

                mine = null;
            }
        }

        /// <summary>
        /// Determine if the <paramref name="level"/> is actually a infested level
        /// </summary>
        /// <param name="level">Mineshaft level to check</param>
        /// <returns></returns>
        private int IsInfestedLevel(MineShaft level)
        {
            //check if this level has items in it
            //this should never be true as it tends to be the divisable by 5 levels that are empty
            if (level == null || level.Objects == null)
            {
                return 0;
            }

            var isSlimeLevel = Helper.Reflection.GetField<NetBool>(level, "netIsSlimeArea").GetValue();
            var isMonsterLevel = Helper.Reflection.GetField<NetBool>(level, "netIsMonsterArea").GetValue();
            
            return isMonsterLevel && isSlimeLevel ? int.MaxValue : isSlimeLevel ? 1 : isMonsterLevel ? 2 : 0;
        }

        private static int GetWidthInPlayArea()
        {
            if (Game1.isOutdoorMapSmallerThanViewport())
            {
                var titleArea = Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea;
                var layerWidth = Game1.currentLocation.map.Layers[0].LayerWidth * 64;
                return titleArea.Right - (titleArea.Right - layerWidth) / 2;
            }

            return Game1.graphics.GraphicsDevice.Viewport.TitleSafeArea.Right;

        }
    }
}

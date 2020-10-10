/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/benrpatterson/MushroomLevels
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

namespace MushroomLevel
{
    public class ModEntry : Mod
    {
        private static readonly List<int> _mushroomLevels = new List<int>();
        private ClickableTextureComponent _icon = null;
        private Color _color = new Color(Color.White.ToVector4());
        public string Text { get; set; }
        
        public override void Entry(IModHelper helper)
        {

            Helper.Events.GameLoop.DayStarted += TimeEvents_AfterDayStarted;
            Helper.Events.GameLoop.ReturnedToTitle += SaveEvents_AfterReturnToTitle;
            Helper.Events.GameLoop.SaveLoaded += SaveEvents_AfterLoad;
            Helper.Events.Display.RenderingHud += GraphicsEvents_OnPreRenderHudEvent;
            Helper.Events.Display.RenderedHud += GraphicsEvents_OnPostRenderHudEvent;
            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            _icon = new ClickableTextureComponent("", new Rectangle(GetWidthInPlayArea() - 134, 290, 40, 40), "", "", Game1.objectSpriteSheet, new Rectangle(190, 273, 18, 14), 3f, false);
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            _mushroomLevels.Clear();
        }

        private void SaveEvents_AfterReturnToTitle(object sender, EventArgs e)
        {
            _mushroomLevels.Clear();
        }

        /// <summary>
        /// Find mushroom levels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            _mushroomLevels.Clear();
            //We only search to the deepest level the player has gone to
            var level = Game1.player.deepestMineLevel;
            //clear active mines we need to start fresh
            MineShaft.activeMines.Clear();

            if (level <= 80)
            {
                Text = "Mine Not Sufficiently Explored";
                return;
            }

            //don't explore desert mines as these don't have mushroom levels
            if (level > 120)
            {
                level = 120;
            }

            LocateMushroomLevels(level);

            if (_mushroomLevels.Any())
            {
                Text = $"{(string.Join(Environment.NewLine, _mushroomLevels.Select(x => x.ToString()).ToArray()))}";
            }
            else
            {
                Text = "None";
            }
        }
        
        /// <summary>
        /// render mushroom on HUD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GraphicsEvents_OnPostRenderHudEvent(object sender, EventArgs e)
        {
            if (Game1.eventUp || _icon == null)
            {
                return;
            }

            var newIconPosition = new Point(GetWidthInPlayArea() - 50, Game1.options.zoomButtons ? 290 : 260);
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


        /// <summary>
        /// Loops through levels 80-<paramref name="maxLevel"/> looking for mushroom levels
        /// Side Effect: Due to the nature of the search checked levels are counted as explored by the game
        /// </summary>
        /// <param name="maxLevel">the max level to search to</param>
        private void LocateMushroomLevels(int maxLevel)
        {
            //80 will always be a no, so start at 81
            for (var level = 81; level <= maxLevel; level++)
            {
                //if this level is divisable by 5 then the level will always result in a false so no need to check them
                //if we already have this level marked as a mushroom level we don't need to check it again
                if (level % 5 == 0 || _mushroomLevels.Contains(level))
                {
                    continue;
                }


                var mine = MineShaft.GetMine($"UndergroundMine{level}");

                //if we have a mushroom level then record it so we can keep track of it 
                //without having to re-check it each time
                if (IsMushroomLevel(mine))
                {
                    _mushroomLevels.Add(level);
                }

                mine = null;
            }
        }

        /// <summary>
        /// Determine if the <paramref name="level"/> is actually a mine level
        /// </summary>
        /// <param name="level">Mineshaft level to check</param>
        /// <returns></returns>
        private bool IsMushroomLevel(MineShaft level)
        {
            //check if this level has items in it
            //this should never be true as it tends to be the divisable by 5 levels that are empty
            if (level == null || level.Objects == null)
            {
                return false;
            }

            //rainbowLights signafies that this level may be a mushroom level
            //If it is a slime or monster area then we are not in a mushroom level
            if (!Helper.Reflection.GetField<NetBool>(level, "rainbowLights").GetValue()
                || Helper.Reflection.GetField<NetBool>(level, "netIsSlimeArea").GetValue()
                || Helper.Reflection.GetField<NetBool>(level, "netIsMonsterArea").GetValue())
            {
                return false;
            }
            
            //check to see if there are any mushroom objects on the level
            //this may return a false negative as sometimes mushroom levels do not contain mushrooms
            //may also produce false positives if a level has couple of mushrooms by chance
            //potentially could be taken out but is here as a final verification
            return GetMushroomCount(level.Objects.Pairs) >= 1;
        }

        /// <summary>
        /// Counts the number of objects that have "Mushroom" in there name
        /// </summary>
        /// <param name="objects">List of objects contained in the level</param>
        /// <returns></returns>
        private int GetMushroomCount(IEnumerable<KeyValuePair<Microsoft.Xna.Framework.Vector2, StardewValley.Object>> objects)
        {
            foreach (var obj in objects)
            {
                if (obj.Value.DisplayName.Contains("Mushroom"))
                {
                    //we found a mushroom so it is most likely a mushroom level
                    return 1;
                }
            }

            return 0;
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

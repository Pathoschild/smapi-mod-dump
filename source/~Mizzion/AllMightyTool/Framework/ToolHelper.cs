/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AllMightyTool.Framework.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyStardewMods.Common;
using StardewModdingAPI;
using StardewValley;

namespace AllMightyTool.Framework
{
    internal sealed class ToolHelper
    {
        //I got the idea of using this set up from Pathoschild Tractor Mod which can be found at https://www.nexusmods.com/stardewvalley/mods/1401

        /// <summary>Simplifies access to private game code.</summary>
        private readonly IReflectionHelper Reflection;

        /// <summary>The mod settings.</summary>
        private readonly ModConfig Config;

        ///<summary>SMAPI's helper class</summary>
        private readonly IModHelper Helper;

        ///<summary>The tool attachments</summary>
        private readonly ITool[] Tools;


        ///<summary>Construct the tool instance</summary>
        public ToolHelper(ModConfig config, IReflectionHelper reflection, IModHelper helper, IEnumerable<ITool> tools)
        {
            this.Reflection = reflection;
            this.Helper = helper;
            this.Tools = tools.ToArray();
        }

        ///<summary>Handle KeyPresses.</summary>
        public void UseTool()
        {
            if (Game1.currentLocation != null)
                this.UpdateTools;
        }

        /// <summary>Draw a radius around the player.</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        public void DrawRadius(SpriteBatch spriteBatch)
        {
            //bool enabled = this.IsEnabled();

            foreach (Vector2 tile in this.GetTileGrid(Game1.player.getTileLocation(), this.Config.ToolLevel))
            {
                // get tile area in screen pixels
                Rectangle area = new Rectangle((int)(tile.X * Game1.tileSize - Game1.viewport.X), (int)(tile.Y * Game1.tileSize - Game1.viewport.Y), Game1.tileSize, Game1.tileSize);

                // choose tile color
                Color color = Color.Green; //enabled ? Color.Green : Color.Red;

                // draw background
                spriteBatch.DrawLine(area.X, area.Y, new Vector2(area.Width, area.Height), color * 0.2f);

                // draw border
                int borderSize = 1;
                Color borderColor = color * 0.5f;
                spriteBatch.DrawLine(area.X, area.Y, new Vector2(area.Width, borderSize), borderColor); // top
                spriteBatch.DrawLine(area.X, area.Y, new Vector2(borderSize, area.Height), borderColor); // left
                spriteBatch.DrawLine(area.X + area.Width, area.Y, new Vector2(borderSize, area.Height), borderColor); // right
                spriteBatch.DrawLine(area.X, area.Y + area.Height, new Vector2(area.Width, borderSize), borderColor); // bottom
            }
        }

        ///<summary>Activate the tool. </summary>
        private void UpdateTools()
        {
            //Variables
            Farmer who = Game1.player;
            GameLocation loc = Game1.currentLocation;
            Tool tool = this.SelectTool;
            Item item = who.CurrentItem;
            ICursorPosition cur = Helper.Input.GetCursorPosition();

            //Gather all the tools
            ITool[] tools = this.GetTools;

            if (!tools.Any())
                return;

            //Grab the tiles to use the tool on
            Vector2[] g = this.GetTileGrid(cur.GrabTile, Config.ToolLevel).ToArray();
        }


        ///<summary>Gets a grid of tiles that we can use</summary>
        /// <param name="start">Starting place of the grid</param>
        /// <param name="radius">How many tiles out from the start should we go</param>
        private IEnumerable<Vector2> GetTileGrid(Vector2 start, int radius)
        {
            for (int x = -radius; x <= radius; x++)
                for (int y = -radius; y <= radius; y++)
                    yield return new Vector2(start.X + x, start.Y + y);
        }
    }
}

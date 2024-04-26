/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrismaticStatue;

internal class StatueOverlay : AchtuurCore.Framework.Overlay
{
    private readonly Color[] Colors = new Color[]
    {
        Color.MediumVioletRed,
        Color.OrangeRed,
        Color.Orange,
        Color.Yellow,
        Color.YellowGreen,
        Color.Green,
        Color.Turquoise,
        Color.Blue,
        Color.Purple,
        Color.Violet,
        Color.Pink,
    };
    private float IndexIncrement = 0.25f / 60f;
    /// <summary>
    /// Index for current statue color. Is a float in order to fake gradient effect
    /// </summary>
    private float ColorIndex;

    public StatueOverlay()
    {
        this.ColorIndex = 0f;
        this.Enabled = false;
    }

    protected override void DrawOverlayToScreen(SpriteBatch spriteBatch)
    {
        foreach(SpedUpMachineGroup group in ModEntry.Instance.SpedupMachineGroups)
        {
            if (group.Location != Game1.currentLocation)
                continue;


            IEnumerable<Vector2> visibleTiles = Tiles.GetVisibleTiles(expand: 1);
            IEnumerable<Vector2> machineTiles = visibleTiles
                .Intersect(group.GetMachineTiles());

            IEnumerable<Vector2> statueTiles = visibleTiles
                .Intersect(group.GetStatueTiles());

            IEnumerable<Vector2> otherTiles = group.Tiles.Except(machineTiles).Except(statueTiles);

            DrawTiles(spriteBatch, machineTiles, color: Color.Magenta, tileTexture: TilePlacementTexture);
            DrawTiles(spriteBatch, statueTiles, color: GetSpeedupStatueColor(), tileTexture: TilePlacementTexture);
            DrawTiles(spriteBatch, otherTiles, color: Color.White, tileTexture: TilePlacementTexture);

        }
    }

    /// <summary>
    /// <para>Draw overlay for automated machines, heavy inspiration from Pathoschild's Automate.OverlayMenu.DrawWorld</para>
    /// <see href="https://github.com/Pathoschild/StardewMods/blob/stable/Automate/Framework/OverlayMenu.cs#L14"/>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void DrawOverlay2(SpriteBatch spriteBatch)
    {
        if (!this.Enabled)
            return;

        int tilegap = 1;
        float color_fac = 0.25f;

        foreach (Vector2 tile in Tiles.GetVisibleTiles(expand: 1))
        {
            // get tile's screen coordinates
            float screenX = tile.X * Game1.tileSize - Game1.viewport.X;
            float screenY = tile.Y * Game1.tileSize - Game1.viewport.Y;
            int tileSize = Game1.tileSize;

            SpedUpMachineGroup group = ModEntry.Instance.GetMachineGroupOnTile(tile);
            GenericSpedUpMachineWrapper machine = ModEntry.Instance.GetMachineWrapperOnTile(group, tile);

            Color? color = null;

            if (machine is not null)
            {
                if (machine.isSpedUp())
                {
                    color = Color.Magenta * color_fac;
                }
                else
                {
                    color = (Color.Purple * 0.75f) * color_fac;
                }
            }
            // Inside the group's tiles, but not on a processing machine
            else if (group is not null)
            {
                if (tile.ContainsObject(SpeedupStatue.ID))
                {
                    color = GetSpeedupStatueColor() * color_fac;
                }
                //if (ModEntry.GetPossibleStatueIDs().Any(id => tile.ContainsObject(id)))
                //{
                //    color = GetSpeedupStatueColor() * color_fac;
                //}
                // Non statue machine in group
                else
                {
                    color = Color.WhiteSmoke * color_fac;
                }
            }


            color ??= Color.Black * 0.5f;

            // draw background
            spriteBatch.DrawLine(screenX + tilegap, screenY + tilegap, new Vector2(tileSize - tilegap * 2, tileSize - tilegap * 2), color);

            if (group is not null)
                DrawEdgeBorders(spriteBatch, group, tile, (Color)color * (1f / color_fac));
        }
    }

    /// <summary>
    /// <para>Draw borders for each unconnected edge of a tile.</para>
    /// <see href="https://github.com/Pathoschild/StardewMods/blob/stable/Automate/Framework/OverlayMenu.cs#L14"/>
    /// </summary>
    /// <param name="spriteBatch">The sprite batch being drawn.</param>
    /// <param name="group">The machine group.</param>
    /// <param name="tile">The group tile.</param>
    /// <param name="color">The border color.</param>
    private void DrawEdgeBorders(SpriteBatch spriteBatch, SpedUpMachineGroup group, Vector2 tile, Color color)
    {
        int borderSize = 3;
        float screenX = tile.X * Game1.tileSize - Game1.viewport.X;
        float screenY = tile.Y * Game1.tileSize - Game1.viewport.Y;
        float tileSize = Game1.tileSize;

        IReadOnlySet<Vector2> tiles = group.Tiles;

        if (tiles is null)
            return;

        // top
        if (!tiles.Contains(new Vector2(tile.X, tile.Y - 1)))
            spriteBatch.DrawLine(screenX, screenY, new Vector2(tileSize, borderSize), color); // top

        // bottom
        if (!tiles.Contains(new Vector2(tile.X, tile.Y + 1)))
            spriteBatch.DrawLine(screenX, screenY + tileSize, new Vector2(tileSize, borderSize), color); // bottom

        // left
        if (!tiles.Contains(new Vector2(tile.X - 1, tile.Y)))
            spriteBatch.DrawLine(screenX, screenY, new Vector2(borderSize, tileSize), color); // left

        // right
        if (!tiles.Contains(new Vector2(tile.X + 1, tile.Y)))
            spriteBatch.DrawLine(screenX + tileSize, screenY, new Vector2(borderSize, tileSize), color); // right
    }

    private Color GetSpeedupStatueColor()
    {
        this.ColorIndex += this.IndexIncrement;
        this.ColorIndex %= Colors.Length;
        // Get index of current and next color
        int i_cur = (int)Math.Floor(this.ColorIndex);
        int i_next = (int)Math.Floor(this.ColorIndex + 1);
        // Get factor of 
        float fac_cur = 1 - (this.ColorIndex - i_cur);
        float fac_next = 1 - (i_next - this.ColorIndex);

        Color col_cur = Colors[i_cur % Colors.Length];
        Color col_next = Colors[i_next % Colors.Length];

        // Return blend of previous and next colour to get a sort of gradient effect over time
        Color blended_color = (col_cur * fac_cur).AddColor(col_next * fac_next);
        return blended_color;

    }

    
}

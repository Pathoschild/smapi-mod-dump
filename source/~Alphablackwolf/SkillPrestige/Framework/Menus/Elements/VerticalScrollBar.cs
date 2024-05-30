/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Framework.Menus.Elements.Buttons;
using SkillPrestige.Logging;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SkillPrestige.Framework.Menus.Elements;

public class VerticalScrollBar
{
    private Rectangle ArrowUpSourceRectangle = new Rectangle(421, 459, 11, 12);
    private Rectangle ArrowDownSourceRectangle = new Rectangle(421, 472, 11, 12);
    private Rectangle RunnerSourceRectangle = new Rectangle(403, 383, 6, 6);
    private Rectangle PuckSourceRectangle = new Rectangle(435, 463, 6, 10);
    private const int PaddingBetweenArrowsAndRunner = 32;

    private int ScrollOffset = 0;

    private TextureButton ScrollUpArrow;
    private TextureButton ScrollDownArrow;
    private TextureBox ScrollBarRunner;
    private TextureBox ScrollBarPuck;
    private bool RenderFailure;

    private float DrawScale = 4f;
    private Dictionary<int, int> ScrollBarPositions = new();
    private bool IsScrolling;
    private bool Drawn;
    private int MaximumScrollOffset;
    private Action<int> OnScroll;
    public Rectangle Bounds;


    public VerticalScrollBar(Rectangle bounds, int totalListCount, int maximumDisplayableEntries, Action<int> onScroll)
    {
        this.Bounds = bounds;
        this.MaximumScrollOffset = totalListCount - maximumDisplayableEntries;
        if (this.MaximumScrollOffset < 0) this.MaximumScrollOffset = 0;
        this.OnScroll = onScroll;
        this.Drawn = false;
        this.LoadArrows();
        this.LoadRunner();
        if(this.MaximumScrollOffset > 0) this.LoadPuck(this.DeterminePuckHeight());
    }

    private void LoadArrows()
    {
        var upArrowBounds = new Rectangle(this.Bounds.X, this.Bounds.Y, this.Bounds.Width, this.Bounds.Width);
        var downArrowBounds = new Rectangle(this.Bounds.X, this.Bounds.Y + this.Bounds.Height - this.Bounds.Width, this.Bounds.Width, this.Bounds.Width);
        this.ScrollUpArrow = new TextureButton(upArrowBounds, Game1.mouseCursors, this.ArrowUpSourceRectangle, this.ScrollUp, "", this.DrawScale, false);
        this.ScrollDownArrow = new TextureButton(downArrowBounds, Game1.mouseCursors, this.ArrowDownSourceRectangle, this.ScrollDown, "", this.DrawScale, false);
    }

    private void LoadRunner()
    {
        int startPointForBarY = this.ScrollUpArrow.Bounds.Y + this.ScrollUpArrow.Bounds.Height + PaddingBetweenArrowsAndRunner;
        int endPointForBarY = this.ScrollDownArrow.Bounds.Y - PaddingBetweenArrowsAndRunner;
        int runnerHeight = endPointForBarY - startPointForBarY;
        var scrollBarRunnerBounds = new Rectangle(this.ScrollUpArrow.Bounds.X + this.Bounds.Width / 2, startPointForBarY, this.Bounds.Width / 2,
            runnerHeight);
        this.ScrollBarRunner = new TextureBox(scrollBarRunnerBounds, Game1.mouseCursors, this.RunnerSourceRectangle)
        {
            Scale = this.DrawScale
        };
    }

    private int DeterminePuckHeight()
    {
        const int minimumPixelsPerMove = 5;
        const int maximumDesiredPixels = 40;
        const int minimumDesiredPixels = 5;
        int totalBarHeight = this.ScrollBarRunner.Bounds.Height;
        int maximumPossiblePixels = totalBarHeight / this.MaximumScrollOffset + this.MaximumScrollOffset * minimumPixelsPerMove;
        switch (maximumPossiblePixels)
        {
            case <= minimumDesiredPixels:
                Logger.LogVerbose($"calculated maximum possible puck height: {maximumPossiblePixels}, due to totalRunnerBarHeight: {totalBarHeight} and {this.MaximumScrollOffset + this.MaximumScrollOffset * minimumPixelsPerMove} needed pixels");
                Logger.LogInformation($"Puck height needs to be too small, cancelling render.");
                this.RenderFailure = true;
                this.Drawn = false;
                return 0;
            case > maximumDesiredPixels:
                return maximumDesiredPixels;
            default:
                return Math.Min(Math.Min(maximumDesiredPixels, maximumPossiblePixels), maximumDesiredPixels * (maximumPossiblePixels / maximumDesiredPixels));
        }
    }
    private void LoadPuck(int puckHeight)
    {
        foreach (int offset in Enumerable.Range(0, this.MaximumScrollOffset + 1))
        {
            if (offset == 0)
            {
                this.ScrollBarPositions.Add(offset, this.ScrollBarRunner.Bounds.Y);
                continue;
            }
            if (offset == this.MaximumScrollOffset)
            {
                this.ScrollBarPositions.Add(offset, this.ScrollBarRunner.Bounds.Y + this.ScrollBarRunner.Bounds.Height - puckHeight);
                continue;
            }
            this.ScrollBarPositions.Add(offset, this.ScrollBarRunner.Bounds.Y + this.ScrollBarRunner.Bounds.Height / this.MaximumScrollOffset * offset - puckHeight / 2);
        }

        int yPosition = this.ScrollBarPositions.Any()
            ? this.ScrollBarPositions[this.ScrollOffset]
            : this.ScrollBarRunner.Bounds.Y;
        var scrollBarBounds = new Rectangle(this.ScrollBarRunner.Bounds.X, yPosition, this.ScrollBarRunner.Bounds.Width, puckHeight);
        this.ScrollBarPuck = new TextureBox(scrollBarBounds, Game1.mouseCursors, this.PuckSourceRectangle)
        {
            Scale = this.DrawScale
        };
    }

    public void ScrollUp()
    {
        if (!this.Drawn || this.ScrollOffset <= 0)
        {
            this.ScrollOffset = 0;
            return;
        }
        this.ScrollOffset--;
        this.UpdateScrollBarPosition();
        Game1.playSound("shwip");
        this.OnScroll(this.ScrollOffset);
    }

    public void ScrollDown()
    {
        if (!this.Drawn || this.ScrollOffset >= this.MaximumScrollOffset)
        {
            this.ScrollOffset = this.MaximumScrollOffset;
            return;
        }
        this.ScrollOffset++;
        this.UpdateScrollBarPosition();
        Game1.playSound("shwip");
        this.OnScroll(this.ScrollOffset);
    }

    private void ScrollToIndex(int index)
    {
        if (!this.Drawn || index == this.ScrollOffset) return;
        this.ScrollOffset = index;
        this.UpdateScrollBarPosition();
        Game1.playSound("shwip");
        this.OnScroll(this.ScrollOffset);
    }

    private int GetClosestScrollIndexLocation(int y)
    {
        return this.ScrollBarPositions.MinBy(position => Math.Abs(position.Value - y)).Key;
    }

    private void UpdateScrollBarPosition()
    {
        this.ScrollBarPuck.Bounds = new Rectangle(this.ScrollBarPuck.Bounds.X, this.ScrollBarPositions[this.ScrollOffset], this.ScrollBarPuck.Bounds.Width, this.ScrollBarPuck.Bounds.Height);
    }

    public void OnCursorMoved(CursorMovedEventArgs e)
    {
        this.ScrollUpArrow.OnCursorMoved(e);
        this.ScrollDownArrow.OnCursorMoved(e);
    }

    public void OnButtonPressed(ButtonPressedEventArgs e, bool isClick)
    {
        if (!this.Drawn) return;
        this.ScrollUpArrow.OnButtonPressed(e, isClick);
        this.ScrollDownArrow.OnButtonPressed(e, isClick);
        if (e.Button != SButton.MouseLeft || !this.ScrollBarPuck.Bounds.Contains(Game1.getMouseX(), Game1.getMouseY())) return;
        this.IsScrolling = true;
        Game1.playSound("shiny4");
    }

    public void ReceiveScrollWheelAction(int direction)
    {
        switch (direction)
        {
            case > 0 when this.ScrollOffset > 0:
                this.ScrollUp();
                break;
            case < 0 when this.ScrollOffset < this.MaximumScrollOffset:
                this.ScrollDown();
                break;
        }
    }

    public void leftClickHeld(int x, int y)
    {
        if (!this.IsScrolling)
            return;
        this.ScrollToIndex(this.GetClosestScrollIndexLocation(y));
    }

    public void releaseLeftClick(int x, int y)
    {
        this.IsScrolling = false;
    }


    public void DrawScrollBar(SpriteBatch spriteBatch)
    {
        if (this.RenderFailure) return;
        this.Drawn = true;
        this.ScrollUpArrow.Draw(spriteBatch);
        this.ScrollDownArrow.Draw(spriteBatch);
        this.ScrollBarRunner.Draw(spriteBatch);
        this.ScrollBarPuck.Draw(spriteBatch);
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore;
using AchtuurCore.Extensions;
using AchtuurCore.Framework.Borders;
using AchtuurCore.Framework.GUI;
using AchtuurCore.Utility;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishCatalogue.Drawing;
internal class FishSpawnsPage : IClickableMenu
{
    private Vector2 Size => borderDrawer.BorderSize();
    private float Width => Size.X;
    private float Height => Size.Y;
    private float DesiredWidth => Math.Min(Game1.viewport.Width * 0.4f, 1000f);
    private float DesiredHeight => Math.Min(Game1.viewport.Height * 0.4f, 1200f);
    private Vector2 TopLeft => Utility.getTopLeftPositionForCenteringOnScreen((int)Width, (int)Height);

    BorderDrawer borderDrawer;

    /// <summary>
    /// The actual bobber thingy that you drag
    /// </summary>
    ClickableTextureComponent scrollBar;
    /// <summary>
    /// The background of the scrollbar
    /// </summary>
    Rectangle scrollBarRunner;
    /// <summary>
    /// Up button of the scrollbar
    /// </summary>
    ClickableTextureComponent upButton;
    /// <summary>
    /// Down button of the scrollbar
    /// </summary>
    ClickableTextureComponent downButton;


    ClickableTextureComponent springButton;

    List<Tab> tabs;
    Tab selectedTab;

    /// <summary>
    /// Number of rows of labels shown on screen at the same time
    /// </summary>
    private int ShownRows => Math.Max(15 * (int)DesiredHeight / 1200, 7);
    /// <summary>
    /// Index of top label shown on screen
    /// </summary>
    private int TopLabelIndex = 0;

    /// <summary>
    /// Number of ItemLabel columns in the GridLabel
    /// </summary>
    private int column_width = 4;

    private int NumRows => FishCatalogue.AllFishData.Values
            .Where(fish => fish.CanBeCaughtThisSeason()).Count();

    private int ScrollPages => NumRows - ShownRows;

    private bool scrolling = false;

    public FishSpawnsPage() : base(0, 0, 0, 0, showUpperRightCloseButton: true)
    {
        borderDrawer = new BorderDrawer();
        tabs = new();
        this.tabs.Add(new Tab("Spring", "Spring Fish", ModEntry.seasonTexture));
        this.tabs.Add(new Tab("Spring", "Summer Fish", ModEntry.seasonTexture));
        selectedTab = tabs.First();

        CreateButtons();
        CreateScrollBar();
    }

    public void Enable()
    {
        Update();
        if (Game1.activeClickableMenu is null)
            Game1.activeClickableMenu = this;
        else if(Game1.activeClickableMenu is FishSpawnsPage)
            Game1.activeClickableMenu = null;
    }

    private void Update()
    {
        UpdateBorders();
        UpdateDimensions();
        RepositionButtons();
        RepositionScrollBar();
        RepositionTabs();
        SetTopLabelIndex(TopLabelIndex); // updates scrollbar bobber position
    }
    
    // stuff that should not be updated in the final build
    private void DebugUpdate()
    {
        AchtuurCore.Utility.Debug.DebugOnlyExecute(() =>
        {
            UpdateDimensions();
            UpdateBorders();
            this.upperRightCloseButton.setPosition(50, 50);
            CreateScrollBar();
        });
    }

    private void CreateScrollBar()
    {
        // this was taken basically directly from the stardew source code somewhere
        this.upButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 64, this.yPositionOnScreen + 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f, false);
        this.downButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + 64, this.yPositionOnScreen + this.height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f, false);
        this.scrollBar = new ClickableTextureComponent(new Rectangle(this.upButton.bounds.X + 12, this.upButton.bounds.Y + this.upButton.bounds.Height + 4, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f, false);
        int runner_height = downButton.bounds.Top - upButton.bounds.Bottom - 12;
        this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upButton.bounds.Y + this.upButton.bounds.Height + 4, this.scrollBar.bounds.Width, runner_height);
    }

    private void CreateButtons()
    {
        this.springButton = new("Spring", new Rectangle((int)TopLeft.X - 32, (int)TopLeft.Y + 64, 32, 32), "", "hover", Game1.mouseCursors, new Rectangle(293, 360, 24, 24), 4f);
    }

    private void RepositionScrollBar()
    {
        this.upButton.bounds.X = this.xPositionOnScreen + this.width + 64;
        this.upButton.bounds.Y = this.yPositionOnScreen + 64;
        this.downButton.bounds.X = this.xPositionOnScreen + this.width + 64;
        this.downButton.bounds.Y = this.yPositionOnScreen + this.height - 64;
        this.scrollBar.bounds.X = this.upButton.bounds.X + 12;
        this.scrollBar.bounds.Y = this.upButton.bounds.Y + this.upButton.bounds.Height + 4;
        int runner_height = downButton.bounds.Top - upButton.bounds.Bottom - 12;
        this.scrollBarRunner = new Rectangle(this.scrollBar.bounds.X, this.upButton.bounds.Y + this.upButton.bounds.Height + 4, this.scrollBar.bounds.Width, runner_height);
    }

    private void RepositionTabs()
    {
        Vector2 offset = new Vector2(this.xPositionOnScreen + 32, this.yPositionOnScreen - 42);
        foreach (Tab t in tabs)
        {
            t.SetPosition(offset);
            offset.X += 64;
        }
    }

    private void RepositionButtons()
    {
        this.upperRightCloseButton.setPosition(this.xPositionOnScreen + (int)Width, this.yPositionOnScreen - 64);
        this.springButton.bounds.X = (int)TopLeft.X - 32;
        this.springButton.bounds.Y = (int)TopLeft.Y + 64;
    }

    public override void draw(SpriteBatch sb)
    {
        Update();

        DrawTabs(sb); //draw tabs behind everything
        base.draw(sb);
        DrawLabels(sb);
        DrawScrollBar(sb);
        DrawButtons(sb);
        drawMouse(sb);
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        base.gameWindowSizeChanged(oldBounds, newBounds);
        Update();
    }

    private void DrawLabels(SpriteBatch sb)
    {
        borderDrawer.Draw(sb, TopLeft, fixed_width: DesiredWidth);
    }

    private void DrawButtons(SpriteBatch sb)
    {
        springButton.draw(sb);
        //springButton.drawItem(sb);
        this.upperRightCloseButton.draw(sb);
    }

    private void DrawScrollBar(SpriteBatch sb)
    {
	    IClickableMenu.drawTextureBox(sb, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), this.scrollBarRunner.X, this.scrollBarRunner.Y, this.scrollBarRunner.Width, this.scrollBarRunner.Height, Color.White, 4f, true, -1f);
        upButton.draw(sb);
        downButton.draw(sb);
        scrollBar.draw(sb);
    }
    
    private void DrawTabs(SpriteBatch sb)
    {
        foreach (Tab t in tabs)
        {
            t.Draw(sb);
        }
    }

    public override void performHoverAction(int x, int y)
    {
        base.performHoverAction(x, y);
        this.upButton.scale = 4f;
        this.downButton.scale = 4f;
        if (this.upButton.containsPoint(x, y))
            this.upButton.scale *= 1.1f;
        else if (this.downButton.containsPoint(x, y))
            this.downButton.scale *= 1.1f;
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        base.receiveLeftClick(x, y, playSound);
        if (this.upButton.containsPoint(x, y))
            OnClickUpArrow();
        else if (this.downButton.containsPoint(x, y))
            OnClickDownArrow();
        else if (this.scrollBar.containsPoint(x, y))
            scrolling = true;
        else if (this.tabs.First(tab => tab.Contains(x, y)) is Tab t)
        {
            selectedTab.Deselect();
            selectedTab = t;
            selectedTab.Select();
        }
    }

    public override void receiveScrollWheelAction(int direction)
    {
        base.receiveScrollWheelAction(direction);
        if (direction > 0)
            OnClickUpArrow();
        else
            OnClickDownArrow();
    }

    public override void releaseLeftClick(int x, int y)
    {
        base.releaseLeftClick(x, y);
        scrolling = false;
    }

    private void OnClickUpArrow()
    {
        SetTopLabelIndex(TopLabelIndex - 1);
    }

    private void OnClickDownArrow()
    {
        SetTopLabelIndex(TopLabelIndex + 1);
        
    }

    public override void leftClickHeld(int x, int y)
    {
        base.leftClickHeld(x, y);
        if (!scrolling)
            return;

        int maxHeight = scrollBarRunner.Height - scrollBar.bounds.Height;
        float perc = (float)(y - scrollBarRunner.Y) / maxHeight;
        perc = Math.Clamp(perc, 0.0f, 1.0f);
        SetTopLabelIndex((int)(perc * ScrollPages));
    }

    private void OnMoveScrollBar(object sender, CursorMovedEventArgs e)
    {
        Vector2 cursorPos = ModEntry.Instance.Helper.Input.GetCursorPosition().GetScaledScreenPixels();
        // make sure cursor height is in between the scrollBarRunner bounds
        // in case the cursor is dragged above or below the runner
        int maxCursorHeight = scrollBarRunner.Height;
        float clampedCursorHeight = Math.Clamp(cursorPos.Y, scrollBarRunner.Y, scrollBarRunner.Y + scrollBarRunner.Height) - scrollBarRunner.Y;
        float perc = clampedCursorHeight / maxCursorHeight;
        SetTopLabelIndex((int)(perc * ScrollPages));
    }


    private void OnReleaseScrollBar(object sender, ButtonReleasedEventArgs e)
    {
        if (e.Button != StardewModdingAPI.SButton.MouseLeft)
            return;
        ModEntry.Instance.Helper.Events.Input.CursorMoved -= OnMoveScrollBar;
    }

    private void SetTopLabelIndex(int idx)
    {
        TopLabelIndex = Math.Clamp(idx, 0, ScrollPages);
        scrollBar.bounds.Y = ScrollBarHeightForPage(TopLabelIndex);
        UpdateBorders();
    }
    private int ScrollBarHeightForPage(int page) {
        // using some fp casts here to make sure nothing gets rounded down
        // height per page is height of the runner minus scrollbar height, so it doesn't clip through the end
        float height_per_page = (float)(scrollBarRunner.Height - scrollBar.bounds.Height) / (float)ScrollPages;
        return (int)((float)page * height_per_page) + scrollBarRunner.Y;
    }

    private void UpdateDimensions()
    {
        this.xPositionOnScreen = (int)TopLeft.X;
        this.yPositionOnScreen = (int)TopLeft.Y;
        this.width = (int)Width;
        this.height = (int)Height;
        CreateScrollBar();
    }

    private void UpdateBorders()
    {
        borderDrawer.Reset();
        borderDrawer.AddBorder(CreateBorders());
    }

    private IEnumerable<Label> CreateBorders()
    {
        if (FishCatalogue.AllFishData is null)
            yield break;

        IEnumerable<IEnumerable<Label>> labels = FishCatalogue.AllFishData.Values
            .OrderBy(fish => fish.Name)
            //.Where(fish => fish.CanBeCaughtThisSeason())
            .Skip(this.TopLabelIndex)
            .Take(this.ShownRows)
            .Select(fish => fish.GenerateSpawnConditionLabel());

        // Make sure all labels have the same number of columns
        // To align the grid
        column_width = labels.Max(label => label.Count());
        labels = labels.Select(label => label
        .Concat(Enumerable.Repeat(new EmptyLabel(), column_width - label.Count())));

        GridLabel grid_lab = new GridLabel(labels.SelectMany(label => label));
        grid_lab.SetNumberOfColumns(column_width);
        grid_lab.SetFixedWidth(this.DesiredWidth / (column_width - 1));
        grid_lab.SetFixedHeight(this.DesiredHeight / (ShownRows - 2));
        yield return grid_lab;
    }
}

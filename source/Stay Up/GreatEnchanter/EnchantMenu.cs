/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/su226/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;

namespace Su226.GreatEnchanter {
  class EnchantMenu : IClickableMenu {
    private Rectangle[] buttons = new Rectangle[4];
    private BaseEnchantment[] enchantments;
    private bool[] incompatible;
    private int offset;
    private int maxOffset;

    private ClickableTextureComponent upArrow;
    private ClickableTextureComponent downArrow;
    private ClickableTextureComponent scrollBar;
    private Rectangle scrollBarTrack;
    private bool scrolling;

    private InventoryMenu inventory;
    private Item hoverItem;
    private int selected;

    public EnchantMenu() : base(
      (Game1.uiViewport.Width - 800) / 2,
      (Game1.uiViewport.Height  - 600) / 2,
      800,
      600,
      true
    ) {
      enchantments = BaseEnchantment.GetAvailableEnchantments().ToArray();
      incompatible = new bool[enchantments.Length];
      inventory = new InventoryMenu(0, 0, false);
      int buttonHeight = (height - 32 - inventory.height) / buttons.Length;
      for (int i = 0; i < buttons.Length; i++) {
        buttons[i] = new Rectangle(0, 0, width - 32, buttonHeight);
      }
      maxOffset = enchantments.Length - buttons.Length;
      upArrow = new ClickableTextureComponent(
        new Rectangle(0, 0, 44, 48),
        Game1.mouseCursors,
        new Rectangle(421, 459, 11, 12),
        4f
      );
      downArrow = new ClickableTextureComponent(
        new Rectangle(0, 0, 44, 48),
        Game1.mouseCursors,
        new Rectangle(421, 472, 11, 12),
        4f
      );
      scrollBar = new ClickableTextureComponent(
        new Rectangle(0, 0, 24, 40),
        Game1.mouseCursors,
        new Rectangle(435, 463, 6, 10),
        4f
      );
      scrollBarTrack = new Rectangle(
        0,
        0,
        scrollBar.bounds.Width,
        height - inventory.height - 140
      );
      UpdateSelected();
      PlaceWidgets();
    }

    public void PlaceWidgets() {
      for (int i = 0; i < buttons.Length; i++) {
        buttons[i].X = xPositionOnScreen + 16;
        buttons[i].Y = yPositionOnScreen + 16 + i * buttons[i].Height;
      }
      upArrow.bounds.X = xPositionOnScreen + width + 16;
      upArrow.bounds.Y = yPositionOnScreen + 16;
      downArrow.bounds.X = xPositionOnScreen + width + 16;
      downArrow.bounds.Y = yPositionOnScreen + height - inventory.height - 64;
      scrollBarTrack.X = upArrow.bounds.X + 12;
      scrollBarTrack.Y = upArrow.bounds.Y + upArrow.bounds.Height + 4;
      scrollBar.bounds.X = scrollBarTrack.X;
      PlaceScrollBar();
      inventory.movePosition(
        xPositionOnScreen + 16 - inventory.xPositionOnScreen,
        yPositionOnScreen + height - inventory.height - 4 - inventory.yPositionOnScreen
      );
    }

    public void PlaceScrollBar() {
      scrollBar.bounds.Y = (int)((scrollBarTrack.Height - scrollBar.bounds.Height) * 1.0 * offset / maxOffset);
      scrollBar.bounds.Y += scrollBarTrack.Y;
    }

    public bool ScrollUp() {
      if (offset > 0) {
        offset--;
        PlaceScrollBar();
        return true;
      }
      return false;
    }

    public bool ScrollDown() {
      if (offset < maxOffset) {
        offset++;
        PlaceScrollBar();
        return true;
      }
      return false;
    }

    public void UpdateSelected() {
      Item item = Game1.player.items.Count > selected ? Game1.player.items[selected] : null;
      for (int i = 0; i < enchantments.Length; i++) {
        incompatible[i] = !enchantments[i].CanApplyTo(item);
      }
    }

    public bool HasEnchantment(int index) {
      Item item = Game1.player.items.Count > selected ? Game1.player.items[selected] : null;
      if (item is Tool tool) {
        return tool.enchantments.Contains(enchantments[index]);
      }
      return false;
    }

    public override void gameWindowSizeChanged(Rectangle oldrect, Rectangle newrect) {
      base.gameWindowSizeChanged(oldrect, newrect);
      PlaceWidgets();
    }


    public override void performHoverAction(int x, int y) {
      upArrow.tryHover(x, y);
      downArrow.tryHover(x, y);
      scrollBar.tryHover(x, y);
      hoverItem = inventory.hover(x, y, Game1.player.CursorSlotItem);
    }

    public override void receiveLeftClick(int x, int y, bool playSound) {
      Item item = Game1.player.items.Count > selected ? Game1.player.items[selected] : null;
      for (int i = 0; i < buttons.Length; i++) {
        PlaceScrollBar();
        if (buttons[i].Contains(x, y)) {
          if (item is Tool tool) {
            Game1.playSound("smallSelect");
            if (tool.enchantments.Contains(enchantments[i + offset])) {
              tool.enchantments.Remove(enchantments[i + offset]);
            } else {
              tool.enchantments.Add(enchantments[i + offset]);
            }
          }
          return;
        }
      }
      if (upArrow.containsPoint(x, y) && ScrollUp()) {
        Game1.playSound("shwip");
        upArrow.scale = upArrow.baseScale;
      }
      if (downArrow.containsPoint(x, y) && ScrollDown()) {
        Game1.playSound("shwip");
        downArrow.scale = upArrow.baseScale;
      }
      if (scrollBarTrack.Contains(x, y)) {
        scrolling = true;
      }
      for (int i = 0; i < 36; i++) {
        if (inventory.inventory[i].containsPoint(x, y)) {
          Game1.playSound("stoneStep");
          selected = i;
          UpdateSelected();
          return;
        }
      }
    }

    public override void leftClickHeld(int x, int y) {
      if (scrolling) {
        int scrollTop = y - (scrollBarTrack.Y + scrollBar.bounds.Height / 2);
        int scrollHeight = scrollBarTrack.Height - scrollBar.bounds.Height;
        double ratio = Math.Min(Math.Max(scrollTop * 1.0 / scrollHeight, 0), 1);
        int newOffset = (int)Math.Round(ratio * maxOffset);
        if (newOffset != offset) {
          Game1.playSound("shiny4");
          offset = newOffset;
          PlaceScrollBar();
        }
      }
    }

    public override void releaseLeftClick(int x, int y) {
      base.releaseLeftClick(x, y);
      scrolling = false;
    }

    public override void receiveScrollWheelAction(int direction) {
      if (direction > 0) {
        if (ScrollUp()) {
          Game1.playSound("shiny4");
        }
      } else {
        if (ScrollDown()) {
          Game1.playSound("shiny4");
        }
      }
    }


    public override void draw(SpriteBatch b) {
      if (!Game1.options.showMenuBackground) {
        b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
      }
      IClickableMenu.drawTextureBox(
        b,
        Game1.mouseCursors,
        new Rectangle(384, 373, 18, 18),
        xPositionOnScreen,
        yPositionOnScreen,
        width,
        height,
        Color.White,
        4f
      );
      for (int i = 0; i < buttons.Length; i++) {
        IClickableMenu.drawTextureBox(
          b,
          Game1.mouseCursors,
          new Rectangle(384, 396, 15, 15),
          buttons[i].X,
          buttons[i].Y,
          buttons[i].Width,
          buttons[i].Height + 4,
          buttons[i].Contains(Game1.getMouseX(), Game1.getMouseY()) ? Color.Wheat : Color.White,
          4f,
          false
        );
        IClickableMenu.drawTextureBox(
          b,
          Game1.mouseCursors,
          new Rectangle(HasEnchantment(i + offset) ? 236 : 227, 425, 9, 9),
          buttons[i].X + 24,
          buttons[i].Y + (buttons[i].Height - 36) / 2,
          36,
          36,
          Color.White,
          4f,
          false
        );
        SpriteText.drawString(
          b,
          enchantments[i + offset].GetDisplayName(),
          buttons[i].X + 72,
          buttons[i].Y + buttons[i].Height / 2 - 24,
          color: incompatible[i + offset] ? 2 : -1 
        );
      }
      upArrow.draw(b);
      downArrow.draw(b);
      IClickableMenu.drawTextureBox(
        b,
        Game1.mouseCursors,
        new Rectangle(403, 383, 6, 6),
        scrollBarTrack.X,
        scrollBarTrack.Y,
        scrollBarTrack.Width,
        scrollBarTrack.Height,
        Color.White,
        4f
      );
      scrollBar.draw(b);
      inventory.draw(b);
      drawBox(b, inventory.inventory[selected].bounds, 4, Color.Blue * 0.5f);
      IClickableMenu.drawToolTip(b, inventory.hoverText, inventory.hoverTitle, hoverItem);
      drawMouse(b);
    }

    public void drawBox(SpriteBatch b, Rectangle rect, int border, Color color) {
      b.Draw(Game1.fadeToBlackRect, new Rectangle(rect.X, rect.Y, rect.Width - border, border), Color.Blue); // Up
      b.Draw(Game1.fadeToBlackRect, new Rectangle(rect.X + rect.Width - border, rect.Y, border, rect.Height - border), Color.Blue); // Right
      b.Draw(Game1.fadeToBlackRect, new Rectangle(rect.X + border, rect.Y + rect.Height - border, rect.Width - border, border), Color.Blue); // Down
      b.Draw(Game1.fadeToBlackRect, new Rectangle(rect.X, rect.Y + border, border, rect.Height - border), Color.Blue); // Left
    }
  }
}
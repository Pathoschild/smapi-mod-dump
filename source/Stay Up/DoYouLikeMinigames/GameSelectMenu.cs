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
using StardewValley.Minigames;
using System;

namespace Su226.DoYouLikeMinigames {
  class GameSelectMenu : IClickableMenu {
    private Rectangle[] buttons = new Rectangle[6];
    private int offset;
    private int maxOffset;

    private ClickableTextureComponent upArrow;
    private ClickableTextureComponent downArrow;
    private ClickableTextureComponent scrollBar;
    private Rectangle scrollBarTrack;
    private bool scrolling;
 
    private string error;
    private int errorTimer;
    private Random random = new Random();

    private Playable[] games = new Playable[] {
      new MinigamePlayable("abigail_game", () => new AbigailGame()),
      new MinigamePlayable("abigail_game_two", () => {
        if (M.Config.checkFriendship && Game1.player.getFriendshipHeartLevelForNPC("Abigail") < 2) {
          throw new CannotPlay("no_friendship");
        }
        return new AbigailGameTwo();
      }),
      new CalicoPlayable("calico_jack", 0, false),
      new CalicoPlayable("calico_jack_small", 100, false),
      new CalicoPlayable("calico_jack_big", 1000, true),
      new CranePlayable("crane", () => {
        if (M.Config.checkMoney) {
          if (Game1.player.Money < 500) {
            throw new CannotPlay("no_money");
          } else {
            Game1.player.Money -= 500;
          }
        }
        return new CraneGame();
      }),
      new CranePlayable("crane_practice", () => new CranePractice()),
      new MinigamePlayable("darts", () => {
        if (M.Config.checkIsland && !Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatFixed")) {
          throw new CannotPlay("no_island");
        }
        return new Darts();
      }),
      new FairPlayable("fishing", () => new FishingGameFixed()),
      new KartPlayable("minecart_endless", () => new MineCart(0, 2)),
      new KartPlayable("minecart_progress", () => new MineCart(0, 3)),
      new KartPlayable("old_minecart_endless", () => new OldMineCart(0, 2)),
      new KartPlayable("old_minecart_progress", () => new OldMineCart(0, 3)),
      new ClubPlayable("slots", () => new Slots()),
      new FairPlayable("target", () => new TargetGameFixed())
    };

    public GameSelectMenu() : base(
      (Game1.uiViewport.Width - 800) / 2,
      (Game1.uiViewport.Height  - 600) / 2,
      800,
      600,
      true
    ) {
      int buttonHeight = (height - 32) / buttons.Length;
      for (int i = 0; i < buttons.Length; i++) {
        buttons[i] = new Rectangle(0, 0, width - 32, buttonHeight);
      }
      maxOffset = games.Length - buttons.Length;
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
        height - 140
      );
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
      downArrow.bounds.Y = yPositionOnScreen + height - 64;
      scrollBarTrack.X = upArrow.bounds.X + 12;
      scrollBarTrack.Y = upArrow.bounds.Y + upArrow.bounds.Height + 4;
      scrollBar.bounds.X = scrollBarTrack.X;
      PlaceScrollBar();
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

    public override void update(GameTime time) {
      if (errorTimer > 0) {
        if (--errorTimer == 0) {
          error = null;
        }
      }
    }

    public override void gameWindowSizeChanged(Rectangle oldrect, Rectangle newrect) {
      base.gameWindowSizeChanged(oldrect, newrect);
      PlaceWidgets();
    }

    public override void performHoverAction(int x, int y) {
      upArrow.tryHover(x, y);
      downArrow.tryHover(x, y);
      scrollBar.tryHover(x, y);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true) {
      base.receiveLeftClick(x, y);
      for (int i = 0; i < buttons.Length; i++) {
        if (buttons[i].Contains(x, y)) {
          try {
            games[i + offset].Play();
            this.exitThisMenu();
          } catch (CannotPlay e) {
            error = e.Message;
            Game1.playSound("cancel");
            errorTimer = 120;
          }
          break;
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
        SpriteText.drawString(
          b,
          games[i + offset].name,
          buttons[i].X + 48,
          buttons[i].Y + buttons[i].Height / 2 - 24
        );
      }
      if (error != null) {
        SpriteText.drawString(
          b,
          error,
          xPositionOnScreen + 16 + random.Next(-8, 8),
          yPositionOnScreen + height + 16 + random.Next(-8, 8),
          color: 2
        );
      } else {
        SpriteText.drawString(
          b,
          string.Format("{0} - {1} ({2})", offset + 1, offset + buttons.Length, games.Length),
          xPositionOnScreen + 16,
          yPositionOnScreen + height + 16,
          color: 4
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
      drawMouse(b);
    }
  }
}

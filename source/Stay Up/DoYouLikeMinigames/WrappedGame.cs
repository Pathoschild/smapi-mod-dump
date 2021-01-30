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
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewModdingAPI;
using System.Collections.Generic;

namespace Su226.DoYouLikeMinigames {
  class WrappedGame<T> : IMinigame where T : IMinigame {
    public T game;
    public virtual bool tick(GameTime time) => game.tick(time);
    public virtual bool overrideFreeMouseMovement() => game.overrideFreeMouseMovement();
    public virtual bool doMainGameUpdates() => game.doMainGameUpdates();
    public virtual void receiveLeftClick(int x, int y, bool playSound) => game.receiveLeftClick(x, y, playSound);
    public virtual void leftClickHeld(int x, int y) => game.leftClickHeld(x, y);
    public virtual void receiveRightClick(int x, int y, bool playSound) => game.receiveRightClick(x, y, playSound);
    public virtual void releaseLeftClick(int x, int y) => game.releaseLeftClick(x, y);
    public virtual void releaseRightClick(int x, int y) => game.releaseRightClick(x, y);
    public virtual void receiveKeyPress(Keys k) => game.receiveKeyPress(k);
    public virtual void receiveKeyRelease(Keys k) => game.receiveKeyRelease(k);
    public virtual void draw(SpriteBatch b) => game.draw(b);
    public virtual void changeScreenSize() => game.changeScreenSize();
    public virtual void unload() => game.unload();
    public virtual void receiveEventPoke(int data) => game.receiveEventPoke(data);
    public virtual string minigameId() => game.minigameId();
    public virtual bool forceQuit() => game.forceQuit();
  }

  class AbigailGameTwo: WrappedGame<AbigailGame> {
    public AbigailGameTwo() {
      game = new AbigailGame(true);
    }

    public override void receiveKeyPress(Keys k) {
      base.receiveKeyPress(k);
      if (k == Keys.Escape) {
        game.quit = true;
      }
    }
  }

  class CalicoJackExitable: WrappedGame<CalicoJack> {
    private int toBet;
    private bool highStakes;

    private ClickableComponent playAgain;
    private IReflectedField<bool> showingResultScreen;

    public CalicoJackExitable(int toBet = -1, bool highStakes = false) {
      game = new CalicoJack(toBet, highStakes);
      this.toBet = toBet;
      this.highStakes = highStakes;
      playAgain = M.Helper.Reflection.GetField<ClickableComponent>(game, "playAgain").GetValue();
      showingResultScreen = M.Helper.Reflection.GetField<bool>(game, "showingResultsScreen");
    }

    public override void receiveKeyPress(Keys k) {
      base.receiveKeyPress(k);
      if (k == Keys.Escape) {
        Game1.currentMinigame = null;
        Game1.playSound("bigDeSelect");
      }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true) {
      if (showingResultScreen.GetValue() && playAgain.containsPoint(x, y) && (
        toBet == 0 || Game1.player.clubCoins >= toBet || !M.Config.checkQiCoin
      )) {
        Game1.currentMinigame = new CalicoJackExitable(toBet, highStakes);
        Game1.playSound("smallSelect");
        return;
      }
      base.receiveLeftClick(x, y, playSound);
    }

    public override void draw(SpriteBatch b) {
      base.draw(b);
      if (showingResultScreen.GetValue() && Game1.player.clubCoins < toBet && (
        M.Config.checkQiCoin || toBet == 0)
      ) {
        b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        IClickableMenu.drawTextureBox(
          b,
          Game1.mouseCursors,
          new Rectangle(403, 373, 9, 9),
          playAgain.bounds.X,
          playAgain.bounds.Y,
          playAgain.bounds.Width,
          playAgain.bounds.Height,
          Color.White,
          4f * playAgain.scale
        );
        SpriteText.drawString(b, playAgain.label, playAgain.bounds.X + 32, playAgain.bounds.Y + 8);
        b.End();
      }
    }
  }

  class CranePractice : WrappedGame<CraneGame> {
    private IList<Item> collectedItems;

    public CranePractice() {
      game = new CraneGame();
      collectedItems = game.GetObjectOfType<CraneGame.GameLogic>().collectedItems;
    }

    public override bool tick(GameTime time) {
      collectedItems.Clear();
      return base.tick(time);
    }
  }

  class FishingGameFixed : WrappedGame<FishingGame> {
    private GameLocation map;
    private Vector2 pos;
    private int facing;

    public FishingGameFixed() {
      map = Game1.player.currentLocation;
      pos = Game1.player.position;
      facing = Game1.player.facingDirection;
      game = new FishingGame();
      GameLocation loc = M.Helper.Reflection.GetField<GameLocation>(game, "location").GetValue();
      LocationRequest request = new LocationRequest(loc.name, loc.isStructure, loc);
      Game1.warpFarmer(request, 14, 7, 3);
    }

    public override void unload() {
      LocationRequest request = new LocationRequest(map.name, map.isStructure, map);
      request.OnWarp += () => {
        Game1.player.Position = pos;
      };
      Game1.warpFarmer(request, 0, 0, facing);
      base.unload();
    }
  }

  class TargetGameFixed : WrappedGame<TargetGame> {
    private GameLocation map;
    private Vector2 pos;
    private int facing;

    public TargetGameFixed() {
      map = Game1.player.currentLocation;
      pos = Game1.player.position;
      facing = Game1.player.facingDirection;
      game = new TargetGame();
      GameLocation loc = M.Helper.Reflection.GetField<GameLocation>(game, "location").GetValue();
      LocationRequest request = new LocationRequest(loc.name, loc.isStructure, loc);
      Game1.warpFarmer(request, 8, 13, 0);
    }

    public override void unload() {
      LocationRequest request = new LocationRequest(map.name, map.isStructure, map);
      request.OnWarp += () => {
        Game1.player.Position = pos;
      };
      Game1.warpFarmer(request, 0, 0, facing);
      base.unload();
    }
  }
}
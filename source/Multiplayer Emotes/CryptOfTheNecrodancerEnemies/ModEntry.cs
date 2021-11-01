/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using CryptOfTheNecrodancerEnemies.Framework.Constants;
using CryptOfTheNecrodancerEnemies.Framework.Extensions;
using CryptOfTheNecrodancerEnemies.Framework.Patches;
using CryptOfTheNecrodancerEnemies.Framework.Patches.Monsters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;

namespace CryptOfTheNecrodancerEnemies {

  public class ModEntry : Mod, IAssetLoader {

    public static IMonitor ModMonitor { get; private set; }
    public static IModHelper ModHelper { get; private set; }

    public override void Entry(IModHelper helper) {

      ModMonitor = Monitor;
      ModHelper = Helper;

      ModPatchManager patchManager = new(helper, new List<IClassPatch>{
        // MonsterPatch.ParseMonsterInfoPatch.CreatePatch(helper.Reflection),
        // MonsterPatch.ReloadSpritePatch.CreatePatch(helper.Reflection),
        SpriteBatchPatch.InternalDrawPatch.CreatePatch(helper.Reflection),
        //AnimatedSpritePatch.LoadTexturePatch.CreatePatch(helper.Reflection),
        ////BatPatch.ReloadSpritePatch.CreatePatch(helper.Reflection),
        //CharacterPatch.SpriteGetterPatch.CreatePatch(helper.Reflection),
        //CharacterPatch.SpriteSetterPatch.CreatePatch(helper.Reflection),
        ////CharacterPatch.GetShadowOffsetPatch.CreatePatch(helper.Reflection),
        ////AnimatedSpritePatch.AnimatePatch.CreatePatch(helper.Reflection),
      });
      patchManager.ApplyPatch();

      helper.ConsoleCommands.Add("reload_sprites", $"Reload the mod sprites.", this.ReloadSpritesCommand);

      helper.Events.World.LocationListChanged += OnLocationListChanged;
      helper.Events.World.NpcListChanged += OnNpcListChanged;

#if DEBUG
      var monsters = "Green Slime\nDust Spirit\nBat\nFrost Bat\nLava Bat\nIridium Bat\nStone Golem\n" +
        "Wilderness Golem\nGrub\nFly\nFrost Jelly\nSludge\nShadow Guy\nGhost\nCarbon Ghost\nDuggy\n" +
        "Rock Crab\nLava Crab\nIridium Crab\nFireball\nSquid Kid\nSkeleton Warrior\nCrow\nFrog\nCat\n" +
        "Shadow Brute\nShadow Shaman\nSkeleton\nSkeleton Mage\nMetal Head\nSpiker\nBug\nMummy\nBig Slime\n" +
        "Serpent\nPepper Rex\nTiger Slime\nLava Lurk\nHot Head\nMagma Sprite\nMagma Duggy\nMagma Sparker\n" +
        "False Magma Cap\nDwarvish Sentry\nPutrid Ghost\nShadow Sniper\nSpider\nRoyal Serpent\nBlue Squid";
      helper.ConsoleCommands.Add("spawn", $"Spawn monster to current player position. List: \n{monsters}", this.SpawnCommand);

      helper.Events.GameLoop.SaveLoaded += this.DebugOnSaveLoaded;
      helper.Events.Input.ButtonPressed += this.DebugOnButtonPressed;
#endif
    }

    private void OnLocationListChanged(object sender, LocationListChangedEventArgs e) {
      foreach (var location in e.Added) {
        PrepareMonsterSprites(location.characters);
      }
    }

    private void OnNpcListChanged(object sender, NpcListChangedEventArgs e) {
      if (!e.IsCurrentLocation) return;
      PrepareMonsterSprites(e.Added);
    }

    private void PrepareMonsterSprites(IEnumerable<NPC> collection) {
      foreach (var npc in collection) {
        if (npc is Monster) {
          var textureName = npc.Sprite.Texture?.Name;
          if (Sprites.Assets.TryGetFromNullableKey(textureName, out SpriteAsset spriteAsset) && spriteAsset.ShouldResize) {
            npc.Scale = spriteAsset.Scale;
            npc.Sprite.SpriteWidth = spriteAsset.SourceRectangle.Width;
            npc.Sprite.SpriteHeight = spriteAsset.SourceRectangle.Height;
            npc.Sprite.UpdateSourceRect();
          }
        }
      }
    }

    private void ReloadSpritesCommand(string arg1, string[] arg2) {
      this.Monitor.Log($"Reloading sprites...", LogLevel.Info);
      Sprites.ReloadSpriteAssets();
      this.Monitor.Log($"Finished reloading sprites.", LogLevel.Info);
    }

    /// <summary>Get whether this instance can load the initial version of the given asset.</summary>
    /// <param name="asset">Basic metadata about the asset being loaded.</param>
    public bool CanLoad<T>(IAssetInfo asset) {
      return Sprites.Assets.Any((entry) => asset.AssetNameEquals(entry.Key));
    }

    /// <summary>Load a matched asset.</summary>
    /// <param name="asset">Basic metadata about the asset being loaded.</param>
    public T Load<T>(IAssetInfo asset) {
      return this.Helper.Content.Load<T>(Sprites.Assets[asset.AssetName].File);
    }

#if DEBUG
    /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void DebugOnSaveLoaded(object sender, SaveLoadedEventArgs e) {
      if (!Context.IsMainPlayer) return;

      // Initialize and setup debug world
      SetupDebugWorld();
    }

    private void SpawnCommandMonster(string name, Vector2 position) {
      SpawnCommand("spawn", new[] { name, $"{position.X}", $"{position.Y}" });
    }

    private void SpawnCommand(string command, string[] args) {
      if (!Context.IsMainPlayer) return;

      var position = Game1.player.getStandingPosition();
      if (args.Length > 1 && int.TryParse(args[1], out int x) && int.TryParse(args[2], out int y)) {
        position.X = x;
        position.Y = y;
      }

      if (!Game1.game1.parseDebugInput($"monster {args[0]} {position.X} {position.Y}")) {
        this.Monitor.Log($"Data for \"{args[0]}\" monster not found.", LogLevel.Error);
      }
    }

    private void DebugOnButtonPressed(object sender, ButtonPressedEventArgs e) {
      var tile = Helper.Input.GetCursorPosition().Tile;
      switch (e.Button) {
        case SButton.NumPad0:
          Game1.game1.parseDebugInput("setUpBigFarm");
          break;
        case SButton.NumPad1:
          //SpawnMonster.SquidKid(tile);
          //SpawnMonster.SquidKidDangerous(tile);
          //SpawnMonster.Duggy(tile);
          //SpawnMonster.DuggyDangerous(tile);
          //SpawnMonster.MagmaDuggy(tile);
          //SpawnMonster.Bat(tile);
          //SpawnMonster.BatDangerous(tile);
          //SpawnMonster.HauntedSkull(tile);
          //SpawnMonster.HauntedSkullDangerous(tile);
          //SpawnMonster.FrostSlime(tile);
          //SpawnMonster.SludgeSlime(tile);
          //SpawnMonster.TigerSlime(tile);
          //SpawnMonster.PrismaticSlime(tile);
          //SpawnMonster.BigSlime(tile);
          //SpawnMonster.BigSlimeGreen(tile);
          //SpawnMonster.BigSlimeBlue(tile);
          //SpawnMonster.BigSlimeRed(tile);
          //SpawnMonster.BigSlimePurple(tile);
          //SpawnMonster.Spider(tile);
          SpawnMonster.SkeletonMage(tile);
          break;
        case SButton.NumPad2:
          Game1.isTimePaused = false;
          _frameByFrameButton = e.Button;
          this.Helper.Events.Display.RenderedWorld += OnDebugRenderedWorld;
          break;
        case SButton.NumPad3:
          Game1.game1.parseDebugInput("pausetime");
          break;
        case SButton.NumPad9:
          Sprites.ReloadSpriteAssets();
          //Helper.Content.InvalidateCache(asset => asset.DataType == typeof(Texture2D) && Sprites.Assets.ContainsKey(asset.AssetName));
          break;
      }
    }

    private SButton _frameByFrameButton = SButton.None;
    private void OnDebugRenderedWorld(object sender, RenderedWorldEventArgs e) {
      if (Helper.Input.IsDown(_frameByFrameButton)) return;
      Game1.isTimePaused = true;
      this.Helper.Events.Display.RenderedWorld -= OnDebugRenderedWorld;
    }

    private void SetupDebugWorld() {
      Game1.game1.parseDebugInput("nosave");

      //Game1.game1.parseDebugInput("zoomlevel 40");
      Game1.game1.parseDebugInput("zoomlevel 110");

      if (Context.IsMainPlayer) {

        // Pause time and set it to 09:00
        Game1.game1.parseDebugInput("pausetime");
        Game1.game1.parseDebugInput("invincible");
        Game1.game1.parseDebugInput("time 0900");

        Game1.game1.parseDebugInput("warp Farm 64 15");
        Game1.game1.parseDebugInput("weapon 62");

        /*
        Game1.game1.parseDebugInput("pet 64 15");
        Game1.game1.parseDebugInput("petToFarm");

        //Game1.game1.parseDebugInput("setUpBigFarm");
        Game1.game1.parseDebugInput("setUpFarm");

        // Coop Animals
        Game1.game1.parseDebugInput("animal Chicken");
        Game1.game1.parseDebugInput("animal Duck");
        Game1.game1.parseDebugInput("animal Rabbit");
        Game1.game1.parseDebugInput("animal Dinosaur");

        // Barn Animals
        Game1.game1.parseDebugInput("animal Cow");
        Game1.game1.parseDebugInput("animal Goat");
        Game1.game1.parseDebugInput("animal Sheep");
        Game1.game1.parseDebugInput("animal Pig");
        */
      }

    }


#endif

  }

}

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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Characters;
using StardewValley.Locations;

namespace Su226.StayUp {
  class M {
    public static IModHelper Helper;
    public static IMonitor Monitor;
    public static Config Config;
  }
  class StayUp : Mod {
    private LightTransition light;

    private bool canCallNewDay; // Prevent sleep repeatedly.
    private bool restoreData;

    private string map;
    private Vector2? pos;
    private int facing;
    private ISittable sitted;
    private float stamina;
    private int health;

    private Horse horse;
    private string horseMap;
    private Vector2 horsePos;
    private int horseFacing;

    public override void Entry(IModHelper helper) {
      Monitor.Log("Starting.");
      M.Helper = helper;
      M.Monitor = Monitor;
      M.Config = helper.ReadConfig<Config>();

      helper.Events.GameLoop.DayStarted += this.OnDayStarted;
      helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;

      if (M.Config.editFishes) {
        Monitor.Log("Fish editing enabled.");
        helper.Content.AssetEditors.Add(new FishEditor());
      }

      if (M.Config.morningLight != -1) {
        Monitor.Log("Light transition enabled.");
        this.light = new LightTransition();
      }
    }

    private void OnDayStarted(object o, DayStartedEventArgs e) {
      if (this.restoreData) {
        if (M.Config.keepFarmer) {
          Monitor.Log("Restore player position.");
          LocationRequest request = Game1.getLocationRequest(map);
          request.OnWarp += delegate {
            Game1.fadeToBlackAlpha = M.Config.smoothSaving ? 1 : -.2f; // Hide fading from black
            if (this.pos != null) { // Restore exact position if it's not mine or dungeon.
              Game1.player.Position = this.pos.Value;
            }
            if (this.sitted != null) { // Sit down
              this.sitted.AddSittingFarmer(Game1.player);
              Game1.player.sittingFurniture = this.sitted;
              Game1.player.isSitting.Value = true;
            }
          };
          // Place player at the entrance if it's mine or dungeon.
          if (request.Location is VolcanoDungeon dungeon) {
            this.pos = null;
            Point point = dungeon.startPosition.Value;
            Game1.warpFarmer(request, point.X, point.Y, 2);
          } else if (request.Location is MineShaft) {
            this.pos = null;
            Game1.warpFarmer(request, 6, 6, 2);
          } else {
            Game1.warpFarmer(request, 0, 0, this.facing);
          }
          Game1.fadeToBlackAlpha = 1.2f; // Hide fading to black
          if (Game1.player.mount != null) { // Remove the orphaned horse.
            Game1.getFarm().characters.Remove(Game1.getFarm().getCharacterFromName(Game1.player.horseName));
          }
        } else {
          Monitor.Log("Discard player position.");
          Horse mountedHorse = Game1.player.mount;
          if (mountedHorse != null) { // Dismount and remove horse
            mountedHorse.dismount();
            mountedHorse.currentLocation.characters.Remove(mountedHorse);
          }
          Game1.player.changeOutOfSwimSuit(); // Reset from bathhub
          Game1.player.swimming.Value = false;
        }
        if (M.Config.keepStamina) {
          Monitor.Log("Restore player stamina.");
          Game1.player.stamina = this.stamina;
        }
        if (M.Config.keepHealth) {
          Monitor.Log("Restore player health.");
          Game1.player.health = this.health;
        }
        if (M.Config.keepHorse && this.horse != null) {
          Monitor.Log("Restore horse position.");
          Game1.warpCharacter(this.horse, this.horseMap, this.horsePos / 64);
          this.horse.faceDirection(this.horseFacing);
        }
        this.restoreData = false;
      }
      this.canCallNewDay = false;
    }

    private void OnTimeChanged(object o, TimeChangedEventArgs e) {
      if (Game1.dayTimeMoneyBox.timeShakeTimer != 0 && M.Config.noTimeShake) {
        Monitor.Log("Time shake supressed.");
        Game1.dayTimeMoneyBox.timeShakeTimer = 0;
      }
      if ((e.NewTime == 2400 || e.NewTime == 2500) && M.Config.noTiredEmote) {
        Monitor.Log("Tired emote supressed.");
        Game1.player.isEmoting = false;
      }
      if (e.NewTime == 2550 && M.Config.stayUp) {
        Monitor.Log("Stay up.");
        this.canCallNewDay = M.Config.newDayAt6Am;
        Game1.timeOfDay = 150;
      }
      if (e.NewTime == 600 && this.canCallNewDay) {
        this.SavePlayer();
        this.NewDay();
      }
    }

    private void SavePlayer() {
      Monitor.Log("Save player data.");
      this.restoreData = true;
      this.map = Game1.player.currentLocation.NameOrUniqueName;
      this.pos = Game1.player.Position;
      this.facing = Game1.player.facingDirection;
      this.sitted = Game1.player.sittingFurniture;
      this.stamina = Game1.player.stamina;
      this.health = Game1.player.health;
      this.horse = Game1.getCharacterFromName<Horse>(Game1.player.horseName, false);
      if (this.horse != null) {
        Monitor.Log("Save horse data.");
        this.horseMap = this.horse.currentLocation.NameOrUniqueName;
        this.horsePos = this.horse.Position;
        this.horseFacing = this.horse.FacingDirection;
      }
    }

    private void NewDay() {
      Monitor.Log("Start new day.");
      ReadyCheckDialog.behavior doNewDay = delegate {
        Game1.player.lastSleepLocation.Value = Game1.player.currentLocation.NameOrUniqueName;
        Game1.player.lastSleepPoint.Value = Game1.player.getTileLocationPoint();
        Game1.dialogueUp = false; // Close "Go to bed" dialogue to prevent stuck.
        Game1.currentMinigame = null;
        Game1.activeClickableMenu?.emergencyShutDown();
        Game1.activeClickableMenu = null;
        Game1.newDay = true;
        Game1.newDaySync = new NewDaySynchronizer();
        Game1.fadeScreenToBlack();
        Game1.fadeToBlackAlpha = M.Config.smoothSaving ? 0f : 1.2f;
      };
      if (Game1.IsMultiplayer) {
        Game1.activeClickableMenu?.emergencyShutDown();
        Game1.activeClickableMenu = new ReadyCheckDialog("sleep", false, doNewDay);
      } else {
        doNewDay(null);
      }
    }
  }
}
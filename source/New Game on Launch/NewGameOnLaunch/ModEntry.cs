using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace NewGameOnLaunch
{
    class ModEntry : Mod {
        ModConfig Config;

        public override void Entry(IModHelper helper) {
            GameEvents.FirstUpdateTick += FirstUpdateTick;
            Config = Helper.ReadConfig<ModConfig>();
        }

        public Vector2 GetDefaultSpawnPosition() {
            var bed = (Utility.PointToVector2((Game1.player.currentLocation as StardewValley.Locations.FarmHouse).getBedSpot()) * Game1.tileSize);
            bed.X -= Game1.tileSize; //Spawn a tile to the side of the bed to not instantly go to the sleep dialog
            return bed;
        }

        public void FasterNewDay() {
            Game1.currentMinigame = null;
            Game1.newDay = false;
            Game1.newDaySync = new NewDaySynchronizer();
            Game1.nonWarpFade = false;

            Game1.player.currentEyes = 1;
            Game1.player.blinkTimer = 0;
            Game1.player.CanMove = true;
            Game1.pauseTime = 0.0f;

            if (Game1.activeClickableMenu == null || Game1.dialogueUp)
                return;
            Game1.activeClickableMenu.emergencyShutDown();
            Game1.exitActiveMenu();
        }

        public void CustomizeCharacter() {
            Game1.player.Name = Config.CharacterName;
            Game1.player.farmName.Value = Config.FarmName;
            Game1.player.favoriteThing.Value = Config.FavoriteThing;
            Game1.player.displayName = Game1.player.Name;
            Game1.player.changeAccessory(Config.Accessory);
            Game1.player.changeEyeColor(Config.EyeColor);
            Game1.player.changeGender(Config.Male);
            Game1.player.changeHairColor(Config.HairColor);
            Game1.player.changeHairStyle(Config.HairStyle);
            Game1.player.changePants(Config.PantsColor);
            Game1.player.changeShirt(Config.ShirtType);
            Game1.player.changeSkinColor(Config.SkinColor);
            Game1.player.isCustomized.Value = true;
        }

        public void LoadFirstDay() {
            Game1.loadForNewGame(false);
            Game1.player.Position = GetDefaultSpawnPosition();
            Game1.player.isInBed.Value = true;
            Game1.saveOnNewDay = false;
            FasterNewDay();
            Game1.eventUp = false;
            Game1.dayOfMonth = 1;
            Game1.setGameMode(3);
        }

        public void FirstUpdateTick(object sender, EventArgs args) {
            CustomizeCharacter();
            LoadFirstDay();
        }
    }
}

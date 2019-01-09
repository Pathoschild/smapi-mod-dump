using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Patches.WorldChangeEventPatches
{
    public class setUpPatch
    {
        private static CustomFarmManager farmManager;

        public setUpPatch(CustomFarmManager farmManager) {
            setUpPatch.farmManager = farmManager;
        }

        public static bool Prefix(WorldChangeEvent __instance, ref bool __result) {
            NetInt evt = (NetInt)Traverse.Create(__instance).Field("whichEvent").GetValue();
            if (!farmManager.Canon && evt.Value == 1) {
                Game1.currentLightSources.Clear();
                Traverse.Create(__instance).Field("location").SetValue(null);
                int targetXTile = 64;
                int targetYTile = 116;
                //__instance.cutsceneLengthTimer = 8000;
                Traverse.Create(__instance).Field("cutsceneLengthTimer").SetValue(8000);
                //__instance.wasRaining = Game1.isRaining;
                Traverse.Create(__instance).Field("wasRaining").SetValue(Game1.isRaining);
                Game1.isRaining = false;

                GameLocation loc = (GameLocation)Traverse.Create(__instance).Field("location").GetValue();
                loc = Game1.getLocationFromName("Farm");
                loc.resetForPlayerEntry();

                Utility.addSprinklesToLocation(loc, targetXTile, 12, 7, 7, 15000, 150, Color.LightCyan, null, false);
                Utility.addStarsAndSpirals(loc, targetXTile, 12, 7, 7, 15000, 150, Color.White, null, false);
                Game1.player.activeDialogueEvents.Add("cc_Greenhouse", 3);
                //__instance.sound = "junimoMeep1";
                Traverse.Create(__instance).Field("sound").SetValue("junimoMeep1");
                Game1.currentLightSources.Add(new LightSource(4, new Vector2((float)targetXTile, (float)targetYTile) * 64f, 4f, Color.DarkGoldenrod));
                loc.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Rectangle(294, 1432, 16, 16), 300f, 4, 999, new Vector2(1792f, 704f), false, false) {
                    scale = 4f,
                    layerDepth = 1f,
                    xPeriodic = true,
                    xPeriodicLoopTime = 2000f,
                    xPeriodicRange = 16f,
                    light = true,
                    lightcolor = Color.DarkGoldenrod,
                    lightRadius = 1f
                });
                //this.soundInterval = 800;
                Traverse.Create(__instance).Field("soundInterval").SetValue(800);

                //this.soundTimer = this.soundInterval;
                Traverse.Create(__instance).Field("soundTimer").SetValue(800);
                Traverse.Create(__instance).Field("location").SetValue(loc);
                Game1.currentLocation = loc;
                Game1.fadeClear();
                Game1.nonWarpFade = true;
                Game1.timeOfDay = 2400;
                Game1.displayHUD = false;
                Game1.viewportFreeze = true;
                Game1.player.position.X = -999999f;
                Game1.viewport.X = Math.Max(0, Math.Min(loc.map.DisplayWidth - Game1.viewport.Width, targetXTile * 64 - Game1.viewport.Width / 2));
                Game1.viewport.Y = Math.Max(0, Math.Min(loc.map.DisplayHeight - Game1.viewport.Height, targetYTile * 64 - Game1.viewport.Height / 2));
                Game1.changeMusicTrack("nightTime");
                __result = false;
                return false;
            }
            return true;
        }
    }
}

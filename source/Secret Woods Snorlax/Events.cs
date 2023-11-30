/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/SecretWoodsSnorlax
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ichortower.SecretWoodsSnorlax
{
    internal class Events
    {
        public static int FluteId = -1;
        public static bool FluteHeardToday = false;
        public static JsonAssets.IApi JAApi = null;

        public static void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            string path;
            JAApi = ModEntry.HELPER.ModRegistry.GetApi<JsonAssets.IApi>(
                    "spacechase0.JsonAssets");
            if (JAApi is null) {
                ModEntry.MONITOR.Log("CRITICAL: could not load the Json " +
                        "Assets API. This shouldn't be possible, so this mod " +
                        "install is probably broken.", LogLevel.Error);
                ModEntry.MONITOR.Log("Please try reinstalling, updating (if " +
                        "available), or complaining to ichortower about it.",
                        LogLevel.Error);
            }
            else {
                path = Path.Combine(ModEntry.HELPER.DirectoryPath,
                        "assets", "[JA] Embedded Pack");
                JAApi.LoadAssets(path);
            }

            /* change moved location if lunna is installed */
            bool haveLunna = ModEntry.HELPER.ModRegistry.IsLoaded("Rafseazz.LunnaCP");
            if (haveLunna) {
                Constants.vec_MovedPosition = new Vector2(7f, 7f);
            }

            /* Load the flute musics (sound effects).
             * They're short, but snarfing an ogg does take time */
            Thread t = new Thread((ThreadStart)delegate {
                Ogg.LoadSound(Constants.name_FluteCue, Path.Combine(
                        ModEntry.HELPER.DirectoryPath, "assets", "melody.ogg"));
                Ogg.LoadSound(Constants.name_FluteCueShort, Path.Combine(
                        ModEntry.HELPER.DirectoryPath, "assets", "melody_short.ogg"));
            });
            t.Start();

            /* Set up event commands for the wizard event */
            var SCApi = ModEntry.HELPER.ModRegistry.GetApi<SpaceCore.IApi>(
                    "spacechase0.SpaceCore");
            if (SCApi is null) {
                ModEntry.MONITOR.Log("CRITICAL: could not load the SpaceCore " +
                        "API. This shouldn't be possible, so this mod " +
                        "install is probably broken.", LogLevel.Error);
                ModEntry.MONITOR.Log("Please try reinstalling, updating (if " +
                        "available), or complaining to ichortower about it.",
                        LogLevel.Error);
            }
            else {
                MethodInfo giveKeyMethod = typeof(Events).GetMethod(
                        "command_giveKey", BindingFlags.Static | BindingFlags.Public);
                MethodInfo holdKeyMethod = typeof(Events).GetMethod(
                        "command_holdKey", BindingFlags.Static | BindingFlags.Public);
                SCApi.AddEventCommand("SWS_giveKey", giveKeyMethod);
                SCApi.AddEventCommand("SWS_holdKey", holdKeyMethod);
            }
        }

        public static void getFluteId()
        {
            if (FluteId == -1) {
                FluteId = JAApi.GetObjectId(Constants.name_Flute);
            }
        }

        public static void command_giveKey(Event e, GameLocation location,
                GameTime time, string[] split)
        {
            getFluteId();
            Object flute = new Object(Vector2.Zero, FluteId, 1);
            e.farmer.addItemByMenuIfNecessary(flute);
            e.CurrentCommand++;
        }

        public static void command_holdKey(Event e, GameLocation location,
                GameTime time, string[] split)
        {
            getFluteId();
            e.farmer.holdUpItemThenMessage(new Object(Vector2.Zero, FluteId, 1));
            e.CurrentCommand++;
        }


        public static void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            /*
             * Spawn the snorlax. If the forest log is already gone, put him
             * in the moved location and flag it. Otherwise, replace the log.
             */
            Forest forest = (Forest)Game1.getLocationFromName("Forest");
            if (Game1.player.mailReceived.Contains(Constants.mail_SnorlaxMoved)) {
                forest.log = new SnorlaxLog(Constants.vec_MovedPosition);
                return;
            }
            if (forest.log is null) {
                forest.log = new SnorlaxLog(Constants.vec_MovedPosition);
                Game1.player.mailReceived.Add(Constants.mail_SnorlaxMoved);
            }
            else {
                forest.log = new SnorlaxLog(Constants.vec_BlockingPosition);
            }

            getFluteId();
            FluteHeardToday = false;
        }

        public static void OnSaving(object sender, SavingEventArgs e)
        {
            /*
             * Here, we revert the snorlax to how the log would be in vanilla:
             * if he's moved, delete the log, and if he hasn't, restore it.
             */
            Forest forest = (Forest)Game1.getLocationFromName("Forest");
            // check the mail id first; fall back to current location
            if (Game1.player.mailReceived.Contains(Constants.mail_SnorlaxMoved)) {
                forest.log = null;
                return;
            }
            if (forest.log != null && forest.log is SnorlaxLog) {
                if (forest.log.tile.Value == Constants.vec_BlockingPosition) {
                    forest.log = new ResourceClump(602, 2, 2, new Vector2(1f, 6f));
                }
                else { //if (forest.log.tile.Value.X == 3f)
                    forest.log = null;
                }
            }
        }

        /*
         * Feels a bit yucky listening to input events to play the flute.
         * Harmony probably closer to target, but I'm trying not to use it
         * in this mod.
         */
        public static void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            getFluteId();
            if (Game1.player.ActiveObject is null || Game1.player.ActiveObject
                    .ParentSheetIndex != FluteId) {
                return;
            }
            foreach (var button in e.Pressed) {
                if (button.IsActionButton()) {
                    Events.PlayFlute(button);
                    break;
                }
            }
        }

        private static void PlayFlute(SButton button)
        {
            bool normalGameplay = !Game1.eventUp && !Game1.isFestival() &&
                    !Game1.fadeToBlack && !Game1.player.swimming.Value &&
                    !Game1.player.bathingClothes.Value &&
                    !Game1.player.onBridge.Value &&
                    Game1.player.CanMove && !Game1.freezeControls &&
                    Game1.player.freezePause <= 0;
            if (!normalGameplay) {
                return;
            }
            GameLocation loc = Game1.player.currentLocation;
            // can't play inside, unless it's your house
            if (!loc.IsOutdoors && !loc.Name.Equals("FarmHouse")) {
                string text = ModEntry.HELPER.Translation.Get("flute.dontPlayHere");
                Game1.drawObjectDialogue(text.Replace("{{p}}", Game1.player.displayName));
                return;
            }
            if (Game1.player.currentLocation.Name.Equals("Forest") &&
                    Game1.player.getTileX() <= 6 &&
                    Game1.player.getTileY() <= 10) {
                // suppressing input prevents inspecting snorlax while
                // starting these cutscenes
                var snorlax = (Game1.player.currentLocation as Forest).log as SnorlaxLog;
                if (snorlax != null && !snorlax.HasMoved()) {
                    ModEntry.HELPER.Input.Suppress(button);
                    WakeUpCutscene();
                    return;
                }
                else if (!FluteHeardToday) {
                    ModEntry.HELPER.Input.Suppress(button);
                    RelistenCutscene();
                    return;
                }
            }
            int nowFacing = Game1.player.FacingDirection;
            int msPerBeat = Constants.msPerBeat;
            Game1.player.faceDirection(2);
            Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[6]{
                new FarmerSprite.AnimationFrame(98, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(99, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(100, 2*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(98, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(99, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(98, 2*msPerBeat, true, false),
            }, delegate {
                Game1.player.faceDirection(nowFacing);
            });
            loc.playSoundAt(Constants.name_FluteCueShort, Game1.player.getTileLocation());
            Game1.player.freezePause = 8*msPerBeat;
        }

        private static void WakeUpCutscene()
        {
            Game1.freezeControls = true;
            Game1.player.CanMove = false;
            Game1.player.faceDirection(2);
            Game1.player.onBridge.Value = true;
            int tally = 0;
            int beforeSongPause = 1500;
            int afterSongPause = 1200;
            int msPerBeat = Constants.msPerBeat;

            var snorlax = (Game1.player.currentLocation as Forest).log as SnorlaxLog;
            var startloc = new Vector2(2f, 7f);
            var endloc = new Vector2(4f, 5f);

            Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[14]{
                new FarmerSprite.AnimationFrame(16, 2*beforeSongPause/3, false, false),
                new FarmerSprite.AnimationFrame(98, 1*beforeSongPause/3, true, false),
                new FarmerSprite.AnimationFrame(98, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(99, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(100, 2*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(98, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(99, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(98, 3*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(100, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(98, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(99, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(98, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(99, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(100, 4*msPerBeat, true, false),
            });
            DelayedAction.functionAfterDelay(delegate {
                Game1.player.currentLocation.playSoundAt(Constants.name_FluteCue,
                        Game1.player.getTileLocation());
            }, beforeSongPause);
            tally += beforeSongPause + 18*msPerBeat;

            DelayedAction.functionAfterDelay(delegate {
                Game1.player.faceGeneralDirection(startloc * 64f, 0, false, true);
            }, tally);
            tally += afterSongPause;

            DelayedAction.functionAfterDelay(delegate {
                Game1.player.currentLocation.playSoundAt("sandyStep", startloc);
                Game1.player.currentLocation.playSoundAt("croak", startloc);
                snorlax.parentSheetIndex.Value = 1;
                Game1.player.doEmote(16);
                Game1.player.setRunning(true);
                Game1.player.controller = new PathFindController(Game1.player,
                        Game1.player.currentLocation, new Point(5, 10), 0,
                        delegate {
                            Game1.player.faceGeneralDirection(
                                    startloc * 64f, 0, false, true);
                        });
            }, tally);
            tally += 2400;

            DelayedAction.functionAfterDelay(delegate {
                snorlax.JumpAside();
                Game1.player.currentLocation.playSoundAt("dwoop", startloc);
            }, tally);
            tally += 1500;

            DelayedAction.functionAfterDelay(delegate {
                Game1.player.currentLocation.playSoundAt("secret1", endloc);
                Game1.player.mailReceived.Add(Constants.mail_SnorlaxMoved);
                FluteHeardToday = true;
            }, tally);
            tally += 2000;

            DelayedAction.functionAfterDelay(delegate {
                Game1.freezeControls = false;
                Game1.player.CanMove = true;
                Game1.player.onBridge.Value = false;
            }, tally);
        }

        private static void heartSprite(Vector2 tileP)
        {
            var heart = new TemporaryAnimatedSprite("LooseSprites\\Cursors",
                    new Microsoft.Xna.Framework.Rectangle(211, 428, 7, 6),
                    2000f, 1, 0,
                    tileP * 64f + new Vector2(48f, -32f),
                    false, false, 1f, 0f, Color.White, 4f, 0f, 0f, 0f){
                        motion = new Vector2(0f, -0.5f),
                        alphaFade = 0.01f
                    };
            Game1.player.currentLocation.TemporarySprites.Add(heart);
            Game1.player.currentLocation.playSoundAt("dwop", tileP);
        }

        // bunch of copy-paste from WakeUpCutscene.
        // less work than adding logic to the other one.
        private static void RelistenCutscene()
        {
            Game1.freezeControls = true;
            Game1.player.CanMove = false;
            Game1.player.faceDirection(2);
            Game1.player.onBridge.Value = true;
            int tally = 0;
            int beforeSongPause = 1500;
            int afterSongPause = 1200;
            int msPerBeat = Constants.msPerBeat;

            var snorlax = (Game1.player.currentLocation as Forest).log as SnorlaxLog;
            var soundloc = Constants.vec_MovedPosition + new Vector2(1f, 1f);

            Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[14]{
                new FarmerSprite.AnimationFrame(16, 2*beforeSongPause/3, false, false),
                new FarmerSprite.AnimationFrame(98, 1*beforeSongPause/3, true, false),
                new FarmerSprite.AnimationFrame(98, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(99, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(100, 2*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(98, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(99, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(98, 3*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(100, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(98, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(99, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(98, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(99, 1*msPerBeat, true, false),
                new FarmerSprite.AnimationFrame(100, 4*msPerBeat, true, false),
            });
            DelayedAction.functionAfterDelay(delegate {
                Game1.player.currentLocation.playSoundAt(Constants.name_FluteCue,
                        Game1.player.getTileLocation());
            }, beforeSongPause);

            DelayedAction.functionAfterDelay(delegate {
                snorlax.parentSheetIndex.Value = 1;
            }, beforeSongPause + 1000);
            DelayedAction.functionAfterDelay(delegate {
                heartSprite(soundloc);
            }, beforeSongPause + 2000);
            tally += beforeSongPause + 18*msPerBeat;

            DelayedAction.functionAfterDelay(delegate {
                Game1.player.faceGeneralDirection(soundloc * 64f, 0, false, true);
            }, tally);
            tally += afterSongPause;

            DelayedAction.functionAfterDelay(delegate {
                snorlax.JumpInPlace();
                Game1.player.currentLocation.playSoundAt("dwoop", soundloc);
            }, tally);
            tally += 1000;

            DelayedAction.functionAfterDelay(delegate {
                heartSprite(soundloc);
            }, tally);
            tally += 500;

            DelayedAction.functionAfterDelay(delegate {
                FluteHeardToday = true;
                Game1.freezeControls = false;
                Game1.player.CanMove = true;
                Game1.player.onBridge.Value = false;
            }, tally);
        }

        public static void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            /* tricky cheat here, splitting on '/'. through SMAPI, the asset
             * names passed to this function are already normalized */
            if (e.NameWithoutLocale.IsDirectlyUnderPath("Characters/Dialogue")) {
                string npcName = e.NameWithoutLocale.BaseName.Split("/")[2];
                for (int i = 1; i <= 3; ++i) {
                    var hintline = ModEntry.HELPER.Translation.Get(
                            $"hint.{i}.{npcName}");
                    string key = $"{Constants.ct_Prefix}{i}";
                    if (hintline.HasValue()) {
                        e.Edit(asset => {
                            var dict = asset.AsDictionary<string, string>();
                            dict.Data[key] = hintline.ToString();
                        });
                    }
                }
            }

            /*
             * add the wizard event. actually three entries, since there's a
             * fork and rejoin to change one line depending on if you have
             * already moved the snorlax (only old saves should see that line).
             */
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/WizardHouse")) {
                string asset = "assets/events.json";
                if (ModEntry.HELPER.ModRegistry.IsLoaded(
                        "FlashShifter.StardewValleyExpandedCP")) {
                    asset = "assets/events_sve.json";
                }
                var snorlax = ModEntry.HELPER.ModContent.Load
                        <Dictionary<string,string>>(asset);
                foreach (var entry in snorlax) {
                    string val = entry.Value.Replace("{{moved}}",
                            Constants.mail_SnorlaxMoved);
                    int haveIndex = -1;
                    int start = 0;
                    while ((haveIndex = val.IndexOf("{{i18n", start)) != -1) {
                        int tail = val.IndexOf("}}", haveIndex+1);
                        string subkey = val.Substring(haveIndex+7, tail-(haveIndex+7));
                        string subval = ModEntry.HELPER.Translation.Get(subkey);
                        val = val.Remove(haveIndex, tail+2-haveIndex)
                                .Insert(haveIndex, subval);
                        start += subval.Length;
                    }
                    snorlax[entry.Key] = val;
                }
                e.Edit(asset => {
                    var dict = asset.AsDictionary<string, string>();
                    foreach (var entry in snorlax) {
                        dict.Data[entry.Key] = entry.Value;
                    }
                });
            }

            /* add the empty farm events that set the CTs when the hints mail
             * is active */
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Farm")) {
                e.Edit(asset => {
                    var dict = asset.AsDictionary<string, string>();
                    for (int i = 1; i <= 3; ++i) {
                        string key = $"19112010{i}/n {Constants.mail_SnorlaxHints}";
                        if (i > 1) {
                            key += $"/e 19112010{i-1}/A {Constants.ct_Prefix}{i-1}";
                        }
                        string script = "continue/-100 -100/farmer -1000 -1000 0" +
                                "/ignoreEventTileOffset" +
                                $"/addConversationTopic {Constants.ct_Prefix}{i} 2" +
                                "/pause 50/end";
                        dict.Data[key] = script;
                    }
                });
            }
        }
    }
}

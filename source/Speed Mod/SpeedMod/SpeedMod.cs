/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Xebeth/StardewValley-SpeedMod
**
*************************************************/

using ModSettingsTabApi.Framework.Interfaces;
using ModSettingsTabApi.Events;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using StardewModdingAPI;
using System.Reflection;
using System.Threading;
using StardewValley;
using System.IO;
using System;

using Timer = System.Timers.Timer;

namespace SpeedMod
{
    public class SpeedMod : Mod, ISettingsTabApi
    {
        private static int _fileSystemWatcherEvents;
        private double _castingCooldownDelta;
        private FileSystemWatcher _watcher;
        private static float _sparklesID;
        private HUDMessage _hudMessage;
        private int _cooldownSeconds;
        private int _consumedStamina;
        private int _castingCooldown;
        private Timer _coolDownTimer;
        private TimeSpan? _cooldown;
        private DateTime? _lastUse;
        private int _healthAtStart;
        private Buff _buff;

#pragma warning disable CS0067
        public event EventHandler<OptionsChangedEventArgs> OptionsChanged;
#pragma warning restore CS0067
        public static ModConfig Config { get; private set; }

        private static event EventHandler<TeleportSuccessArgs> OnTeleportSuccess;

        public override void Entry(IModHelper modHelper)
        {
            const string ModSettingsTabUniqueID = "GilarF.ModSettingsTab";

            _sparklesID = ModManifest.UniqueID.GetHashCode();
            Config = Helper.ReadConfig<ModConfig>();

            modHelper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
            modHelper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            modHelper.Events.GameLoop.SaveLoaded += SpeedMod_GameLoaded;
            modHelper.Events.Input.ButtonPressed += Input_ButtonPressed;
            modHelper.Events.GameLoop.DayEnding += GameLoop_DayEnding;

            OnTeleportSuccess += SpeedMod_OnTeleportSuccess;

            if (Config != null)
            {
                Monitor.Log(Config.ToString(), LogLevel.Info);

                modHelper.Events.GameLoop.GameLaunched += (sender, args) =>
                {
                    if (InstallModTabSettingsIntegration(ModSettingsTabUniqueID) == false)
                        StartConfigurationWatcher();
                };
            }
        }

        private void StartConfigurationWatcher()
        {
            if (Assembly.GetExecutingAssembly().IsFullyTrusted)
            {
                _watcher = new FileSystemWatcher(Helper.DirectoryPath, "config.json")
                {
                    NotifyFilter = NotifyFilters.LastWrite,
                    IncludeSubdirectories = false,
                    EnableRaisingEvents = true
                };

                _watcher.Changed += OnConfigChanged;
                Monitor.Log("Configuration watcher is active", LogLevel.Info);
            }
            else
            {
                Monitor.Log("The assembly is not fully trusted so it cannot watch for filesystem changes");
            }
        }

        private bool InstallModTabSettingsIntegration(string modID)
        {
            var modSettingsTab = Helper.ModRegistry.Get(modID);
            var tabSettings = modSettingsTab?.Manifest.Name ?? modID;

            try
            {
                if (modSettingsTab != null)
                {
                    Monitor.Log($"{tabSettings} is present", LogLevel.Info);

                    var modSettingsTabApi = Helper.ModRegistry.GetApi<IModTabSettingsApi>(modID);
                    var modTab = modSettingsTabApi?.GetMod(ModManifest.UniqueID);

                    if (modTab != null)
                    {
                        Monitor.Log($"{tabSettings} integration success", LogLevel.Info);

                        modTab.OptionsChanged += ModTabSettingsOptionsChanged;

                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Monitor.Log($"{tabSettings} integration failed : could not connect to the API ({e.GetType()})", LogLevel.Warn);
            }

            return false;
        }

        private async void OnConfigChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                const int debounceDelay = 250;

                // Filter several calls in short period of time
                Interlocked.Increment(ref _fileSystemWatcherEvents);

                await Task.Delay(debounceDelay);

                if (Interlocked.Decrement(ref _fileSystemWatcherEvents) == 0)
                {
                    if (RefreshConfig(out var error))
                    {
                        Game1.addHUDMessage(new HUDMessage($"{ModManifest.Name}: Settings updated", 2));
                    }
                    else if (!string.IsNullOrWhiteSpace(error))
                    {
                        Monitor.Log(error, LogLevel.Error);
                    }
                }
                else
                {
                    Monitor.Log($"Skipped a config change event because the last one occurred less than {debounceDelay}ms ago");
                }
            }
        }

        private bool RefreshConfig(out string error)
        {
            error = "";

            try
            {
                // disable the config watcher since ReadConfig writes back the file after reading it
                _watcher.EnableRaisingEvents = false;
                Config = Helper.ReadConfig<ModConfig>();
            }
            catch (Exception e)
            {
                error = e.Message;

                return false;
            }
            finally
            {
                // restore the config watcher
                _watcher.EnableRaisingEvents = true;
            }
            
            if (Config != null)
            {
                Monitor.Log(Config.ToString(), LogLevel.Info);

                // check if the teleportation is available under the new configuration
                if (CanTeleport())
                {
                    // reset the cooldown if necessary
                    ResetCooldown();
                }

                return true;
            }

            return false;
        }

        private void SpeedMod_OnTeleportSuccess(object sender, TeleportSuccessArgs e)
        {
            FinishTeleport(e.Farmer, false);
        }

        private void ModTabSettingsOptionsChanged(object sender, OptionsChangedEventArgs e)
        {
            e.Reloaded = RefreshConfig(out var error);

            if (!e.Reloaded)
                Monitor.Log(error, LogLevel.Error);
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            ResetCooldown();
        }

        private void SpeedMod_GameLoaded(object sender, EventArgs e)
        {
            ResetCooldown();
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button == Config.TeleportHomeKey)
            {
                var player = Game1.player;

                if (_castingCooldown > 0)
                {
                    if (Config.CanPlayerInterrupt)
                        FinishTeleport(player, true);
                }
                else if (Context.CanPlayerMove)
                    InitiateTeleport(player);
            }
        }


        private void GameLoop_UpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (_castingCooldown > 0)
            {
                RefreshUI();
            }
        }

        private void GameLoop_UpdateTicked(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            var player = Game1.player;

            if (Context.CanPlayerMove && !player.hasBuff(Buff.slimed) && !player.hasBuff(Buff.tipsy))
                player.addedSpeed = Math.Max(Config.SpeedModifier, player.addedSpeed);

            if (_castingCooldown > 0)
            {
                double healthTotal = Config.DamageThresholdBasedOnTotalHealth ? player.maxHealth : _healthAtStart;
                var forceStop = (player.health <= 0 || Game1.timeOfDay >= 2600);
                double healthDiff = _healthAtStart - player.health;

                _castingCooldown -= Game1.currentGameTime.ElapsedGameTime.Milliseconds;

                UpdateHudMessage(_castingCooldown);
                UpdateStamina(player);

                if (forceStop || (Config.CanDamageInterrupt && healthDiff > 0 && HasDamageInterrupted(healthDiff, healthTotal)))
                {
                    FinishTeleport(player, true);

                    if (!forceStop)
                    {
                        if (Config.EnableTeleportationEffects)
                            Game1.screenGlowOnce(Color.DarkRed, false);
                        if (Config.EnableTeleportationSounds)
                            Game1.playSound("breakingGlass");
                    }
                }
                else if (Config.EnableTeleportationEffects)
                    player.startGlowing(Color.BlueViolet, false, 0.05f);
            }
        }

        private static bool HasDamageInterrupted(double healthDiff, double healthTotal)
        {
            var threshold = Config.DamageThreshold / 100.0;

            if (threshold < 0.0)
                threshold = 0.0;
            else if (threshold > 0.9)
                threshold = 0.9;

            return healthDiff / healthTotal > threshold;
        }

        private void UpdateHudMessage(double cooldown)
        {
            var previous = _cooldownSeconds;

            _cooldownSeconds = (int)(cooldown / 1000);

            if (_cooldownSeconds < previous)
            {
                RemoveHudMessage();

                if (_cooldownSeconds > 0)
                    DisplayHudMessage("CountDownMessage", 1000, new {timeLeft = _cooldownSeconds});
                else
                    DisplayHudMessage("TeleportingNow", 1000);
            }
        }

        private void UpdateStamina(Farmer who)
        {
            if (_castingCooldown > 0)
            {
                var remainingStaminaToConsume = Config.StaminaCost - _consumedStamina;
                var cooldown = ModConfig.Cooldown(Config.CastCooldown * 1000, 500);

                if (remainingStaminaToConsume > 0)
                {
                    _castingCooldownDelta += Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;

                    if (_castingCooldownDelta >= cooldown / (Config.StaminaCost + 1.0))
                    {
                        var staminaDelta = Math.Round(Config.StaminaCost * _castingCooldownDelta / cooldown);
                        var staminaToConsume = (int)Math.Min(staminaDelta , remainingStaminaToConsume);

                        _consumedStamina += staminaToConsume;
                        who.stamina -= staminaToConsume;
                        _castingCooldownDelta = 0;
                    }
                }
            }
        }

        private void RefreshUI()
        {
            if (_buff != null && Game1.buffsDisplay.otherBuffs.Contains(_buff))
            {
                Game1.buffsDisplay.update(Game1.currentGameTime);
            }
        }

        private static void FreezePlayer(Farmer who, int? cooldown = null)
        {
            // block input if necessary
            if (cooldown.HasValue)
            {
                who.completelyStopAnimatingOrDoingAction();
                who.freezePause = cooldown.Value;
                who.forceTimePass = true;
                who.hasMoved = false;
                who.faceDirection(2);
            }
            // update the effects if necessary
            if (Config.EnableTeleportationEffects)
            {
                who.jitterStrength = 1f;
            }
            else
            {
                who.FarmerSprite.StopAnimation();
            }
        }

        private void InitiateTeleport(Farmer who)
        {
            if (!Context.IsWorldReady || who.currentLocation.IsFarm || (!Config.EnabledInMultiplayer && Game1.IsMultiplayer) || !Context.CanPlayerMove)
                return;

            if (!CanTeleport())
            {
                Game1.drawObjectDialogue(Helper.Translation.Get("TeleportOnCooldown", new { timeLeft = $"{Config.RecastCooldown - _cooldown:mm\\:ss}" }));
                who.doEmote(15);
            }
            else if (who.stamina < Config.StaminaCost + 1)
            {
                Game1.drawObjectDialogue(Helper.Translation.Get("NotEnoughStamina"));
                who.doEmote(28);
            }
            else if (Game1.dialogueUp == false)
            {
                var location = who.currentLocation;
                var responses = location.createYesNoResponses();
                var teleportQuestion = Helper.Translation.Get("TeleportQuestion");

                location.createQuestionDialogue(teleportQuestion, responses, AnswerTeleportHome);
                location.lastQuestionKey = "TeleportQuestion";
            }
        }

        public void AnswerTeleportHome(Farmer who, string whichAnswer)
        {
            switch (whichAnswer)
            {
                case "Yes":
                    TeleportHome(who);
                break;
            }
        }

        private bool CanTeleport()
        {
            if (Game1.waitingToPassOut())
                return false;

            if (_lastUse.HasValue)
            {
                _cooldown = DateTime.Now.Subtract(_lastUse.Value);

                return _cooldown.Value >= Config.RecastCooldown;
            }
            else
            {
                _cooldown = null;
            }

            return true;
        }

        private void TeleportHome(Farmer who)
        {
            if (!Context.IsWorldReady || !CanTeleport())
                return;
            
            // start the casting cooldown
            _castingCooldown = ModConfig.Cooldown(Config.CastCooldown * 1000, 500);
            _healthAtStart = who.health;
            // freeze the player
            FreezePlayer(who, _castingCooldown);
            // play the teleportation sound if necessary
            if (Config.EnableTeleportationSounds)
                Game1.playSound("wand");
            // update the HUD
            DisplayBuff(Buff.frozen, "TeleportationTrance", "TeleportBuffSource");
            UpdateHudMessage(_castingCooldown);
            // display the effects if necessary
            StartTeleportAnimation(who, _castingCooldown);
        }

        private void DisplayBuff(int icon, string descriptionKey, string sourceKey)
        {
            if (_buff == null || !Game1.buffsDisplay.otherBuffs.Contains(_buff))
            {
                var buffName = Helper.Translation.Get(descriptionKey);
                var buffSource = Helper.Translation.Get(sourceKey);

                _buff = new Buff(buffName, _castingCooldown, sourceKey, icon) { displaySource = buffSource };
                Game1.buffsDisplay.addOtherBuff(_buff);
            }
        }

        private void RemoveBuff()
        {
            if (_buff != null && Game1.buffsDisplay.otherBuffs.Contains(_buff))
            {
                Game1.buffsDisplay.otherBuffs.Remove(_buff);
                _buff = null;
            }
        }

        private void DisplayHudMessage(string translationKey, double cooldown, object tokens = null, int? icon = null)
        {
            if (_hudMessage == null || !Game1.hudMessages.Contains(_hudMessage))
            {
                var countdownMessage = Helper.Translation.Get(translationKey, tokens);
                if (icon.HasValue && icon.Value > 0)
                    _hudMessage = new HUDMessage(countdownMessage, icon.Value);
                else
                    _hudMessage = new HUDMessage(countdownMessage, Color.OrangeRed, (float)cooldown) { noIcon = true };
                    
                Game1.addHUDMessage(_hudMessage);
            }
        }

        private void RemoveHudMessage()
        {
            if (_hudMessage != null && Game1.hudMessages.Contains(_hudMessage))
            {
                Game1.hudMessages.Remove(_hudMessage);
                _hudMessage = null;
            }
        }

        private static void StopTeleportAnimation(Farmer who)
        {
            if (!Config.EnableTeleportationEffects)
                return;

            DelayedAction.functionAfterDelay(() => Game1.currentLocation.removeTemporarySpritesWithIDLocal(_sparklesID), 1000);

            who.FarmerSprite.StopAnimation();
            who.stopJittering();
            who.stopGlowing();

            Game1.fadeToBlackAlpha = 0.99f;
            Game1.staminaShakeTimer = 0;
            Game1.displayFarmer = true;
            Game1.screenGlow = false;
        }
        
        private static void StartTeleportAnimation(Farmer who, int castingCooldown)
        {
            who.FarmerSprite.animateOnce(new[]
            {
                new FarmerSprite.AnimationFrame(57, castingCooldown),
                new FarmerSprite.AnimationFrame(who.FarmerSprite.CurrentFrame, 0, false, false, PlayWarpAnimation, true)
            });

            if (Config.EnableTeleportationEffects)
            {
                Game1.staminaShakeTimer = castingCooldown;
                Game1.screenGlowOnce(Color.LightBlue, false);
                SpellEffects.AddSprinklesToLocation(who.currentLocation, _sparklesID, who.getTileX(), who.getTileY(), 16, 16, castingCooldown, 20, Color.White, null, true);
            }
        }

        private static void PlayWarpAnimation(Farmer who)
        {
            if (Config.EnableTeleportationEffects)
            { 
                for (var i = 0; i < 12; i++)
                {
                    Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(354, Game1.random.Next(25, 75), 6, 1, new Vector2(Game1.random.Next((int)who.Position.X - 256, (int)who.Position.X + 192),
                                                                                                Game1.random.Next((int)who.Position.Y - 256, (int)who.Position.Y + 192)), false, Game1.random.NextDouble() < 0.5));
                }
                Game1.displayFarmer = false;
                Game1.flashAlpha = 1f;
            }
            // teleport with style
            if (Config.EnableTeleportationSounds)
                Game1.playSound("thunder");
            // disable damage during the screen fade
            who.temporarilyInvincible = true;
            who.temporaryInvincibilityTimer = -2000;
            who.freezePause = 1000;
            // teleport the player
            DelayedAction.fadeAfterDelay(() =>
            {
                // warp the player
                if (Config.TeleportToBed)
                    Game1.warpHome();
                else
                    Game1.warpFarmer("Farm", 64, 15, 2);
                // trigger the teleportation event
                OnTeleportSuccess?.Invoke(null, new TeleportSuccessArgs(who));
            }, 1000);

            if (Config.EnableTeleportationEffects)
            {
                var rectangle = new Rectangle(who.GetBoundingBox().X, who.GetBoundingBox().Y, 64, 64);
                var j = 0;

                rectangle.Inflate(192, 192);

                for (var x = who.getTileX() + 8; x >= who.getTileX() - 8; x--, j++)
                {
                    Game1.currentLocation.temporarySprites.AddRange(new[]
                    {
                        new TemporaryAnimatedSprite(6, new Vector2(x, who.getTileY()) * 64f, Color.White, 8, false, 50f)
                        {
                            delayBeforeAnimationStart = j * 25,
                            motion = new Vector2(-0.25f, 0f),
                            layerDepth = 1f
                        }
                    });
                }
            }
        }
        private void FinishTeleport(Farmer who, bool interrupted)
        {
            if (interrupted)
            {
                // display the message
                RemoveHudMessage();
                DisplayHudMessage("TeleportInterrupted", 1000);
            }
            else
            {
                var staminaLeftToConsume = Config.StaminaCost - _consumedStamina;

                // consume the stamina
                if (staminaLeftToConsume > 0)
                    who.stamina -= staminaLeftToConsume;

                StartCooldown(Config.RecastCooldown);
            }
            // reset the casting cooldown
            _consumedStamina = 0;
            _castingCooldown = 0;
            // free the player
            who.forceCanMove();
            // disable the invincibility
            who.temporaryInvincibilityTimer = 0;
            who.temporarilyInvincible = false;
            // prevent the "go to sleep" question from popping
            who.freezePause = 500;

            // disabled the effects if necessary
            if (Config.EnableTeleportationEffects)
                StopTeleportAnimation(who);
            // remove the buff
            RemoveBuff();
        }

        private void StartCooldown(TimeSpan cooldown)
        {
            ResetCooldown();
            // start the timer
            _lastUse = DateTime.Now;
            _coolDownTimer = new Timer
            {
                Interval = ModConfig.Cooldown(cooldown.TotalMilliseconds, Config.CastCooldown * 1000),
                AutoReset = false,
                Enabled = true
            };

            _coolDownTimer.Elapsed += (s, e) => ResetCooldown();
        }

        private void ResetCooldown()
        {
            _coolDownTimer?.Stop();
            _coolDownTimer?.Dispose();
            _coolDownTimer = null;
            _consumedStamina = 0;
            _castingCooldown = 0;
            _cooldown = null;
            _lastUse = null;
        }
    }
}
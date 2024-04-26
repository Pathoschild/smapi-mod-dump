/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RealClock.Models;
using RealClock.Network;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Text;

namespace RealClock
{
    internal sealed class RealClock : Mod
    {
        private ModConfig Config;
        public int LastTimeInterval { get; set; }
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            Helper.Events.Display.RenderedHud += OnRenderedHud;
        }
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            GenericModConfigMenuIntegration.AddConfig(
                Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu"),
                ModManifest,
                Helper,
                Config
            );
        }
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Config.TimeSpeedControl)
            {
                return;
            }

            int delta;
            if (Game1.gameTimeInterval < LastTimeInterval)
            {
                delta = Game1.gameTimeInterval;
                LastTimeInterval = 0;
            }
            else
            {
                delta = Game1.gameTimeInterval - LastTimeInterval;
            }

            Game1.gameTimeInterval = LastTimeInterval + (int)(delta * .7f / Config.SecondsToMinutes);

            LastTimeInterval = Game1.gameTimeInterval;
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if (!Game1.displayHUD || Game1.eventUp || Game1.gameMode != 3 || Game1.freezeControls || Game1.panMode || Game1.HostPaused || Game1.game1.takingMapScreenshot)
            {
                return;
            }

            SpriteBatch b = e.SpriteBatch;

            int exactTime = Game1.timeOfDay + (Game1.gameTimeInterval / (700 + Game1.currentLocation.ExtraMillisecondsPerInGameMinute));
            string _timeText = GetTimeFormmated(exactTime);

            DayTimeMoneyBox dayTimeMoneyBox = Game1.dayTimeMoneyBox;

            Rectangle sourceRect = new(333, 431, 71, 43);
            Vector2 offset = new(108f, 112f);
            Rectangle bounds = new(360, 459, 40, 9);
            b.Draw(Game1.mouseCursors, dayTimeMoneyBox.position + offset, (Rectangle?)bounds, Color.White, 0f, Vector2.Zero, 4f, 0, 0.9f);

            int timeShakeTimer = dayTimeMoneyBox.timeShakeTimer;
            SpriteFont font = Game1.dialogueFont;

            Vector2 txtSize = font.MeasureString(_timeText);
            Vector2 timePosition = new((sourceRect.X * 0.55f) - (txtSize.X / 2f) + (timeShakeTimer > 0 ? Game1.random.Next(-2, 3) : 0), (sourceRect.Y * (LocalizedContentManager.CurrentLanguageLatin ? 0.31f : 0.31f)) - (txtSize.Y / 2f) + (timeShakeTimer > 0 ? Game1.random.Next(-2, 3) : 0));
            bool nofade = Game1.shouldTimePass() || Game1.fadeToBlack || Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2000.0 > 1000.0;
            Utility.drawTextWithShadow(b, _timeText, font, dayTimeMoneyBox.position + timePosition, exactTime >= 2400 ? Color.Red : Game1.textColor * (nofade ? 1f : 0.5f));
        }

        private string GetTimeFormmated(int exactTime)
        {
            string _amString = Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10370");
            string _pmString = Game1.content.LoadString("Strings\\StringsFromCSFiles:DayTimeMoneyBox.cs.10371");

            StringBuilder _timeText = new();
            if (Config.Show24Hours)
            {
                if (exactTime / 100 % 24 <= 9)
                {
                    _ = _timeText.Append('0');
                }
                _ = _timeText.AppendEx(exactTime / 100 % 24);
            }
            else
            {
                _ = exactTime / 100 % 12 == 0 ? _timeText.Append("12") : _timeText.AppendEx(exactTime / 100 % 12);
            }

            _ = _timeText.Append(':');

            if (exactTime / 10 % 10 == 0)
            {
                _ = _timeText.Append('0');
            }

            _ = _timeText.AppendEx(exactTime % 100);

            if (!Config.Show24Hours)
            {
                switch (LocalizedContentManager.CurrentLanguageCode)
                {
                    case LocalizedContentManager.LanguageCode.en:
                    case LocalizedContentManager.LanguageCode.it:
                        _ = _timeText.Append(' ');
                        _ = exactTime is < 1200 or >= 2400 ? _timeText.Append(_amString) : _timeText.Append(_pmString);
                        break;
                    case LocalizedContentManager.LanguageCode.ko:
                        _ = exactTime is < 1200 or >= 2400 ? _timeText.Append(_amString) : _timeText.Append(_pmString);
                        break;
                    case LocalizedContentManager.LanguageCode.ja:
                        StringBuilder _temp = new();
                        _ = _temp.Append(_timeText);
                        _ = _timeText.Clear();
                        if (exactTime is < 1200 or >= 2400)
                        {
                            _ = _timeText.Append(_amString);
                            _ = _timeText.Append(' ');
                            _ = _timeText.AppendEx(_temp);
                        }
                        else
                        {
                            _ = _timeText.Append(_pmString);
                            _ = _timeText.Append(' ');
                            _ = _timeText.AppendEx(_temp);
                        }
                        break;
                    case LocalizedContentManager.LanguageCode.mod:
                        _ = _timeText.Clear();
                        _ = _timeText.Append(LocalizedContentManager.FormatTimeString(exactTime, LocalizedContentManager.CurrentModLanguage.ClockTimeFormat));
                        break;
                    default:
                        _ = exactTime is < 1200 or >= 2400 ? _timeText.Append("am") : _timeText.Append("pm");
                        break;
                }
            }

            return _timeText.ToString();
        }
    }
}

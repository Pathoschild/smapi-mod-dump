using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using System;
using TwilightShards.Stardew.Common;

namespace ClimatesOfFerngillRebuild
{
    /// <summary>This tracks white out details</summary>
    internal class FerngillWhiteOut : ISDVWeather
    {
        /// <summary> This is for the second snow overlay.</summary>
        private Vector2 snowPos;
        private Vector2 snowPos2;
        public event EventHandler<WeatherNotificationArgs> OnUpdateStatus;
        private bool IsWhiteout { get; set; }
        public bool IsWeatherVisible => IsWhiteout;
        private SDVTime ExpirTime;
        private SDVTime BeginTime;

        public string WeatherType => "WhiteOut";
        public void SetWeatherExpirationTime(SDVTime t)
        {
            ExpirTime = new SDVTime(t);
        }
        public void SetWeatherBeginTime(SDVTime t)
        {
            BeginTime = new SDVTime(t);
        }

        public SDVTime WeatherExpirationTime => (ExpirTime ?? new SDVTime(0600));
        public SDVTime WeatherBeginTime => (BeginTime ?? new SDVTime(0600));
        public bool WeatherInProgress => (SDVTime.CurrentTime >= BeginTime && SDVTime.CurrentTime <= ExpirTime && BeginTime != ExpirTime);

        public FerngillWhiteOut()
        {
            ExpirTime = new SDVTime(2600);
            BeginTime = new SDVTime(0600);
        }

        public void ForceWeatherStart()
        {
            IsWhiteout = true;
        }

        public void ForceWeatherEnd()
        {
            IsWhiteout = false;
            ExpirTime = new SDVTime(SDVTime.CurrentIntTime - 10);
            UpdateStatus(WeatherType, false);
        }

        public void UpdateStatus(string weather, bool status)
        {
            if (OnUpdateStatus == null) return;

            WeatherNotificationArgs args = new WeatherNotificationArgs(weather, status);
            OnUpdateStatus(this, args);
        }

        public void OnNewDay()
        {
            IsWhiteout = false;
            BeginTime = new SDVTime(0600);
            ExpirTime = new SDVTime(0600);
        }

        public void Reset()
        {
            IsWhiteout = false;
            BeginTime = new SDVTime(0600);
            ExpirTime = new SDVTime(0600);
        }

        public void SetWeatherTime(SDVTime begin, SDVTime end)
        {
            BeginTime = new SDVTime(begin);
            ExpirTime = new SDVTime(end);
        }

        public void CreateWeather()
        {
            BeginTime = new SDVTime(0600);
            ExpirTime = new SDVTime(2600);
            if (ClimatesOfFerngill.Dice.NextDouble() >= .5 && ClimatesOfFerngill.Dice.NextDouble() < .8)
            {
                ExpirTime = new SDVTime(Game1.getModeratelyDarkTime());
            }
            if (ClimatesOfFerngill.Dice.NextDouble() >= .8 && ClimatesOfFerngill.Dice.NextDouble() < .95)
            {
                ExpirTime = new SDVTime((BeginTime.ReturnIntTime() + 1000));
            }
            if (ClimatesOfFerngill.Dice.NextDouble() >= .95)
            {
                ExpirTime = new SDVTime((BeginTime.ReturnIntTime() + 500));
            }

            if (SDVTime.CurrentTime >= BeginTime)
            {
                IsWhiteout = true;
                UpdateStatus(WeatherType, true);
            }
        }

        public void EndWeather()
        {
            if (IsWeatherVisible)
            {
                IsWhiteout = false;
                ExpirTime = new SDVTime(SDVTime.CurrentIntTime - 10);
                UpdateStatus(WeatherType, false);
            }
        }

        public string DebugWeatherOutput()
        {
            string s = "";
            s += $"Weather (WO) {WeatherType} is {IsWeatherVisible}, Progress: {WeatherInProgress}, Begin Time {BeginTime} to End Time {ExpirTime}.";

            return s;
        }

        public void SecondUpdate()
        {
        }

        public void UpdateWeather()
        {
            if (!IsWeatherVisible)
            {
                return;
            }

            if (SDVTime.CurrentTime >= WeatherExpirationTime)
            {
                IsWhiteout = false;
                UpdateStatus(WeatherType, false);
            }
        }

        public void MoveWeather()
        {

        }

        public override string ToString()
        {
            return $"White Out Weather from {BeginTime} to {ExpirTime}. Visible: {IsWeatherVisible}. ";
        }

        public void DrawWeather()
        {
            if (IsWeatherVisible && !(Game1.currentLocation is Desert))
            {
                snowPos = Game1.updateFloatingObjectPositionForMovement(snowPos, new Vector2(Game1.viewport.X, Game1.viewport.Y),
                    Game1.previousViewportPosition, -1f);
                snowPos.X %= (16 * Game1.pixelZoom);
                Vector2 position = new Vector2();
                float num1 = -16 * Game1.pixelZoom + snowPos.X % (16 * Game1.pixelZoom);
                while ((double)num1 < Game1.viewport.Width)
                {
                    float num2 = -12 * Game1.pixelZoom + snowPos.Y % (12 * Game1.pixelZoom);
                    while (num2 < (double)Game1.viewport.Height)
                    {
                        position.X = (int)num1;
                        position.Y = (int)num2;
                        Game1.spriteBatch.Draw(Game1.mouseCursors, position, new Microsoft.Xna.Framework.Rectangle?
                            (new Microsoft.Xna.Framework.Rectangle
                                (368 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + 225) % 1200.0) / 75 * 16, 192, 16, 16)),
                            Color.White * Game1.options.snowTransparency, 0.0f, Vector2.Zero,
                            Game1.pixelZoom + 1f / 1000f, SpriteEffects.None, 1f);
                        num2 += 16 * Game1.pixelZoom;
                    }
                    num1 += 16 * Game1.pixelZoom;
                }
            }

            if (IsWeatherVisible && !(Game1.currentLocation is Desert))
            {
                snowPos2 = Game1.updateFloatingObjectPositionForMovement(snowPos2, new Vector2(Game1.viewport.X, Game1.viewport.Y),
                            Game1.previousViewportPosition, -1f);
                snowPos2.X %= (12 * Game1.pixelZoom);
                Vector2 position = new Vector2();
                float num1 = -12 * Game1.pixelZoom + snowPos2.X % (12 * Game1.pixelZoom);
                while ((double)num1 < Game1.viewport.Width)
                {
                    float num2 = -8 * Game1.pixelZoom + snowPos2.Y % (8 * Game1.pixelZoom);
                    while (num2 < (double)Game1.viewport.Height)
                    {
                        position.X = (int)num1;
                        position.Y = (int)num2;
                        Game1.spriteBatch.Draw(Game1.mouseCursors, position, new Microsoft.Xna.Framework.Rectangle?
                            (new Microsoft.Xna.Framework.Rectangle
                            (368 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + 125) % 1200.0) / 75 * 16, 192, 16, 16)),
                            Color.White * Game1.options.snowTransparency, 0.0f, Vector2.Zero,
                            Game1.pixelZoom + 1f / 1000f, SpriteEffects.None, 1f);
                        num2 += 16 * Game1.pixelZoom;
                    }
                    num1 += 16 * Game1.pixelZoom;
                }
            }
        }
    }
}
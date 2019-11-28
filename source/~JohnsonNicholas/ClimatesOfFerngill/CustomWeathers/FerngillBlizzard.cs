using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using System;
using TwilightShards.Common;
using TwilightShards.Stardew.Common;

namespace ClimatesOfFerngillRebuild
{
    /// <summary>This tracks blizzard details</summary>
    internal class FerngillBlizzard : ISDVWeather
    {
        /// <summary> This is for the second snow overlay.</summary>
        private Vector2 snowPos;
        public event EventHandler<WeatherNotificationArgs> OnUpdateStatus;
        private bool IsBlizzard { get; set; }
        public bool IsWeatherVisible => IsBlizzard;
        private SDVTime ExpirTime;
        public bool IsBloodMoon;
        private SDVTime BeginTime;

        public string WeatherType => "Blizzard";
        public void SetWeatherExpirationTime(SDVTime t){
            ExpirTime = new SDVTime(t);
        }
        public void SetWeatherBeginTime(SDVTime t){
            BeginTime = new SDVTime(t);
        }

        public SDVTime WeatherExpirationTime => (ExpirTime ?? new SDVTime(0600));
        public SDVTime WeatherBeginTime => (BeginTime ?? new SDVTime(0600));
        public bool WeatherInProgress => (SDVTime.CurrentTime >= BeginTime && SDVTime.CurrentTime <= ExpirTime && BeginTime != ExpirTime);

        public FerngillBlizzard()
        {
            ExpirTime = new SDVTime(0600);
            BeginTime = new SDVTime(0600);
        }

        /* work on creating various buff code here */
        public void UpdateStatus(string weather, bool status)
        { 
            if (OnUpdateStatus == null) return;

            WeatherNotificationArgs args = new WeatherNotificationArgs(weather, status);
            OnUpdateStatus(this, args);
        }

        public void ForceWeatherStart()
        {
            IsBlizzard = true;
            ExpirTime = new SDVTime(2600);
        }

        public void ForceWeatherEnd()
        {
            IsBlizzard = false;
            ExpirTime = new SDVTime(0600);
        }

        public void OnNewDay()
        {
            IsBlizzard = false;
        }

        public void Reset()
        {
            IsBlizzard = false;
            ExpirTime = new SDVTime(0600);
            BeginTime = new SDVTime(0600);
        }

        public void SetWeatherTime(SDVTime begin, SDVTime end)
        {
            BeginTime = new SDVTime(begin);
            ExpirTime = new SDVTime(end);
        }

        public void SyncWeather()
        {

        }

        public string DebugWeatherOutput()
        {
            string s = "";
            s += $"Weather {WeatherType} is {IsWeatherVisible}, Progress: {WeatherInProgress}, Begin Time {BeginTime} to End Time {ExpirTime}";
            return s;
        }

        public void SecondUpdate()
        {
        }

        public void CreateWeather()
        {
            //Blizzards opt to mostly being all day. 
            BeginTime = new SDVTime(0600);
            ExpirTime = new SDVTime(2800);
            if (ClimatesOfFerngill.Dice.NextDouble() >= .5 && ClimatesOfFerngill.Dice.NextDouble() < .8)
            {
                Console.WriteLine($"Truly Dark: {Game1.getTrulyDarkTime()} Moderately: {Game1.getModeratelyDarkTime()} Starting: {Game1.getStartingToGetDarkTime()}");
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
                IsBlizzard = true;
                UpdateStatus(WeatherType, true);
            }
        }

        public void EndWeather()
        {
            if (IsWeatherVisible)
            {
                IsBlizzard = false;
                ExpirTime = new SDVTime(SDVTime.CurrentIntTime - 10);
                UpdateStatus(WeatherType, false);
            }
        }

        public void UpdateWeather()
        {
            if (!IsWeatherVisible)
            {
                return;
            }

            if (SDVTime.CurrentTime >= WeatherExpirationTime)
            {
                IsBlizzard = false;
                UpdateStatus(WeatherType, false);
            }
        }

        public void MoveWeather()
        {

        }

        public override string ToString()
        {
            return $"Blizzard Weather from {BeginTime} to {ExpirTime}. Visible: {IsWeatherVisible}. ";
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
                Color snowColor = IsBloodMoon ? Color.Red * Game1.options.snowTransparency : Color.White * Game1.options.snowTransparency;
                while ((double)num1 < Game1.viewport.Width)
                {
                    float num2 = -16 * Game1.pixelZoom + snowPos.Y % (16 * Game1.pixelZoom);
                    while (num2 < (double)Game1.viewport.Height)
                    {
                        position.X = (int)num1;
                        position.Y = (int)num2;
                        Game1.spriteBatch.Draw(Game1.mouseCursors, position, new Microsoft.Xna.Framework.Rectangle?
                            (new Microsoft.Xna.Framework.Rectangle
                                (368 + (int)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds + 150) % 1200.0) / 75 * 16, 192, 16, 16)),
                            snowColor, 0.0f, Vector2.Zero,
                            Game1.pixelZoom + 1f / 1000f, SpriteEffects.None, 1f);
                        num2 += 16 * Game1.pixelZoom;
                    }
                    num1 += 16 * Game1.pixelZoom;
                }
            }
        }
    }
}
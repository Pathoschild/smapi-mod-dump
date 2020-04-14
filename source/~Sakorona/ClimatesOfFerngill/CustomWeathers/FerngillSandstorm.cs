using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Diagnostics;
using TwilightShards.Stardew.Common;
using static ClimatesOfFerngillRebuild.Sprites;

namespace ClimatesOfFerngillRebuild.Weathers
{
    class FerngillSandstorm : ISDVWeather
    {
        public event EventHandler<WeatherNotificationArgs> OnUpdateStatus;

        public static Rectangle SpriteSource = new Rectangle(0, 0, 64, 64);
        private Color SandstormColor = Color.IndianRed * 1.25f;
        public bool BloodMoon { get; set; }
        private float SandstormAlpha { get; set; }
        private Vector2 SandstormPosition { get; set; }
        private SDVTime ExpirTime { get; set; }
        private SDVTime BeginTime { get; set; }
        public bool IsWeatherVisible => SandstormAlpha > 0f;
        public bool WeatherInProgress => (SDVTime.CurrentTime >= BeginTime && SDVTime.CurrentTime <= ExpirTime && BeginTime != ExpirTime);
        public string WeatherType => "Sandstorm";
        private bool FadeOutSandstorm { get; set; }
        private bool FadeInSandstorm { get; set; }
        private Stopwatch SandstormElapsed { get; set; }
        public SDVTime WeatherExpirationTime => (ExpirTime ?? new SDVTime(0600));
        public SDVTime WeatherBeginTime => (BeginTime ?? new SDVTime(0600));

        public void ForceWeatherStart()
        {
            SandstormAlpha = 1f;
            BeginTime = new SDVTime(0600);
            ExpirTime = new SDVTime(0600);

            if (ClimatesOfFerngill.WeatherOpt.ShowLighterFog)
                SandstormAlpha = .3f;

            UpdateStatus(WeatherType, true);
        }

        public void ForceWeatherEnd()
        {
            ExpirTime = new SDVTime(SDVTime.CurrentTime - 10);
            FadeOutSandstorm = true;
            SandstormElapsed.Start();
            UpdateStatus(WeatherType, false);
        }

        public void SetWeatherExpirationTime(SDVTime t)
        {
            ExpirTime = new SDVTime(t);
        }
        public void SetWeatherBeginTime(SDVTime t)
        {
            BeginTime = new SDVTime(t);
        }

        /// <summary> Default constructor. </summary>
        internal FerngillSandstorm()
        {
            BeginTime = new SDVTime(0600);
            ExpirTime = new SDVTime(0600);
            BloodMoon = false;
            SandstormElapsed = new Stopwatch();
        }

        public void OnNewDay()
        {
            Reset();
        }

        public void Reset()
        {
            BeginTime = new SDVTime(0600);
            ExpirTime = new SDVTime(0600);
            BloodMoon = false;
            SandstormAlpha = 0f;
            FadeOutSandstorm = false;
            FadeInSandstorm = false;
            SandstormElapsed.Reset();
        }

        public void SecondUpdate()
        {
        }

        public void CreateWeather()
        {
            SandstormAlpha = 1f;
            BeginTime = new SDVTime(0600);
            

            if (ClimatesOfFerngill.WeatherOpt.ShowLighterFog)
            {
                SandstormAlpha = .3f;
            }

            double WeatChance = ClimatesOfFerngill.Dice.NextDoublePositive();

            if (WeatChance > 0 && WeatChance < .25)
                this.ExpirTime = new SDVTime(1130);
            else if (WeatChance >= .25 && WeatChance < .32)
                this.ExpirTime = new SDVTime(1210);
            else if (WeatChance >= .32 && WeatChance < .41)
                this.ExpirTime = new SDVTime(1420);
            else if (WeatChance >= .41 && WeatChance < .55)
                this.ExpirTime = new SDVTime(1500);
            else if (WeatChance >= .55 && WeatChance < .7)
                this.ExpirTime = new SDVTime(1610);
            else if (WeatChance >= .7 && WeatChance < .8)
                this.ExpirTime = new SDVTime(1720);
            else if (WeatChance >= .8 && WeatChance < .9)
                this.ExpirTime = new SDVTime(1800);
            else if (WeatChance >= .9 && WeatChance < .95)
                this.ExpirTime = new SDVTime(1920);
            else if (WeatChance >= .95 && WeatChance < .98)
                this.ExpirTime = new SDVTime(2000);
            else if (WeatChance >= .98 && WeatChance < .99)
                this.ExpirTime = new SDVTime(2110);
            else if (WeatChance >= .99)
                this.ExpirTime = new SDVTime(2400);

            if (SDVTime.CurrentTime >= BeginTime)
                UpdateStatus(WeatherType, true);
        }

        public void EndWeather()
        {
            if (IsWeatherVisible)
            {
                ExpirTime = new SDVTime(SDVTime.CurrentTime - 10);
                FadeOutSandstorm = true;
                SandstormElapsed.Start();
                UpdateStatus(WeatherType, false);
            }
        }

        public string DebugWeatherOutput()
        {
            string s = "";
            s += $"Weather {WeatherType} is {IsWeatherVisible}, Progress: {WeatherInProgress}, Begin Time {BeginTime} to End Time {ExpirTime}. Alpha is {SandstormAlpha}.";
            s += $"{Environment.NewLine} Color is {SandstormColor}. Position is {SandstormPosition.ToString()}, with Fade Out Timer being {FadeOutSandstorm} and In {FadeInSandstorm}";
            s += $"{Environment.NewLine} Elapsed on timer: {SandstormElapsed.ElapsedMilliseconds} ";

            return s;
        }

        public void SetWeatherTime(SDVTime begin, SDVTime end)
        {
            BeginTime = new SDVTime(begin);
            ExpirTime = new SDVTime(end);
        }

        public override string ToString()
        {
            return $"Sandstorm Weather from {BeginTime} to {ExpirTime}. Visible: {IsWeatherVisible}.  Alpha: {SandstormAlpha}.";
        }

        public void UpdateWeather()
        {
            if (WeatherBeginTime is null || WeatherExpirationTime is null)
                return;

            if (SDVTime.CurrentTime >= WeatherExpirationTime && IsWeatherVisible)
            {
                //ClimatesOfFerngill.Logger.Log("Triggering the end of the sandstorm");
                FadeOutSandstorm = true;
                SandstormElapsed.Start();
                UpdateStatus(WeatherType, false);
            }

            if (WeatherInProgress && !IsWeatherVisible)
            {
                //ClimatesOfFerngill.Logger.Log("Triggering the beginning of the sandstorm");
                FadeInSandstorm = true;
                SandstormElapsed.Start();
                UpdateStatus(WeatherType, true);
            }
        }

        internal void SetEveningFog()
        {
            SDVTime STime, ETime;
            STime = new SDVTime(Game1.getStartingToGetDarkTime());
            STime.AddTime(ClimatesOfFerngill.Dice.Next(-25, 80));

            ETime = new SDVTime(STime);
            ETime.AddTime(ClimatesOfFerngill.Dice.Next(120, 310));

            STime.ClampToTenMinutes();
            ETime.ClampToTenMinutes();
            this.SetWeatherTime(STime, ETime);
        }

        public void DrawWeather()
        {
            if (ClimatesOfFerngill.WeatherOpt.SandstormsInDesertOnly && !(Game1.currentLocation is Desert))
                return;

            if (IsWeatherVisible)
            {
                Texture2D fogTexture = ClimatesOfFerngill.OurIcons.DarudeTexture;

                Vector2 position = new Vector2();
                    float num1 = -64 * Game1.pixelZoom + (int)(SandstormPosition.X % (double)(64 * Game1.pixelZoom));
                    while (num1 < (double)Game1.graphics.GraphicsDevice.Viewport.Width)
                    {
                        float num2 = -64 * Game1.pixelZoom + (int)(SandstormPosition.Y % (double)(64 * Game1.pixelZoom));
                        while ((double)num2 < Game1.graphics.GraphicsDevice.Viewport.Height)
                        {
                            position.X = (int)num1;
                            position.Y = (int)num2;

                            if (BloodMoon)
                            {
                                SandstormColor = Color.DarkRed;
                            }

                            Game1.spriteBatch.Draw(fogTexture, position, new Microsoft.Xna.Framework.Rectangle?
                                    (SpriteSource), SandstormAlpha > 0.0 ? SandstormColor * SandstormAlpha : Color.Black * 0.95f, 0.0f, Vector2.Zero, Game1.pixelZoom + 1f / 1000f, SpriteEffects.None, 1f);
                            num2 += 64 * Game1.pixelZoom;
                        }
                        num1 += 64 * Game1.pixelZoom;
                }
            }
        }

        public void UpdateStatus(string weather, bool status)
        {
            if (OnUpdateStatus == null) return;

            WeatherNotificationArgs args = new WeatherNotificationArgs(weather, status);
            OnUpdateStatus(this, args);
        }

        public string FogDescription(double fogRoll, double fogChance)
        {
            return $"With roll {fogRoll.ToString("N3")} against {fogChance}, there will be a sandstorm today from {WeatherBeginTime} to {WeatherExpirationTime}";
        }

        public void MoveWeather()
        {
            const float FadeTime = 3120f;
            if (FadeOutSandstorm)
            {
                if (ClimatesOfFerngill.WeatherOpt.ShowLighterFog)
                    SandstormAlpha = .3f - (SandstormElapsed.ElapsedMilliseconds / FadeTime);
                else
                    SandstormAlpha = 1f - (SandstormElapsed.ElapsedMilliseconds / FadeTime);

                if (SandstormAlpha <= 0)
                {
                    SandstormAlpha = 0;
                    FadeOutSandstorm = false;
                    SandstormElapsed.Stop();
                    SandstormElapsed.Reset();
                }
            }

            if (FadeInSandstorm)
            {
                //as above, but the reverse.
                SandstormAlpha = (SandstormElapsed.ElapsedMilliseconds / FadeTime);
                if (SandstormAlpha >= 1)
                {
                    SandstormAlpha = 1;
                    FadeInSandstorm = false;
                    SandstormElapsed.Stop();
                    SandstormElapsed.Reset();
                }
            }

            if (IsWeatherVisible)
            {
                this.SandstormPosition = Game1.updateFloatingObjectPositionForMovement(SandstormPosition,
                    new Vector2(Game1.viewport.X, Game1.viewport.Y), Game1.previousViewportPosition, -1f);
                SandstormPosition = new Vector2((SandstormPosition.X + 0.5f) % (64 * Game1.pixelZoom) + WeatherDebris.globalWind,
                    (SandstormPosition.Y + 0.5f) % (64 * Game1.pixelZoom));
            }
        }
    }
}

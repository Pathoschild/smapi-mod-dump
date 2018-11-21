using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Diagnostics;
using TwilightShards.Common;
using TwilightShards.Stardew.Common;
using static ClimatesOfFerngillRebuild.Sprites;

namespace ClimatesOfFerngillRebuild
{
    /// <summary> This tracks fog details </summary>
    internal class FerngillFog : ISDVWeather
    {
        public event EventHandler<WeatherNotificationArgs> OnUpdateStatus;

        public static Rectangle FogSource = new Rectangle(0, 0, 64, 64);
        private Color FogColor = Color.White * 1.25f;

        internal Icons Sheet;

        /// <summary> The Current Fog Type </summary>
        internal FogType CurrentFogType { get; set; }

        private bool VerboseDebug { get; set; }
        public bool BloodMoon { get; set; }
        private IMonitor Monitor { get; set; }

        /// <summary>  The alpha attribute of the fog. </summary>
        private float FogAlpha { get; set; }

        /// <summary> Fog Position. For drawing. </summary>
        private Vector2 FogPosition { get; set; }

        /// <summary> Sets the expiration time of the fog </summary>
        private SDVTime ExpirTime { get; set; }
        private SDVTime BeginTime { get; set; }

        public bool IsWeatherVisible => (CurrentFogType != FogType.None);

        public string WeatherType => "Fog";

        /// <summary> Returns the expiration time of fog. Note that this doesn't sanity check if the fog is even visible. </summary>
        public SDVTime WeatherExpirationTime => (ExpirTime ?? new SDVTime(0600));
        public SDVTime WeatherBeginTime => (BeginTime ?? new SDVTime(0600));
        public bool WeatherInProgress
        {
            get
            {
                if (BeginTime is null || ExpirTime is null)
                    return false;
                if (SDVTime.CurrentTime is null)
                    Console.WriteLine("CURRENT TIME IS NULL.");
                if (WeatherBeginTime is null)
                    Console.WriteLine("WBT is null");
                if (WeatherExpirationTime is null)
                    Console.WriteLine("WET is null");

                return (SDVTime.CurrentTime >= WeatherBeginTime && SDVTime.CurrentTime <= WeatherExpirationTime);
            }
        }

        /// <summary> Sets the fog expiration time. </summary>
        /// <param name="t">The time for the fog to expire</param>
        public void SetWeatherExpirationTime(SDVTime t) => ExpirTime = t;
        public void SetWeatherBeginTime(SDVTime t) => BeginTime = t;
        public SDVTimePeriods FogTimeSpan { get; set;}
        private MersenneTwister Dice { get; set; }
        private WeatherConfig ModConfig { get; set; }
        private bool FadeOutFog { get; set; }
        private bool FadeInFog { get; set; }
        private Stopwatch FogElapsed { get; set; }

        /// <summary> Default constructor. </summary>
        internal FerngillFog(Icons Sheet, bool Verbose, IMonitor Monitor, MersenneTwister Dice, WeatherConfig config, SDVTimePeriods FogPeriod)
        {
            this.Sheet = Sheet;
            CurrentFogType = FogType.None;
            ExpirTime = null;
            VerboseDebug = Verbose;
            this.Monitor = Monitor;
            this.BloodMoon = false;
            this.Dice = Dice;
            this.ModConfig = config;
            this.FogTimeSpan = FogPeriod;
            FogElapsed = new Stopwatch();
        }

        /// <summary> This function resets the fog for a new day. </summary>
        public void OnNewDay()
        {
            Reset();
        }

        /// <summary> This function resets the fog. </summary>
        public void Reset()
        {
            CurrentFogType = FogType.None;
            BeginTime = null;
            ExpirTime = null;
            BloodMoon = false;
            FogAlpha = 0f;
            FadeOutFog = false;
            FadeInFog = false;
            FogElapsed.Reset(); 
        }

        /// <summary>Returns a string describing the fog type. </summary>
        /// <param name="CurrentFogType">The type of the fog being looked at.</param>
        /// <returns>The fog type</returns>
        internal static string DescFogType(FogType CurrentFogType)
        {
            switch (CurrentFogType)
            {
                case FogType.None:
                    return "None";
                case FogType.Blinding:
                    return "Blinding";
                case FogType.Normal:
                    return "Normal";
                default:
                    return "ERROR";
            }
        }

        public void SecondUpdate()
        {
        }

        /// <summary>This function creates the fog </summary>
        public void CreateWeather()
        {
            this.FogAlpha = 1f;
            //First, let's determine the type.
            //... I am a dumb foxgirl. A really dumb one. 
            if (Dice.NextDoublePositive() <= .001)
                CurrentFogType = FogType.Blinding;
            else
                CurrentFogType = FogType.Normal;

            if (ModConfig.ShowLighterFog)
            {
                this.FogAlpha = .6f;
            }

            //now determine the fog expiration time
            double FogChance = Dice.NextDoublePositive();

            /*
             * So we should rarely have full day fog, and it should on average burn off around 9am. 
             * So, the strongest odds should be 820 to 930, with sharply falling off odds until 1200. And then
             * so, extremely rare odds for until 7pm and even rarer than midnight.
             */

            if (FogTimeSpan == SDVTimePeriods.Morning)
            {
                BeginTime = new SDVTime(0600);
                if (FogChance > 0 && FogChance < .25)
                    this.ExpirTime = new SDVTime(830);
                else if (FogChance >= .25 && FogChance < .32)
                    this.ExpirTime = new SDVTime(900);
                else if (FogChance >= .32 && FogChance < .41)
                    this.ExpirTime = new SDVTime(930);
                else if (FogChance >= .41 && FogChance < .55)
                    this.ExpirTime = new SDVTime(950);
                else if (FogChance >= .55 && FogChance < .7)
                    this.ExpirTime = new SDVTime(1040);
                else if (FogChance >= .7 && FogChance < .8)
                    this.ExpirTime = new SDVTime(1120);
                else if (FogChance >= .8 && FogChance < .9)
                    this.ExpirTime = new SDVTime(1200);
                else if (FogChance >= .9 && FogChance < .95)
                    this.ExpirTime = new SDVTime(1220);
                else if (FogChance >= .95 && FogChance < .98)
                    this.ExpirTime = new SDVTime(1300);
                else if (FogChance >= .98 && FogChance < .99)
                    this.ExpirTime = new SDVTime(1910);
                else if (FogChance >= .99)
                    this.ExpirTime = new SDVTime(2400);
            }
            else
            {
                BeginTime = new SDVTime(Game1.getModeratelyDarkTime());
                BeginTime.AddTime(Dice.Next(-15, 90));

                ExpirTime = new SDVTime(BeginTime);
                ExpirTime.AddTime(Dice.Next(120, 310));

                BeginTime.ClampToTenMinutes();
                ExpirTime.ClampToTenMinutes();
            }

            if (SDVTime.CurrentTime >= BeginTime)
                UpdateStatus(WeatherType, true);
        }

        public void EndWeather()
        {
            if (IsWeatherVisible)
            {
                ExpirTime = new SDVTime(SDVTime.CurrentTime - 10);
                CurrentFogType = FogType.None;
                UpdateStatus(WeatherType, false);
            }
        }

        public void SetWeatherTime(SDVTime begin, SDVTime end)
        {
            BeginTime = new SDVTime(begin);
            ExpirTime = new SDVTime(end);
        }

        public override string ToString()
        {
            return $"Fog Weather from {BeginTime} to {ExpirTime}. Visible: {IsWeatherVisible}.  Alpha: {FogAlpha}.";
        }

        public void UpdateWeather()
        {
            if (WeatherBeginTime is null || WeatherExpirationTime is null)
                return;

            if (WeatherInProgress && !IsWeatherVisible)
            {            
                CurrentFogType = FogType.Normal;
                FadeInFog = true;
                FogElapsed.Start();
                UpdateStatus(WeatherType, true);
            }

            if (WeatherExpirationTime <= SDVTime.CurrentTime && IsWeatherVisible)
            {
                FadeOutFog = true;
                FogElapsed.Start();
                UpdateStatus(WeatherType, false);
            }
        }
        
        internal void SetEveningFog()
        {
            SDVTime STime, ETime;
            STime = new SDVTime(Game1.getStartingToGetDarkTime());
            STime.AddTime(Dice.Next(-25, 80));

            ETime = new SDVTime(STime);
            ETime.AddTime(Dice.Next(120, 310));

            STime.ClampToTenMinutes();
            ETime.ClampToTenMinutes();
            this.SetWeatherTime(STime, ETime);
        }

        public void DrawWeather()
        {
            if (IsWeatherVisible)
            {
                if (CurrentFogType != FogType.Blinding)
                {
                    //Game1.outdoorLight = fogLight;
                    Texture2D fogTexture = null;
                    Vector2 position = new Vector2();
                    float num1 = -64* Game1.pixelZoom + (int)(FogPosition.X % (double)(64 * Game1.pixelZoom));
                    while (num1 < (double)Game1.graphics.GraphicsDevice.Viewport.Width)
                    {
                        float num2 = -64 * Game1.pixelZoom + (int)(FogPosition.Y % (double)(64 * Game1.pixelZoom));
                        while ((double)num2 < Game1.graphics.GraphicsDevice.Viewport.Height)
                        {
                            position.X = (int)num1;
                            position.Y = (int)num2;
                            
                            fogTexture = Sheet.FogTexture;

                            if (Game1.isStartingToGetDarkOut())
                            {
                                FogColor = Color.LightBlue;
                            }
                            if (BloodMoon)
                            {
                                FogColor = Color.DarkRed;
                            }

                            Game1.spriteBatch.Draw(fogTexture, position, new Microsoft.Xna.Framework.Rectangle?
                                    (FogSource), FogAlpha > 0.0 ? FogColor * FogAlpha : Color.Black * 0.95f, 0.0f, Vector2.Zero, Game1.pixelZoom + 1f / 1000f, SpriteEffects.None, 1f);
                            num2 += 64 * Game1.pixelZoom;
                        }
                        num1 += 64 * Game1.pixelZoom;
                    }
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
             return $"With roll {fogRoll.ToString("N3")} against {fogChance}, there will be fog today from {WeatherBeginTime} to {WeatherExpirationTime} with type {CurrentFogType}";
        }

        public void MoveWeather()
        {
            float FogFadeTime = 2690f;
            if (FadeOutFog)
            {
                // we want to fade out the fog over 3 or so seconds, so we need to process a fade from 100% to 45%
                // So, 3000ms for 55% or 54.45 repeating. But this is super fast....
                // let's try 955ms.. or 1345..
                // or 2690.. so no longer 3s. :<
                FogAlpha = 1 - (FogElapsed.ElapsedMilliseconds / FogFadeTime);

                if (FogAlpha <= 0)
                {
                    FogAlpha = 0;
                    CurrentFogType = FogType.None;
                    FadeOutFog = false;
                    FogElapsed.Stop();
                    FogElapsed.Reset();
                }
            }

            if (FadeInFog)
            {
                //as above, but the reverse.
                FogAlpha = (FogElapsed.ElapsedMilliseconds / FogFadeTime);
                if (FogAlpha >= 1)
                {
                    FogAlpha = 1;
                    FadeInFog = false;
                    FogElapsed.Stop();
                    FogElapsed.Reset();
                }
            }

            if (IsWeatherVisible)
            {
                //Game1.outdoorLight = fogLight;
                this.FogPosition = Game1.updateFloatingObjectPositionForMovement(FogPosition,
                    new Vector2(Game1.viewport.X, Game1.viewport.Y), Game1.previousViewportPosition, -1f);
                FogPosition = new Vector2((FogPosition.X + 0.5f) % (64 * Game1.pixelZoom),
                    (FogPosition.Y + 0.5f) % (64 * Game1.pixelZoom));
            }
        }
    }
}
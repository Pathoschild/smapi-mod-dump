/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using TwilightShards.Stardew.Common;

namespace ClimatesOfFerngillRebuild
{
    /// <summary> This tracks fog details </summary>
    internal class FerngillFog : ISDVWeather
    {
        private const int Texturesize = 128;
        public event EventHandler<WeatherNotificationArgs> OnUpdateStatus;
        public static Rectangle FogSource = new Rectangle(0, 0, Texturesize, Texturesize);
        private Color _fogColor = Color.White * 1.25f;
        /// <summary> The Current Fog Type </summary>
        internal FogType CurrentFogType { get; set; }
        public bool BloodMoon { get; set; }
        /// <summary> The calculated alpha attribute of fog to use when fading fog in/out. </summary>
        private float FogTargetAlpha { get; set; }
        /// <summary> The currently-shown alpha attribute of the fog. </summary>
        private float FogAlpha { get; set; }
        /// <summary> Fog Position. For drawing. </summary>
        private Vector2 FogPosition { get; set; }
        /// <summary> Sets the expiration time of the fog </summary>
        private SDVTime ExpirationTime { get; set; }
        private SDVTime BeginTime { get; set; }
        public bool IsWeatherVisible => (CurrentFogType != FogType.None);
        public string WeatherType => "Fog";
        /// <summary> Returns the expiration time of fog. Note that this doesn't sanity check if the fog is even visible. </summary>
        public SDVTime WeatherExpirationTime => (ExpirationTime ?? new SDVTime(0600));
        public SDVTime WeatherBeginTime => (BeginTime ?? new SDVTime(0600));
        public bool WeatherInProgress => (SDVTime.CurrentTime >= BeginTime && SDVTime.CurrentTime <= ExpirationTime && WeatherBeginTime != WeatherExpirationTime);

        /// <summary> Sets the fog expiration time. </summary>
        /// <param name="t">The time for the fog to expire</param>
        public void SetWeatherExpirationTime(SDVTime t)
        {
            ExpirationTime = new SDVTime(t);
        }
        public void SetWeatherBeginTime(SDVTime t)
        {
            BeginTime = new SDVTime(t);
        }
        public SDVTimePeriods FogTimeSpan { get; set;}
        private bool FadeOutFog { get; set; }
        private bool FadeInFog { get; set; }
        private Stopwatch FogElapsed { get; }
        public void SetFogTargetAlpha()
        {
            if (ClimatesOfFerngill.WeatherOpt.ShowLighterFog)
            {
                if (Game1.isRaining)
                    this.FogTargetAlpha = .25f;
                else
                    this.FogTargetAlpha = .35f;
            }
            else
            {
                this.FogTargetAlpha = .7f;
            }

            if (CurrentFogType == FogType.Light)
                this.FogTargetAlpha = .6f; //.4f is barely visible

            if (CurrentFogType == FogType.Blinding)
                this.FogTargetAlpha = .95f; 
        }

        /// <summary> Default constructor. </summary>
        internal FerngillFog(SDVTimePeriods fogPeriod)
        {
            CurrentFogType = FogType.None;
            BeginTime = new SDVTime(0600);
            ExpirationTime = new SDVTime(0600);
            BloodMoon = false;
            FogTimeSpan = fogPeriod;
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
            BeginTime = new SDVTime(0600);
            ExpirationTime = new SDVTime(0600);
            BloodMoon = false;
            FogAlpha = 0f;
            FogTargetAlpha = 0f;
            FadeOutFog = false;
            FadeInFog = false;
            FogElapsed.Reset(); 
        }

        public void ForceWeatherStart()
        {
            SetFogTargetAlpha();
            this.FogAlpha = this.FogTargetAlpha;
            CurrentFogType = FogType.Normal;
        }

        public void ForceWeatherEnd()
        {
            ExpirationTime = new SDVTime(SDVTime.CurrentTime - 10);
            CurrentFogType = FogType.None;
            FadeOutFog = true;
            SetFogTargetAlpha();
            FogElapsed.Start();
            UpdateStatus(WeatherType, false);
        }

        public string DebugWeatherOutput()
        {
            string s = "";
            s += $"Weather {WeatherType} is {IsWeatherVisible}, Progress: {WeatherInProgress}, Begin Time {BeginTime} to End Time {ExpirationTime}. Alpha is {FogAlpha}.";
            s += $"{Environment.NewLine} Color is {_fogColor}. Position is {FogPosition}, with Fade Out Timer being {FadeOutFog} and In {FadeInFog}";
            s += $"{Environment.NewLine} Fog Type is {CurrentFogType}";
            return s;
        }

        /// <summary>Returns a string describing the fog type. </summary>
        /// <param name="currentFogType">The type of the fog being looked at.</param>
        /// <returns>The fog type</returns>
        internal static string DescFogType(FogType currentFogType)
        {
            switch (currentFogType)
            {
                case FogType.None:
                    return "None";
                case FogType.Light:
                    return "Light";
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

        public void CreateWeather()
        {
            CreateWeather(FogType.Normal, false);
        }

        /// <summary>This function creates the fog </summary>
        public void CreateWeather(FogType fType, bool force)
        {
            CurrentFogType = FogType.Normal;

            if (ClimatesOfFerngill.Dice.NextDoublePositive() < .25)
                CurrentFogType = FogType.Light;
            
            else if (ClimatesOfFerngill.Dice.NextDoublePositive() > .90)
                CurrentFogType = FogType.Blinding;

            if (force)
                CurrentFogType = fType;

            SetFogTargetAlpha();

            this.FogAlpha = this.FogTargetAlpha;

            //now determine the fog expiration time
            double fogChance = ClimatesOfFerngill.Dice.NextDoublePositive();

            /*
             * So we should rarely have full day fog, and it should on average burn off around 9am. 
             * So, the strongest odds should be 820 to 930, with sharply falling off odds until 1200. And then
             * so, extremely rare odds for until 7pm and even rarer than midnight.
             */

            if (FogTimeSpan == SDVTimePeriods.Morning)
            {
                BeginTime = new SDVTime(0600);
                if (fogChance > 0 && fogChance < .25)
                    this.ExpirationTime = new SDVTime(830);
                else if (fogChance >= .25 && fogChance < .32)
                    this.ExpirationTime = new SDVTime(900);
                else if (fogChance >= .32 && fogChance < .41)
                    this.ExpirationTime = new SDVTime(930);
                else if (fogChance >= .41 && fogChance < .55)
                    this.ExpirationTime = new SDVTime(950);
                else if (fogChance >= .55 && fogChance < .7)
                    this.ExpirationTime = new SDVTime(1040);
                else if (fogChance >= .7 && fogChance < .8)
                    this.ExpirationTime = new SDVTime(1120);
                else if (fogChance >= .8 && fogChance < .9)
                    this.ExpirationTime = new SDVTime(1200);
                else if (fogChance >= .9 && fogChance < .95)
                    this.ExpirationTime = new SDVTime(1220);
                else if (fogChance >= .95 && fogChance < .98)
                    this.ExpirationTime = new SDVTime(1300);
                else if (fogChance >= .98 && fogChance < .99)
                    this.ExpirationTime = new SDVTime(1910);
                else if (fogChance >= .99)
                    this.ExpirationTime = new SDVTime(2400);
            }
            else
            {
                BeginTime = new SDVTime(Game1.getModeratelyDarkTime());
                BeginTime.AddTime(ClimatesOfFerngill.Dice.Next(-15, 90));

                ExpirationTime = new SDVTime(BeginTime);
                ExpirationTime.AddTime(ClimatesOfFerngill.Dice.Next(120, 310));

                BeginTime.ClampToTenMinutes();
                ExpirationTime.ClampToTenMinutes();
            }

            if (SDVTime.CurrentTime >= BeginTime)
                UpdateStatus(WeatherType, true);
        }

        public void EndWeather()
        {
            if (IsWeatherVisible)
            {
                ExpirationTime = new SDVTime(SDVTime.CurrentTime - 10);
                CurrentFogType = FogType.None;
                FadeOutFog = true;
                SetFogTargetAlpha();
                FogElapsed.Start();
                UpdateStatus(WeatherType, false);
            }
        }

        public void SetWeatherTime(SDVTime begin, SDVTime end)
        {
            BeginTime = new SDVTime(begin);
            ExpirationTime = new SDVTime(end);
        }

        public override string ToString()
        {
            return $"Fog Weather from {BeginTime} to {ExpirationTime}. Visible: {IsWeatherVisible}.  Alpha: {FogAlpha}.";
        }

        public void UpdateWeather()
        {
            if (WeatherBeginTime is null || WeatherExpirationTime is null)
                return;

            if (WeatherInProgress && !IsWeatherVisible)
            {            
                CurrentFogType = FogType.Normal;
                FadeInFog = true;
                SetFogTargetAlpha();
                FogElapsed.Start();
                UpdateStatus(WeatherType, true);
            }

            if (SDVTime.CurrentTime >= WeatherExpirationTime && IsWeatherVisible)
            {
                FadeOutFog = true;
                SetFogTargetAlpha();
                FogElapsed.Start();
                UpdateStatus(WeatherType, false);
            }
        }
        
        internal void SetEveningFog()
        {
            var sTime = new SDVTime(Game1.getStartingToGetDarkTime());
            sTime.AddTime(ClimatesOfFerngill.Dice.Next(-25, 80));

            var eTime = new SDVTime(sTime);
            eTime.AddTime(ClimatesOfFerngill.Dice.Next(120, 310));

            sTime.ClampToTenMinutes();
            eTime.ClampToTenMinutes();
            SetWeatherTime(sTime, eTime);
        }

        public void DrawWeather()
        {
            if (IsWeatherVisible)
            {
                if (!(Game1.currentLocation is Desert) || (ClimatesOfFerngill.WeatherOpt.DisplayFogInDesert && Game1.currentLocation is Desert))
                {
                    Texture2D fogTexture = ClimatesOfFerngill.OurIcons.FogTexture;
                    Texture2D blindingTexture = ClimatesOfFerngill.OurIcons.BlindingFogTexture;
                    Texture2D lightTexture = ClimatesOfFerngill.OurIcons.LightFogTexture;

                    Vector2 position = new Vector2();
                    float num1 = -Texturesize* Game1.pixelZoom + (int)(FogPosition.X % (double)(Texturesize * Game1.pixelZoom));
                    while (num1 < (double)Game1.graphics.GraphicsDevice.Viewport.Width)
                    {
                        float num2 = -Texturesize * Game1.pixelZoom + (int)(FogPosition.Y % (double)(Texturesize * Game1.pixelZoom));
                        while ((double)num2 < Game1.graphics.GraphicsDevice.Viewport.Height)
                        {
                            position.X = (int)num1;
                            position.Y = (int)num2;
                            
                            if (Game1.isStartingToGetDarkOut())
                            {
                                _fogColor = Color.LightSteelBlue;
                            }
                            if (BloodMoon)
                            {
                                _fogColor = Color.DarkRed;
                            }

                            if (CurrentFogType == FogType.Blinding)
                            {
                                //_fogColor = Color.Black;

                                Game1.spriteBatch.Draw(blindingTexture, position, new Rectangle?(FogSource),
                                    FogAlpha > 0.0 ? _fogColor * FogAlpha : Color.Black * 0.95f, 0.0f, Vector2.Zero,
                                    Game1.pixelZoom + 1f / 1000f, SpriteEffects.None, 1f);
                            }
                            else if (CurrentFogType == FogType.Light)
                            {
                                Game1.spriteBatch.Draw(lightTexture, position, new Rectangle?(FogSource),
                                    FogAlpha > 0.0 ? _fogColor * FogAlpha : Color.Black * 0.95f, 0.0f, Vector2.Zero,
                                    Game1.pixelZoom + 1f / 1000f, SpriteEffects.None, 1f);
                            }
                            else
                            {
                                Game1.spriteBatch.Draw(fogTexture, position, new Rectangle?(FogSource), FogAlpha > 0.0 ? _fogColor * FogAlpha : Color.Black * 0.95f, 0.0f, Vector2.Zero, Game1.pixelZoom + 1f / 1000f, SpriteEffects.None, 1f);
                            }

                            num2 += Texturesize * Game1.pixelZoom;
                        }
                        num1 += Texturesize * Game1.pixelZoom;
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
             return $"With roll {fogRoll:N3} against {fogChance}, there will be fog today from {WeatherBeginTime} to {WeatherExpirationTime} with type {CurrentFogType}";
        }

        public void MoveWeather()
        {
            const float fogFadeTime = 3120f;
            if (FadeOutFog)
            {
                // we want to fade out the fog over 3 or so seconds, so we need to process a fade from 100% to 45%
                // So, 3000ms for 55% or 54.45 repeating. But this is super fast....
                // let's try 955ms.. or 1345..
                // or 2690.. so no longer 3s. :<
                FogAlpha = FogTargetAlpha * (this.FogTargetAlpha - (FogElapsed.ElapsedMilliseconds / fogFadeTime));
       
                if (FogAlpha <= 0)
                {
                    FogAlpha = 0;
                    FogTargetAlpha = 0;
                    CurrentFogType = FogType.None;
                    FadeOutFog = false;
                    FogElapsed.Stop();
                    FogElapsed.Reset();
                }
            }

            if (FadeInFog)
            {
                //as above, but the reverse.
                FogAlpha = FogTargetAlpha * (FogElapsed.ElapsedMilliseconds / fogFadeTime);
                if (FogAlpha >= FogTargetAlpha)
                {
                    FogAlpha = FogTargetAlpha;
                    FadeInFog = false;
                    FogElapsed.Stop();
                    FogElapsed.Reset();
                }
            }

            if (IsWeatherVisible && Game1.shouldTimePass() )
            {
                if (Game1.isDebrisWeather) {
                    this.FogPosition = Game1.updateFloatingObjectPositionForMovement(FogPosition,
                        new Vector2(Game1.viewport.X, Game1.viewport.Y), Game1.previousViewportPosition, -1f);
                    FogPosition = new Vector2((FogPosition.X + 0.5f) % (Texturesize * Game1.pixelZoom) + WeatherDebris.globalWind,
                        (FogPosition.Y + 0.5f) % (Texturesize * Game1.pixelZoom));
                }
                else {
                    //Game1.outdoorLight = fogLight;
                    this.FogPosition = Game1.updateFloatingObjectPositionForMovement(FogPosition,
                        new Vector2(Game1.viewport.X, Game1.viewport.Y), Game1.previousViewportPosition, -1f);
                    FogPosition = new Vector2((FogPosition.X + 0.5f) % (Texturesize * Game1.pixelZoom),
                        (FogPosition.Y + 0.5f) % (Texturesize * Game1.pixelZoom));
                }
            }
        }
    }
}

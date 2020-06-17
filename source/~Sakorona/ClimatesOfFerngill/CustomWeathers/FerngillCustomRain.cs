using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using TwilightShards.Common;
using TwilightShards.Stardew.Common;

namespace ClimatesOfFerngillRebuild.Weathers
{
    /// <summary>
    /// This class handles rain that needs to be drawn without being actual rain. 
    /// </summary>
    internal class FerngillCustomRain : ISDVWeather
    {
        protected bool IsBloodRain;
        protected static Color rainColor;
        protected static Color backRainColor;
        protected int SecondCount = 0;
        protected int SecondCountB = 0;
        protected RainDrop[] rainDrops;
        protected bool FlipSequence = false;
        protected int RainbowColor = 0;
        protected int RainbowColorB = 0;
        protected SpriteBatch b = new SpriteBatch(Game1.graphics.GraphicsDevice);

        protected readonly BlendState lightingBlend = new BlendState(){
            ColorBlendFunction = BlendFunction.ReverseSubtract,
            ColorDestinationBlend = Blend.One,
            ColorSourceBlend = Blend.SourceColor
        };
        
        public bool IsWeatherVisible { get; private set; }
        public string WeatherType => "customrain";
        public bool WeatherInProgress => (IsWeatherVisible);
        public SDVTime WeatherExpirationTime { get; private set; }
        public SDVTime WeatherBeginTime { get; private set; }
        
        protected List<Color> colorsOfTheRainbow = new List<Color>
        {
            Color.DarkRed,
            Color.Red,
            Color.Pink,
            Color.OrangeRed,
            Color.Orange,
            Color.LightGoldenrodYellow,
            Color.Yellow,
            Color.GreenYellow,
            Color.Green,
            Color.LightGreen,
            Color.SeaGreen,
            Color.Cyan,
            Color.LightBlue,
            Color.Blue,
            Color.Navy,
            Color.BlueViolet,
            Color.Indigo,
            Color.Violet,
            Color.DarkViolet
        };

        public event EventHandler<WeatherNotificationArgs> OnUpdateStatus;

        public FerngillCustomRain(int numOfDrops, bool BloodRain = true)
        {
            rainDrops = new RainDrop[numOfDrops];
            backRainColor = Color.Blue;
            rainColor = Color.White;
            IsBloodRain = BloodRain;

            for (int index = 0; index < this.rainDrops.Length; ++index)
            {
                this.rainDrops[index] = new RainDrop(ClimatesOfFerngill.Dice.Next(Game1.viewport.Width),
                    ClimatesOfFerngill.Dice.Next(Game1.viewport.Height), ClimatesOfFerngill.Dice.Next(4), ClimatesOfFerngill.Dice.Next(70));
            }
        }
        
        public void CreateWeather()
        {
            WeatherBeginTime = new SDVTime(0600);
            WeatherExpirationTime = new SDVTime(2600);

            UpdateStatus("customrain", true);
            IsWeatherVisible = true;
        }

        public void ForceWeatherStart()
        {
            IsWeatherVisible  = true;
        }

        //TODO: FIX ME
        public string DebugWeatherOutput()
        {
            string s = "";
            s += $"Weather {WeatherType} is {IsWeatherVisible}, Progress: {WeatherInProgress}, Begin Time {WeatherBeginTime} to End Time {WeatherExpirationTime}.";
            return s;
        }
        public void SetWeatherTime(SDVTime begin, SDVTime end)
        {
            SetWeatherBeginTime(begin);
            SetWeatherExpirationTime(end);
        }

        public void SetWeatherBeginTime(SDVTime begin)
        {
            WeatherBeginTime = begin;
        }

        public void SetWeatherExpirationTime(SDVTime end)
        {
            WeatherExpirationTime = end;
        }

        public void DrawWeather()
        {
            if (Game1.drawLighting)
            {
                b.Begin(SpriteSortMode.Deferred,lightingBlend, SamplerState.LinearClamp, null, null);
                b.Draw((Texture2D)Game1.lightmap, Vector2.Zero, Game1.lightmap.Bounds, Color.White, 0.0f, Vector2.Zero, (float)(Game1.options.lightingQuality / 2), SpriteEffects.None, 1f);
                if (Game1.currentLocation.IsOutdoors && !(Game1.currentLocation is Desert))
                    b.Draw(Game1.staminaRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.OrangeRed * 0.45f);
                b.End();
            }

            if (Game1.currentLocation != null && (Game1.currentLocation.IsOutdoors && !(Game1.currentLocation is Desert)))
                Game1.spriteBatch.Draw(Game1.staminaRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, backRainColor * 0.2f);
            
            if (Game1.currentLocation.IsOutdoors && (!Game1.currentLocation.Name.Equals("Desert") && !(Game1.currentLocation is Summit)) && (!Game1.eventUp || Game1.currentLocation.isTileOnMap(new Vector2(Game1.viewport.X / 64, Game1.viewport.Y / 64))))
            {
                for (int index = 0; index < this.rainDrops.Length; ++index)
                    Game1.spriteBatch.Draw(Game1.rainTexture, this.rainDrops[index].position, Game1.getSourceRectForStandardTileSheet(Game1.rainTexture, this.rainDrops[index].frame, -1, -1), rainColor);
            }
        }

        public void UpdateWeather()
        {
        }

        public void ForceWeatherEnd()
        {
            WeatherBeginTime = new SDVTime(0600);
            WeatherExpirationTime = new SDVTime(0600);

            UpdateStatus("customrain", false);
            IsWeatherVisible = false;
        }

        public void MoveWeather()
        {
            UpdateRaindropPosition();

            if (Game1.currentLocation.IsOutdoors)
                for (int index = 0; index < this.rainDrops.Length; ++index)
                {
                    TimeSpan timeSpan;
                    if (this.rainDrops[index].frame == 0)
                    {
                        ref int local = ref this.rainDrops[index].accumulator;
                        int num3 = local;
                        timeSpan = Game1.currentGameTime.ElapsedGameTime;
                        int milliseconds = timeSpan.Milliseconds;
                        local = num3 + milliseconds;
                        if (this.rainDrops[index].accumulator >= 70)
                        {
                            this.rainDrops[index].position += new Vector2(
                                index * 8 / this.rainDrops.Length - 16,
                                32 - index * 8 / this.rainDrops.Length);
                            this.rainDrops[index].accumulator = 0;
                            if (ClimatesOfFerngill.Dice.NextDouble() < 0.1)
                                ++this.rainDrops[index].frame;
                            if (this.rainDrops[index].position.Y > (double) (Game1.viewport.Height + 64))
                                this.rainDrops[index].position.Y = -64f;
                        }
                    }
                    else
                    {
                        ref int local = ref this.rainDrops[index].accumulator;
                        int num3 = local;
                        timeSpan = Game1.currentGameTime.ElapsedGameTime;
                        int milliseconds = timeSpan.Milliseconds;
                        local = num3 + milliseconds;
                        if (this.rainDrops[index].accumulator > 70)
                        {
                            this.rainDrops[index].frame = (this.rainDrops[index].frame + 1) % 4;
                            this.rainDrops[index].accumulator = 0;
                            if (this.rainDrops[index].frame == 0)
                                this.rainDrops[index].position = new Vector2(
                                    (float)ClimatesOfFerngill.Dice.Next(Game1.viewport.Width),
                                    (float)ClimatesOfFerngill.Dice.Next(Game1.viewport.Height));
                        }
                    }
                }
        }

        public void SecondUpdate()
        {
            if (!Context.IsWorldReady)
                return;

            SecondCount++;
            SecondCountB++;

            if (IsBloodRain)
            {
                backRainColor = Color.Red;
                rainColor = Color.DarkRed;
            }
            else
            {
                if (SecondCount == 1)
                {
                    SecondCount = 0;
                    rainColor = colorsOfTheRainbow[RainbowColor];
                    if (RainbowColor >= (colorsOfTheRainbow.Count - 1))
                        FlipSequence = true;

                    if (RainbowColor <= 0)
                        FlipSequence = false;

                    RainbowColor += (FlipSequence) ? -1 : 1;
                }

                if (SecondCountB == 3)
                {
                    backRainColor = colorsOfTheRainbow[RainbowColorB];
                    SecondCount = 0;
                    if (RainbowColorB >= (colorsOfTheRainbow.Count - 1))
                        FlipSequence = true;

                    if (RainbowColorB <= 0)
                        FlipSequence = false;

                    RainbowColorB += (FlipSequence) ? -1 : 1;
                }
            }
        }

        public void UpdateRaindropPosition()
        {
            int diffX = Game1.viewport.X - (int) Game1.previousViewportPosition.X;
            int diffY = Game1.viewport.Y - (int)Game1.previousViewportPosition.Y;

            for (int index = 0; index < this.rainDrops.Length; ++index)
            {
                this.rainDrops[index].position.X -= (float)diffX * 1f;
                this.rainDrops[index].position.Y -= (float)diffY * 1f;

                if ((double)this.rainDrops[index].position.Y > (double)(Game1.viewport.Height + 64))
                    this.rainDrops[index].position.Y = -64f;
                else if ((double)this.rainDrops[index].position.X < -64.0)
                    this.rainDrops[index].position.X = (float)Game1.viewport.Width;
                else if ((double)this.rainDrops[index].position.Y < -64.0)
                    this.rainDrops[index].position.Y = (float)Game1.viewport.Height;
                else if ((double)this.rainDrops[index].position.X > (double)(Game1.viewport.Width + 64))
                    this.rainDrops[index].position.X = -64f;
            }
        }


        public void EndWeather()
        {
            WeatherExpirationTime = new SDVTime(Game1.timeOfDay);
            UpdateStatus("customrain", false);
            IsWeatherVisible = false;
        }

        public void OnNewDay()
        {
            Reset();
        }

        public void ResetRainSize(int newSize)
        {
            Array.Resize(ref rainDrops,newSize);            
            for (int i = 0; i < this.rainDrops.Length; i++)
            {
                this.rainDrops[i] = new RainDrop(ClimatesOfFerngill.Dice.Next(Game1.viewport.Width),
                    ClimatesOfFerngill.Dice.Next(Game1.viewport.Height), ClimatesOfFerngill.Dice.Next(4), ClimatesOfFerngill.Dice.Next(70));
            }
        }

        public void Reset()
        {
            IsWeatherVisible = false;
            SecondCount = SecondCountB = RainbowColor = RainbowColorB = 0;
            for (int i = 0; i < this.rainDrops.Length; i++)
            {
                this.rainDrops[i] = new RainDrop(ClimatesOfFerngill.Dice.Next(Game1.viewport.Width),
                    ClimatesOfFerngill.Dice.Next(Game1.viewport.Height), ClimatesOfFerngill.Dice.Next(4), ClimatesOfFerngill.Dice.Next(70));
            }
        }


        public void UpdateStatus(string weather, bool status)
        {
            if (OnUpdateStatus == null) return;

            WeatherNotificationArgs args = new WeatherNotificationArgs(weather, status);
            OnUpdateStatus(this, args);
        }


    }
}

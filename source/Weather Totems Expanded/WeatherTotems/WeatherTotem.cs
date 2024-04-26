/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/WeatherTotems
**
*************************************************/

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Objects;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI;
using StardewValley.GameData.LocationContexts;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using xTile.Dimensions;

namespace WeatherTotems
{
    public class WeatherTotem
    {
        private static IModHelper helper;
        private static IManifest manifest;

        // Get access to the required SMAPI apis
        public static void Initialise( IModHelper helper, IManifest manifest)
        {
            WeatherTotem.helper = helper;
            WeatherTotem.manifest = manifest;
        }

		private static void SetWeather(string weather)
		{
            if (Game1.currentLocation.GetLocationContextId() == "Default")
            {
                if (Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season) == false)
                {
                    Game1.netWorldState.Value.WeatherForTomorrow = (Game1.weatherForTomorrow = weather);
                }
            }
            else
            {
                Game1.currentLocation.GetWeather().WeatherForTomorrow = weather;
            }
            if (weather == "GreenRain")
            {
                Game1.player.modData[$"{manifest.UniqueID}/GreenTotemUse"] = "true";
            }
            else
            {
                Game1.player.modData[$"{manifest.UniqueID}/GreenTotemUse"] = "false";
            }
        }

        public static bool UseWeatherTotem(Farmer who, int totemtype)
		{
            var multiplayer = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
			bool changedweather = false;

            // Get location context
            var location_context = Game1.currentLocation.GetLocationContextId();		
			string message = "Nothing";
			
			// Get asset key for animation sprites
			string assetkey = helper.ModContent.GetInternalAssetName("assets/loosesprites.png").Name;

            switch (totemtype)
            {
                case 0:
                    SetWeather("Sun");
                    message = i18n.string_SunTotemUse();
                    changedweather = true;
                    break;
                case 1:
                    foreach (var weather in Game1.currentLocation.GetLocationContext().WeatherConditions)
                    {
                        if (weather.Weather == "Wind")
                        {
                            SetWeather("Wind");
                            message = i18n.string_WindTotemUse();
                            changedweather = true;
                            break;
                        }
                    }
                    break;
                case 2:
                    foreach (var weather in Game1.currentLocation.GetLocationContext().WeatherConditions)
                    {
                        if (weather.Weather == "Snow")
                        {
                            SetWeather("Snow");
                            message = i18n.string_SnowTotemUse();
                            changedweather = true;
                            break;
                        }
                    }

                    break;
                case 3:
                    foreach (var weather in Game1.currentLocation.GetLocationContext().WeatherConditions)
                    {
                        if (weather.Weather == "Storm")
                        {
                            SetWeather("Storm");
                            message = i18n.string_ThunderTotemUse();
                            changedweather = true;
                            break;
                        }
                    }
                    break;
                case 4:
                    if (Game1.season == Season.Summer && Game1.dayOfMonth != 28 && location_context == "Default")
                    {
                        SetWeather("GreenRain");
                        message = i18n.string_GreenRainTotemUse();
                        changedweather = true;
                    }
                    break;
            }
            if (changedweather == false)
            {
                return changedweather;
            }

            Game1.pauseThenMessage(2000, message);

            // Totem activation stuff
            Game1.screenGlow = false;
			who.currentLocation.playSound("thunder");
			who.canMove = false;
			switch (totemtype)
			{
				case 0:
					Game1.screenGlowOnce(Color.Gold, hold: false);
					break;
				case 1:
					Game1.screenGlowOnce(Color.Lavender, hold: false);
					break;
				case 2:
					Game1.screenGlowOnce(Color.AliceBlue, hold: false);
					break;
				case 3:
					Game1.screenGlowOnce(Color.DarkSlateBlue, hold: false);
					break;
                case 4:
                    Game1.screenGlowOnce(Color.OliveDrab, hold: false);
                    break;
            }
			Game1.player.faceDirection(2);
			Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[1]
			{
				new FarmerSprite.AnimationFrame(57, 2000, secondaryArm: false, flip: false, Farmer.canMoveNow, behaviorAtEndOfFrame: true)
			});

			// Play totem animation 
			for (int i = 0; i < 6; i++)
			{
				void showsprites(string asset, Microsoft.Xna.Framework.Rectangle area)
                {
					multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(asset, area, 9999f, 1, 999, who.Position + new Vector2(0f, -128f), flicker: false, flipped: false, 1f, 0.01f, Color.White * 0.8f, 2f, 0.01f, 0f, 0f)
					{
						motion = new Vector2((float)Game1.random.Next(-10, 11) / 10f, -2f),
						delayBeforeAnimationStart = i * 200
					});
				}

				void showspritefromtotemtype(Microsoft.Xna.Framework.Rectangle whichareaforthunderanimation)
                {
					switch (totemtype)
					{
						case 0:
							showsprites(assetkey, new Microsoft.Xna.Framework.Rectangle(0, 16, 24, 24));
							break;
						case 1:
							showsprites(assetkey, new Microsoft.Xna.Framework.Rectangle(24, 16, 51, 24));
							break;
						case 2:
							showsprites(assetkey, new Microsoft.Xna.Framework.Rectangle(75, 16, 25, 25));
							break;
						case 3:
							showsprites("LooseSprites\\Cursors", whichareaforthunderanimation);
                            break;
                        case 4:
                            showsprites(assetkey, new Microsoft.Xna.Framework.Rectangle(100, 17, 20, 24));
                            break;
					}
				}

				showspritefromtotemtype(new Microsoft.Xna.Framework.Rectangle(645, 1079, 36, 56));
				showspritefromtotemtype(new Microsoft.Xna.Framework.Rectangle(648, 1045, 52, 33));
				showspritefromtotemtype(new Microsoft.Xna.Framework.Rectangle(648, 1045, 52, 33));				
			}
            Microsoft.Xna.Framework.Rectangle totemarea(int whichtotem)
            {
                switch (whichtotem)
                {
                    case 0:
                        return new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16);
                    case 1:
                        return new Microsoft.Xna.Framework.Rectangle(16, 0, 16, 16);
                    case 2:
                        return new Microsoft.Xna.Framework.Rectangle(32, 0, 16, 16);
                    case 3:
                        return new Microsoft.Xna.Framework.Rectangle(48, 0, 16, 16);
                    default:
                        return new Microsoft.Xna.Framework.Rectangle(64, 0, 16, 16);
                }
            }
            multiplayer.broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite(assetkey,totemarea(totemtype), 9999f, 1, 999, Game1.player.Position + new Vector2(0f, -96f), flicker: false, flipped: false, 1f, 0.0075f, Color.White * 0.8f, 4f, 0.015f, 0f, 0f)
			{
				motion = new Vector2(0f, -7f),
				acceleration = new Vector2(0f, 0.1f),
				scaleChange = 0.015f,
				alpha = 1f,
				alphaFade = 0.0075f,
				shakeIntensity = 1f,
				initialPosition = Game1.player.Position + new Vector2(0f, -96f),
				xPeriodic = true,
				xPeriodicLoopTime = 1000f,
				xPeriodicRange = 4f,
				layerDepth = 1f
			}); ;

            DelayedAction.playSoundAfterDelay("rainsound", 2000);
			return changedweather;
		}
	}
}

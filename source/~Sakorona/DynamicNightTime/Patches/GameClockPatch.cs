/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewValley;
using TwilightShards.Stardew.Common;

namespace DynamicNightTime.Patches
{
    class GameClockPatch
    {
        public static void Postfix()
        {
            SDVTime sunriseSTime = DynamicNightTime.GetSunrise();
            //sunriseSTime.AddTime(-10);
            int sunriseTime = sunriseSTime.ReturnIntTime();
            SDVTime sunsetT = DynamicNightTime.GetSunset();
            sunsetT.AddTime(-20);
            
            int astronTime = DynamicNightTime.GetMorningAstroTwilight().ReturnIntTime();

            //colors
            //sunrise 255,159,80
            //Color sunrise = new Color(0, 96, 175);
            //preSunrise - 200, 205, 227 - mask (55,50,28);

            if (DynamicNightTime.LunarDisturbancesLoaded && DynamicNightTime.MoonAPI.IsSolarEclipse())
            {
                Game1.outdoorLight = (Game1.eveningColor * .93f);
                return;
            }

            Color moonLight = new Color(0,0,0);

            if (DynamicNightTime.LunarDisturbancesLoaded && !Game1.isRaining && !Game1.isSnowing)
            {
                moonLight = DynamicNightTime.GetLunarLightDifference();
            }

            var weather = DynamicNightTime.ClimatesAPI?.GetCurrentWeatherName() ?? "error";

            bool shouldDarken = Game1.isRaining || ((DynamicNightTime.ClimatesLoaded && weather.Contains("overcast")));

            if (Game1.timeOfDay <= astronTime)
            {
                Color oldLight = (Game1.eveningColor * .93f);

                if (DynamicNightTime.LunarDisturbancesLoaded && DynamicNightTime.MoonAPI.IsMoonUp(Game1.timeOfDay)) { 
                    oldLight.R = (byte)(oldLight.R - moonLight.R);
                    oldLight.G = (byte)(oldLight.G - moonLight.G);
                    oldLight.B = (byte)(oldLight.B - moonLight.B);
                }
                Game1.outdoorLight = oldLight;
            }

            else if (Game1.timeOfDay >= astronTime && Game1.timeOfDay < sunriseTime)
            {
                if (shouldDarken) { 
                    float minEff = SDVTime.MinutesBetweenTwoIntTimes(astronTime, Game1.timeOfDay) + (float)Math.Min(10.0, Game1.gameTimeInterval / 700);
                    float percentage = (minEff / SDVTime.MinutesBetweenTwoIntTimes(sunriseTime, astronTime));
                    Game1.outdoorLight = new Color((byte)(237 - (158 * percentage)), (byte)(185 - (126 * percentage)), (byte)(74 - (51 * percentage)), (byte)(237 - (161 * percentage)));
                }
                else
                { 
                    float minEff = SDVTime.MinutesBetweenTwoIntTimes(astronTime, Game1.timeOfDay) + (float)Math.Min(10.0, Game1.gameTimeInterval / 700);
                    float percentage = (minEff / SDVTime.MinutesBetweenTwoIntTimes(sunriseTime, astronTime));
                    //means delta r is -255, delta g is -159, delta b is +175 from evening to sunrise
                    //Normal sunrise is 0,96,175. Rainy sunrises are.. 0,50,148?
                    //However, this is set to a light blue-gray
                    
                    Color destColor = new Color((byte)(255 - (200 * percentage)), (byte)(255 - (205 * percentage)), (byte)(28 * percentage));
                    Game1.outdoorLight = destColor;
                }

                /*
                else if (Game1.timeOfDay >= sunriseTime && Game1.timeOfDay < sunriseEnd)
                {
                if (ShouldDarken)
                {
                    Game1.outdoorLight = Game1.ambientLight * 0.3f;
                }
                else
                {
                    float minEff = SDVTime.MinutesBetweenTwoIntTimes(astronTime, Game1.timeOfDay) + (float)Math.Min(10.0, Game1.gameTimeInterval / 700);
                    float percentage = (minEff / SDVTime.MinutesBetweenTwoIntTimes(sunriseTime, astronTime));
                    //means delta r is -255, delta g is -159, delta b is +175 from evening to sunrise
                    //Normal sunrise is 0,96,175. Rainy sunrises are.. 0,50,148?
                    //We're coming from 55 50 28 to 0 96 175
                    Color destColor = new Color(55 + (200 * percentage), 50 + (109 * percentage), 28 + (62* percentage));
                    //Color destColor = sunrise;
                    Game1.outdoorLight = destColor;
                }*/
            }
            else if (Game1.timeOfDay >= sunriseTime && Game1.timeOfDay <= Game1.getStartingToGetDarkTime())
            {
                if (shouldDarken)
                {
                    Game1.outdoorLight = Game1.ambientLight * 0.3f;
                }
                else 
                {
                    //
                    //preSunrise - 200, 205, 227 - mask (55,50,28); 
                    //flips to orange (243,206,155) then fades to noon - mask (9, 49, 100)
                    //Goes from [0,96,175] to [0,5,1] to [0,98,193]
                    int solarNoon = DynamicNightTime.GetSolarNoon().ReturnIntTime();
                    if (Game1.timeOfDay < solarNoon)
                    {
                        //this is really the mask color, tho.
                        var newSunrise = DynamicNightTime.NightConfig.MoreOrangeSunrise ? new Color(13,72,147) : new Color(9,49,100);

                        float minEff = SDVTime.MinutesBetweenTwoIntTimes(Game1.timeOfDay, sunriseTime) + (float)Math.Min(10.0, Game1.gameTimeInterval / 700);
                        float percentage = (minEff / SDVTime.MinutesBetweenTwoIntTimes(sunriseTime, solarNoon));

                        float tgtColorR = newSunrise.R - 0;
                        float tgtColorG = newSunrise.G - 5;
                        float tgtColorB = newSunrise.B - 1;
                        Color destColor = new Color((byte)(newSunrise.R - (tgtColorR*percentage)), (byte)(newSunrise.G -(tgtColorG*percentage)),(byte)(newSunrise.B -(tgtColorB*percentage)));
                        Game1.outdoorLight = destColor;
                    }
                    if (Game1.timeOfDay == solarNoon)
                    {
                        Color destColor = new Color(0,5,1);
                        Game1.outdoorLight = destColor;
                    }
                    if (Game1.timeOfDay > solarNoon)
                    {
                        float minEff = SDVTime.MinutesBetweenTwoIntTimes(Game1.timeOfDay, solarNoon) + (float)Math.Min(10.0, Game1.gameTimeInterval / 700);
                        float percentage = (minEff / SDVTime.MinutesBetweenTwoIntTimes(sunriseTime, solarNoon));
                        Color destColor = new Color(0, (byte)(5 + (93 * percentage)), (byte)(1 + (192 * percentage)));
                        Game1.outdoorLight = destColor;
                    }
                }
            }
            else if (Game1.timeOfDay >= Game1.getStartingToGetDarkTime())
            {
                //Goes from [0,98,193] to [255,255,0]. We should probably space this out so that civil is still fairly bright.
                int sunset = sunsetT.ReturnIntTime();
                int sunsetEnding = DynamicNightTime.GetSunset().ReturnIntTime();
                int astroTwilight = DynamicNightTime.GetAstroTwilight().ReturnIntTime();
                //Color navalColor = new Color(120,178,113);
                if (shouldDarken)
                {
                    if (Game1.timeOfDay >= Game1.getTrulyDarkTime())
                    {
                        float num = Math.Min(0.93f, (float)(0.75 + ((double)((int)((double)(Game1.timeOfDay - Game1.timeOfDay % 100) + (double)(Game1.timeOfDay % 100 / 10) * 16.6599998474121) - Game1.getTrulyDarkTime()) + (double)Game1.gameTimeInterval / 7000.0 * 16.6000003814697) * 0.000624999986030161));
                        Game1.outdoorLight = (Game1.isRaining ? Game1.ambientLight : Game1.eveningColor) * num;
                    }
                    else if (Game1.timeOfDay >= Game1.getStartingToGetDarkTime())
                    {
                        float num = Math.Min(0.93f, (float)(0.300000011920929 + ((double)((int)((double)(Game1.timeOfDay - Game1.timeOfDay % 100) + (double)(Game1.timeOfDay % 100 / 10) * 16.6599998474121) - Game1.getStartingToGetDarkTime()) + (double)Game1.gameTimeInterval / 7000.0 * 16.6000003814697) * 0.00224999990314245));
                        Game1.outdoorLight = (Game1.isRaining ? Game1.ambientLight : Game1.eveningColor) * num;
                    }
                }
                else
                {
                    //civil
                    if (Game1.timeOfDay > sunset && Game1.timeOfDay <= sunsetEnding)
                    {
                        //orange. >:(
                        //so much orange.
                        //rgba(220,168,26,1.00)
                        Game1.outdoorLight = new Color(35,87,228);
                    }
                    else if (Game1.timeOfDay > sunsetEnding)
                    {
                        float minEff = SDVTime.MinutesBetweenTwoIntTimes(Game1.timeOfDay, sunset) + (float)Math.Min(10.0, Game1.gameTimeInterval / 700);
                        float percentage = (minEff / SDVTime.MinutesBetweenTwoIntTimes(sunset, astroTwilight));
                        //clamp percentage. :|
                        if (percentage < 0) percentage = 0;
                        if (percentage > 1) percentage = 1;

                        int redTarget, greenTarget, blueTarget, alphaTarget;

                        switch (DynamicNightTime.NightConfig.NightDarknessLevel)
                        {
                            //42,55,127
                            case 1:
                            default:
                                redTarget = 213;
                                greenTarget = 200;
                                blueTarget = 128;
                                alphaTarget = 40;
                                break;
                            //+20,+20,+0,+30
                            case 2:
                                redTarget = 200;
                                greenTarget = 220;
                                blueTarget = 128;
                                alphaTarget = 50;
                                break;
                            //9,17,127
                            case 3:
                                redTarget = 220;
                                greenTarget = 240;
                                blueTarget = 128;
                                alphaTarget = 20;
                                break;
                            //-10,-10,0,-20
                            case 4:
                                redTarget = 230;
                                greenTarget = 250;
                                blueTarget = 128;
                                alphaTarget = 20;
                                break;
                            case 5:
                                redTarget = 250;
                                greenTarget = 250;
                                blueTarget = 120;
                                alphaTarget = 10;
                                break;
                        }

                        //subtract the origin color.
                        redTarget -= 35;
                        greenTarget -= 87;
                        blueTarget = 228 - blueTarget;
                        //Goes from [0,98,193] <-former, now from [35,87,228] 
                        
                        var destColor = new Color(r: (byte)(35 + (redTarget * percentage)), g: (byte)(87 + (greenTarget * percentage)), b: (byte)(228 - (blueTarget * percentage)), a: (byte)(255 - (alphaTarget * percentage)));


                        //[222,222,15]

                        if (DynamicNightTime.LunarDisturbancesLoaded && DynamicNightTime.MoonAPI.IsMoonUp(Game1.timeOfDay))
                        {
                            //start adding the moon in naval light
                            minEff = SDVTime.MinutesBetweenTwoIntTimes(Game1.timeOfDay, sunset) + (float)Math.Min(10.0, Game1.gameTimeInterval / 700);
                            percentage = (minEff  / SDVTime.MinutesBetweenTwoIntTimes(sunset, astroTwilight));
                            //clamp percentage. :|
                            if (percentage < 0) percentage = 0;
                            if (percentage > 1) percentage = 1;

                            int rRaw = (int)(destColor.R - (moonLight.R * percentage));
                            int gRaw = (int)(destColor.G - (moonLight.G * percentage));
                            int bRaw = (int)(destColor.B - (moonLight.B * percentage));

                            byte R, G, B;

                            R = DynamicNightTime.ClampByteValue(rRaw);
                            G = DynamicNightTime.ClampByteValue(gRaw);
                            B = DynamicNightTime.ClampByteValue(bRaw);

                            destColor.R = R;
                            destColor.G = G;
                            destColor.B = B;
                        }
                        Game1.outdoorLight = destColor;
                    }
                }
            }
        }
    }
}

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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using TwilightShards.Common;
using TwilightShards.Stardew.Common;
using SObject = StardewValley.Object;

namespace ClimatesOfFerngillRebuild
{
    class FerngillThunderFrenzy : ISDVWeather
    {
        public string WeatherType => "ThunderFrenzy";

        private SDVTime BeginTime;
        private SDVTime ExpirTime;
        private bool IsThorAngry;

        public event EventHandler<WeatherNotificationArgs> OnUpdateStatus;
        public SDVTime WeatherExpirationTime => (ExpirTime ?? new SDVTime(0600));
        public SDVTime WeatherBeginTime => (BeginTime ?? new SDVTime(0600));
        public void SetWeatherExpirationTime(SDVTime t)
        {
            ExpirTime = new SDVTime(t);
        }
        public void SetWeatherBeginTime(SDVTime t)
        {
            BeginTime = new SDVTime(t);
        }
        public bool WeatherInProgress => (SDVTime.CurrentTime >= WeatherBeginTime && SDVTime.CurrentTime <= WeatherExpirationTime);
        public bool IsWeatherVisible => IsThorAngry;

        /// <summary> Default constructor. </summary>
        internal FerngillThunderFrenzy()
        {

        }

        public void SetWeatherTime(SDVTime begin, SDVTime end)
        {
            BeginTime = new SDVTime(begin);
            ExpirTime = new SDVTime(end);
        }

        public void OnNewDay()
        {
            IsThorAngry = false;
            BeginTime = new SDVTime(0600);
            ExpirTime = new SDVTime(0600);
        }

        public void ForceWeatherStart()
        {
            IsThorAngry = true;
            BeginTime = new SDVTime(0600);
            ExpirTime = new SDVTime(2600);
        }

        public void ForceWeatherEnd()
        {
            ExpirTime = new SDVTime(SDVTime.CurrentTime - 10);
            IsThorAngry = false;
            UpdateStatus(WeatherType, false);
        }

        public void Reset()
        {
            IsThorAngry = false;
            BeginTime = new SDVTime(0600);
            ExpirTime = new SDVTime(0600);
        }

        public void UpdateStatus(string weather, bool status)
        {
            if (OnUpdateStatus == null) return;

            WeatherNotificationArgs args = new WeatherNotificationArgs(weather, status);
            OnUpdateStatus(this, args);
        }

        public string DebugWeatherOutput()
        {
            string s = "";
            s += $"Weather {WeatherType} is {IsWeatherVisible}, Progress: {WeatherInProgress}, Begin Time {BeginTime} to End Time {ExpirTime}.";

            return s;
        }

        public void MoveWeather()
        {
        }

        public void DrawWeather()
        {
        }

        public void SecondUpdate()
        {
        }

        public void CreateWeather()
        {
            //set the begin and end time
            SDVTime stormStart = new SDVTime(1150 + (ClimatesOfFerngill.Dice.Next(0, 230)));
            stormStart.ClampToTenMinutes();

            BeginTime = new SDVTime(stormStart);

            //control for more variance
            if (ClimatesOfFerngill.Dice.NextDouble() < ClimatesOfFerngill.WeatherOpt.MoreSevereThunderFrenzyOdds)
                stormStart.AddTime(ClimatesOfFerngill.Dice.Next(240, 380));
            else
                stormStart.AddTime(ClimatesOfFerngill.Dice.Next(30, 190));

            stormStart.ClampToTenMinutes();
            ExpirTime = new SDVTime(stormStart);
        }

        public void UpdateWeather()
        {
            if (WeatherBeginTime is null || WeatherExpirationTime is null)
                return;

            if (WeatherInProgress && !IsWeatherVisible)
            {
                IsThorAngry = true;
                UpdateStatus(WeatherType, true);
            }

            if (WeatherExpirationTime <= SDVTime.CurrentTime && IsWeatherVisible)
            {
                IsThorAngry = false;
                UpdateStatus(WeatherType, false);
            }

            if (IsWeatherVisible)
            {
               if (ClimatesOfFerngill.WeatherOpt.Verbose)
                    ClimatesOfFerngill.Logger.Log("Performing Lightning Strikes");
               for (int i = 0; i <= ClimatesOfFerngill.Dice.Next(3,12); i++)
               {
                    if (ClimatesOfFerngill.WeatherOpt.Verbose) ClimatesOfFerngill.Logger.Log($"Attempt #{i}");
                    PerformLightningStrike();
               }
            }
        }

        public void PerformLightningStrike()
        {
            Double diceRoll = ClimatesOfFerngill.Dice.NextDouble();

            if (diceRoll < 0.4 - Game1.player.team.AverageDailyLuck())
            {
                if (ClimatesOfFerngill.WeatherOpt.Verbose)
                    ClimatesOfFerngill.Logger.Log($"Condition 1 with {diceRoll}. There will be a strike attempt.");

                Farm.LightningStrikeEvent lightningStrikeEvent = new Farm.LightningStrikeEvent
                {
                    bigFlash = true
                };
                Farm locationFromName = Game1.getLocationFromName("Farm") as Farm;
                List<Vector2> source = new List<Vector2>();
                foreach (KeyValuePair<Vector2, SObject> keyValuePair in locationFromName.objects.Pairs)
                {
                    if (keyValuePair.Value.bigCraftable.Value && keyValuePair.Value.ParentSheetIndex == 9)
                        source.Add(keyValuePair.Key);
                }
                if (source.Count > 0)
                {
                    for (int index1 = 0; index1 < 2; ++index1)
                    {
                        Vector2 index2 = source.ElementAt(ClimatesOfFerngill.Dice.Next(source.Count));
                        if (locationFromName.objects[index2].heldObject.Value == null)
                        {
                            locationFromName.objects[index2].heldObject.Value = new SObject(787, 1, false, -1, 0);
                            locationFromName.objects[index2].MinutesUntilReady = 3000 - Game1.timeOfDay;
                            locationFromName.objects[index2].shakeTimer = 1000;
                            lightningStrikeEvent.createBolt = true;
                            lightningStrikeEvent.boltPosition = index2 * 64f + new Vector2(32f, 0.0f);
                            locationFromName.lightningStrikeEvent.Fire(lightningStrikeEvent);
                            return;
                        }
                    }
                }
                //make strikes more likely. Very more likely.
                if (ClimatesOfFerngill.Dice.NextDouble() >= 0.8 - Game1.player.team.AverageDailyLuck())
                    return;

                try
                {
                    KeyValuePair<Vector2, TerrainFeature> keyValuePair = locationFromName.terrainFeatures.Pairs.ElementAt(ClimatesOfFerngill.Dice.Next(locationFromName.terrainFeatures.Count()));
                    if (!(keyValuePair.Value is FruitTree))
                    {
                        int num = !(keyValuePair.Value is HoeDirt) || (keyValuePair.Value as HoeDirt).crop == null ? 0 : (!(bool)(keyValuePair.Value as HoeDirt).crop.dead.Value ? 1 : 0);
                        if (ClimatesOfFerngill.UseSafeLightningApi && !(ClimatesOfFerngill.SafeLightningAPI is null))
                        {
                            ClimatesOfFerngill.SafeLightningAPI.StrikeLightningSafely(keyValuePair.Key);
                        }
                        else if (keyValuePair.Value.performToolAction((Tool)null, 50, keyValuePair.Key, (GameLocation)locationFromName))
                        {
                            lightningStrikeEvent.destroyedTerrainFeature = true;
                            lightningStrikeEvent.createBolt = true;
                            locationFromName.terrainFeatures.Remove(keyValuePair.Key);
                            lightningStrikeEvent.boltPosition = keyValuePair.Key * 64f + new Vector2(32f, (float)sbyte.MinValue);
                        }
                        if (num != 0)
                        {
                            if (keyValuePair.Value is HoeDirt)
                            {
                                if ((keyValuePair.Value as HoeDirt).crop != null)
                                {
                                    if ((bool)((keyValuePair.Value as HoeDirt).crop.dead.Value))
                                    {
                                        lightningStrikeEvent.createBolt = true;
                                        lightningStrikeEvent.boltPosition = keyValuePair.Key * 64f + new Vector2(32f, 0.0f);
                                    }
                                }
                            }
                        }
                    }
                    else if (keyValuePair.Value is FruitTree)
                    {
                        (keyValuePair.Value as FruitTree).struckByLightningCountdown.Value = 4;
                        (keyValuePair.Value as FruitTree).shake(keyValuePair.Key, true, (GameLocation)locationFromName);
                        lightningStrikeEvent.createBolt = true;
                        lightningStrikeEvent.boltPosition = keyValuePair.Key * 64f + new Vector2(32f, (float)sbyte.MinValue);
                    }
                }
                catch (Exception)
                {
                }
                locationFromName.lightningStrikeEvent.Fire(lightningStrikeEvent);
            }
            else
            {
                if (ClimatesOfFerngill.Dice.NextDouble() >= 0.1)
                    return;
                (Game1.getLocationFromName("Farm") as Farm).lightningStrikeEvent.Fire(new Farm.LightningStrikeEvent()
                {
                    smallFlash = true
                });
            }
        }

        public void EndWeather()
        {
            if (IsWeatherVisible)
            {
                ExpirTime = new SDVTime(SDVTime.CurrentTime - 10);
                UpdateStatus(WeatherType, false);
            }
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwilightShards.Stardew.Common;

namespace RealisticWeatherEffects
{
    public class RWEConfig
    {
        public double PerRainIncreaseChance = .0265;
    }

    public class RealisticWeatherEffects : Mod
    {
        private static Integrations.IClimatesOfFerngillAPI ClimateAPI;
        private bool DoNothing;
        internal static RWEConfig ModConfig;

        public override void Entry(IModHelper helper)
        {
            ModConfig = Helper.ReadConfig<RWEConfig>();
            DoNothing = false;

            //load hooks
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private void OnGameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            ClimateAPI = SDVUtilities.GetModApi<Integrations.IClimatesOfFerngillAPI>(Monitor, Helper, "KoihimeNakamura.ClimatesOfFerngill", "1.5.0-beta.14");

            if (ClimateAPI is null)
            {
                Monitor.Log("This mod has encountered a error with the API being missing", LogLevel.Alert);
                DoNothing = true;
            }
        }

        private void OnDayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            if (DoNothing) return;
            Monitor.Log($"Testing API: {ClimateAPI.GetAmtOfRainSinceDay1()}, {ClimateAPI.GetNumDaysOfStreak()} and {ClimateAPI.GetCurrentWeatherStreak()}");
        }
    }
}

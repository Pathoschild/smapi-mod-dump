/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/RandomStartDay
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace RandomStartDay
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig config;

        public override void Entry(IModHelper helper)
        {
            this.config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.Specialized.LoadStageChanged += this.Specialized_LoadStageChanged;
        }

        private void Specialized_LoadStageChanged(object sender, LoadStageChangedEventArgs e)
        {

            if (e.NewStage == LoadStage.CreatedInitialLocations)
            {
                // exit method if allowed seasons have invalid value (other than spring, summer, fall, winter)
                for (int i = 0; i < config.allowedSeasons.Length; i++)
                {
                    switch (config.allowedSeasons[i])
                    {
                        case "spring":
                            break;
                        case "summer":
                            break;
                        case "fall":
                            break;
                        case "winter":
                            break;
                        default:
                            this.Monitor.Log("array \"allowedSeasons\" contains invalid value(s). Valid values are: \"spring\", \"summer\", \"fall\", \"winter\". This mod did NOT work.", LogLevel.Error);
                            return;
                    }
                }

                // randomize
                Random random = new Random((int)Game1.uniqueIDForThisGame);
                if (config.isRandomSeedUsed == false)
                {
                    // if not use seed, generate new 9 digits numbers
                    Random random2 = new Random();
                    random = new Random (random2.Next(999999999) +1);
                }
                int dom = random.Next(config.MaxOfDayOfMonth) + 1;
                string cs = config.allowedSeasons[random.Next(config.allowedSeasons.Length)];

                // apply
                Game1.dayOfMonth = dom;
                Game1.currentSeason = cs;
                // make sure outside not dark, for Dynamic Night Time
                Game1.timeOfDay = 1200;
            }
        }
    }
}

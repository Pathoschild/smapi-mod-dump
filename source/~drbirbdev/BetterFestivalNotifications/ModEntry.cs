/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using BirbShared.Mod;
using StardewModdingAPI;
using StardewValley;

namespace BetterFestivalNotifications
{
    public class ModEntry : Mod
    {
        [SmapiInstance]
        internal static ModEntry Instance;
        [SmapiConfig]
        internal static Config Config;
        [SmapiAsset]
        internal static Assets Assets;

        internal ITranslationHelper I18n => this.Helper.Translation;

        public override void Entry(IModHelper helper)
        {
            ModClass mod = new ModClass();
            mod.Parse(this);
            this.Helper.Events.GameLoop.TimeChanged += this.GameLoop_TimeChanged;
        }

        private void GameLoop_TimeChanged(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            string key = Game1.currentSeason + Game1.Date.DayOfMonth;
            if (Assets.FestivalNotifications.ContainsKey(key))
            {
                List<int> alertTimes = Assets.FestivalNotifications[key];
                for (int i = 0; i < alertTimes.Count; i++)
                {
                    if (alertTimes[i] == e.NewTime)
                    {
                        if (i == 0)
                        {
                            if (Config.PlayStartSound)
                            {
                                Game1.playSound(Config.StartSound);
                            }
                        }
                        else if (i > 1 && alertTimes.Count > 2)
                        {
                            if (Config.PlayOverSound)
                            {
                                Game1.playSound(Config.OverSound);
                            }
                            if (Config.ShowOverNotification)
                            {
                                Game1.showGlobalMessage(I18n.Get("festivalOver"));
                            }
                        }
                        else
                        {
                            if (Config.PlayWarnSound)
                            {
                                Game1.playSound(Config.WarnSound);
                            }
                            if (Config.ShowWarnNotification)
                            {
                                Game1.showGlobalMessage(I18n.Get("festivalWarn"));
                            }
                        }
                    }
                }
            }
        }
    }
}

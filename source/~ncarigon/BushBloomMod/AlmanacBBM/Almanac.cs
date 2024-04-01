/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using Leclair.Stardew.Almanac.Models;
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common;
using Leclair.Stardew.Almanac;
using SObject = StardewValley.Object;

namespace BushBloomMod.Patches.Integrations {
    internal static class Almanac {
        private static readonly Api api = new();
        private static Translation translation = null!;
        private static Config Config = null!;

        public static void Register(IManifest manifest, IModHelper helper, IMonitor monitor, Config config) {
            Config = config;
            helper.Events.GameLoop.GameLaunched += (s, e) => {
                var api = helper.ModRegistry.GetApi<IAlmanacAPI>("leclair.almanac");
                if (api is not null) {
                    translation = ModEntry.Instance.Helper.Translation.Get("page.notices.season");
                    api.SetNoticesHook(manifest, new Func<int, WorldDate, IEnumerable<IRichEvent>>(GetBushNotices));
                    monitor.Log($"Set notices hook for Almanac integration.", LogLevel.Debug);
                }
            };
        }

        private static IEnumerable<IRichEvent> GetBushNotices(int seed, WorldDate date) {
            if (Config.EnableALMIntegration) {
                foreach (var sched in api.GetActiveSchedules(date.Season, date.DayOfMonth)) {
                    var item = new SObject(sched.Item1, 1);
                    var first_day = sched.Item2 == date;
                    yield return new RichEvent(
                        null,
                        first_day ?
                            FlowHelper.Translate(
                                translation,
                                new {
                                    item = item.DisplayName,
                                    start = new SDate(sched.Item2.DayOfMonth, sched.Item2.Season, sched.Item2.Year).ToLocaleString(withYear: false),
                                    end = new SDate(sched.Item3.DayOfMonth, sched.Item3.Season, sched.Item3.Year).ToLocaleString(withYear: false),
                                },
                                align: Alignment.Middle
                            ) : null,
                        SpriteHelper.GetSprite(item),
                        item
                    );
                }
            }
        }
    }
}
/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System;

namespace ChangeFarmCaves
{
    internal class ModEntry : Mod
    {
        internal static IModHelper IHelper;
        internal static IMonitor IMonitor;
        internal static ITranslationHelper ITranslations;
        internal static Config IConfig;

        public override void Entry(IModHelper helper)
        {
            IHelper = Helper;
            IMonitor = Monitor;
            ITranslations = Helper.Translation;
            IConfig = Helper.ReadConfig<Config>();

            Helper.Events.GameLoop.GameLaunched += (s, e) => Patcher.Patch(helper);
            Helper.Events.GameLoop.DayStarted += (s, e) =>
            {
                if (!Game1.IsMasterGame) return;
                var farmCave = Game1.getLocationFromName("FarmCave");
                string data = farmCave.modData.ContainsKey("ChangeFarmCaves.ShouldChange") ? farmCave.modData["ChangeFarmCaves.ShouldChange"] : null;
                if (data is not null)
                {
                    new Event().answerDialogue(data.Split(',')[0], Convert.ToInt32(data.Split(',')[1]));
                    farmCave.modData.Remove("ChangeFarmCaves.ShouldChange");
                }
            };
        }
    }
}

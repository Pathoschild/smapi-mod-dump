/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/millerscout/StardewMillerMods
**
*************************************************/

using RetroactiveStardew;
using StardewModdingAPI;

namespace AchievementGetter
{
    public class ModEntry : Mod
    {
        internal ModConfig Config { get; set; }
        private RetroAchievements RetroAchievement;
        private RetroSpecials RetroSpecials;
        private RetroMail RetroMail;

        public override void Entry(IModHelper helper)
        {
            this.Config = helper.ReadConfig<ModConfig>();

            this.RetroAchievement = new RetroAchievements(helper, this.Monitor, this.Config);
            this.RetroSpecials = new RetroSpecials(helper, this.Monitor, this.Config);
            this.RetroMail = new RetroMail(helper, this.Monitor, this.Config);

            helper.Events.GameLoop.DayStarted += this.RetroAchievement.DayStarted;
            helper.Events.GameLoop.DayStarted += this.RetroSpecials.DayStarted;
            helper.Events.GameLoop.DayStarted += this.RetroMail.DayStarted;
        }
    }
}

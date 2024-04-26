/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ncarigon/StardewValleyMods
**
*************************************************/

using BushBloomMod.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace BushBloomMod {
    internal sealed class ModEntry : Mod {
        public static ModEntry Instance { get; private set; }

        public Configuration Config;

        public override void Entry(IModHelper helper) {
            Instance = this;
            Config = Configuration.Register(helper);
            Bushes.Register();
            //Almanac.Register(this.ModManifest);
            helper.Events.GameLoop.GameLaunched += (_, _) => Schedule.ReloadSchedules();
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.Content.AssetRequested += Schedule.Content_AssetRequested;
        }

        // INFO: We need to manually re-trigger bush.dayupdate() in order to allow CP to apply
        // content updates which would normally only happen after the base game has already
        // called that function.
        [EventPriority(EventPriority.Low)]
        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e) {
            // find all bushes and update for real
            Bushes.UpdateAllBushes();
        }

        public override object GetApi() => new Api();
    }
}
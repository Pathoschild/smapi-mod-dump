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
using BushBloomMod.Patches.Integrations;
using StardewModdingAPI;

namespace BushBloomMod {
    internal sealed class ModEntry : Mod {
        public override void Entry(IModHelper helper) {
            var config = Config.Register(helper);
            Bushes.Register(helper, this.Monitor, config);
            Schedule.Register(helper, this.Monitor, config);
            //Almanac.Register(this.ModManifest, helper, this.Monitor, config);
            Automate.Register(this.ModManifest, helper, this.Monitor, config);
            helper.Events.GameLoop.GameLaunched += (s, e) => Schedule.ReloadEntries();
        }

        public override object GetApi() => new Api();
    }
}
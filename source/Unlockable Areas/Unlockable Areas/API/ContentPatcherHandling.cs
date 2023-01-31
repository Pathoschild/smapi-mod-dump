/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-unlockable-areas
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContentPatcher;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Unlockable_Areas.Lib;

namespace Unlockable_Areas.API
{
    public class ContentPatcherHandling
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;
        public static ContentPatcherHandling Singleton;

        public bool IsConditionsApiReady => throw new NotImplementedException();

        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;

            if (Helper.ModRegistry.IsLoaded("Pathoschild.ContentPatcher"))
                Helper.Events.GameLoop.GameLaunched += gameLaunched;
        }

        private static void gameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var api = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            api.RegisterToken(Mod.ModManifest, "UnlockablePurchased", getValue);
        }

        public static IEnumerable<string> getValue()
        {
            if (!Context.IsWorldReady)
                yield break;

            if (SaveDataEvents.Data != null)
                foreach (var e in SaveDataEvents.Data.UnlockablePurchased)
                    if (e.Value == true)
                        yield return e.Key;
        }
    }
}
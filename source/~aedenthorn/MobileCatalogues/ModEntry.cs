/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace MobileCatalogues
{
    public class ModEntry : Mod
    {
        public static ModEntry context;

        internal static ModConfig Config;

        public static IMobilePhoneApi api;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            context = this;
            Config = Helper.ReadConfig<ModConfig>();
            if (!Config.EnableMod)
                return;

            HelperEvents.Initialize(Helper, Monitor, Config);
            Catalogues.Initialize(Helper, Monitor, Config);
            CataloguesApp.Initialize(Helper, Monitor, Config);
            Visuals.Initialize(Helper, Monitor, Config);

            Helper.Events.GameLoop.GameLaunched += HelperEvents.GameLoop_GameLaunched;
        }

    }
}

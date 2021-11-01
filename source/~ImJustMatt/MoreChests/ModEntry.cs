/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace MoreChests
{
    using Common.Helpers;
    using Common.Integrations.MoreChests;
    using Common.Services;
    using Services;
    using StardewModdingAPI;

    public class ModEntry : Mod
    {
        internal const string ModPrefix = "MoreChests";
        private IMoreChestsAPI _api;

        internal ServiceManager ServiceManager { get; private set; }

        /// <inheritdoc />
        public override void Entry(IModHelper helper)
        {
            // Init
            Log.Init(this.Monitor);
            this.ServiceManager = new(this.Helper, this.ModManifest);
            this._api = new MoreChestsAPI(this);

            // Services
            this.ServiceManager.Create(new []
            {
                typeof(AssetHandler),
                typeof(ContentPackLoader),
                typeof(CustomChestManager),
                typeof(InventoryHandler),
                typeof(ModConfigService),
            });
        }

        /// <inheritdoc />
        public override object GetApi()
        {
            return this._api;
        }
    }
}
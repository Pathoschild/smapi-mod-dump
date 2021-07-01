/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pepoluan/StackSplitRedux
**
*************************************************/

namespace StackSplitRedux
    {
    internal class Mod : StardewModdingAPI.Mod
        {
        /// <summary>
        /// These Convenience Properties are here so we don't have to keep passing a ref to Helper as params.
        /// </summary>
        #region Convenience Properties
        internal static StardewModdingAPI.Mod Instance;
        internal static StardewModdingAPI.ITranslationHelper I18n { get => Instance.Helper.Translation; }
        internal static StardewModdingAPI.IReflectionHelper Reflection { get => Instance.Helper.Reflection; }
        internal static StardewModdingAPI.IInputHelper Input { get => Instance.Helper.Input; }
        internal static StardewModdingAPI.Events.IModEvents Events { get => Instance.Helper.Events; }
        internal static StardewModdingAPI.IModRegistry Registry { get => Instance.Helper.ModRegistry; }
        internal static ModConfig Config;
        #endregion

        private static StackSplit StackSplitRedux;

        public override void Entry(StardewModdingAPI.IModHelper helper) {
            Mod.Instance = this;

            if (DetectConflict()) return;

            Log.Info($"{this.ModManifest.UniqueID} version {typeof(Mod).Assembly.GetName().Version} (API version {API.Version}) is loading...");
            Mod.Config = helper.ReadConfig<ModConfig>();
            Mod.StackSplitRedux = new();
            }

        public override object GetApi() {
            return new API(Mod.StackSplitRedux);
            }

        public bool DetectConflict() {
            bool conflict = false;
            foreach (var mID in StaticConfig.ConflictingMods) {
                if (Mod.Registry.IsLoaded(mID)) {
                    Log.Alert($"{mID} detected!");
                    conflict = true;
                    }
                }
            if (conflict) {
                Log.Error("Conflicting mods detected! Will abort loading this mod to prevent conflict/issues!");
                Log.Error("Please upload the log to https://smapi.io/log and tell pepoluan about this!");
                }
            return conflict;
            }
        }
    }

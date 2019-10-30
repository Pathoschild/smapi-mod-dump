using StardewModdingAPI;

namespace JoysOfEfficiency.Core
{
    /// <summary>
    /// This class holds mod and config instance and exposes some useful methods.
    /// </summary>
    internal class InstanceHolder
    {
        private static ModEntry ModInstance { get; set; }

        public static Config Config { get; private set; }
        public static IMonitor Monitor => ModInstance.Monitor;
        private static IModHelper Helper => ModInstance.Helper;
        public static ITranslationHelper Translation => Helper.Translation;
        public static IReflectionHelper Reflection => Helper.Reflection;

        /// <summary>
        /// Sets mod's entry　point and configuration instance. 
        /// </summary>
        /// <param name="modInstance">the mod instance</param>
        /// <param name="conf">the configuration instance</param>
        public static void Init(ModEntry modInstance, Config conf)
        {
            ModInstance = modInstance;
            Config = conf;
        }
        
        /// <summary>
        /// Writes settings to '(ModFolder)/config.json'.
        /// </summary>

        public static void WriteConfig()
        {
            Helper?.WriteConfig(Config);
        }
    }
}

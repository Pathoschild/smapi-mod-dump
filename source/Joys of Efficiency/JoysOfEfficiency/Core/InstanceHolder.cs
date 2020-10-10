/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace JoysOfEfficiency.Core
{
    /// <summary>
    /// This class holds mod and config instance and exposes some useful methods.
    /// </summary>
    internal class InstanceHolder
    {
        private static ModEntry ModInstance { get; set; }

        public static Config Config { get; private set; }
        private static IModHelper Helper => ModInstance.Helper;
        public static ITranslationHelper Translation => Helper.Translation;
        public static IReflectionHelper Reflection => Helper.Reflection;
        public static Multiplayer Multiplayer => Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();

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

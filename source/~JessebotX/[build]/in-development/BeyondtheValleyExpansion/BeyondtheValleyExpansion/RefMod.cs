using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeyondTheValleyExpansion.Framework;
using StardewModdingAPI;

namespace BeyondTheValleyExpansion
{
    /// <summary>
    /// SMAPI interfaces and specific mod references
    /// </summary>
    class RefMod
    {
        /********* 
         ** SMAPI fields
         *********/
        /// <summary> provides simplified API's for writing mods </summary>
        public static IModHelper ModHelper;
        /// <summary> encapsulates monitoring and logging for a given module</summary>
        public static IMonitor ModMonitor;
        /// <summary> provides translations stored in the mods i18n folder</summary>
        public static ITranslationHelper i18n;
        /// <summary> describes this mod's manifest </summary>
        public static IManifest ModManifest;

        /*********
         ** Mod-specific
         *********/
        /// <summary> how many content packs are installed </summary>
        public static int contentPacksInstalled;
        /// <summary> the mod's configuration file </summary>
        public static ModConfig Config;
    }
}

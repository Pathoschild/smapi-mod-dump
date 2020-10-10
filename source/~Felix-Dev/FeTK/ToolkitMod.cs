/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Felix-Dev/StardewMods
**
*************************************************/

using FelixDev.StardewMods.FeTK.Framework.Services;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelixDev.StardewMods.FeTK
{
    /// <summary>The mod entry point.</summary>
    internal class ToolkitMod : Mod
    {
        /// <summary>Provides access to the simplified APIs for writing mods provided by SMAPI.</summary>
        public static IModHelper ModHelper { get; private set; }

        /// <summary>Provides access to the <see cref="IMonitor"/> API provided by SMAPI.</summary>
        public static IMonitor _Monitor { get; private set; }

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            _Monitor = this.Monitor;

            ServiceFactory.Setup(new MailManager());
        }
    }
}

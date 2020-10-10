/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

namespace Entoarox.Framework.Core
{
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>If extra, possibly somewhat cheaty console commands should be enabled.</summary>
        public bool TrainerCommands = true;

        /// <summary>If every location should be treated like the greenhouse when it comes to crops.</summary>
        public bool GreenhouseEverywhere = false;
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace SatoCore
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The singleton instance of <see cref="ModEntry"/>.</summary>
        public static ModEntry Instance { get; private set; }


        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
        }
    }
}

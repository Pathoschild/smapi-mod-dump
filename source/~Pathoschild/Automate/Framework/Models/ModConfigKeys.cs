/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using Pathoschild.Stardew.Common.Input;

namespace Pathoschild.Stardew.Automate.Framework.Models
{
    /// <summary>A set of parsed key bindings.</summary>
    internal class ModConfigKeys
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The keys which toggle the automation overlay.</summary>
        public KeyBinding ToggleOverlay { get; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="toggleOverlay">The keys which toggle the automation overlay.</param>
        public ModConfigKeys(KeyBinding toggleOverlay)
        {
            this.ToggleOverlay = toggleOverlay;
        }
    }
}

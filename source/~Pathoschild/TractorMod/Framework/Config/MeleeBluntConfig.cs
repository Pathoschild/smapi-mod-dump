/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

#nullable disable

namespace Pathoschild.Stardew.TractorMod.Framework.Config
{
    /// <summary>Configuration for the melee blunt weapon attachment.</summary>
    internal class MeleeBluntConfig
    {
        /// <summary>Whether to attack monsters.</summary>
        public bool AttackMonsters { get; set; } = false;

        /// <summary>Whether to break containers in the mine.</summary>
        public bool BreakMineContainers { get; set; } = true;
    }
}

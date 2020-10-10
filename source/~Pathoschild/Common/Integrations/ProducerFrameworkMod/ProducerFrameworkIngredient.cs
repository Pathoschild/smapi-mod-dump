/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.Common.Integrations.ProducerFrameworkMod
{
    /// <summary>Metadata about an input ingredient for a Producer Framework Mod machine.</summary>
    internal class ProducerFrameworkIngredient
    {
        /// <summary>The ID for the input ingredient, or <c>null</c> for a context tag ingredient.</summary>
        public int? InputId { get; set; }

        /// <summary>The number of the ingredient needed.</summary>
        public int Count { get; set; }
    }
}

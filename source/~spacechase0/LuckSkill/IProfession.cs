/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace LuckSkill
{
    /// <summary>A luck skill profession.</summary>
    public interface IProfession
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The unique profession ID in <see cref="Farmer.professions"/>.</summary>
        int Id { get; }

        /// <summary>The default English display name.</summary>
        string DefaultName { get; }

        /// <summary>The translated display name.</summary>
        string Name { get; }

        /// <summary>The default description text.</summary>
        string DefaultDescription { get; }

        /// <summary>The translated description text.</summary>
        string Description { get; }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using HarmonyLib;

namespace DeluxeJournal.Patching
{
    /// <summary>A set of Harmony patches.</summary>
    internal interface IPatch
    {
        /// <summary>The name of this instance.</summary>
        string Name { get; }

        /// <summary>Apply Harmony patches.</summary>
        /// <param name="harmony">Harmony instance.</param>
        void Apply(Harmony harmony);
    }
}

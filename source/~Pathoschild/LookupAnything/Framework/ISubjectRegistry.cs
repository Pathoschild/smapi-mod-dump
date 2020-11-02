/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using Pathoschild.Stardew.LookupAnything.Framework.Lookups;

namespace Pathoschild.Stardew.LookupAnything.Framework
{
    /// <summary>A central registry for subject lookups.</summary>
    internal interface ISubjectRegistry
    {
        /*********
        ** Methods
        *********/
        /// <summary>Get the subject for an in-game entity, if available.</summary>
        /// <param name="entity">The entity instance.</param>
        ISubject GetByEntity(object entity);
    }
}

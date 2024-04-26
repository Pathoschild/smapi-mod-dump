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

namespace Pathoschild.Stardew.LookupAnything.Framework.Fields
{
    /// <summary>A field which links to another entry.</summary>
    internal interface ILinkField : ICustomField
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Get the subject the link points to, or <c>null</c> to stay on the current subject.</summary>
        ISubject? GetLinkSubject();
    }
}

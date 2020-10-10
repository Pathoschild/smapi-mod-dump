/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace StackEverything.ObjectCopiers
{
    /// <summary>Copies objects.</summary>
    internal interface ICopier<T>
    {
        /*********
        ** Methods
        *********/

        /// <summary>Copy the given object.</summary>
        /// <param name="obj">The object to copy</param>
        /// <returns>A copy of the object.</returns>
        T Copy(T obj);
    }
}

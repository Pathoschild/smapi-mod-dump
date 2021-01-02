/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/quicksilverfox/StardewMods
**
*************************************************/

namespace EmptyHands.Framework
{
    /// <summary>The input mapping configuration.</summary>
    /// <typeparam name="T">The control type.</typeparam>
    internal class InputMapConfiguration<T>
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The control which rotates the toolbar up (i.e. show the previous inventory row).</summary>
        public T SetToNothing { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Get whether the specified key is valid.</summary>
        /// <param name="key">The key to check.</param>
        public bool IsValidKey(T key)
        {
            return key != null && !key.Equals(default(T));
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

namespace GeodeInfoMenu.Menus
{
    /// <summary>Represents a menu that has a scrollbar.</summary>
    internal interface IScrollableMenu
    {
        /// <summary>Set the current scrollbar index</summary>
        /// <param name="index">Which index to use</param>
        void SetCurrentIndex(int index);

        /// <summary>Get the current scrollbar index</summary>
        /// <returns>The index of the scrollbar</returns>
        int GetCurrentIndex();
    }
}

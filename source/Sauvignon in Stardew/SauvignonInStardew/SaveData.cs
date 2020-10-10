/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JDvickery/Sauvignon-in-Stardew
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SauvignonInStardew
{
    /// <summary>The model for mod data stored in the save file.</summary>
    internal class SaveData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The tile coordinates for winery buildings.</summary>
        public List<Point> WineryCoords { get; set; } = new List<Point>();
    }
}

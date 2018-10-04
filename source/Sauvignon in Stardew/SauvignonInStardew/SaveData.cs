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

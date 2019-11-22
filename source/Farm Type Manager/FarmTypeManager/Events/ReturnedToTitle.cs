using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Tasks performed when the player returns to the title screen from an active game session.</summary>
        private void ReturnedToTitle(object sender, EventArgs e)
        {
            Utility.FarmDataList = new List<FarmData>(); //clear this list to avoid any possible errors caused by a previous farm's data
        }
    }
}

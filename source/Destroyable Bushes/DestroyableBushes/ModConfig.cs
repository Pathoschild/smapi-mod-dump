using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Monsters;
using Harmony;
using System.Diagnostics;

namespace DestroyableBushes
{
    /// <summary>A collection of this mod's config.json file settings.</summary>
    public class ModConfig
    {
        /// <summary>If true, all bushes should be made destroyable. If false, only bushes at the locations in <see cref="DestroyableBushLocations"/> should be made destroyable.</summary>
        public bool AllBushesAreDestroyable { get; set; } = true;
        /// <summary>A list of in-game locations where bushes should be made destroyable. If <see cref="AllBushesAreDestroyable"/> is true, this list is not used.</summary>
        public List<string> DestroyableBushLocations { get; set; } = new List<string>();
        /// <summary>The number of wood pieces dropped by each type of bush when destroyed.</summary>
        public AmountOfWoodDropped AmountOfWoodDropped { get; set; } = new AmountOfWoodDropped();
    }

    /// <summary>A group of config.json file settings. Sets the number of wood pieces dropped by each type of bush when destroyed.</summary>
    public class AmountOfWoodDropped
    {
        public int SmallBushes { get; set; } = 2;
        public int MediumBushes { get; set; } = 4;
        public int LargeBushes { get; set; } = 8;
        public int GreenTeaBushes { get; set; } = 0;
    }

}

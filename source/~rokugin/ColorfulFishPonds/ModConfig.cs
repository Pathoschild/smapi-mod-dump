/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/rokugin/StardewMods
**
*************************************************/

namespace ColorfulFishPonds {
    internal class ModConfig {

        public bool ModEnabled { get; set; } = true;
        public bool DyeColors { get; set; } = true;
        public int RequiredPopulation { get; set; } = 2;
        public bool DisableSingleRecolors { get; set; } = false;
        public bool Debugging { get; set; } = false;
        public float PrismaticSpeed { get; set; } = 0.3f;

    }
}

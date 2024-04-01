/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alanperrow/StardewModding
**
*************************************************/

namespace FasterPathSpeed
{
    // Note: These comments correspond to the variable name for that flooring type in SDV source code.
    public class CustomPathSpeedBuffValues
    {
        // boardwalk
        public float WoodPath { get; set; } = 1f;

        // wood
        public float WoodFloor { get; set; } = 1f;

        // plankFlooring
        public float RusticPlankFloor { get; set; } = 1f;

        // ghost
        public float WeatheredFloor { get; set; } = 1f;

        // straw
        public float StrawFloor { get; set; } = 1f;

        // gravel
        public float GravelPath { get; set; } = 1f;

        // cobblestone
        public float CobblestonePath { get; set; } = 1f;

        // steppingStone
        public float SteppingStonePath { get; set; } = 1f;

        // stone
        public float StoneFloor { get; set; } = 1f;

        // townFlooring
        public float StoneWalkwayFloor { get; set; } = 1f;

        // brick
        public float BrickFloor { get; set; } = 1.25f;

        // colored_cobblestone
        public float CrystalPath { get; set; } = 1.5f;

        // iceTile
        public float CrystalFloor { get; set; } = 1.5f;
    }
}

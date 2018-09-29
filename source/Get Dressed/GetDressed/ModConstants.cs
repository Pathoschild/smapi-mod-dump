using System.IO;
using StardewModdingAPI;
using StardewValley;

namespace GetDressed
{
    /// <summary>Internal constants used throughout the mod code.</summary>
    internal static class ModConstants
    {
        /// <summary>The relative path to the current per-save config file, or <c>null</c> if the save isn't loaded yet.</summary>
        public static string PerSaveConfigPath => Constants.SaveFolderName != null
            ? Path.Combine("psconfigs", $"{Constants.SaveFolderName}.json")
            : null;

        /// <summary>The game's current zoom level.</summary>
        public static float ZoomLevel = Game1.options.zoomLevel;

        /// <summary>The maximum number of favorites.</summary>
        public const int MaxFavorites = 37;

        /// <summary>The texture heights of shoes in the female overrides.</summary>
        public static readonly int[] FemaleShoeSpriteHeights = new int[21] { 15, 16, 14, 13, 12, 16, 16, 15, 16, 10, 13, 13, 13, 14, 14, 11, 14, 14, 14, 16, 13 };

        /// <summary>The texture heights of shoes in the male overrides.</summary>
        public static readonly int[] MaleShoeSpriteHeights = new int[21] { 11, 16, 15, 14, 13, 16, 16, 14, 16, 12, 14, 14, 15, 15, 16, 13, 15, 16, 16, 16, 15 };
    }
}

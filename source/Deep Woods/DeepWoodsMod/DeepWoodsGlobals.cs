
using Microsoft.Xna.Framework;
using xTile.Dimensions;
using static DeepWoodsMod.DeepWoodsRandom;

namespace DeepWoodsMod
{
    class DeepWoodsGlobals
    {
        public readonly static string WOODS_OBELISK_BUILDING_NAME = "Woods Obelisk";
        public readonly static string EARTH_OBELISK_BUILDING_NAME = "Earth Obelisk";

        public readonly static string SAVE_FILE_NAME = "DeepWoodsModSave";

        public readonly static string DEFAULT_OUTDOOR_TILESHEET_ID = "DefaultOutdoor";
        public readonly static string LAKE_TILESHEET_ID = "WaterBorderTiles";

        public readonly static Location WOODS_WARP_LOCATION = new Location(26, 31);
        public readonly static int NUM_TILES_PER_LIGHTSOURCE = 16;
        public readonly static int MINIMUM_TILES_FOR_BAUBLE = 16;
        public readonly static int MINIMUM_TILES_FOR_LEAVES = 16;
        public readonly static int MINIMUM_TILES_FOR_MONSTER = 16;
        public readonly static int MINIMUM_TILES_FOR_TERRAIN_FEATURE = 4;
        public readonly static int NUM_MONSTER_SPAWN_TRIES = 4;
        public readonly static Chance CHANCE_FOR_WATER_LILY = new Chance(8);
        public readonly static Chance CHANCE_FOR_BLOSSOM_ON_WATER_LILY = new Chance(30);
        public readonly static int TIME_BEFORE_DELETION_ALLOWED = 100;
        public readonly static string UNIQUE_NAME_FOR_EASTER_EGG_ITEMS = "DeepWoodsModEasterEggItemIHopeThisNameIsUniqueEnoughToNotMessOtherModsUpw5365r6zgdhrt6u";
        public readonly static string WOODS_OBELISK_WIZARD_MAIL_ID = "DeepWoodsModWoodsObeliskMailFromWizarddawfafb735h";

        public readonly static Color DAY_LIGHT = new Color(150, 120, 50, 255);
        public readonly static Color NIGHT_LIGHT = new Color(255, 255, 50, 255);

        public readonly static int EASTER_EGG_REPLACEMENT_ITEM = 305;   // Void Egg

        public static class MessageId
        {
            public const string RequestMetadata = nameof(MessageId.RequestMetadata);
            public const string Metadata = nameof(MessageId.Metadata);
            public const string RequestWarp = nameof(MessageId.RequestWarp);
            public const string Warp = nameof(MessageId.Warp);
            public const string SetLowestLevelReached = nameof(MessageId.SetLowestLevelReached);
            public const string SetUnicornStardropReceived = nameof(MessageId.SetUnicornStardropReceived);
            public const string AddLocation = nameof(MessageId.AddLocation);
            public const string RemoveLocation = nameof(MessageId.RemoveLocation);
        }
    }
}

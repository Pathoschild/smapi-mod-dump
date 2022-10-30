/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/DeepWoodsMod
**
*************************************************/

using Microsoft.Xna.Framework;

namespace DeepWoodsMod
{
    public class DeepWoodsGlobals
    {
        public readonly static string WOODS_OBELISK_BUILDING_NAME = "Woods Obelisk";
        public readonly static string EARTH_OBELISK_BUILDING_NAME = "Earth Obelisk";

        public readonly static string SAVE_FILE_NAME = "DeepWoodsModSave";

        public readonly static string DEFAULT_OUTDOOR_TILESHEET_ID = "DefaultOutdoor";
        public readonly static string INFESTED_OUTDOOR_TILESHEET_ID = "InfestedOutdoor";
        public readonly static string LAKE_TILESHEET_ID = "WaterBorderTiles";

        public readonly static string UNIQUE_NAME_FOR_EASTER_EGG_ITEMS = "DeepWoodsModEasterEggItemIHopeThisNameIsUniqueEnoughToNotMessOtherModsUpw5365r6zgdhrt6u";
        public readonly static string WOODS_OBELISK_WIZARD_MAIL_ID = "DeepWoodsModWoodsObeliskMailFromWizarddawfafb735h";

        public readonly static int EASTER_EGG_REPLACEMENT_ITEM = 305;   // Void Egg

        public readonly static int TIME_BEFORE_DELETION_ALLOWED = 100;

        public readonly static Color DAY_LIGHT = new(150, 120, 50, 255);
        public readonly static Color NIGHT_LIGHT = new(255, 255, 50, 255);

        // The maximum possible luck level in vanilla SDV is 8: 5 from Magic Rock Candy, 1 from Ginger Ale, and 2 from two Lucky Rings.
        public readonly static int MAXIMUM_POSSIBLE_LUCKLEVEL = 8;

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

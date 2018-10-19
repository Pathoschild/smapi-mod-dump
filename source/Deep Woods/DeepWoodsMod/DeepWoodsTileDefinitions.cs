
using static DeepWoodsMod.DeepWoodsRandom;

namespace DeepWoodsMod
{
    class DeepWoodsTileDefinitions
    {
        public static readonly int DEBUG_PINK = 68;
        public static readonly int DEBUG_DARK = 115;
        public static readonly int DEBUG_YELLOW = 226;
        public static readonly int DEBUG_RED = 292;

        public static readonly int PLAIN_FOREST_BACKGROUND = 946;

        public static readonly WeightedInt[] FOREST_BACKGROUND = new WeightedInt[] {
            new WeightedInt(PLAIN_FOREST_BACKGROUND, 80),
            new WeightedInt(971, 10),
            new WeightedInt(996, 10)
        };

        public static readonly int FOREST_ROW_TREESTUMP_LEFT = 1144;
        public static readonly int FOREST_ROW_TREESTUMP_RIGHT = 1145;

        public static readonly int[] WATER_LILY = new int[] { 1293, 1294, 1295, 1296 };
        public static readonly int[] WATER_LILY_WITH_BLOSSOM = new int[] { 1318, 1319, 1320, 1321 };
        public static readonly int WATER_LILY_SHADOW = 1299;
        public static readonly int[] WATER_LILY_FRAMERATES = new int[] { 400, 450, 500, 550, 600 };

        public static readonly WeightedInt[] WATER_TILES = new WeightedInt[] {
            // Normal
            new WeightedInt(1246, 100),
            new WeightedInt(1271, 100),
            new WeightedInt(1274, 100),
            // Stones
            new WeightedInt(1247, 10),
            new WeightedInt(1248, 10),
            new WeightedInt(1249, 10),
            new WeightedInt(1272, 10),
            new WeightedInt(1273, 10),
            new WeightedInt(1324, 10),
            // Shells
            // new WeightedValue(1297, 20),
            // Clam
            // new WeightedValue(1298, 20),
            // Plants
            new WeightedInt(1322, 50),
            new WeightedInt(1323, 50),

        };

        public class GrassTiles
        {
            public static readonly int BLACK = 1094;

            public static readonly WeightedInt[] DARK = new WeightedInt[] {
                new WeightedInt(380, 80),
                new WeightedInt(156, 20)
            };

            public static readonly WeightedInt[] BRIGHT = new WeightedInt[] {
                new WeightedInt(175, 100),
                new WeightedInt(275, 100),
                new WeightedInt(402, 100),
                new WeightedInt(400, 15),
                new WeightedInt(401, 15),
                new WeightedInt(150, 15),
                new WeightedInt(254, 5),
                new WeightedInt(255, 5),
                new WeightedInt(256, 5)
            };

            public static readonly WeightedInt[] NORMAL = new WeightedInt[] {
                new WeightedInt(351, 100),
                new WeightedInt(300, 10),
                new WeightedInt(304, 10),
                new WeightedInt(305, 10),
                new WeightedInt(329, 1)
            };
        }

        public class DeepWoodsLichtungTileMatrix
        {
            public int HORIZONTAL;
            public int VERTICAL;
            public int CONCAVE_CORNER;
            public int CONVEX_CORNER;
            public int INVERSE_CONVEX_CORNER;
            public int VERTICAL_TO_STEEP;
            public int STEEP_TO_HORIZONTAL;

            public int[] WATER_HORIZONTAL;
            public int[] WATER_VERTICAL;
            public int WATER_CONCAVE_CORNER;
            public int WATER_CONVEX_CORNER;

            public DeepWoodsLichtungTileMatrix() { }

            public static readonly DeepWoodsLichtungTileMatrix LEFT_TO_TOP = new DeepWoodsLichtungTileMatrix()
            {
                HORIZONTAL = 376,
                VERTICAL = 352,
                CONCAVE_CORNER = 377,
                CONVEX_CORNER = 353,
                INVERSE_CONVEX_CORNER = 378,
                VERTICAL_TO_STEEP = 331,
                STEEP_TO_HORIZONTAL = 278,

                WATER_HORIZONTAL = new int[] { 1, 2 },
                WATER_VERTICAL = new int[] { 8, 16 },
                WATER_CONCAVE_CORNER = 0,
                WATER_CONVEX_CORNER = 33,
            };

            public static readonly DeepWoodsLichtungTileMatrix TOP_TO_RIGHT = new DeepWoodsLichtungTileMatrix()
            {
                HORIZONTAL = 350,
                VERTICAL = 376,
                CONCAVE_CORNER = 375,
                CONVEX_CORNER = 403,
                INVERSE_CONVEX_CORNER = 353,
                VERTICAL_TO_STEEP = 276,
                STEEP_TO_HORIZONTAL = 311,

                WATER_HORIZONTAL = new int[] { 11, 19 },
                WATER_VERTICAL = new int[] { 1, 2 },
                WATER_CONCAVE_CORNER = 3,
                WATER_CONVEX_CORNER = 32,
            };

            public static readonly DeepWoodsLichtungTileMatrix RIGHT_TO_BOTTOM = new DeepWoodsLichtungTileMatrix()
            {
                HORIZONTAL = 326,
                VERTICAL = 350,
                CONCAVE_CORNER = 325,
                CONVEX_CORNER = 328,
                INVERSE_CONVEX_CORNER = 403,
                VERTICAL_TO_STEEP = 261,
                STEEP_TO_HORIZONTAL = 301,

                WATER_HORIZONTAL = new int[] { 25, 26 },
                WATER_VERTICAL = new int[] { 11, 19 },
                WATER_CONCAVE_CORNER = 27,
                WATER_CONVEX_CORNER = 34,
            };

            public static readonly DeepWoodsLichtungTileMatrix BOTTOM_TO_LEFT = new DeepWoodsLichtungTileMatrix()
            {
                HORIZONTAL = 352,
                VERTICAL = 326,
                CONCAVE_CORNER = 327,
                CONVEX_CORNER = 378,
                INVERSE_CONVEX_CORNER = 328,
                VERTICAL_TO_STEEP = 303,
                STEEP_TO_HORIZONTAL = 281,

                WATER_HORIZONTAL = new int[] { 8, 16 },
                WATER_VERTICAL = new int[] { 25, 26 },
                WATER_CONCAVE_CORNER = 24,
                WATER_CONVEX_CORNER = 35,
            };
        }

        public class DeepWoodsRowTileMatrix
        {
            public int[] FOREST_BACK;
            public int[] FOREST_FRONT;

            public int[] FOREST_LEFT_BACK;
            public int[] FOREST_LEFT_FRONT;

            public int[] FOREST_RIGHT_BACK;
            public int[] FOREST_RIGHT_FRONT;

            public int FOREST_LEFT_CORNER_BACK;
            public int FOREST_LEFT_CONCAVE_CORNER;
            public int FOREST_LEFT_CONVEX_CORNER;

            public int FOREST_RIGHT_CORNER_BACK;
            public int FOREST_RIGHT_CONCAVE_CORNER;
            public int FOREST_RIGHT_CONVEX_CORNER;

            public int DARK_GRASS_FRONT;
            public int DARK_GRASS_LEFT;
            public int DARK_GRASS_RIGHT;
            public int DARK_GRASS_LEFT_CONCAVE_CORNER;
            public int DARK_GRASS_LEFT_CONVEX_CORNER;
            public int DARK_GRASS_RIGHT_CONCAVE_CORNER;
            public int DARK_GRASS_RIGHT_CONVEX_CORNER;

            public int BLACK_GRASS_FRONT;
            public int BLACK_GRASS_LEFT;
            public int BLACK_GRASS_RIGHT;
            public int BLACK_GRASS_LEFT_CONCAVE_CORNER;
            public int BLACK_GRASS_LEFT_CONVEX_CORNER;
            public int BLACK_GRASS_RIGHT_CONCAVE_CORNER;
            public int BLACK_GRASS_RIGHT_CONVEX_CORNER;

            public int BRIGHT_GRASS_FRONT;
            public int BRIGHT_GRASS_LEFT;
            public int BRIGHT_GRASS_RIGHT;
            public int BRIGHT_GRASS_LEFT_CONCAVE_CORNER;
            public int BRIGHT_GRASS_LEFT_CONVEX_CORNER;
            public int BRIGHT_GRASS_RIGHT_CONCAVE_CORNER;
            public int BRIGHT_GRASS_RIGHT_CONVEX_CORNER;

            public int BRIGHT_GRASS_TINY_FRONT;  // 326 -> 302
            public int BRIGHT_GRASS_LEFT_STEEP_CORNER;   // 328 -> 261
            public int BRIGHT_GRASS_RIGHT_STEEP_CORNER; // 378 -> 281
            public int BRIGHT_GRASS_FRONT_RIGHT_STEEP_CORNER;    // 301
            public int BRIGHT_GRASS_FRONT_LEFT_STEEP_CORNER; // 303

            public bool HAS_BLACK_GRASS;

            public DeepWoodsRowTileMatrix() { }

            public static readonly DeepWoodsRowTileMatrix TOP = new DeepWoodsRowTileMatrix()
            {
                FOREST_BACK = new int[] { 941/*942, 943*/ },
                FOREST_FRONT = new int[] { 967, 968 },

                FOREST_LEFT_BACK = new int[] { 1015 },
                FOREST_LEFT_FRONT = new int[] { 991, 1016 },

                FOREST_RIGHT_BACK = new int[] { 995 },
                FOREST_RIGHT_FRONT = new int[] { 994, 1019 },

                FOREST_LEFT_CORNER_BACK = 940,
                FOREST_LEFT_CONCAVE_CORNER = 966,
                FOREST_LEFT_CONVEX_CORNER = 992,

                FOREST_RIGHT_CORNER_BACK = 945,
                FOREST_RIGHT_CONCAVE_CORNER = 969,
                FOREST_RIGHT_CONVEX_CORNER = 993,

                DARK_GRASS_FRONT = 405,
                DARK_GRASS_LEFT = 381,
                DARK_GRASS_RIGHT = 379,
                DARK_GRASS_LEFT_CONCAVE_CORNER = 357,
                DARK_GRASS_LEFT_CONVEX_CORNER = 406,
                DARK_GRASS_RIGHT_CONCAVE_CORNER = 407,
                DARK_GRASS_RIGHT_CONVEX_CORNER = 404,

                BLACK_GRASS_FRONT = 1119,
                BLACK_GRASS_LEFT = 1121,
                BLACK_GRASS_RIGHT = 1096,
                BLACK_GRASS_LEFT_CONCAVE_CORNER = 1092,
                BLACK_GRASS_LEFT_CONVEX_CORNER = 1120,
                BLACK_GRASS_RIGHT_CONCAVE_CORNER = 1093,
                BLACK_GRASS_RIGHT_CONVEX_CORNER = 1095,

                BRIGHT_GRASS_FRONT = 326,
                BRIGHT_GRASS_LEFT = 350,
                BRIGHT_GRASS_RIGHT = 352,
                BRIGHT_GRASS_LEFT_CONCAVE_CORNER = 325,
                BRIGHT_GRASS_LEFT_CONVEX_CORNER = 328,
                BRIGHT_GRASS_RIGHT_CONCAVE_CORNER = 327,
                BRIGHT_GRASS_RIGHT_CONVEX_CORNER = 378,

                BRIGHT_GRASS_TINY_FRONT = 302,  // 326 -> 302
                BRIGHT_GRASS_LEFT_STEEP_CORNER = 261,   // 328 -> 261
                BRIGHT_GRASS_RIGHT_STEEP_CORNER = 281, // 378 -> 281
                BRIGHT_GRASS_FRONT_RIGHT_STEEP_CORNER = 301,    // 301
                BRIGHT_GRASS_FRONT_LEFT_STEEP_CORNER = 303,// 303

                HAS_BLACK_GRASS = true
            };

            public static readonly DeepWoodsRowTileMatrix BOTTOM = new DeepWoodsRowTileMatrix()
            {
                FOREST_BACK = new int[] { 1068, 1069 },
                FOREST_FRONT = new int[] { 1042, 1043 },

                FOREST_LEFT_BACK = new int[] { 1015 },
                FOREST_LEFT_FRONT = new int[] { 991, 1016 },

                FOREST_RIGHT_BACK = new int[] { 995 },
                FOREST_RIGHT_FRONT = new int[] { 994, 1019 },

                FOREST_LEFT_CORNER_BACK = 1065,
                FOREST_LEFT_CONCAVE_CORNER = 1041,
                FOREST_LEFT_CONVEX_CORNER = 1017,

                FOREST_RIGHT_CORNER_BACK = 1070,
                FOREST_RIGHT_CONCAVE_CORNER = 1044,
                FOREST_RIGHT_CONVEX_CORNER = 1018,

                DARK_GRASS_FRONT = 355,
                DARK_GRASS_LEFT = 381,
                DARK_GRASS_RIGHT = 379,
                DARK_GRASS_LEFT_CONCAVE_CORNER = 382,
                DARK_GRASS_LEFT_CONVEX_CORNER = 356,
                DARK_GRASS_RIGHT_CONCAVE_CORNER = 332,
                DARK_GRASS_RIGHT_CONVEX_CORNER = 354,

                BRIGHT_GRASS_FRONT = 376,
                BRIGHT_GRASS_LEFT = 350,
                BRIGHT_GRASS_RIGHT = 352,
                BRIGHT_GRASS_LEFT_CONCAVE_CORNER = 375,
                BRIGHT_GRASS_LEFT_CONVEX_CORNER = 403,
                BRIGHT_GRASS_RIGHT_CONCAVE_CORNER = 377,
                BRIGHT_GRASS_RIGHT_CONVEX_CORNER = 353,

                BRIGHT_GRASS_TINY_FRONT = 277,
                BRIGHT_GRASS_LEFT_STEEP_CORNER = 311,
                BRIGHT_GRASS_RIGHT_STEEP_CORNER = 331,
                BRIGHT_GRASS_FRONT_RIGHT_STEEP_CORNER = 276,
                BRIGHT_GRASS_FRONT_LEFT_STEEP_CORNER = 278,

                HAS_BLACK_GRASS = false
            };

            public static readonly DeepWoodsRowTileMatrix LEFT = new DeepWoodsRowTileMatrix()
            {
                FOREST_BACK = new int[] { 1015 },
                FOREST_FRONT = new int[] { 991, 1016 },

                FOREST_LEFT_BACK = new int[] { 941/*942, 943*/ },
                FOREST_LEFT_FRONT = new int[] { 967, 968 },

                FOREST_RIGHT_BACK = new int[] { 1068, 1069 },
                FOREST_RIGHT_FRONT = new int[] { 1042, 1043 },

                FOREST_LEFT_CORNER_BACK = 940,
                FOREST_LEFT_CONCAVE_CORNER = 966,
                FOREST_LEFT_CONVEX_CORNER = 992,

                FOREST_RIGHT_CORNER_BACK = 1040,
                FOREST_RIGHT_CONCAVE_CORNER = 1041,
                FOREST_RIGHT_CONVEX_CORNER = 1017,

                DARK_GRASS_FRONT = 381,
                DARK_GRASS_LEFT = 405,
                DARK_GRASS_RIGHT = 355,
                DARK_GRASS_LEFT_CONCAVE_CORNER = 357,
                DARK_GRASS_LEFT_CONVEX_CORNER = 406,
                DARK_GRASS_RIGHT_CONCAVE_CORNER = 382,
                DARK_GRASS_RIGHT_CONVEX_CORNER = 356,

                BRIGHT_GRASS_FRONT = 350,
                BRIGHT_GRASS_LEFT = 326,
                BRIGHT_GRASS_RIGHT = 376,
                BRIGHT_GRASS_LEFT_CONCAVE_CORNER = 325,
                BRIGHT_GRASS_LEFT_CONVEX_CORNER = 328,
                BRIGHT_GRASS_RIGHT_CONCAVE_CORNER = 375,
                BRIGHT_GRASS_RIGHT_CONVEX_CORNER = 403,

                BRIGHT_GRASS_TINY_FRONT = 286,
                BRIGHT_GRASS_LEFT_STEEP_CORNER = 301,
                BRIGHT_GRASS_RIGHT_STEEP_CORNER = 276,
                BRIGHT_GRASS_FRONT_RIGHT_STEEP_CORNER = 261,
                BRIGHT_GRASS_FRONT_LEFT_STEEP_CORNER = 311,

                HAS_BLACK_GRASS = false
            };

            public static readonly DeepWoodsRowTileMatrix RIGHT = new DeepWoodsRowTileMatrix()
            {
                FOREST_BACK = new int[] { 995 },
                FOREST_FRONT = new int[] { 994, 1019 },

                FOREST_LEFT_BACK = new int[] { 941/*942, 943*/ },
                FOREST_LEFT_FRONT = new int[] { 967, 968 },

                FOREST_RIGHT_BACK = new int[] { 1068, 1069 },
                FOREST_RIGHT_FRONT = new int[] { 1042, 1043 },

                FOREST_LEFT_CORNER_BACK = 945,
                FOREST_LEFT_CONCAVE_CORNER = 969,
                FOREST_LEFT_CONVEX_CORNER = 993,

                FOREST_RIGHT_CORNER_BACK = 1069,
                FOREST_RIGHT_CONCAVE_CORNER = 1044,
                FOREST_RIGHT_CONVEX_CORNER = 1018,

                DARK_GRASS_FRONT = 379,
                DARK_GRASS_LEFT = 405,
                DARK_GRASS_RIGHT = 355,
                DARK_GRASS_LEFT_CONCAVE_CORNER = 407,
                DARK_GRASS_LEFT_CONVEX_CORNER = 404,
                DARK_GRASS_RIGHT_CONCAVE_CORNER = 332,
                DARK_GRASS_RIGHT_CONVEX_CORNER = 354,

                BRIGHT_GRASS_FRONT = 352,
                BRIGHT_GRASS_LEFT = 326,
                BRIGHT_GRASS_RIGHT = 376,
                BRIGHT_GRASS_LEFT_CONCAVE_CORNER = 327,
                BRIGHT_GRASS_LEFT_CONVEX_CORNER = 378,
                BRIGHT_GRASS_RIGHT_CONCAVE_CORNER = 377,
                BRIGHT_GRASS_RIGHT_CONVEX_CORNER = 353,

                BRIGHT_GRASS_TINY_FRONT = 306,
                BRIGHT_GRASS_LEFT_STEEP_CORNER = 303,
                BRIGHT_GRASS_RIGHT_STEEP_CORNER = 278,
                BRIGHT_GRASS_FRONT_RIGHT_STEEP_CORNER = 281,
                BRIGHT_GRASS_FRONT_LEFT_STEEP_CORNER = 331,

                HAS_BLACK_GRASS = false
            };
        }

        public class DeepWoodsCornerTileMatrix
        {
            public int[] HORIZONTAL_BACK;
            public int[] HORIZONTAL_FRONT;
            public int[] VERTICAL_BACK;
            public int[] VERTICAL_FRONT;
            public int CONCAVE_CORNER_DIAGONAL_BACK;
            public int CONCAVE_CORNER_HORIZONTAL_BACK;
            public int CONCAVE_CORNER_VERTICAL_BACK;
            public int CONCAVE_CORNER;
            public int CONVEX_CORNER;

            public int DARK_GRASS_HORIZONTAL;
            public int DARK_GRASS_VERTICAL;
            public int DARK_GRASS_CONCAVE_CORNER;
            public int DARK_GRASS_CONVEX_CORNER;

            public int BLACK_GRASS_HORIZONTAL;
            public int BLACK_GRASS_VERTICAL;
            public int BLACK_GRASS_CONCAVE_CORNER;
            public int BLACK_GRASS_CONVEX_CORNER;

            public bool HAS_BLACK_GRASS;

            public DeepWoodsCornerTileMatrix() { }

            public static readonly DeepWoodsCornerTileMatrix TOP_LEFT = new DeepWoodsCornerTileMatrix()
            {
                HORIZONTAL_BACK = new int[] { 941/*942, 943*/ },
                HORIZONTAL_FRONT = new int[] { 967, 968 },
                VERTICAL_BACK = new int[] { /*990,*/ 1015 },
                VERTICAL_FRONT = new int[] { 991, 1016 },
                CONCAVE_CORNER_DIAGONAL_BACK = 940,
                CONCAVE_CORNER_HORIZONTAL_BACK = 941,
                CONCAVE_CORNER_VERTICAL_BACK = 965,
                CONCAVE_CORNER = 966,
                CONVEX_CORNER = 992,
                DARK_GRASS_HORIZONTAL = 405,
                DARK_GRASS_VERTICAL = 381,
                DARK_GRASS_CONCAVE_CORNER = 357,
                DARK_GRASS_CONVEX_CORNER = 406,
                BLACK_GRASS_HORIZONTAL = 1119,
                BLACK_GRASS_VERTICAL = 1121,
                BLACK_GRASS_CONCAVE_CORNER = 1092,
                BLACK_GRASS_CONVEX_CORNER = 1120,
                HAS_BLACK_GRASS = true
            };

            public static readonly DeepWoodsCornerTileMatrix TOP_RIGHT = new DeepWoodsCornerTileMatrix()
            {
                HORIZONTAL_BACK = new int[] { 942 },
                HORIZONTAL_FRONT = new int[] { 967, 968 },
                VERTICAL_BACK = new int[] { 995 },
                VERTICAL_FRONT = new int[] { 994, 1019 },
                CONCAVE_CORNER_DIAGONAL_BACK = 945,
                CONCAVE_CORNER_HORIZONTAL_BACK = 944 /*942?*/,
                CONCAVE_CORNER_VERTICAL_BACK = 970,
                CONCAVE_CORNER = 969,
                CONVEX_CORNER = 993,
                DARK_GRASS_HORIZONTAL = 405,
                DARK_GRASS_VERTICAL = 379,
                DARK_GRASS_CONCAVE_CORNER = 407,
                DARK_GRASS_CONVEX_CORNER = 404,
                BLACK_GRASS_HORIZONTAL = 1119,
                BLACK_GRASS_VERTICAL = 1096,
                BLACK_GRASS_CONCAVE_CORNER = 1093,
                BLACK_GRASS_CONVEX_CORNER = 1095,
                HAS_BLACK_GRASS = true
            };

            public static readonly DeepWoodsCornerTileMatrix BOTTOM_LEFT = new DeepWoodsCornerTileMatrix()
            {
                HORIZONTAL_BACK = new int[] { 1068, 1069 },
                HORIZONTAL_FRONT = new int[] { 1042, 1043 },
                VERTICAL_BACK = new int[] { 990, 1015 },
                VERTICAL_FRONT = new int[] { 991, 1016 },
                CONCAVE_CORNER_DIAGONAL_BACK = 1065,
                CONCAVE_CORNER_HORIZONTAL_BACK = 1066,
                CONCAVE_CORNER_VERTICAL_BACK = 1040,
                CONCAVE_CORNER = 1041,
                CONVEX_CORNER = 1017,
                DARK_GRASS_HORIZONTAL = 355,
                DARK_GRASS_VERTICAL = 381,
                DARK_GRASS_CONCAVE_CORNER = 382,
                DARK_GRASS_CONVEX_CORNER = 356,
                HAS_BLACK_GRASS = false
            };

            public static readonly DeepWoodsCornerTileMatrix BOTTOM_RIGHT = new DeepWoodsCornerTileMatrix()
            {
                HORIZONTAL_BACK = new int[] { 1068, 1069 },
                HORIZONTAL_FRONT = new int[] { 1042, 1043 },
                VERTICAL_BACK = new int[] { 995 },
                VERTICAL_FRONT = new int[] { 994, 1019 },
                CONCAVE_CORNER_DIAGONAL_BACK = 1070,
                CONCAVE_CORNER_HORIZONTAL_BACK = 1069,
                CONCAVE_CORNER_VERTICAL_BACK = 1045,
                CONCAVE_CORNER = 1044,
                CONVEX_CORNER = 1018,
                DARK_GRASS_HORIZONTAL = 355,
                DARK_GRASS_VERTICAL = 379,
                DARK_GRASS_CONCAVE_CORNER = 332,
                DARK_GRASS_CONVEX_CORNER = 354,
                HAS_BLACK_GRASS = false
            };
        }

    }
}

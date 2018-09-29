using System;

// ReSharper disable ArrangeStaticMemberQualifier
namespace TehPers.Stardew.Framework {
    [Flags]
    public enum WaterType {
        /** <summary>Game ID is 1</summary> **/
        LAKE = 1,

        /** <summary>Game ID is 0</summary> **/
        RIVER = 2,

        /** <summary>Game ID is -1</summary> **/
        BOTH = LAKE | RIVER
    }

    [Flags]
    public enum Weather {
        SUNNY = 1,
        RAINY = 2,
        BOTH = SUNNY | RAINY
    }

    [Flags]
    public enum Season {
        SPRING = 1,
        SUMMER = 2,
        FALL = 4,
        WINTER = 8,

        SPRINGSUMMER = SPRING | SUMMER,
        SPRINGFALL = SPRING | FALL,
        SUMMERFALL = SUMMER | FALL,
        SPRINGSUMMERFALL = SPRING | SUMMER | FALL,
        SPRINGWINTER = SPRING | WINTER,
        SUMMERWINTER = SUMMER | WINTER,
        SPRINGSUMMERWINTER = SPRING | SUMMER | WINTER,
        FALLWINTER = FALL | WINTER,
        SPRINGFALLWINTER = SPRING | FALL | WINTER,
        SUMMERFALLWINTER = SUMMER | FALL | WINTER,
        SPRINGSUMMERFALLWINTER = SPRING | SUMMER | FALL | WINTER
    }
}
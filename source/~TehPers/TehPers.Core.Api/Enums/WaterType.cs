using System;

namespace TehPers.Core.Api.Enums {
    /// <summary>The different possible types of bodies of water.</summary>
    [Flags]
    public enum WaterType : byte {
        /** <summary>Game ID is 1</summary> **/
        Lake = 1,

        /** <summary>Game ID is 0</summary> **/
        River = 2,

        /** <summary>Game ID is -1</summary> **/
        Both = WaterType.Lake | WaterType.River
    }
}

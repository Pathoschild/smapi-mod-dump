using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //contains configuration settings for forage item generation behavior
        private class ForageSettings
        {
            public ForageSpawnArea[] Areas { get; set; }
            public int PercentExtraSpawnsPerForagingLevel { get; set; }
            public object[] SpringItemIndex { get; set; } //changed from int[] to object[] in version 1.4.0
            public object[] SummerItemIndex { get; set; } //changed from int[] to object[] in version 1.4.0
            public object[] FallItemIndex { get; set; } //changed from int[] to object[] in version 1.4.0
            public object[] WinterItemIndex { get; set; } //changed from int[] to object[] in version 1.4.0
            public int[] CustomTileIndex { get; set; }

            //default constructor: configure default forage generation settings
            public ForageSettings()
            {
                Areas = new ForageSpawnArea[] { new ForageSpawnArea() }; //a set of "SpawnArea" objects, describing where forage items can spawn on each map
                PercentExtraSpawnsPerForagingLevel = 0; //multiplier to give extra forage per level of foraging skill; default is +0%, since the native game lacks this mechanic

                //the "parentSheetIndex" values for each type of forage item allowed to spawn in each season (the numbers found in ObjectInformation.xnb)
                SpringItemIndex = new object[] { 16, 20, 22, 257 };
                SummerItemIndex = new object[] { 396, 398, 402, 404 };
                FallItemIndex = new object[] { 281, 404, 420, 422 };
                WinterItemIndex = new object[0];

                CustomTileIndex = new int[0]; //an extra list of tilesheet indices, for use by players who want to make some custom tile detection
            }
        }
    }
}
using System.Collections.Generic;

namespace priceDrops
{
    // Heart levels and corresponding discount values in percent.
    // Explanation of the standard values:
    // Player receives a 10% discount when he has 3 hearts or more with the NPC
    // 25% discount at 7 hearts or more
    // 50% discount at 10 hearts or more
    // Change these values to what you think is fair.
    class ModConfig
    {
        public int heartLevel1 { get; set; } = 3;
        public int heartLevel2 { get; set; } = 7;
        public int heartLevel3 { get; set; } = 10;

        public int disc1 { get; set; } = 10;
        public int disc2 { get; set; } = 25;
        public int disc3 { get; set; } = 50;
        public int bonusDisc { get; set; } = 5;

        public List<string> customNPCs { get; set; } = new List<string> { "placeHolder1", "placeHolder2" };
    }
}

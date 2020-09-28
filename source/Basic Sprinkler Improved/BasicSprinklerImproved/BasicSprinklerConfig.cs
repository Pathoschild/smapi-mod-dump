using System;
using StardewModdingAPI;

namespace BasicSprinklerImproved
{
    public class BasicSprinklerConfig
    {
        public String patternType { get; set; }

        public Int32 northArea { get; set; }
        public Int32 southArea { get; set; }
        public Int32 eastArea { get; set; }
        public Int32 westArea { get; set; }

        public BasicSprinklerConfig()
        {
            this.patternType = "horizontal";

            //Note: It doesn't actually matter what areas we're using with a default pattern. It will construct based on type. This just sets a good example.
            this.northArea = 0;
            this.southArea = 0;
            this.eastArea = 2;
            this.westArea = 2;
        }

        public BasicSprinklerConfig(string type, int[] dims)
        {
            this.patternType = type;

            this.northArea = dims[0];
            this.southArea = dims[1];
            this.eastArea = dims[2];
            this.westArea = dims[3];
        }
    }
}

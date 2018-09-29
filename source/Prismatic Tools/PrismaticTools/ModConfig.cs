
namespace PrismaticTools.Framework {
    public class ModConfig {
        public bool UseSprinklersAsScarecrows { get; set; }
        public bool UseSprinklersAsLamps { get; set; }
        public int SprinklerRange { get; set; }
        public int PrismaticToolLength { get; set; }
        public int PrismaticToolWidth { get; set; }

        public ModConfig() {
            UseSprinklersAsScarecrows = true;
            UseSprinklersAsLamps = true;
            SprinklerRange = 3;
            PrismaticToolLength = 7;
            PrismaticToolWidth = 2;
        }
    }
}

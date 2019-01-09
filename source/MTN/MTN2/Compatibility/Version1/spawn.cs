using MTN2.MapData;

namespace MTN2.Compatibility {
    public abstract class spawn<T> {
        //English setting names
        public string itemName;
        public string seasons;
        public string spawntype;

        //Systematic setting values
        public int itemId = 0;
        public SpawningSeason SeasonsToSpawn = SpawningSeason.allYear;
        public SpawnType SpawnType = SpawnType.areaBound;

        //Map
        public string mapName;

        //Area Binding
        public Area area;

        //Tile Binding
        public int TileIndex = 0;

        //Chance Probability
        public float chance = 0.70f;

        //Additive Probability
        public float rainAddition = 0;
        public float newMonthAddition = 0;
        public float newYearAddition = 0;

        //Amount
        public int minimumAmount = 1;
        public int maximumAmount = 0;

        //Amount Multiplers
        public int rainMultipler = 1;
        public int newMonthMultipler = 1;
        public int newYearMultipler = 1;

        //Cooldown
        public int minCooldown = 1;
        public int maxCooldown = 0;
        public int daysTilNextSpawn = 1;

        //Tile Requirements
        public string tileType = "All";
        public bool diggable = true;

        //Validation
        protected bool valid = false;
        public bool isValid {
            get {
                return valid;
            }
        }
    }
}

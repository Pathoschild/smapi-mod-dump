using System.Collections.Generic;
using StardewValley;
using StardewValleyEsp.Config;

namespace StardewValleyEsp.Detectors
{
    class Detector : IDetector
    {
        private readonly Dictionary<string, IDetector> detectors = new Dictionary<string, IDetector>();
        private readonly Settings settings;

        public EntityList Entities { get; set; } = new EntityList();
        public GameLocation Location { get; set; }

        public Detector(Settings settings)
        {
            this.settings = settings;
        }

        public Detector AddDetector(string type)
        {
            IDetector d = null;
            switch (type)
            {
                case "NPC":
                    d = new NPCDetector(settings);
                    break;
                case "Object":
                    d = new ObjectDetector(settings);
                    break;
                case "FarmAnimal":
                    d = new FarmAnimalDetector(settings);
                    break;
                case "WaterEntity":
                    d = new WaterEntityDetector(settings);
                    break;
                case null:
                    return this;
            }
            detectors.Add(type, d);
            return this;
        }

        public EntityList Detect()
        {
            Entities.Clear();
            foreach (var kvp in detectors)
                Entities.AddRange(kvp.Value.Detect());
            return Entities;
        }

        public IDetector SetLocation(GameLocation loc)
        {
            foreach (var kvp in detectors)
                kvp.Value.SetLocation(loc);
            Location = loc;
            return this;
        }
    }
}

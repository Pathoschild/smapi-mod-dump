using System.Collections.Generic;
using StardewValley;
using StardewValleyEsp.Config;

namespace StardewValleyEsp.Detectors
{
    internal enum DetectorType
    {
        NPC,
        Object,
        FarmAnimal,
        WaterEntity,
        Crop
    }

    class Detector : IDetector
    {
        private readonly Dictionary<DetectorType, IDetector> detectors = new Dictionary<DetectorType, IDetector>();
        private readonly Settings settings;

        public EntityList Entities { get; set; } = new EntityList();
        public GameLocation Location { get; set; }

        public Detector(Settings settings)
        {
            this.settings = settings;
        }

        public Detector AddDetector(DetectorType type)
        {
            IDetector d;
            switch (type)
            {
                case DetectorType.NPC:
                    d = new NPCDetector(settings);
                    break;
                case DetectorType.Object:
                    d = new ObjectDetector(settings);
                    break;
                case DetectorType.FarmAnimal:
                    d = new FarmAnimalDetector(settings);
                    break;
                case DetectorType.WaterEntity:
                    d = new WaterEntityDetector(settings);
                    break;
                case DetectorType.Crop:
                    d = new CropDetector(settings);
                    break;
                default:
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

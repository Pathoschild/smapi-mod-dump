

namespace AutoAnimalDoors.StardewValleyWrapper
{
    class ModConfig
    {
        public bool AutoOpenEnabled { get; set; } = true;
        public int AnimalDoorOpenTime { get; set; } = 730;
        public int AnimalDoorCloseTime { get; set; } = 1800;
        public bool OpenDoorsWhenRaining { get; set; } = false;
        public bool OpenDoorsDuringWinter { get; set; } = false;
    }
}

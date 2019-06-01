

namespace AutoAnimalDoors.StardewValleyWrapper
{
    class ModConfig
    {
        public int AnimalDoorOpenTime { get; set; } = 730;
        public int AnimalDoorCloseTime { get; set; } = 1800;
        public int CoopRequiredUpgradeLevel { get; set; } = 1;
        public int BarnRequiredUpgradeLevel { get; set; } = 1;
        public bool AutoOpenEnabled { get; set; } = true;
        public bool OpenDoorsWhenRaining { get; set; } = false;
        public bool OpenDoorsDuringWinter { get; set; } = false;
    }
}

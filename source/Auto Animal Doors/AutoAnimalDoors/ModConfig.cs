/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/taggartaa/AutoAnimalDoors
**
*************************************************/



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

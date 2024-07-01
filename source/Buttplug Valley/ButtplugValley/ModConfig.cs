/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DryIcedTea/Buttplug-Valley
**
*************************************************/

using StardewModdingAPI;

namespace ButtplugValley
{
    public sealed class ModConfig
    {
        public bool VibrateOnStoneBroken { get; set; } = true;
        public bool VibrateOnDamageTaken { get; set; } = true;
        
        public bool VibrateOnEnemyKilled { get; set; } = true;
        public bool VibrateOnFishCollected { get; set; } = true;
        public bool VibrateOnCropAndMilkCollected { get; set; } = true;
        public bool VibrateOnFlowersCollected { get; set; } = true;
        public bool VibrateOnForagingCollected { get; set; } = true;
        public bool VibrateOnTreeBroken { get; set; } = true;
        public bool VibrateOnDayStart { get; set; } = true;
        public bool VibrateOnDayEnd { get; set; } = true;
        
        public bool VibrateOnFishingMinigame { get; set; } = true;

        public bool VibrateOnFishingRodUsage { get; set; } = true;

        public bool VibrateOnArcade { get; set; } = true;
        
        public bool VibrateOnDialogue { get; set; } = true;
        
        public bool VibrateOnHorse { get; set; } = true;
        
        public bool VibrateOnGrass { get; set; } = true;
        
        public bool VibrateOnTreeHit { get; set; } = true;
        
        public bool VibrateOnTreeFell { get; set; } = true;

        public bool VibrateOnKiss { get; set; } = true;
        

        public bool VibrateOnSexScene { get; set; } = true;

        public bool VibrateOnRainsInteractionMod { get; set; } = true;

        public bool VibrateOnDarkClubMoans { get; set; } = true;

        public bool VibrateOnDarkClubSex { get; set; } = true;

        //DEBUG TEMP STUFF
        public bool StonePickedUpDebug { get; set; } = false;      

        public bool KeepAlive { get; set; } = true;

        public int StoneBrokenLevel { get; set; } = 35;
        public int DamageTakenMax { get; set; } = 100;     
        public int EnemyKilledLevel { get; set; } = 35;
        public int FishCollectedBasic { get; set; } = 30;
        public int CropAndMilkBasic { get; set; } = 30;
        public int FlowerBasic { get; set; } = 30;
        public int ForagingBasic { get; set; } = 30;
        public int SilverLevel { get; set; } = 55;
        public int GoldLevel { get; set; } = 85;
        public int IridiumLevel { get; set; } = 100;
        public int TreeBrokenLevel { get; set; } = 80;
        public int DayStartLevel { get; set; } = 50;
        public int DayEndMax { get; set; } = 100;
        public int MaxFishingVibration { get; set; } = 100;
        public int ArcadeLevel { get; set; } = 50;
        public int KeepAliveInterval { get; set; } = 30;
        public int KeepAliveLevel { get; set; } = 5;
        public int DialogueLevel { get; set; } = 50;
        public int HorseLevel { get; set; } = 50;
        public int SexSceneLevel { get; set; } = 75;
        public int RainsInteractionModLevel { get; set; } = 75;
        public int DarkClubMoanLevel { get; set; } = 50;
        public int MaxDarkClubSexLevel { get; set; } = 100;

        public SButton StopVibrations { get; set; } = SButton.P;
        public SButton DisconnectButtplug { get; set; } = SButton.I;
        public SButton ReconnectButtplug { get; set; } = SButton.K;

        public string IntifaceIP { get; set; } = "localhost:12345";
        
        public int QueueLength { get; set; } = 20;
        
        public int GrassLevel { get; set; } = 400;
        
        public int TreeChopLevel { get; set; } = 50;
        
        public int TreeFellLevel { get; set; } = 100;
        
        public int WateringCanLevel { get; set; } = 25;
        
        public int HoeLevel { get; set; } = 25;
    }
}
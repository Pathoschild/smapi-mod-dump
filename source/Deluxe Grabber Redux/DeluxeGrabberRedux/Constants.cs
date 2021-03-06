/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using System.Collections.Generic;

namespace DeluxeGrabberRedux
{
    static class Tools
    {
        public const string Axe = "Axe";
        public const string Hoe = "Hoe";
        public const string Pickaxe = "Pickaxe";
        public const string WateringCan = "Watering Can";
        public const string FishingPole = "Rod";
        public const string Pan = "Copper Pan";
    }

    static class ItemIds
    {
        public const int Autograbber = 165;
        public const int GoldenWalnut = 73;
        public const int SpringOnion = 399;
        public const int Ginger = 829;
        public const int MushroomBox = 128;
        public const int Truffle = 430;
        public const int Slime = 766;
        public const int PetrifiedSlime = 557;
        public const int Salmonberry = 296;
        public const int Blackberry = 410;
        public const int TeaLeaves = 815;
        public const int MixedSeeds = 770;
        public const int Fiber = 771;
        public const int QiFruit = 889;
        public const int Sunflower = 421;
        public const int SunflowerSeed = 431;
        public const int Wheat = 262;
        public const int Hay = 178;
        public const int Coal = 382;
        public const int ArtifactSpot = 590;
        public const int SnowYam = 416;
        public const int WinterRoot = 412;
        public const int RiceShoot = 273;
        public const int LostBook = 102;
        public const int Clay = 330;
        public const int FarmTotem = 688;
        public const int QiBeans = 890;
        public const int GuntherBones = 881;
        public const int Hardwood = 709;
        public const int CopperOre = 378;
        public const int IronOre = 380;
        public const int GoldOre = 384;
        public const int IridiumOre = 386;
        public const int Diamond = 72;
        public const int Emerald = 60;
        public const int OmniGeode = 749;
        public const int FireQuartz = 82;
        public const int EarthCrystal = 86;
        public const int FrozenTear = 84;
        public const int FossilizedTail = 822;
        public const int TaroTuber = 831;
        public const int PineCone = 311;
        public const int Acorn = 309;
        public const int MapleSeed = 310;
        public const int MahoganySeed = 292;
        public const int Coconut = 88;
        public const int GoldenCoconut = 791;
        public const int Hazelnut = 408;



        // not an object id
        public const int LuckyRing = 859;

        public const int FlowersCategory = -80;

        public static readonly Dictionary<int, int> FertilizerQualities = new Dictionary<int, int>()
        {
            {368, 1 },
            {369, 2 },
            {919, 3 }
        };
    }

    static class Skills
    {
        public const int Farming = 0;
        public const int Foraging = 2;
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Types
{
    public static class KnownHats
    {
        public static Hat None = -1,
        CowboyHat = 0,
        BowlerHat = 1,
        TopHat = 2,
        Sombrero = 3,
        StrawHat = 4,
        OfficialCap = 5,
        BlueBonnet = 6,
        PlumChapeau = 7,
        SkeletonMask = 8,
        GoblinMask = 9,
        ChickenMask = 10,
        Earmuffs = 11,
        DelicateBow = 12,
        Tropiclip = 13,
        ButterflyBow = 14,
        HuntersCap = 15,
        TruckerHat = 16,
        SailorsCap = 17,
        GoodOlCap = 18,
        Fedora = 19,
        CoolCap = 20,
        LuckyBow = 21,
        PolkaBow = 22,
        GnomesCap = 23,
        EyePatch = 24,
        SantaHat = 25,
        Tiara = 26,
        HardHat = 27,
        Souwester = 28,
        Daisy = 29,
        WatermelonBand = 30,
        MouseEars = 31,
        CatEars = 32,
        CowgalHat = 33,
        CowpokeHat = 34,
        ArchersCap = 35,
        PandaHat = 36,
        BlueCowboyHat = 37,
        RedCowboyHat = 38,
        ConeHat = 39,
        LivingHat = 40,
        EmilysMagicHat = 41,
        MushroomCap = 42,
        DinosaurHat = 43,
        TotemMask = 44,
        LogoCap = 45,
        WearableDwarfHelm = 46,
        FashionHat = 47,
        PumpkinMask = 48,
        HairBone = 49,
        KnightsHelmet = 50,
        SquiresHelmet = 51,
        SpottedHeadscarf = 52,
        Beanie = 53,
        FloppyBeanie = 54,
        FishingHat = 55,
        BlobfishMask = 56,
        PartyHat_Red = 57,
        PartyHat_Blue = 58,
        PartyHat_Green = 59,
        ArcaneHat = 60,
        ChefHat = 61,
        PirateHat = 62,
        FlatToppedHat = 63,
        ElegantTurban = 64,
        WhiteTurban = 65,
        GarbageHat = 66,
        GoldenMask = 67,
        PropellerHat = 68,
        BridalVeil = 69,
        WitchHat = 70,
        CopperPan = 71,
        GreenTurban = 72,
        MagicCowboyHat = 73,
        MagicTurban = 74,
        GoldenHelmet = 75,
        DeluxePirateHat = 76,
        PinkBow = 77,
        FrogHat = 78,
        SmallCap = 79,
        BluebirdMask = 80,
        DeluxeCowboyHat = 81,
        MrQisHat = 82,
        DarkCowboyHat = 83,
        RadioactiveGoggles = 84,
        SwashbucklerHat = 85,
        QiMask = 86,
        StarHelmet = 87,
        Sunglasses = 88,
        Goggles = 89,
        ForagersHat = 90,
        TigerHat = 91,
        ThreeQuestionMarks = 92,
        WarriorHelmet = 93;

        static Dictionary<string, Hat> lut_ids = new Dictionary<string, Hat>();
        static Dictionary<string, Hat> lut_names = new Dictionary<string, Hat>();

        static KnownHats()
        {
            foreach (var field in typeof(KnownHats)
                .GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .Where(x => x.FieldType == typeof(Hat)))
            {
                var hat = field.GetValue(null) as Hat;
                hat.ItemName = field.Name;

                lut_ids.Add(hat.ItemId, hat);
                lut_names.Add(hat.ItemName, hat);
            }
        }

        public static Hat GetById(string itemId)
        {
            if (itemId == null) return KnownHats.None;

            if (lut_ids.TryGetValue(itemId, out Hat knownHat) || lut_names.TryGetValue(itemId, out knownHat))
            {
                return knownHat;
            }
            else
                return new Hat(itemId) { ItemName = itemId };
        }
    }

    public class Hat : AlphanumericItemId
    {
        public Hat(string itemId)
            : base(itemId, ClothingItemType.Hat)
        {
        }

        public static implicit operator Hat(int value)
        {
            return new Hat(value.ToString());
        }

        public static implicit operator Hat(string value)
        {
            return new Hat(value);
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewRoguelike.UI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace StardewRoguelike
{
    public class Perks
    {
        public enum PerkRarity
        {
            Common,
            Uncommon,
            Rare
        }

        #pragma warning disable format
        public enum PerkType
        {
            Fighter          = 0,
            Scout            = 1,
            Gemologist       = 2,
            SpecialCharm     = 3,
            Reflect          = 4,
            Swift            = 5,
            Shield           = 6,
            Immunity         = 7,
            Precision        = 8,
            Discount         = 9,
            Miner            = 10,
            Brute            = 11,
            Defender         = 12,
            BarrelEnthusiast = 13,
            Leech            = 14,
            FoodEnjoyer      = 15,
            Acrobat          = 16,
            Desperado        = 17,
            RabbitsFoot      = 18,
            TurtleShell      = 19,
            Geologist        = 20,
            Scavenger        = 21,
            Sturdy           = 22,
            Indecisive       = 23
        }
        #pragma warning restore format

        public static readonly List<PerkType> CommonPerks = new()
        {
            PerkType.Fighter, PerkType.Scout, PerkType.Gemologist,
            PerkType.SpecialCharm, PerkType.Reflect, PerkType.Swift,
            PerkType.Shield, PerkType.Immunity, PerkType.Precision,
            PerkType.Discount
        };

        public static readonly List<PerkType> UncommonPerks = new()
        {
            PerkType.Miner, PerkType.Brute, PerkType.Defender,
            PerkType.BarrelEnthusiast, PerkType.FoodEnjoyer,
            PerkType.Geologist, PerkType.Scavenger, PerkType.Sturdy,
            PerkType.Indecisive
        };

        public static readonly List<PerkType> RarePerks = new()
        {
            PerkType.Acrobat, PerkType.Desperado, PerkType.RabbitsFoot,
            PerkType.TurtleShell, PerkType.Leech
        };

        public static PerkMenu CurrentMenu = null;

        private static readonly int TotalPerkCount = CommonPerks.Count + UncommonPerks.Count + RarePerks.Count;

        private static readonly float CommonPerkChance = 0.6f;

        private static readonly float UncommonPerkChance = 0.3f;

        private static readonly float RarePerkChance = 0.1f;

        private static readonly HashSet<PerkType> ActivePerks = new();

        public static string GetPerkDisplayName(PerkType perkType)
        {
            return perkType switch
            {
                PerkType.Fighter => "Fighter",
                PerkType.Scout => "Scout",
                PerkType.Gemologist => "Gemologist",
                PerkType.SpecialCharm => "Special Charm",
                PerkType.Reflect => "Reflect",
                PerkType.Swift => "Swift",
                PerkType.Shield => "Shield",
                PerkType.Immunity => "Immunity",
                PerkType.Precision => "Precision",
                PerkType.Discount => "Discount",
                PerkType.Miner => "Miner",
                PerkType.Brute => "Brute",
                PerkType.Defender => "Defender",
                PerkType.BarrelEnthusiast => "Barrel Enthusiast",
                PerkType.Leech => "Leech",
                PerkType.FoodEnjoyer => "Food Enjoyer",
                PerkType.Acrobat => "Acrobat",
                PerkType.Desperado => "Desperado",
                PerkType.RabbitsFoot => "Rabbit's Foot",
                PerkType.TurtleShell => "Turtle Shell",
                PerkType.Geologist => "Geologist",
                PerkType.Scavenger => "Scavenger",
                PerkType.Sturdy => "Sturdy",
                PerkType.Indecisive => "Indecisive",
                _ => "Not Implemented"
            };
        }

        public static string GetPerkDescription(PerkType perkType)
        {
            return perkType switch
            {
                PerkType.Fighter => "Gain 10% extra damage",
                PerkType.Scout => "Gain 50% extra critical hit chance",
                PerkType.Gemologist => "Gems are worth 30% more",
                PerkType.SpecialCharm => "Gain +1 luck",
                PerkType.Reflect => "Enemies take damage on contact with you",
                PerkType.Swift => "Gain 10% more move speed",
                PerkType.Shield => "Reduce damage taken by 10%",
                PerkType.Immunity => "Gain +2 immunity",
                PerkType.Precision => "Increase critical power by 20%",
                PerkType.Discount => "Reduce shop prices by 10%",
                PerkType.Miner => "Gain +1 ore per vein",
                PerkType.Brute => "Gain 15% extra damage",
                PerkType.Defender => "Gain +25 maximum HP",
                PerkType.BarrelEnthusiast => "Barrels are more likely to drop better loot",
                PerkType.Leech => "Attacks have a chance to heal you a small amount",
                PerkType.FoodEnjoyer => "Food heals for 20% more",
                PerkType.Acrobat => "Cooldown on special moves cut in half",
                PerkType.Desperado => "Critical hits are deadlier",
                PerkType.RabbitsFoot => "Gain +3 luck",
                PerkType.TurtleShell => "Reduce incoming damage by 50% every 10 seconds",
                PerkType.Geologist => "Chance for gems to appear in pairs",
                PerkType.Scavenger => "Chance for monsters to drop food",
                PerkType.Sturdy => "Debuffs last half as long",
                PerkType.Indecisive => "You can refresh each merchant once",
                _ => "Not Implemented"
            };
        }

        public static Rectangle GetPerkSourceRect(PerkType perkType)
        {
            return new((int)perkType % 6 * 16, 624 + (int)perkType / 6 * 16, 16, 16);
        }

        public static PerkRarity GetPerkRarity(PerkType perkType)
        {
            if (CommonPerks.Contains(perkType))
                return PerkRarity.Common;
            else if (UncommonPerks.Contains(perkType))
                return PerkRarity.Uncommon;
            else
                return PerkRarity.Rare;
        }

        public static HashSet<PerkType> GetActivePerks()
        {
            return ActivePerks;
        }

        public static PerkType GetRandomPerk()
        {
            double roll = Game1.random.NextDouble();
            if (roll < CommonPerkChance)
                return CommonPerks[Game1.random.Next(CommonPerks.Count)];
            else if (roll < CommonPerkChance + UncommonPerkChance)
                return UncommonPerks[Game1.random.Next(UncommonPerks.Count)];
            else if (roll < CommonPerkChance + UncommonPerkChance + RarePerkChance)
                return RarePerks[Game1.random.Next(RarePerks.Count)];

            throw new Exception("doesnt add up to 100");
        }

        public static PerkType? GetRandomUniquePerk()
        {
            if (HasAllPerks())
                return null;

            PerkType perk = GetRandomPerk();
            while (ActivePerks.Contains(perk))
                perk = GetRandomPerk();

            return perk;
        }

        public static (PerkType?, PerkType?) GetTwoRandomUniquePerks()
        {
            if (HasAllPerks())
                return (null, null);
            else if (ActivePerks.Count + 1 == TotalPerkCount)
                return (GetRandomUniquePerk().Value, null);

            PerkType perk1 = GetRandomUniquePerk().Value;
            PerkType perk2;
            do
            {
                perk2 = GetRandomUniquePerk().Value;
            } while (perk2 == perk1);

            return (perk1, perk2);
        }

        public static void AddPerk(PerkType perkType)
        {
            if (HasPerk(perkType))
                return;

            switch (perkType)
            {
                case PerkType.Fighter:
                    Game1.player.professions.Add(24);
                    break;
                case PerkType.Scout:
                    Game1.player.professions.Add(25);
                    break;
                case PerkType.Gemologist:
                    Game1.player.professions.Add(23);
                    break;
                case PerkType.Miner:
                    Game1.player.professions.Add(18);
                    break;
                case PerkType.Brute:
                    Game1.player.professions.Add(26);
                    break;
                case PerkType.Defender:
                    Roguelike.TrueMaxHP += 25;
                    Game1.player.health = Roguelike.TrueMaxHP;
                    break;
                case PerkType.Acrobat:
                    Game1.player.professions.Add(28);
                    break;
                case PerkType.Geologist:
                    Game1.player.professions.Add(19);
                    break;
                case PerkType.SpecialCharm:
                    Game1.player.addedLuckLevel.Value += 1;
                    break;
                case PerkType.Swift:
                    Game1.player.addedSpeed += 1;
                    break;
                case PerkType.Immunity:
                    Game1.player.immunity += 2;
                    break;
                case PerkType.RabbitsFoot:
                    Game1.player.addedLuckLevel.Value += 3;
                    break;
            }

            ActivePerks.Add(perkType);
        }

        public static void RemovePerk(PerkType perkType)
        {
            if (!HasPerk(perkType))
                return;

            switch (perkType)
            {
                case PerkType.Fighter:
                    Game1.player.professions.Remove(24);
                    break;
                case PerkType.Scout:
                    Game1.player.professions.Remove(25);
                    break;
                case PerkType.Gemologist:
                    Game1.player.professions.Remove(23);
                    break;
                case PerkType.Miner:
                    Game1.player.professions.Remove(18);
                    break;
                case PerkType.Brute:
                    Game1.player.professions.Remove(26);
                    break;
                case PerkType.Defender:
                    Roguelike.TrueMaxHP = Math.Max(0, Roguelike.TrueMaxHP - 25);
                    if (Curse.HasCurse(CurseType.GlassCannon))
                        Game1.player.health = Math.Min(Roguelike.TrueMaxHP / 2, Game1.player.health);
                    else
                        Game1.player.health = Math.Min(Roguelike.TrueMaxHP, Game1.player.health);
                    break;
                case PerkType.Acrobat:
                    Game1.player.professions.Remove(28);
                    break;
                case PerkType.Geologist:
                    Game1.player.professions.Remove(19);
                    break;
                case PerkType.SpecialCharm:
                    Game1.player.addedLuckLevel.Value -= 1;
                    break;
                case PerkType.Swift:
                    Game1.player.addedSpeed -= 1;
                    break;
                case PerkType.Immunity:
                    Game1.player.immunity -= 2;
                    break;
                case PerkType.RabbitsFoot:
                    Game1.player.addedLuckLevel.Value -= 3;
                    break;
            }

            ActivePerks.Remove(perkType);
        }

        public static bool HasPerk(PerkType perkType)
        {
            return ActivePerks.Contains(perkType);
        }

        public static bool HasAllPerks()
        {
            return ActivePerks.Count == TotalPerkCount;
        }

        public static void RemoveAllPerks()
        {
            foreach (PerkType perkType in Enum.GetValues(typeof(PerkType)))
                RemovePerk(perkType);
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Berisan/SpawnMonsters
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace Spawn_Monsters.Monsters
{
    public class MonsterData
    {

        public enum Monster
        {
            GreenSlime,
            BlueSlime,
            RedSlime,
            PurpleSlime,
            YellowSlime,
            BlackSlime,
            GraySlime,
            Bat,
            FrostBat,
            LavaBat,
            IridiumBat,
            HauntedSkull,
            CursedDoll,
            Bug,
            ArmoredBug,
            Duggy,
            DustSpirit,
            Fly,
            MutantFly,
            Ghost,
            CarbonGhost,
            Grub,
            MutantGrub,
            MetalHead,
            Mummy,
            RockCrab,
            LavaCrab,
            IridiumCrab,
            StoneGolem,
            WildernessGolem,
            Serpent,
            ShadowBrute,
            ShadowShaman,
            Skeleton,
            SquidKid,
            PepperRex,
            BigSlime,
            HotHead,
            RoyalSerpent,
            PutridGhost,
            BlueSquid,
            DwarvishSentry,
            Shooter,
            Spider,
            LavaLurk,
            MagmaSprite,
            MagmaSparker,
            SkeletonMage,
            FalseMagmaCap,
            Spiker,
            StickBug,
            TigerSlime,
            MagmaDuggy,
            PrismaticSlime
        }

        private static readonly Dictionary<Monster, MonsterData> data = new Dictionary<Monster, MonsterData>() {
            { Monster.GreenSlime, new MonsterData("Green Slime", typeof(GreenSlime), new object[]{ null, 0 }, "Green Slime", 16, 24, 0, 4, 100, Color.Lime) },
            { Monster.BlueSlime, new MonsterData("Frost Jelly", typeof(GreenSlime), new object[]{ null, 40 }, "Green Slime", 16, 24, 0, 4, 100, Color.DarkTurquoise) },
            { Monster.RedSlime, new MonsterData("Red Sludge", typeof(GreenSlime), new object[]{ null, 80 }, "Green Slime", 16, 24, 0, 4, 100, Color.Red) },
            { Monster.PurpleSlime, new MonsterData("Purple Sludge", typeof(GreenSlime), new object[]{ null, 121 }, "Green Slime", 16, 24, 0, 4, 100, Color.Purple) },
            { Monster.YellowSlime, new MonsterData("Yellow Slime", typeof(GreenSlime), new object[]{ null, new Color(255,255,50) }, "Green Slime", 16, 24, 0, 4, 100, Color.Yellow) },
            { Monster.BlackSlime, new MonsterData("Black Slime", typeof(GreenSlime), new object[]{ null, null }, "Green Slime", 16, 24, 0, 4, 100, Color.Black) },
            { Monster.GraySlime, new MonsterData("Gray Sludge", typeof(GreenSlime), new object[]{ null, 77377 }, "Green Slime", 16, 24, 0, 4, 100, Color.Gray) },

            { Monster.Bat, new MonsterData("Bat", typeof(Bat), new object[] { null }, "Bat") },
            { Monster.FrostBat, new MonsterData("Frost Bat", typeof(Bat), new object[] { null, 40 }, "Frost Bat") },
            { Monster.LavaBat, new MonsterData("Lava Bat", typeof(Bat), new object[] { null, 80 }, "Lava Bat")},
            { Monster.IridiumBat, new MonsterData("Iridium Bat", typeof(Bat), new object[] { null, 171}, "Iridium Bat") },
            { Monster.HauntedSkull, new MonsterData("Haunted Skull", typeof(Bat), new object[] { null, 77377 }, "Haunted Skull", 16, 16, 4, 2)},
            { Monster.CursedDoll, new MonsterData("Cursed Doll", typeof(Bat), new object[] { null, -666 }, "", 16, 16) },

            { Monster.Bug, new MonsterData("Bug", typeof(Bug), new object[] { null, 0 }, "Bug", 16, 16) },
            { Monster.ArmoredBug, new MonsterData("Armored Bug", typeof(Bug), new object[] { null, 121 }, "Armored Bug", 16, 16) },

            { Monster.Duggy, new MonsterData("Duggy", typeof(DuggyFixed), new object[] { null }, "Duggy", 16, 24, 0, 9) },

            { Monster.DustSpirit, new MonsterData("Dust Sprite", typeof(DustSpirit), new object[] { null }, "Dust Spirit") },

            { Monster.Fly, new MonsterData("Cave Fly", typeof(Fly), new object[] { null }, "Fly") },
            { Monster.MutantFly, new MonsterData("Mutant Fly", typeof(Fly), new object[] { null, true }, "Fly", 16, 24, 0, 4, 100, Color.Lime) },

            { Monster.Ghost, new MonsterData("Ghost", typeof(Ghost), new object[] { null }, "Ghost") },
            { Monster.CarbonGhost, new MonsterData("Carbon Ghost", typeof(Ghost), new object[] { null, "Carbon Ghost" }, "Carbon Ghost") },

            { Monster.Grub, new MonsterData("Grub", typeof(Grub), new object[] { null }, "Grub") },
            { Monster.MutantGrub, new MonsterData("Mutant Grub", typeof(Grub), new object[] { null, true }, "Grub", 16, 24, 0, 4, 100, Color.Lime) },

            { Monster.MetalHead, new MonsterData("Metal Head", typeof(MetalHead), new object[] { null, 80 }, "Metal Head", 16, 16) },

            { Monster.Mummy, new MonsterData("Mummy", typeof(Mummy), new object[] { null }, "Mummy", 16, 32) },

            { Monster.RockCrab, new MonsterData("Rock Crab", typeof(RockCrab), new object[] { null }, "Rock Crab") },
            { Monster.LavaCrab, new MonsterData("Lava Crab", typeof(LavaCrab), new object[] { null }, "Lava Crab") },
            { Monster.IridiumCrab, new MonsterData("Iridium Crab", typeof(RockCrab), new object[] { null, "Iridium Crab" }, "Iridium Crab") },

            { Monster.StoneGolem, new MonsterData("Stone Golem", typeof(RockGolem), new object[] { null }, "Stone Golem") },
            { Monster.WildernessGolem, new MonsterData("Wilderness Golem", typeof(RockGolem), new object[] { null, 5 }, "Wilderness Golem") },

            { Monster.PepperRex, new MonsterData("Pepper Rex", typeof(DinoMonster), new object[] { null }, "Pepper Rex", 32, 32) },

            { Monster.BigSlime, new MonsterData("Big Slime", typeof(BigSlime), new object[]{ null, 0 }, "Big Slime", 32, 32, 0, 4, 100, Color.Lime)},

            { Monster.Serpent, new MonsterData("Serpent", typeof(Serpent), new object[] { null }, "Serpent", 32, 32, 0, 9) },

            { Monster.ShadowBrute, new MonsterData("Shadow Brute", typeof(ShadowBrute), new object[] { null }, "Shadow Brute", 16, 32) },
            { Monster.ShadowShaman, new MonsterData("Shadow Shaman", typeof(ShadowShaman), new object[] { null }, "Shadow Shaman") },

            { Monster.Skeleton, new MonsterData("Skeleton", typeof(Skeleton), new object[] { null, false }, "Skeleton", 16, 32) },

            { Monster.SquidKid, new MonsterData("Squid Kid", typeof(SquidKid), new object[] { null }, "Squid Kid", 16, 16) },


            { Monster.DwarvishSentry, new MonsterData("Dwarvish Sentry", typeof(DwarvishSentry), new object[] { null }, "Dwarvish Sentry", 16, 16) },
            { Monster.FalseMagmaCap, new MonsterData("False Magma Cap", typeof(RockCrab), new object[] { null, "False Magma Cap" }, "False Magma Cap") },
            { Monster.HotHead, new MonsterData("Hot Head", typeof(HotHead), new object[] { null }, "Hot Head", 16, 16) },
            { Monster.LavaLurk, new MonsterData("Lava Lurk", typeof(LavaLurk), new object[] { null }, "Lava Lurk", 16, 16) },
            { Monster.MagmaSprite, new MonsterData("Magma Sprite", typeof(Bat), new object[] { null, -555 }, "Magma Sprite", 16, 16) },
            { Monster.StickBug, new MonsterData("Stick Bug", typeof(RockCrab), new object[] { null }, "Stick Bug") },
            { Monster.MagmaSparker, new MonsterData("Magma Sparker", typeof(Bat), new object[] { null, -556 }, "Magma Sparker", 16, 16) },
            { Monster.MagmaDuggy, new MonsterData("Magma Duggy", typeof(DuggyFixed), new object[] {null, true }, "Magma Duggy")},
            { Monster.Spiker, new MonsterData("Spiker", typeof(Spiker), new object[] { null, 0 }, "Spiker", 16, 16) },
            { Monster.TigerSlime, new MonsterData("Tiger Slime", typeof(GreenSlime), new object[] { null, 0 }, "Tiger Slime")},


            { Monster.Shooter, new MonsterData("Shadow Sniper", typeof(Shooter), new object[] { null }, "Shadow Sniper", 32, 32) },
            { Monster.SkeletonMage, new MonsterData("Skeleton Mage", typeof(Skeleton), new object[] { null, true }, "Skeleton Mage", 16, 32) },
            { Monster.Spider, new MonsterData("Spider", typeof(Leaper), new object[] { null }, "Spider", 32, 32, 0, 2) },
            { Monster.PutridGhost, new MonsterData("Putrid Ghost", typeof(Ghost), new object[] { null, "Putrid Ghost" }, "Putrid Ghost") },
            { Monster.BlueSquid, new MonsterData("Blue Squid", typeof(BlueSquid), new object[] { null }, "Blue Squid", 24, 24) },
            { Monster.RoyalSerpent, new MonsterData("Royal Serpent", typeof(Serpent), new object[] { null, "Royal Serpent" }, "Royal Serpent", 32, 32) },

            { Monster.PrismaticSlime, new MonsterData("Prismatic Slime", typeof(GreenSlime), new object[]{ null, 0 }, "Green Slime")},
        };


        public string Displayname { get; }
        public Type Type { get; }
        public object[] ConstructorArgs { get; }

        public string Texturename { get; }
        public int Texturewidth { get; }
        public int Textureheight { get; }
        public int StartingFrame { get; }
        public int NumberOfFrames { get; }
        public int AnimatingInterval { get; }
        public Color TextureColor { get; }

        public int PosShiftX { get; }
        public int PosShiftY { get; }

        public MonsterData(string displayname, Type type, object[] constructorArgs, string texturename, int texturewidth = 16, int textureheight = 24, int startingFrame = 0, int numberOfFrames = 4, int animatingInterval = 100, Color textureColor = default, int posShiftX = 0, int posShiftY = 0) {
            Displayname = displayname;
            Type = type;
            ConstructorArgs = constructorArgs;
            Texturename = texturename;
            Texturewidth = texturewidth;
            Textureheight = textureheight;
            StartingFrame = startingFrame;
            NumberOfFrames = numberOfFrames;
            AnimatingInterval = animatingInterval;
            TextureColor = textureColor;
            PosShiftX = posShiftX;
            PosShiftY = posShiftY;
        }

        public static MonsterData GetMonsterData(Monster monster) {
            if (data.TryGetValue(monster, out MonsterData monsterdata)) {
                return monsterdata;
            }
            return null;
        }

        public ClickableMonsterComponent ToClickableMonsterComponent(Monster monster) {
            return new ClickableMonsterComponent(monster, Texturename, 0, 0, monster == Monster.Spider ? 16 * 4 : Texturewidth * 4,  monster == Monster.Spider ? 16 * 4 : Textureheight * 4, Texturewidth, Textureheight, StartingFrame, NumberOfFrames, AnimatingInterval, TextureColor);
        }

        public static List<ClickableMonsterComponent> ToClickableMonsterComponents() {
            List<ClickableMonsterComponent> components = new List<ClickableMonsterComponent>();
            foreach (KeyValuePair<Monster, MonsterData> keyValuePair in data) {
                components.Add(keyValuePair.Value.ToClickableMonsterComponent(keyValuePair.Key));
            }
            return components;
        }

        public static Monster ForName(String name) {
            foreach (KeyValuePair<Monster, MonsterData> keyValuePair in data) {
                if (name.Equals(keyValuePair.Value.Displayname)) {
                    return keyValuePair.Key;
                }
            }
            return (Monster)(-1);
        }
    }
}

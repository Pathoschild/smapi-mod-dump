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
using System.Linq;

namespace Spawn_Monsters.Monsters
{
    public class MonsterData {

        public enum Monster {
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
            PrismaticSlime,
            IridiumGolem
        }

        static MonsterData() {

            var slimeBuilder = new Builder()
                .WithMonsterType(typeof(GreenSlime))
                .WithTextureName("Green Slime");

            data.Add(Monster.GreenSlime, slimeBuilder
                .WithDisplayname("Green Slime")
                .WithSecondConstructorArg(0)
                .WithTextureColor(Color.Lime)
                .Build());

            data.Add(Monster.BlueSlime, slimeBuilder
                .WithDisplayname("Frost Jelly")
                .WithSecondConstructorArg(40)
                .WithTextureColor(Color.DarkTurquoise)
                .Build());

            data.Add(Monster.RedSlime, slimeBuilder
                .WithDisplayname("Red Sludge")
                .WithSecondConstructorArg(80)
                .WithTextureColor(Color.Red)
                .Build());

            data.Add(Monster.PurpleSlime, slimeBuilder
                .WithDisplayname("Purple Sludge")
                .WithSecondConstructorArg(121)
                .WithTextureColor(Color.Purple)
                .Build());

            data.Add(Monster.YellowSlime, slimeBuilder
                .WithDisplayname("Yellow Slime")
                .WithSecondConstructorArg(new Color(255, 255, 50))
                .WithTextureColor(Color.Yellow)
                .Build());

            data.Add(Monster.BlackSlime, slimeBuilder
                .WithDisplayname("Black Slime")
                .WithSecondConstructorArg(null)
                .WithTextureColor(Color.Black)
                .Build());

            data.Add(Monster.GraySlime, slimeBuilder
                .WithDisplayname("Gray Sludge")
                .WithSecondConstructorArg(77377)
                .WithTextureColor(Color.Gray)
                .Build());

            data.Add(Monster.TigerSlime, slimeBuilder
                .WithDisplayname("Tiger Slime")
                .WithTextureName("Tiger Slime")
                .WithSecondConstructorArg(0)
                .Build());

            data.Add(Monster.PrismaticSlime, slimeBuilder
                .WithDisplayname("Prismatic Slime")
                .WithSecondConstructorArg(0)
                .Build());

            var batBuilder = new Builder()
                .WithMonsterType(typeof(Bat));

            data.Add(Monster.Bat, batBuilder
                .WithDisplayname("Bat")
                .WithTextureName("Bat")
                .Build());

            data.Add(Monster.FrostBat, batBuilder
                .WithDisplayname("Frost Bat")
                .WithTextureName("Frost Bat")
                .WithSecondConstructorArg(40)
                .Build());

            data.Add(Monster.LavaBat, batBuilder
                .WithDisplayname("Lava Bat")
                .WithTextureName("Lava Bat")
                .WithSecondConstructorArg(80)
                .Build());

            data.Add(Monster.IridiumBat, batBuilder
                .WithDisplayname("Iridium Bat")
                .WithTextureName("Iridium Bat")
                .WithSecondConstructorArg(171)
                .Build());

            data.Add(Monster.HauntedSkull, batBuilder
                .WithDisplayname("Haunted Skull")
                .WithTextureName("Haunted Skull")
                .WithSecondConstructorArg(77377)
                .WithTextureHeight(16)
                .AnimationStartingAtFrame(4)
                .AnimationLastsFor(2)
                .Build());

            data.Add(Monster.CursedDoll, batBuilder
                .WithDisplayname("Cursed Doll")
                .WithSecondConstructorArg(-666)
                .WithTextureHeight(16)
                .Build());


            data.Add(Monster.MagmaSprite, new Builder()
                .WithMonsterType(typeof(Bat))
                .WithDisplayname("Magma Sprite")
                .WithTextureName("Magma Sprite")
                .WithTextureHeight(16)
                .WithSecondConstructorArg(-555)
                .Build());

            data.Add(Monster.MagmaSparker, new Builder()
                .WithMonsterType(typeof(Bat))
                .WithDisplayname("Magma Sparker")
                .WithTextureName("Magma Sparker")
                .WithTextureHeight(16)
                .WithSecondConstructorArg(-556)
                .Build());

            var bugBuilder = new Builder()
                .WithMonsterType(typeof(Bug))
                .WithTextureHeight(16);

            data.Add(Monster.Bug, bugBuilder
                .WithDisplayname("Bug")
                .WithTextureName("Bug")
                .WithSecondConstructorArg(0)
                .Build());

            data.Add(Monster.ArmoredBug, bugBuilder
                .WithDisplayname("Armored Bug")
                .WithTextureName("Armored Bug")
                .WithSecondConstructorArg(121)
                .Build());

            data.Add(Monster.Duggy, new Builder()
                .WithMonsterType(typeof(DuggyFixed))
                .WithDisplayname("Duggy")
                .WithTextureName("Duggy")
                .AnimationLastsFor(9)
                .Build());

            data.Add(Monster.DustSpirit, new Builder()
                .WithMonsterType(typeof(DustSpirit))
                .WithDisplayname("Dust Sprite")
                .WithTextureName("Dust Spirit")
                .Build());

            var flyBuilder = new Builder()
                .WithMonsterType(typeof(Fly))
                .WithTextureName("Fly");

            data.Add(Monster.Fly, flyBuilder
                .WithDisplayname("Cave Fly")
                .Build());

            data.Add(Monster.MutantFly, flyBuilder
                .WithDisplayname("Mutant Fly")
                .WithTextureColor(Color.Lime)
                .WithSecondConstructorArg(true)
                .Build());

            var ghostBuilder = new Builder()
                .WithMonsterType(typeof(Ghost));

            data.Add(Monster.Ghost, ghostBuilder
                .WithTextureName("Ghost")
                .WithDisplayname("Ghost")
                .Build());

            data.Add(Monster.CarbonGhost, ghostBuilder
                .WithTextureName("Carbon Ghost")
                .WithDisplayname("Carbon Ghost")
                .WithSecondConstructorArg("Carbon Ghost")
                .Build());

            data.Add(Monster.PutridGhost, ghostBuilder
                .WithDisplayname("Putrid Ghost")
                .WithTextureName("Putrid Ghost")
                .WithSecondConstructorArg("Putrid Ghost")
                .Build());

            var grubBuilder = new Builder()
                .WithMonsterType(typeof(Grub))
                .WithTextureName("Grub");


            data.Add(Monster.Grub, grubBuilder
                .WithDisplayname("Grub")
                .Build());

            data.Add(Monster.MutantGrub, grubBuilder
                .WithDisplayname("Mutant Grub")
                .WithTextureColor(Color.Lime)
                .WithSecondConstructorArg(true)
                .Build());

            data.Add(Monster.MetalHead, new Builder()
                .WithMonsterType(typeof(MetalHead))
                .WithDisplayname("Metal Head")
                .WithTextureName("Metal Head")
                .WithSecondConstructorArg(80)
                .WithTextureHeight(16)
                .Build());

            data.Add(Monster.Mummy, new Builder()
                .WithMonsterType(typeof(Mummy))
                .WithDisplayname("Mummy")
                .WithTextureName("Mummy")
                .WithTextureHeight(32)
                .Build());

            var rockCrabBuilder = new Builder()
                .WithMonsterType(typeof(RockCrab));

            data.Add(Monster.RockCrab, rockCrabBuilder
                .WithDisplayname("Rock Crab")
                .WithTextureName("Rock Crab")
                .WithSecondConstructorArg("Rock Crab")
                .Build());

            data.Add(Monster.LavaCrab, rockCrabBuilder
                .WithDisplayname("Lava Crab")
                .WithTextureName("Lava Crab")
                .WithSecondConstructorArg("Lava Crab")
                .Build());

            data.Add(Monster.IridiumCrab, rockCrabBuilder
                .WithDisplayname("Iridium Crab")
                .WithTextureName("Iridium Crab")
                .WithSecondConstructorArg("Iridium Crab")
                .Build());

            data.Add(Monster.FalseMagmaCap, rockCrabBuilder
                .WithDisplayname("False Magma Cap")
                .WithTextureName("False Magma Cap")
                .WithSecondConstructorArg("False Magma Cap")
                .Build());

            var golemBuilder = new Builder()
                .WithMonsterType(typeof(RockGolem));

            data.Add(Monster.StoneGolem, golemBuilder
                .WithDisplayname("Stone Golem")
                .WithTextureName("Stone Golem")
                .Build());

            data.Add(Monster.WildernessGolem, golemBuilder
                .WithDisplayname("Wilderness Golem")
                .WithTextureName("Wilderness Golem")
                .WithSecondConstructorArg(5)
                .Build());

            // Iridium Golem Constructor checks RNG and farm type, probably needs custom recreation
            // data.Add(Monster.IridiumGolem, golemBuilder
            //    .WithDisplayname("Iridium Golem")
            //    .WithTextureName("Iridium Golem")
            //    .WithSecondConstructorArg(10)
            //    .Build());

            data.Add(Monster.PepperRex, new Builder()
                .WithMonsterType(typeof(DinoMonster))
                .WithDisplayname("Pepper Rex")
                .WithTextureName("Pepper Rex")
                .WithTextureWidth(32)
                .WithTextureHeight(32)
                .Build());

            data.Add(Monster.BigSlime, new Builder()
                .WithMonsterType(typeof(BigSlime))
                .WithDisplayname("Big Slime")
                .WithTextureName("Big Slime")
                .WithSecondConstructorArg(0)
                .WithTextureWidth(32)
                .WithTextureHeight(32)
                .WithTextureColor(Color.Lime)
                .Build());

            var serpentBuilder = new Builder()
                .WithMonsterType(typeof(Serpent))
                .WithTextureWidth(32)
                .WithTextureHeight(32)
                .AnimationLastsFor(9);

            data.Add(Monster.Serpent, serpentBuilder
                .WithDisplayname("Serpent")
                .WithTextureName("Serpent")
                .WithSecondConstructorArg("Serpent")
                .Build());

            data.Add(Monster.RoyalSerpent, serpentBuilder
                .WithDisplayname("Royal Serpent")
                .WithTextureName("Royal Serpent")
                .WithSecondConstructorArg("Royal Serpent")
                .Build());

            data.Add(Monster.ShadowBrute, new Builder()
                .WithMonsterType(typeof(ShadowBrute))
                .WithDisplayname("Shadow Brute")
                .WithTextureName("Shadow Brute")
                .WithTextureHeight(32)
                .Build());

            data.Add(Monster.ShadowShaman, new Builder()
                .WithMonsterType(typeof(ShadowShaman))
                .WithDisplayname("Shadow Shaman")
                .WithTextureName("Shadow Shaman")
                .Build());

            data.Add(Monster.Skeleton, new Builder()
                .WithMonsterType(typeof(Skeleton))
                .WithDisplayname("Skeleton")
                .WithTextureName("Skeleton")
                .WithTextureHeight(32)
                .WithSecondConstructorArg(false)
                .Build());

            data.Add(Monster.SquidKid, new Builder()
                .WithMonsterType(typeof(SquidKid))
                .WithDisplayname("Squid Kid")
                .WithTextureName("Squid Kid")
                .WithTextureHeight(16)
                .Build());

            data.Add(Monster.DwarvishSentry, new Builder()
                .WithMonsterType(typeof(DwarvishSentry))
                .WithDisplayname("Dwarvish Sentry")
                .WithTextureName("Dwarvish Sentry")
                .WithTextureHeight(16)
                .Build());

            data.Add(Monster.HotHead, new Builder()
                .WithMonsterType(typeof(HotHead))
                .WithDisplayname("Hot Head")
                .WithTextureName("Hot Head")
                .WithTextureHeight(16)
                .Build());

            data.Add(Monster.LavaLurk, new Builder()
                .WithMonsterType(typeof(LavaLurk))
                .WithDisplayname("Lava Lurk")
                .WithTextureName("Lava Lurk")
                .WithTextureHeight(16)
                .Build());

            data.Add(Monster.StickBug, new Builder()
                .WithMonsterType(typeof(RockCrab))
                .WithDisplayname("Stick Bug")
                .WithTextureName("Stick Bug")
                .Build());


            data.Add(Monster.MagmaDuggy, new Builder()
                .WithMonsterType(typeof(DuggyFixed))
                .WithDisplayname("Magma Duggy")
                .WithTextureName("Magma Duggy")
                .WithSecondConstructorArg(true)
                .Build());

            data.Add(Monster.Spiker, new Builder()
                .WithMonsterType(typeof(Spiker))
                .WithDisplayname("Spiker")
                .WithTextureName("Spiker")
                .WithTextureHeight(16)
                .WithSecondConstructorArg(0)
                .Build());

            data.Add(Monster.Shooter, new Builder()
                .WithMonsterType(typeof(Shooter))
                .WithDisplayname("Shadow Sniper")
                .WithTextureName("Shadow Sniper")
                .WithTextureWidth(32)
                .WithTextureHeight(32)
                .Build());

            data.Add(Monster.SkeletonMage, new Builder()
                .WithMonsterType(typeof(Skeleton))
                .WithDisplayname("Skeleton Mage")
                .WithTextureName("Skeleton Mage")
                .WithTextureHeight(32)
                .WithSecondConstructorArg(true)
                .Build());

            data.Add(Monster.Spider, new Builder()
                .WithMonsterType(typeof(Leaper))
                .WithDisplayname("Spider")
                .WithTextureName("Spider")
                .WithTextureWidth(32)
                .WithTextureHeight(32)
                .AnimationLastsFor(2)
                .Build());

            data.Add(Monster.BlueSquid, new Builder()
                .WithMonsterType(typeof(BlueSquid))
                .WithDisplayname("Blue Squid")
                .WithTextureName("Blue Squid")
                .WithTextureWidth(24)
                .Build());
        }

        private static readonly Dictionary<Monster, MonsterData> data = new Dictionary<Monster, MonsterData>();


        public string Displayname { get; }
        public Type Type { get; }
        public object SecondConstructorArg { get; }

        public string Texturename { get; }
        public int Texturewidth { get; }
        public int Textureheight { get; }
        public int StartingFrame { get; }
        public int NumberOfFrames { get; }
        public int AnimatingInterval { get; }
        public Color TextureColor { get; }

        public int PosShiftX { get; }
        public int PosShiftY { get; }

        private MonsterData(string displayname, Type type, object secondConstructorArg, string texturename, int texturewidth = 16, int textureheight = 24, int startingFrame = 0, int numberOfFrames = 4, int animatingInterval = 100, Color textureColor = default, int posShiftX = 0, int posShiftY = 0) {
            Displayname = displayname;
            Type = type;
            SecondConstructorArg = secondConstructorArg;
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

        private class Builder
        {
            public Builder() {
                // Hide Constructor
            }

            public string Displayname;
            public Type Type;
            public object ConstructorArg;

            public string Texturename;
            public int Texturewidth = 16;
            public int Textureheight = 24;
            public int StartingFrame = 0;
            public int NumberOfFrames = 4;
            public int AnimatingInterval = 100;
            public Color TextureColor = default;

            public int PosShiftX = 0;
            public int PosShiftY = 0;

            public Builder WithDisplayname(string displayname) {
                var newBuilder = Clone();
                newBuilder.Displayname = displayname;
                return newBuilder;
            }

            public Builder WithMonsterType(Type type) {
                var newBuilder = Clone();
                newBuilder.Type = type;
                return newBuilder;
            }

            public Builder WithSecondConstructorArg(Object arg) {
                var newBuilder = Clone();
                newBuilder.ConstructorArg = arg;
                return newBuilder;
            }

            public Builder WithTextureName(string texturename) {
                var newBuilder = Clone();
                newBuilder.Texturename = texturename;
                return newBuilder;
            }

            public Builder WithTextureWidth(int width) {
                var newBuilder = Clone();
                newBuilder.Texturewidth = width;
                return newBuilder;
            }

            public Builder WithTextureHeight(int height) {
                var newBuilder = Clone();
                newBuilder.Textureheight = height;
                return newBuilder;
            }

            public Builder AnimationStartingAtFrame(int startingFrame) {
                var newBuilder = Clone();
                newBuilder.StartingFrame = startingFrame;
                return newBuilder;
            }

            public Builder AnimationLastsFor(int frameAmount) {
                var newBuilder = Clone();
                newBuilder.NumberOfFrames = frameAmount;
                return newBuilder;
            }

            public Builder TimeBetweenFrames(int time) {
                var newBuilder = Clone();
                newBuilder.AnimatingInterval = time;
                return newBuilder;
            }

            public Builder WithTextureColor(Color color) {
                var newBuilder = Clone();
                newBuilder.TextureColor = color;
                return newBuilder;
            }

            public Builder WithTextureOffsetX(int offset) {
                var newBuilder = Clone();
                newBuilder.PosShiftX = offset;
                return newBuilder;
            }

            public Builder WithTextureOffsetY(int offset) {
                var newBuilder = Clone();
                newBuilder.PosShiftY = offset;
                return newBuilder;
            }

            public MonsterData Build() {
                return new MonsterData(Displayname, Type, ConstructorArg, Texturename, Texturewidth, Textureheight, StartingFrame, NumberOfFrames, AnimatingInterval, TextureColor, PosShiftX, PosShiftY);
            }

            public Builder Clone() {
                return (Builder)this.MemberwiseClone();
            }
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;

namespace CryptOfTheNecrodancerEnemies.Framework.Constants {

  internal static class SpawnMonster {

    private static T Spawn<T>(T monster, bool isDangerous = default) where T : Monster {
      if (isDangerous) {
        monster.Sprite.LoadTexture(monster.Sprite.Texture.Name + "_dangerous");
      }
      monster.Position *= Game1.tileSize;
      Game1.currentLocation.characters.Add(monster);
      return monster;
    }

    #region Angry Roger
    [Obsolete("Unused enemy.")]
    public static AngryRoger AngryRoger(Vector2 position, string name = "Angry Roger") {
      return Spawn(new AngryRoger(position, name));
    }
    #endregion

    #region Bat
    public static Bat Bat(Vector2 position, int mineLevel = default, bool isDangerous = default) {
      return Spawn(new Bat(position, mineLevel), isDangerous);
    }

    public static Bat BatDangerous(Vector2 position) {
      return Bat(position, isDangerous: true);
    }

    public static Bat FrostBat(Vector2 position, bool isDangerous = false) {
      return Bat(position, 79, isDangerous);
    }
    public static Bat FrostBatDangerous(Vector2 position) {
      return FrostBat(position, isDangerous: true);
    }

    public static Bat LavaBat(Vector2 position) {
      return Bat(position, 80);
    }

    public static Bat IridiumBat(Vector2 position) {
      return Bat(position, 171);
    }

    public static Bat CursedDoll(Vector2 position) {
      return Bat(position, -666);
    }

    public static Bat MagmaSprite(Vector2 position) {
      return Bat(position, -555);
    }

    public static Bat MagmaSparker(Vector2 position) {
      return Bat(position, -556);
    }

    public static Bat HauntedSkull(Vector2 position, bool isDangerous = default) {
      return Bat(position, 77377, isDangerous: isDangerous);
    }

    public static Bat HauntedSkullDangerous(Vector2 position) {
      return HauntedSkull(position, isDangerous: true);
    }
    #endregion

    #region Big Slime
    public static BigSlime BigSlime(Vector2 position, int mineLevel = -1) {
      return Spawn(new BigSlime(position, mineLevel));
    }

    public static BigSlime BigSlimeGreen(Vector2 position) {
      return BigSlime(position, 10);
    }

    public static BigSlime BigSlimeBlue(Vector2 position) {
      return BigSlime(position, 40);
    }

    public static BigSlime BigSlimeRed(Vector2 position) {
      return BigSlime(position, 80);
    }
    public static BigSlime BigSlimePurple(Vector2 position) {
      return BigSlime(position, 121);
    }
    #endregion

    #region Blue Squid
    public static BlueSquid BlueSquid(Vector2 position) {
      return Spawn(new BlueSquid(position));
    }
    #endregion

    #region Bug
    public static Bug Bug(Vector2 position, int mineLevel = default, bool isDangerous = default) {
      return Spawn(new Bug(position, mineLevel), isDangerous);
    }

    public static Bug BugDangerous(Vector2 position) {
      return Bug(position, isDangerous: true);
    }

    public static Bug ArmoredBug(Vector2 position, bool isDangerous = default) {
      return Bug(position, 121, isDangerous: isDangerous);
    }

    public static Bug ArmoredBugDangerous(Vector2 position) {
      return ArmoredBug(position, isDangerous: true);
    }
    #endregion

    #region Dust Spirit
    public static DustSpirit DustSpirit(Vector2 position, bool isDangerous = default) {
      return Spawn(new DustSpirit(position), isDangerous);
    }

    public static DustSpirit DustSpiritDangerous(Vector2 position) {
      return DustSpirit(position, isDangerous: true);
    }
    #endregion

    #region Dwarvish Sentry
    public static DwarvishSentry DwarvishSentry(Vector2 position) {
      return Spawn(new DwarvishSentry(position));
    }
    #endregion

    #region Fly
    public static Fly Fly(Vector2 position, bool isDangerous = default) {
      return Spawn(new Fly(position), isDangerous);
    }

    public static Fly FlyDangerous(Vector2 position) {
      return Fly(position, isDangerous: true);
    }
    #endregion

    #region Slime
    public static GreenSlime GreenSlime(Vector2 position, int mineLevel = default) {
      return Spawn(new GreenSlime(position, mineLevel));
    }

    public static GreenSlime FrostSlime(Vector2 position) {
      return GreenSlime(position, 40);
    }

    public static GreenSlime SludgeSlime(Vector2 position) {
      return GreenSlime(position, 121);
    }

    public static GreenSlime TigerSlime(Vector2 position) {
      var monster = GreenSlime(position);
      monster.makeTigerSlime();
      return monster;
    }

    public static GreenSlime PrismaticSlime(Vector2 position) {
      var monster = GreenSlime(position);
      monster.makePrismatic();
      return monster;
    }
    #endregion

    #region Ghost
    public static Ghost Ghost(Vector2 position, string name = "Ghost") {
      return Spawn(new Ghost(position, name));
    }

    public static Ghost CarbonGhost(Vector2 position) {
      return Ghost(position, "Carbon Ghost");
    }

    public static Ghost PutridGhost(Vector2 position) {
      return Ghost(position, "Putrid Ghost");
    }
    #endregion

    #region Spider
    public static Leaper Spider(Vector2 position) {
      return Spawn(new Leaper(position));
    }
    #endregion

    #region Skeleton
    public static Skeleton Skeleton(Vector2 position, bool isMage = default, bool isDangerous = default) {
      return Spawn(new Skeleton(position, isMage), isDangerous);
    }

    public static Skeleton SkeletonDangerous(Vector2 position) {
      return Skeleton(position, isMage: false, isDangerous: true);
    }

    public static Skeleton SkeletonMage(Vector2 position, bool isDangerous = default) {
      return Skeleton(position, isMage: true, isDangerous: isDangerous);
    }

    public static Skeleton SkeletonMageDangerous(Vector2 position) {
      return SkeletonMage(position, isDangerous: true);
    }
    #endregion

    #region Pepper Rex
    public static DinoMonster PepperRex(Vector2 position) {
      return Spawn(new DinoMonster(position));
    }
    #endregion

    #region Duggy
    public static Duggy Duggy(Vector2 position, bool isDangerous = default) {
      return Spawn(new Duggy(position), isDangerous);
    }

    public static Duggy DuggyDangerous(Vector2 position) {
      return Spawn(new Duggy(position), isDangerous: true);
    }

    public static Duggy MagmaDuggy(Vector2 position) {
      return Spawn(new Duggy(position, magmaDuggy: true));
    }
    #endregion

    #region Grub
    public static Grub Grub(Vector2 position, bool isDangerous = default) {
      return Spawn(new Grub(position), isDangerous);
    }

    public static Grub GrubDangerous(Vector2 position) {
      return Grub(position, isDangerous: true);
    }
    #endregion

    #region Hot Head
    public static HotHead HotHead(Vector2 position) {
      return Spawn(new HotHead(position));
    }
    #endregion

    #region Lava Crab
    public static LavaCrab LavaCrab(Vector2 position, bool isDangerous = default) {
      return Spawn(new LavaCrab(position), isDangerous);
    }

    public static LavaCrab LavaCrabDangerous(Vector2 position) {
      return LavaCrab(position, isDangerous: true);
    }
    #endregion

    #region Lava Lurk
    public static LavaLurk LavaLurk(Vector2 position) {
      return Spawn(new LavaLurk(position));
    }
    #endregion

    #region Metal Head
    public static MetalHead MetalHead(Vector2 position, int mineLevel = default, bool isDangerous = default) {
      return Spawn(new MetalHead(position, mineLevel), isDangerous);
    }

    public static MetalHead MetalHeadDangerous(Vector2 position, int mineLevel = default) {
      return MetalHead(position, mineLevel, isDangerous: true);
    }

    public static MetalHead MetalHeadPurple(Vector2 position, bool isDangerous = default) {
      return MetalHead(position, 40, isDangerous);
    }

    public static MetalHead MetalHeadPurpleDangerous(Vector2 position) {
      return MetalHeadPurple(position, isDangerous: true);
    }
    #endregion

    #region Mummy
    public static Mummy Mummy(Vector2 position, bool isDangerous = default) {
      return Spawn(new Mummy(position), isDangerous);
    }

    public static Mummy MummyDangerous(Vector2 position) {
      return Mummy(position, isDangerous: true);
    }
    #endregion

    #region Rock Crab
    public static RockCrab RockCrab(Vector2 position, string name = "Rock Crab", bool isDangerous = default) {
      return Spawn(new RockCrab(position, name), isDangerous);
    }

    public static RockCrab RockCrabDangerous(Vector2 position) {
      return RockCrab(position, isDangerous: true);
    }

    public static RockCrab IridiumCrab(Vector2 position) {
      return RockCrab(position, "Iridium Crab");
    }

    public static RockCrab FalseMagmaCap(Vector2 position) {
      return RockCrab(position, "False Magma Cap");
    }

    public static RockCrab StickBug(Vector2 position) {
      var monster = RockCrab(position);
      monster.makeStickBug();
      return monster;
    }
    #endregion

    #region Rock Golem
    public static RockGolem StoneGolem(Vector2 position, bool alreadySpawned = default, bool isDangerous = default) {
      return Spawn(new RockGolem(position, alreadySpawned: alreadySpawned), isDangerous);
    }

    public static RockGolem StoneGolemDangerous(Vector2 position, bool alreadySpawned = default) {
      return StoneGolem(position, alreadySpawned: alreadySpawned, isDangerous: true);
    }

    public static RockGolem WildernessGolem(Vector2 position) {
      return Spawn(new RockGolem(position, difficultyMod: 0));
    }
    #endregion

    #region Serpent
    public static Serpent Serpent(Vector2 position, string name = "Serpent") {
      return Spawn(new Serpent(position, name));
    }

    public static Serpent RoyalSerpent(Vector2 position) {
      return Serpent(position, "Royal Serpent");
    }
    #endregion

    #region Shadow Brute
    public static ShadowBrute ShadowBrute(Vector2 position, bool isDangerous = default) {
      return Spawn(new ShadowBrute(position), isDangerous);
    }

    public static ShadowBrute ShadowBruteDangerous(Vector2 position) {
      return Spawn(new ShadowBrute(position), isDangerous: true);
    }
    #endregion

    #region Shadow Shaman
    public static ShadowShaman ShadowShaman(Vector2 position, bool isDangerous = default) {
      return Spawn(new ShadowShaman(position), isDangerous);
    }

    public static ShadowShaman ShadowShamanDangerous(Vector2 position) {
      return ShadowShaman(position, isDangerous: true);
    }
    #endregion

    #region Shadow Sniper
    public static Shooter ShadowSniper(Vector2 position, string name = "Shadow Sniper") {
      return Spawn(new Shooter(position, name));
    }

    [Obsolete("Unused enemy. It does not have sprite.")]
    public static Shooter SkeletonGunner(Vector2 position) {
      return ShadowSniper(position, "Skeleton Gunner");
    }
    #endregion

    #region Spiker
    public static Spiker Spiker(Vector2 position, int direction = default) {
      return Spawn(new Spiker(position, direction));
    }
    #endregion

    #region Squid Kid
    public static SquidKid SquidKid(Vector2 position, bool isDangerous = default) {
      return Spawn(new SquidKid(position), isDangerous);
    }

    public static SquidKid SquidKidDangerous(Vector2 position) {
      return SquidKid(position, isDangerous: true);
    }
    #endregion

    public static void OfType(Vector2 position, MonsterType monsterType) {
      switch (monsterType) {
#pragma warning disable CS0618 // Type or member is obsolete
        case MonsterType.AngryRoger:
          AngryRoger(position);
          break;
#pragma warning restore CS0618 // Type or member is obsolete
        case MonsterType.Bat:
          Bat(position);
          break;
        case MonsterType.BatDangerous:
          BatDangerous(position);
          break;
        case MonsterType.FrostBat:
          FrostBat(position);
          break;
        case MonsterType.FrostBatDangerous:
          FrostBatDangerous(position);
          break;
        case MonsterType.LavaBat:
          LavaBat(position);
          break;
        case MonsterType.IridiumBat:
          IridiumBat(position);
          break;
        case MonsterType.CursedDoll:
          CursedDoll(position);
          break;
        case MonsterType.SpawnMagmaSprite:
          MagmaSprite(position);
          break;
        case MonsterType.SpawnMagmaSparker:
          MagmaSparker(position);
          break;
        case MonsterType.SpawnHauntedSkull:
          HauntedSkull(position);
          break;
        case MonsterType.BigSlime:
          BigSlime(position);
          break;
        case MonsterType.BigSlimeGreen:
          BigSlimeGreen(position);
          break;
        case MonsterType.BigSlimeBlue:
          BigSlimeBlue(position);
          break;
        case MonsterType.BigSlimeRed:
          BigSlimeRed(position);
          break;
        case MonsterType.BigSlimePurple:
          BigSlimePurple(position);
          break;
        case MonsterType.BlueSquid:
          BlueSquid(position);
          break;
        case MonsterType.Bug:
          Bug(position);
          break;
        case MonsterType.BugDangerous:
          BugDangerous(position);
          break;
        case MonsterType.ArmoredBug:
          ArmoredBug(position);
          break;
        case MonsterType.ArmoredBugDangerous:
          ArmoredBugDangerous(position);
          break;
        case MonsterType.DustSpirit:
          DustSpirit(position);
          break;
        case MonsterType.DustSpiritDangerous:
          DustSpiritDangerous(position);
          break;
        case MonsterType.DwarvishSentry:
          DwarvishSentry(position);
          break;
        case MonsterType.Fly:
          Fly(position);
          break;
        case MonsterType.FlyDangerous:
          FlyDangerous(position);
          break;
        case MonsterType.GreenSlime:
          GreenSlime(position);
          break;
        case MonsterType.FrostSlime:
          FrostSlime(position);
          break;
        case MonsterType.SludgeSlime:
          SludgeSlime(position);
          break;
        case MonsterType.TigerSlime:
          TigerSlime(position);
          break;
        case MonsterType.PrismaticSlime:
          PrismaticSlime(position);
          break;
        case MonsterType.Ghost:
          Ghost(position);
          break;
        case MonsterType.CarbonGhost:
          CarbonGhost(position);
          break;
        case MonsterType.PutridGhost:
          PutridGhost(position);
          break;
        case MonsterType.Spider:
          Spider(position);
          break;
        case MonsterType.Skeleton:
          Skeleton(position);
          break;
        case MonsterType.SkeletonDangerous:
          SkeletonDangerous(position);
          break;
        case MonsterType.SkeletonMage:
          SkeletonMage(position);
          break;
        case MonsterType.SkeletonMageDangerous:
          SkeletonMageDangerous(position);
          break;
        case MonsterType.PepperRex:
          PepperRex(position);
          break;
        case MonsterType.Duggy:
          Duggy(position);
          break;
        case MonsterType.DuggyDangerous:
          DuggyDangerous(position);
          break;
        case MonsterType.MagmaDuggy:
          MagmaDuggy(position);
          break;
        case MonsterType.Grub:
          Grub(position);
          break;
        case MonsterType.GrubDangerous:
          GrubDangerous(position);
          break;
        case MonsterType.HotHead:
          HotHead(position);
          break;
        case MonsterType.LavaCrab:
          LavaCrab(position);
          break;
        case MonsterType.LavaCrabDangerous:
          LavaCrabDangerous(position);
          break;
        case MonsterType.LavaLurk:
          LavaLurk(position);
          break;
        case MonsterType.MetalHead:
          MetalHead(position);
          break;
        case MonsterType.MetalHeadDangerous:
          MetalHeadDangerous(position);
          break;
        case MonsterType.MetalHeadPurple:
          MetalHeadPurple(position);
          break;
        case MonsterType.MetalHeadPurpleDangerous:
          MetalHeadPurpleDangerous(position);
          break;
        case MonsterType.Mummy:
          Mummy(position);
          break;
        case MonsterType.MummyDangerous:
          MummyDangerous(position);
          break;
        case MonsterType.RockCrab:
          RockCrab(position);
          break;
        case MonsterType.RockCrabDangerous:
          RockCrabDangerous(position);
          break;
        case MonsterType.IridiumCrab:
          IridiumCrab(position);
          break;
        case MonsterType.FalseMagmaCap:
          FalseMagmaCap(position);
          break;
        case MonsterType.StickBug:
          StickBug(position);
          break;
        case MonsterType.StoneGolem:
          StoneGolem(position);
          break;
        case MonsterType.StoneGolemDangerous:
          StoneGolemDangerous(position);
          break;
        case MonsterType.WildernessGolem:
          WildernessGolem(position);
          break;
        case MonsterType.Serpent:
          Serpent(position);
          break;
        case MonsterType.RoyalSerpent:
          RoyalSerpent(position);
          break;
        case MonsterType.ShadowBrute:
          ShadowBrute(position);
          break;
        case MonsterType.ShadowBruteDangerous:
          ShadowBruteDangerous(position);
          break;
        case MonsterType.ShadowShaman:
          ShadowShaman(position);
          break;
        case MonsterType.ShadowShamanDangerous:
          ShadowShamanDangerous(position);
          break;
        case MonsterType.ShadowSniper:
          ShadowSniper(position);
          break;
#pragma warning disable CS0618 // Type or member is obsolete
        case MonsterType.SkeletonGunner:
          SkeletonGunner(position);
          break;
#pragma warning restore CS0618 // Type or member is obsolete
        case MonsterType.Spiker:
          Spiker(position);
          break;
        case MonsterType.SquidKid:
          SquidKid(position);
          break;
        case MonsterType.SquidKidDangerous:
          SquidKidDangerous(position);
          break;
        default:
          Spawn(new Monster());
          break;
      }
    }

  }

}

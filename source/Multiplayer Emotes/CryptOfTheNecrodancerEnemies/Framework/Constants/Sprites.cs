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

namespace CryptOfTheNecrodancerEnemies.Framework.Constants {

  interface ISpriteAsset {
    string Name { get; }
    string File { get; }
    Rectangle SourceRectangle { get; }
    Vector2? Origin { get; }
    float Scale { get; }
  }

  struct SpriteAsset : ISpriteAsset {

    public SpriteAsset(string name, string file, Rectangle sourceRectangle, Vector2? origin = null, float scale = 1f, bool shouldResize = true) {
      Name = name;
      File = file;
      SourceRectangle = sourceRectangle;
      Origin = origin;
      Scale = scale;
      ShouldResize = shouldResize;
    }

    public SpriteAsset(string name, Rectangle sourceRectangle, Vector2? origin = null, int targetSize = Game1.smallestTileSize)
      : this(name, $"assets/{name}.png", sourceRectangle, origin, (float)targetSize / Math.Max(sourceRectangle.Width, sourceRectangle.Height)) {
    }

    public static SpriteAsset DefaultSize(string name) {
      return new(name, $"assets/{name}.png", Rectangle.Empty, shouldResize: false);
    }

    public bool ShouldResize { get; internal set; }

    public string Name { get; internal set; }
    public string File { get; internal set; }

    public Rectangle SourceRectangle { get; internal set; }
    public Vector2? Origin { get; internal set; }
    public float Scale { get; internal set; }
  }

  internal static class Sprites {

    /// <summary>
    /// The <c>Dictionary</c> of sprite assets.
    /// </summary>
    public static Dictionary<string, SpriteAsset> Assets { get; internal set; } = LoadSpriteAssets();

    /// <summary>
    /// Loads again the sprite assets and invalidates the assets from the content cache.
    /// </summary>
    public static void ReloadSpriteAssets() {
      Assets = LoadSpriteAssets();
      ModEntry.ModHelper.Content.InvalidateCache(asset => {
        return asset.DataType == typeof(Texture2D) && Assets.ContainsKey(asset.AssetName);
      });
    }

    /// <summary>
    /// Load the sprite assets.
    /// </summary>
    public static Dictionary<string, SpriteAsset> LoadSpriteAssets() {
      return new() {
        { BatAsset, new(BatAsset, new(0, 0, 24, 24), origin: new(12, 16)) },
        { FrostBatAsset, new(FrostBatAsset, new(0, 0, 24, 24), origin: new(12, 16)) },
        { LavaBatAsset, new(LavaBatAsset, new(0, 0, 24, 24), origin: new(12, 16)) },
        { IridiumBatAsset, new(IridiumBatAsset, new(0, 0, 24, 24), origin: new(12, 16)) },
        { BatDangerousAsset, new(BatDangerousAsset, new(0, 0, 36, 36), origin: new(18, 28)) },
        { FrostBatDangerousAsset, new(FrostBatDangerousAsset, new(0, 0, 36, 36), origin: new(18, 28)) },
        //{ HauntedSkullAsset, new(HauntedSkullAsset, new(0, 0, 30, 30), targetSize: 16f) }, // TODO: Hardcoded size.
        //{ HauntedSkullDangerousAsset, new(HauntedSkullDangerousAsset, new(0, 0, 30, 30)) }, // TODO: Hardcoded size.
        { SpiderAsset, SpriteAsset.DefaultSize(SpiderAsset) },
        { BigSlimeAsset, SpriteAsset.DefaultSize(BigSlimeAsset) },
        { DuggyAsset, new(DuggyAsset, new(0, 0, 24, 24)) },
        { DuggyDangerousAsset, new(DuggyDangerousAsset, new(0, 0, 24, 24)) },
        { MagmaDuggyAsset, new(MagmaDuggyAsset, new(0, 0, 24, 24)) },
        { SquidKidAsset, new(SquidKidAsset, new(0, 0, 30, 30), origin: new(15, 24), targetSize: 20) },
        { SquidKidDangerousAsset, new(SquidKidDangerousAsset, new(0, 0, 30, 30), origin: new(15, 24), targetSize: 20) },
        //{ SkeletonMageAsset, new(SkeletonMageAsset, new(0, 0, 24, 30), targetSize: 32) },
        //{ SkeletonMageDangerousAsset, new(SkeletonMageDangerousAsset, new(0, 0, 24, 30), targetSize: 32) },
      };
    }

    public const string BatAsset = "Characters\\Monsters\\Bat";
    public const string FrostBatAsset = "Characters\\Monsters\\Frost Bat";
    public const string LavaBatAsset = "Characters\\Monsters\\Lava Bat";
    public const string IridiumBatAsset = "Characters\\Monsters\\Iridium Bat";
    public const string BatDangerousAsset = BatAsset + "_dangerous";
    public const string FrostBatDangerousAsset = FrostBatAsset + "_dangerous";
    public const string HauntedSkullAsset = "Characters\\Monsters\\Haunted Skull";
    public const string HauntedSkullDangerousAsset = HauntedSkullAsset + "_dangerous";

    public const string SpiderAsset = "Characters\\Monsters\\Spider";

    public const string BigSlimeAsset = "Characters\\Monsters\\Big Slime";

    public const string DuggyAsset = "Characters\\Monsters\\Duggy";
    public const string DuggyDangerousAsset = DuggyAsset + "_dangerous";
    public const string MagmaDuggyAsset = "Characters\\Monsters\\Magma Duggy";

    public const string SquidKidAsset = "Characters\\Monsters\\Squid Kid";
    public const string SquidKidDangerousAsset = SquidKidAsset + "_dangerous";

    public const string SkeletonMageAsset = "Characters\\Monsters\\Skeleton Mage";
    public const string SkeletonMageDangerousAsset = SkeletonMageAsset + "_dangerous";
  }

}

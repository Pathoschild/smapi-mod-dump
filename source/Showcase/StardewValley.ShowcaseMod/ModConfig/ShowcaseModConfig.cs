using System.Collections.Generic;
using Igorious.StardewValley.DynamicApi2;
using Igorious.StardewValley.DynamicApi2.Constants;
using Igorious.StardewValley.DynamicApi2.Data;
using Igorious.StardewValley.ShowcaseMod.Constants;
using Igorious.StardewValley.ShowcaseMod.Core;
using Igorious.StardewValley.ShowcaseMod.Data;
using Microsoft.Xna.Framework;

namespace Igorious.StardewValley.ShowcaseMod.ModConfig
{
    public sealed class ShowcaseModConfig : DynamicConfiguration
    {
        public List<ShowcaseConfig> Showcases { get; set; } = new List<ShowcaseConfig>();
        public GlowConfig Glows { get; set; } = new GlowConfig();
        public List<RotationEffect> RotationEffects { get; set; } = new List<RotationEffect>();

        public override void CreateDefaultConfiguration()
        {
            Showcases = new List<ShowcaseConfig>
            {
                new ShowcaseConfig
                {
                    ID = 1228,
                    Name = "Showcase",
                    Sprite = new SpriteInfo(0),
                    Tint = new SpriteInfo(2),
                    Layout = new LayoutConfig(ShowcaseLayoutKind.Auto)
                    {
                        SpriteBounds = new Bounds
                        {
                            Top = -2,
                            Bottom = 5,
                            Left = 1,
                            Right = 1,
                        },
                        Columns = 3,
                        Rows = 3,
                    },
                    Size = new Size(2, 2),
                    BoundingBox = new Size(2, 2),
                    Price = 1000,
                    Kind = FurnitureKind.Table,
                    Filter = $"{ItemFilter.ShippableCategory} {CategoryID.Furniture}",
                },
                new ShowcaseConfig
                {
                    ID = 1230,
                    Name = "Wood Stand",
                    Sprite = new SpriteInfo(4),
                    Tint = new SpriteInfo(5),
                    AutoTint = true,
                    Size = new Size(1, 2),
                    BoundingBox = new Size(1, 2),
                    Layout = new LayoutConfig
                    {
                        SpriteBounds = new Bounds
                        {
                            Top = 2,
                            Bottom = 14,
                            Left = 1,
                            Right = 1,
                        },
                        Scale = 0.85f,
                    },
                    Price = 1000,
                    Kind = FurnitureKind.Painting,
                    Filter = $"{nameof(CategoryID.Fish)}",
                },
                new ShowcaseConfig
                {
                    ID = 1826,
                    Name = "Chinese Showcase",
                    Sprite = new SpriteInfo(64),
                    SecondSprite = new SpriteInfo(67),
                    Size = new Size(3, 3),
                    BoundingBox = new Size(3),
                    Layout = new LayoutConfig
                    {
                        SpriteBounds = new Bounds
                        {
                            Top = 13,
                            Bottom = 23,
                            Right = 5,
                            Left = 5,
                        },
                        Columns = 3,
                        Scale = 0.80f,
                    },
                    Price = 1000,
                    Kind = FurnitureKind.Other,
                    Filter = $"{ItemFilter.ShippableCategory} {CategoryID.Furniture} !{nameof(CategoryID.Cooking)}",
                },
                new ShowcaseConfig
                {
                    ID = 1231,
                    Name = "Old Shield",
                    Sprite = new SpriteInfo(6),
                    SecondSprite = new SpriteInfo(7),
                    SecondTint = new SpriteInfo(8),
                    AutoTint = true,
                    Size = new Size(1, 2),
                    BoundingBox = new Size(1, 2),
                    Layout = new LayoutConfig
                    {
                        SpriteBounds = new Bounds
                        {
                            Top = 2,
                            Bottom = 10,
                        },
                    },
                    Price = 1000,
                    Kind = FurnitureKind.Painting,
                    Filter = $"{nameof(CategoryID.Weapon)} {nameof(CategoryID.Tool)}",
                },
                new ShowcaseConfig
                {
                    ID = 1832,
                    Name = "Darkwood Dresser",
                    Sprite = new SpriteInfo(70),
                    Layout = new LayoutConfig
                    {
                        SpriteBounds = new Bounds
                        {
                            Top = -3,
                            Left = 2,
                            Right = 2,
                            Bottom = 21,
                        },
                        AltSpriteBounds = new Bounds
                        {
                            Top = -3,
                            Left = 2,
                            Right = 5,
                            Bottom = 24,
                        },
                        Columns = 2,
                        Scale = 0.85f,
                    },
                    Price = 1000,
                    Kind = FurnitureKind.Dresser,
                    Filter = $"{ItemFilter.ShippableCategory} {CategoryID.Furniture}",
                    Rotations = 4,
                },
                new ShowcaseConfig
                {
                    ID = -1,
                    Name = "Oak Small Table",
                    Description = "A cute table with different colors.",
                    Sprite = new SpriteInfo(1391, TextureKind.Global),
                    Tint = new SpriteInfo(9),
                    Size = new Size(1, 2),
                    BoundingBox = new Size(1),
                    Layout = new LayoutConfig
                    {
                        Scale = 0.875f,
                        SpriteBounds = new Bounds
                        {
                            Top = 4,
                            Bottom = 14,
                            Left = 1,
                            Right = 1,
                        },
                    },
                    Price = 1000,
                    Kind = FurnitureKind.Table,
                    Filter = $"{ItemFilter.ShippableCategory} {CategoryID.Furniture}",
                },
                new ShowcaseConfig
                {
                    ID = -2,
                    Name = "Funny Bookcase",
                    Sprite = new SpriteInfo(75),
                    SecondSprite = new SpriteInfo(77),
                    Size = new Size(2, 3),
                    BoundingBox = new Size(2),
                    Layout = new LayoutConfig(ShowcaseLayoutKind.Manual)
                    {
                        Scale = 0.50f,
                        Positions = new List<ItemPosition>
                        {
                            new ItemPosition(7, 17),
                            new ItemPosition(20, 11),
                            new ItemPosition(3, 26),
                            new ItemPosition(19, 22),
                            new ItemPosition(4, 37),
                            new ItemPosition(17, 32),
                        },
                        Rows = 3,
                        Columns = 2,
                    },
                    Price = 1000,
                    Kind = FurnitureKind.Bookcase,
                    Filter = $"{ItemFilter.ShippableCategory} {CategoryID.Furniture}",
                },
            };

            var iridiumGlow = Color.Lerp(Color.Purple, Color.Magenta, 0.3f);
            Glows = new GlowConfig
            {
                IridiumQualityGlow = new GlowEffect(iridiumGlow),
                GoldQualityGlow = new GlowEffect(Color.Yellow),
                Glows = new List<GlowEffect>
                {
                    new GlowEffect(CategoryID.Weapon, (int)WeaponID.GalaxySword, iridiumGlow),
                    new GlowEffect(CategoryID.Weapon, (int)WeaponID.GalaxyDagger, iridiumGlow),
                    new GlowEffect(CategoryID.Weapon, (int)WeaponID.GalaxyHammer, iridiumGlow),
                    new GlowEffect(CategoryID.Weapon, (int)WeaponID.HolyBlade, Color.White),
                    new GlowEffect(CategoryID.Weapon, (int)WeaponID.DarkSword, Color.Black),
                    new GlowEffect(CategoryID.Weapon, (int)WeaponID.LavaKatana, Color.Red),
                    new GlowEffect(CategoryID.Weapon, (int)WeaponID.NeptunesGlaive, Color.Aqua),
                    new GlowEffect(CategoryID.Weapon, (int)WeaponID.ForestSword, Color.Green),

                    new GlowEffect((int)ObjectID.VoidEssence, Color.Black),
                    new GlowEffect((int)ObjectID.VoidEgg, Color.Black),
                    new GlowEffect((int)ObjectID.VoidMayonnaise, Color.Black),

                    new GlowEffect((int)ObjectID.SolarEssence, Color.Yellow),

                    new GlowEffect((int)ObjectID.FireQuartz, Color.DarkRed),
                    new GlowEffect((int)ObjectID.FrozenTear, Color.Aqua),
                    new GlowEffect((int)ObjectID.Emerald, Color.Green),
                    new GlowEffect((int)ObjectID.Diamond, Color.SkyBlue),
                    new GlowEffect((int)ObjectID.Ruby, Color.Red),
                    new GlowEffect((int)ObjectID.PrismaticShard, Color.White),

                    new GlowEffect((int)ObjectID.Crimsonfish, iridiumGlow),
                    new GlowEffect((int)ObjectID.Angler, iridiumGlow),
                    new GlowEffect((int)ObjectID.Legend, iridiumGlow),
                    new GlowEffect((int)ObjectID.Glacierfish, iridiumGlow),
                    new GlowEffect((int)ObjectID.MutantCarp, iridiumGlow),

                    new GlowEffect((int)ObjectID.VoidSalmon, Color.Black),
                    new GlowEffect((int)ObjectID.LavaEel, Color.Red),
                    new GlowEffect((int)ObjectID.IcePip, Color.SkyBlue),
                },
            };

            RotationEffects = new List<RotationEffect>
            {
                new RotationEffect((int)ObjectID.Wine, -1),
                new RotationEffect((int)ObjectID.Juice, -1),
                new RotationEffect((int)ObjectID.Mead, -1),

                new RotationEffect(CategoryID.Fish, 1),
                new RotationEffect((int)ObjectID.Pufferfish, 0),
                new RotationEffect((int)ObjectID.Eel, 0),
                new RotationEffect((int)ObjectID.Octopus, 0),
                new RotationEffect((int)ObjectID.Squid, 0),
                new RotationEffect((int)ObjectID.LavaEel, 0),
                new RotationEffect((int)ObjectID.Angler, 0),
                new RotationEffect((int)ObjectID.Glacierfish, 0),
                new RotationEffect((int)ObjectID.MutantCarp, 0),
                new RotationEffect((int)ObjectID.SeaCucumber, 0),
                new RotationEffect((int)ObjectID.SuperCucumber, 0),

                new RotationEffect((int)ObjectID.Crab, 0),
                new RotationEffect((int)ObjectID.Cockle, 0),
                new RotationEffect((int)ObjectID.Shrimp, 0),
                new RotationEffect((int)ObjectID.Snail, 0),
                new RotationEffect((int)ObjectID.Periwinkle, 1),
                new RotationEffect((int)ObjectID.Oyster, 0),

                new RotationEffect(CategoryID.Tool, -1),
                new RotationEffect(CategoryID.Tool, (int)ToolID.WateringCan, 0),
                new RotationEffect(CategoryID.Tool, (int)ToolID.MilkPail, 0),
                new RotationEffect(CategoryID.Tool, (int)ToolID.Shears, 1),
                new RotationEffect(CategoryID.Tool, (int)ToolID.ReturnScepter, -1),
                new RotationEffect(CategoryID.Tool, (int)ToolID.Pan, 0),
                
                new RotationEffect(CategoryID.Weapon, 3),
                new RotationEffect(CategoryID.Weapon, (int)WeaponID.Slingshot, 0),
                new RotationEffect(CategoryID.Weapon, (int)WeaponID.MasterSlingshot, 0),
                new RotationEffect(CategoryID.Weapon, (int)WeaponID.GalaxySlingshot, 0),
                new RotationEffect(CategoryID.Weapon, (int)WeaponID.Scythe, -1),
                new RotationEffect(CategoryID.Weapon, WeaponKind.Club.ToString(), -1),
            };
        }
    }
}
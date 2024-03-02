/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-desert-bloom-farm
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using Unlockable_Bundles.Lib;
using Unlockable_Bundles.API;
using Unlockable_Bundles.Lib.Enums;

namespace Desert_Bloom.Lib
{
    public class AssetsRequested
    {
        public static Mod Mod;
        public static IMonitor Monitor;
        public static IModHelper Helper;

        public static void main()
        {
            Mod = ModEntry.Mod;
            Monitor = ModEntry._Monitor;
            Helper = ModEntry._Helper;

            Helper.Events.Content.AssetRequested += assetRequested;
        }

        private static void assetRequested(object sender, AssetRequestedEventArgs e)
        {
            load_tilesets(sender, e);

            if (!ModEntry.IsMyFarm())
                return;

            load_unlockables(sender, e);
            load_unlockable_textures(sender, e);
            load_unlockable_maps(sender, e);
            edit_pet_bowl(sender, e);
        }

        private static void load_unlockables(object sender, AssetRequestedEventArgs e)
        {
            if (!e.NameWithoutLocale.IsEquivalentTo("UnlockableBundles/Bundles"))
                return;

            var list = new List<UnlockableModel>();

            if (!canTalkToJunimo())
                return;

            load_mine_area(sender, e, list);
            load_mill_areas(sender, e, list);
            load_oasis_area(sender, e, list);

            e.Edit(asset => {
                Type valueType = asset.Data.GetType().GetGenericArguments().LastOrDefault();

                var data = asset.AsDictionary<string, UnlockableModel>().Data;

                foreach (var unlockable in list)
                    data.Add(unlockable.ID, unlockable);
            });
        }

        private static void load_oasis_area(object sender, AssetRequestedEventArgs e, List<UnlockableModel> list)
        {
            list.Add(new UnlockableModel() {
                ID = "DLX.Desert_Bloom.Oasis_Tunnel",
                Location = "Farm",
                InteractionSound = "junimoMeep1",
                InteractionShake = true,
                BundleName = "Forgotten In Time",
                BundleDescription = Helper.Translation.Get("oasis_tunnel"),
                ShopPosition = new Vector2(121, 51),
                ShopTexture = "DLX.Desert_Bloom.Junimo_Shop_Blue",
                ShopAnimation = "0-11@100",
                Price = {
                    { "322", 50 }, //Wooden Fence
                    { "286", 10 }, //Cherry Bomb
                    { "287", 5 }, //Bomb
                    },
                EditMap = "DLX.Desert_Bloom.Oasis_Tunnel.Overlay",
                EditMapPosition = new Vector2(120, 48),
                EditMapMode = PatchMapMode.ReplaceByLayer
            });

            list.Add(new UnlockableModel() {
                ID = "DLX.Desert_Bloom.Oasis_1",
                Location = "Farm",
                InteractionSound = "junimoMeep1",
                InteractionShake = true,
                BundleName = "Spring Rejuvenation",
                BundleDescription = Helper.Translation.Get("oasis_1"),
                ShopPosition = new Vector2(123, 21),
                ShopTexture = "DLX.Desert_Bloom.Junimo_Shop_Blue",
                ShopAnimation = "0-11@100",
                Price = {
                    { "140", 2 }, //Walleye
                    { "143", 2 }, //Catfish
                    { "141", 2 }, //Perch
                    { "702", 2 }, //Chub
                    { "144", 2 }, //Pike
                    { "164", 2 }, //Sandfish
                    { "165", 2 }, //Scorpion Carp 
                },
                EditMap = "NONE",
            });

            if (!API.UnlockableBundlesHandler.unlocked("DLX.Desert_Bloom.Oasis_1"))
                return;

            list.Add(new UnlockableModel() {
                ID = "DLX.Desert_Bloom.Oasis_2",
                Location = "Farm",
                InteractionSound = "junimoMeep1",
                InteractionShake = true,
                BundleName = "Only What's Best For You",
                BundleDescription = Helper.Translation.Get("oasis_2"),
                ShopPosition = new Vector2(123, 21),
                ShopTexture = "DLX.Desert_Bloom.Junimo_Shop_Blue",
                ShopAnimation = "0-11@100",
                Price = {
                    { "766", 50 }, //Slime
                    { "684", 50 }, //Bug Meat
                    { "153", 5 }, //Green Algae
                    { "157", 5 }, //White Algae
                    { "685", 999 }, //Bait
                    { "774", 5 }, //Wild Bait
                },
                EditMap = "NONE",
            });

            if (!API.UnlockableBundlesHandler.unlocked("DLX.Desert_Bloom.Oasis_2"))
                return;

            list.Add(new UnlockableModel() {
                ID = "DLX.Desert_Bloom.Oasis_3",
                Location = "Farm",
                InteractionSound = "junimoMeep1",
                InteractionShake = true,
                BundleName = "Legendary Angler",
                BundleDescription = Helper.Translation.Get("oasis_3"),
                ShopPosition = new Vector2(123, 21),
                ShopTexture = "DLX.Desert_Bloom.Junimo_Shop_Blue",
                ShopAnimation = "0-11@100",
                Price = {
                    { "160, 899", 1 }, //Angler / Ms. Angler
                    { "159, 898", 1 }, //Crimsonfish / Son of Crimsonfish
                    { "775, 902", 1 }, //Glacierfish / Son of Glacierfish
                    { "163, 900", 1 }, //Legend Fish / Legend II
                    { "682, 901", 1 }  //Mutant Carp / Radioactive Carp
                },
                EditMap = "NONE",
            });
        }

        private static void load_mill_areas(object sender, AssetRequestedEventArgs e, List<UnlockableModel> list)
        {
            list.Add(new UnlockableModel() {
                ID = "DLX.Desert_Bloom.Mill_Tier1",
                Location = "Farm",
                InteractionSound = "junimoMeep1",
                InteractionShake = true,
                BundleName = "Makeshift Repairs",
                BundleDescription = Helper.Translation.Get("mill_tier1"),
                ShopPosition = new Vector2(47, 16),
                ShopTexture = "DLX.Desert_Bloom.Junimo_Shop_Green",
                ShopAnimation = "0-11@100",
                Price = {
                    { "Money", 1000 },
                    { "388", 100 }, //Wood
                    { "771", 50 } //Fiber

                },
                EditMap = "DLX.Desert_Bloom.Mill_1",
                EditMapPosition = new Vector2(33, 16),
                EditMapMode = PatchMapMode.ReplaceByLayer
            });

            if (!API.UnlockableBundlesHandler.unlocked("DLX.Desert_Bloom.Mill_Tier1"))
                return;

            list.Add(new UnlockableModel() {
                ID = "DLX.Desert_Bloom.Mill_Tier2",
                Location = "Farm",
                InteractionSound = "junimoMeep1",
                InteractionShake = true,
                BundleName = "Leaking Plumbing",
                BundleDescription = Helper.Translation.Get("mill_tier2"),
                ShopPosition = new Vector2(47, 16),
                ShopTexture = "DLX.Desert_Bloom.Junimo_Shop_Green",
                ShopAnimation = "0-11@100",
                Price = {
                    { "Money", 5000 },
                    { "334", 5 }, //Copper Bar
                    { "92", 50 }, //Sap
                    { "388", 25 } //Wood
                },
                EditMap = "DLX.Desert_Bloom.Mill_2",
                EditMapPosition = new Vector2(31, 11),
                EditMapMode = PatchMapMode.ReplaceByLayer
            });

            if (!API.UnlockableBundlesHandler.unlocked("DLX.Desert_Bloom.Mill_Tier2"))
                return;

            list.Add(new UnlockableModel() {
                ID = "DLX.Desert_Bloom.Mill_Tier3",
                Location = "Farm",
                InteractionSound = "junimoMeep1",
                InteractionShake = true,
                BundleName = "Gear It Up!",
                BundleDescription = Helper.Translation.Get("mill_tier3"),
                ShopPosition = new Vector2(60, 9),
                ShopTexture = "DLX.Desert_Bloom.Junimo_Shop_Green",
                ShopAnimation = "0-11@100",
                Price = {
                    { "Money", 15000 },
                    { "335", 5 }, //Iron Bar
                    { "324", 20 } //Iron Fence
                },
                EditMap = "DLX.Desert_Bloom.Mill_3",
                EditMapPosition = new Vector2(24, 7),
                EditMapMode = PatchMapMode.ReplaceByLayer
            });

            if (!API.UnlockableBundlesHandler.unlocked("DLX.Desert_Bloom.Mill_Tier3"))
                return;

            list.Add(new UnlockableModel() {
                ID = "DLX.Desert_Bloom.Mill_Tier4",
                Location = "Farm",
                InteractionSound = "junimoMeep1",
                InteractionShake = true,
                BundleName = "The Final Piece",
                BundleDescription = Helper.Translation.Get("mill_tier4"),
                ShopPosition = new Vector2(60, 9),
                ShopTexture = "DLX.Desert_Bloom.Junimo_Shop_Green",
                ShopAnimation = "0-11@100",
                Price = {
                    { "Money", 25000 },
                    { "336", 5 }, //Gold Bar
                    { "787", 4 }, //Battery Pack
                    { "709", 50 } //Hardwood
                },
                EditMap = "DLX.Desert_Bloom.Mill_4",
                EditMapPosition = new Vector2(86, 42),
                EditMapMode = PatchMapMode.ReplaceByLayer
            });

            if (!API.UnlockableBundlesHandler.unlocked("DLX.Desert_Bloom.Mill_Tier4"))
                return;

            list.Add(new UnlockableModel() {
                ID = "DLX.Desert_Bloom.Mill_Tier5",
                Location = "Farm",
                InteractionSound = "junimoMeep1",
                InteractionShake = true,
                BundleName = "Breaking Limits",
                BundleDescription = Helper.Translation.Get("mill_tier5"),
                ShopPosition = new Vector2(60, 9),
                ShopTexture = "DLX.Desert_Bloom.Junimo_Shop_Green",
                ShopAnimation = "0-11@100",
                Price = {
                    { "Money", 100000 },
                    { "337", 5 }, //Iridium Bar
                    { "428", 25 }, //Cloth
                    { "74", 1 }, //Prismatic Shard
                    { "369", 99 }, //Quality Fertilizer
                    { "371", 99 }, //Quality Retaining Soil
                    { "466", 99 }, //Speed-Gro
                    { "805", 99 } //Tree Fertilizer
                },
                EditMap = "NONE",
            });
        }

        private static void load_mine_area(object sender, AssetRequestedEventArgs e, List<UnlockableModel> list)
        {
            list.Add(new UnlockableModel() {
                ID = "DLX.Desert_Bloom.Mining_Area_1",
                Location = "Farm",
                InteractionSound = "junimoMeep1",
                InteractionShake = true,
                BundleName = "A Good Deed",
                BundleDescription = Helper.Translation.Get("mine_1"),
                ShopPosition = new Vector2(5, 56),
                ShopTexture = "DLX.Desert_Bloom.Junimo_Shop_Orange",
                ShopType = ShopType.SpeechBubble,
                ShopAnimation = "0-11@100",
                Price = {
                    { "172, 168, 169, 170, 171, 167", 40 } //Soggy Newspaper, Trash, Driftwood, Broken Glasses, Broken CD, Joja Cola
                },
                BundleReward = {
                    { "money", 5000 },
                    { "72", 1 } //Diamond
                },
                EditMap = "DLX.Desert_Bloom.Mine_Area_Clean",
                EditMapPosition = new Vector2(3, 53),
                EditMapMode = PatchMapMode.ReplaceByLayer
            });

            if (!API.UnlockableBundlesHandler.unlocked("DLX.Desert_Bloom.Mining_Area_1"))
                return;

            list.Add(new UnlockableModel() {
                ID = "DLX.Desert_Bloom.Mining_Area_2",
                Location = "Farm",
                InteractionSound = "junimoMeep1",
                InteractionShake = true,
                BundleName = "Jack Of All Tastes",
                BundleDescription = Helper.Translation.Get("mine_2"),
                ShopPosition = new Vector2(5, 56),
                ShopTexture = "DLX.Desert_Bloom.Junimo_Shop_Orange",
                ShopAnimation = "0-11@100",
                Price = {
                    { "243", 1 }, //Miner's Treat
                    { "240", 1 }, //Farmer's Lunch
                    { "206", 1 }, //Pizza
                    { "244", 1 }, //Roots Platter
                    { "233", 1 }, //Ice Cream
                    { "227", 1 }, //Sashimi
                },
                EditMap = "NONE",
            });

            if (!API.UnlockableBundlesHandler.unlocked("DLX.Desert_Bloom.Mining_Area_2"))
                return;

            list.Add(new UnlockableModel() {
                ID = "DLX.Desert_Bloom.Mining_Area_3",
                Location = "Farm",
                InteractionSound = "junimoMeep1",
                InteractionShake = true,
                BundleName = "Desert Cravings",
                BundleDescription = Helper.Translation.Get("mine_3"),
                ShopPosition = new Vector2(5, 56),
                ShopTexture = "DLX.Desert_Bloom.Junimo_Shop_Orange",
                ShopAnimation = "0-11@100",
                Price = {
                    { "90", 30 }, //Cactus Fruit
                    { "88", 30 }, //Cocnut
                },
                EditMap = "NONE",
            });
        }

        private static void load_tilesets(object sender, AssetRequestedEventArgs e)
        {
            var prefix = "";

            if (Helper.ModRegistry.IsLoaded("DaisyNiko.EarthyRecolour"))
                prefix = "ER_";

            if (e.NameWithoutLocale.IsEquivalentTo("Maps/Desert_Bloom_Cracked_Floor"))
                e.LoadFromModFile<Texture2D>($"Assets/Tileset/{prefix}Desert_Bloom_Cracked_Floor.png", AssetLoadPriority.Medium);

            if (e.NameWithoutLocale.IsEquivalentTo("Maps/Desert_Bloom_Tiles"))
                e.LoadFromModFile<Texture2D>($"Assets/Tileset/{prefix}Desert_Bloom_Tiles.png", AssetLoadPriority.Medium);
        }

        private static void load_unlockable_textures(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("DLX.Desert_Bloom.Junimo_Shop_Blue"))
                e.LoadFromModFile<Texture2D>("Assets/ShopTextures/Junimo_Shop_Blue.png", AssetLoadPriority.Medium);

            if (e.NameWithoutLocale.IsEquivalentTo("DLX.Desert_Bloom.Junimo_Shop_Orange"))
                e.LoadFromModFile<Texture2D>("Assets/ShopTextures/Junimo_Shop_Orange.png", AssetLoadPriority.Medium);

            if (e.NameWithoutLocale.IsEquivalentTo("DLX.Desert_Bloom.Junimo_Shop_Green"))
                e.LoadFromModFile<Texture2D>("Assets/ShopTextures/Junimo_Shop_Green.png", AssetLoadPriority.Medium);
        }

        private static void load_unlockable_maps(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("DLX.Desert_Bloom.Oasis_Tunnel.Overlay"))
                e.LoadFromModFile<xTile.Map>("Assets/Overlays/Oasis_Tunnel.tmx", AssetLoadPriority.Medium);

            if (e.NameWithoutLocale.IsEquivalentTo("DLX.Desert_Bloom.Mine_Area_Clean"))
                e.LoadFromModFile<xTile.Map>("Assets/Overlays/Mine_Area_Clean.tmx", AssetLoadPriority.Medium);

            if (e.NameWithoutLocale.StartsWith("DLX.Desert_Bloom.Mill_"))
                e.LoadFromModFile<xTile.Map>($"Assets/Overlays/Mill_{e.NameWithoutLocale.BaseName.Last()}.tmx", AssetLoadPriority.Medium);
        }

        private static void edit_pet_bowl(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Buildings/Pet Bowl"))
                e.LoadFromModFile<Texture2D>("Assets/Tileset/Pet Bowl.png", AssetLoadPriority.Low);
        }

        //private static bool canTalkToJunimo() => true;
        private static bool canTalkToJunimo() => (Game1.MasterPlayer.hasOrWillReceiveMail("canReadJunimoText") || Game1.MasterPlayer.mailReceived.Contains("JojaMember"));
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/personal-indoor-farm
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Buffs;
using StardewValley.GameData.Locations;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Shops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalIndoorFarm.Lib
{
    public class AssetRequested
    {
        private static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public const string FarmsAsset = "DLX.PIF/Farms";
        public const string SpriteSheetAsset = "DLX.PIF_SpriteSheet";
        public static void Initialize()
        {
            Mod = ModEntry.Mod;
            Monitor = Mod.Monitor;
            Helper = Mod.Helper;

            Helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        [EventPriority(EventPriority.Low)]
        private static void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(FarmsAsset)) {
                //We need to provide an empty initial dictionary where others can append their changes
                e.LoadFrom(delegate () {
                    return new Dictionary<string, PersonalFarmModel>() { };
                }, AssetLoadPriority.Exclusive);

            } else if (e.NameWithoutLocale.IsEquivalentTo(SpriteSheetAsset)) {
                e.LoadFrom(delegate () {
                    return Helper.ModContent.Load<Texture2D>("assets/SpriteSheet.png");
                }, AssetLoadPriority.Medium);

            } else if (e.NameWithoutLocale.IsEquivalentTo("Data/Furniture")) {
                //If you want more dimensions just add a new furniture and make sure it's called "DLX.PIF_Door_<id>"
                e.Edit(asset => {
                    var editor = asset.AsDictionary<string, string>();
                    editor.Data.Add($"{Door.ItemId}_Aether",
                        "Dimension Door - Aether" + //name
                        "/painting" + //type
                        "/1 3" + //tilesheet size
                        "/1 3" + //bounding box size
                        "/1" + //rotations
                        "/1000" + //price
                        "/-1" + //placement restriction
                        $"/{Helper.Translation.Get("Door1.Name")}" + //display name
                        "/0" + //Index in sprite sheet
                        $"/{SpriteSheetAsset}"
                    );

                    editor.Data.Add($"{Door.ItemId}_Erebus",
                        "Dimension Door - Erebus" + //name
                        "/painting" + //type
                        "/1 3" + //tilesheet size
                        "/1 3" + //bounding box size
                        "/1" + //rotations
                        "/15000" + //price
                        "/-1" + //placement restriction
                        $"/{Helper.Translation.Get("Door2.Name")}" + //display name
                        "/6" + //Index in sprite sheet
                        $"/{SpriteSheetAsset}"
                    );

                    editor.Data.Add($"{Door.ItemId}_Chaos",
                        "Dimension Door - Chaos" + //name
                        "/painting" + //type
                        "/1 3" + //tilesheet size
                        "/1 3" + //bounding box size
                        "/1" + //rotations
                        "/40000" + //price
                        "/-1" + //placement restriction
                        $"/{Helper.Translation.Get("Door3.Name")}" + //display name
                        "/7" + //Index in sprite sheet
                        $"/{SpriteSheetAsset}"
                    );

                    editor.Data.Add(Painting.ItemId,
                        "Painting of the Season" + //name
                        "/painting" + //type
                        "/1 2" + //tilesheet size
                        "/1 2" + //bounding box size
                        "/1" + //rotations
                        "/500" + //price
                        "/-1" + //placement restriction
                        $"/{Helper.Translation.Get("Painting.Name")}" + //display name
                        "/1" + //Index in sprite sheet
                        $"/{SpriteSheetAsset}"
                    );
                });

            } else if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects")) {
                e.Edit(asset => {
                    var editor = asset.AsDictionary<string, ObjectData>();

                    editor.Data.Add(SpaceTimeSynchronizer.ItemId, new ObjectData() {
                        Name = "Space Time Synchronizer",
                        DisplayName = Helper.Translation.Get("Synchronizer.Name"),
                        Description = Helper.Translation.Get("Synchronizer.Description"),
                        Type = "Cooking",
                        Category = 0,
                        Price = 100,
                        Texture = SpriteSheetAsset,
                        SpriteIndex = 5,
                        Edibility = -4,
                        IsDrink = true,
                        Buffs = new List<ObjectBuffData>() { new ObjectBuffData() {
                            BuffId = SpaceTimeSynchronizer.BuffId,
                            Duration = 30
                        } },
                        ExcludeFromRandomSale = true,
                        ExcludeFromShippingCollection = true,
                        ExcludeFromFishingCollection = true,
                    });
                });

            } else if (e.NameWithoutLocale.IsEquivalentTo("Data/Buffs")) {
                e.Edit(asset => {
                    var editor = asset.AsDictionary<string, BuffData>();

                    editor.Data.Add(SpaceTimeSynchronizer.BuffId, new BuffData {
                        Duration = 30,
                        GlowColor = "Purple",
                        DisplayName = Helper.Translation.Get("Synchronizer.Buff.Name"),
                        Description = Helper.Translation.Get("Synchronizer.Buff.Description"),
                        IconSpriteIndex = 14,
                        IconTexture = "TileSheets\\BuffsIcons",
                        ActionsOnApply = new() { SpaceTimeSynchronizer.BuffTriggerAction }
                    });
                });

            } else if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops")) {
                e.Edit(asset => {
                    var editor = asset.AsDictionary<string, ShopData>();
                    editor.Data["Carpenter"].Items.AddRange(new ShopItemData[] {
                        new ShopItemData() { Id = $"{Door.QualifiedItemId}_Aether", ItemId = $"{Door.QualifiedItemId}_Aether" },
                        new ShopItemData() { Id = $"{Door.QualifiedItemId}_Erebus", ItemId = $"{Door.QualifiedItemId}_Erebus" },
                        new ShopItemData() { Id = $"{Door.QualifiedItemId}_Chaos", ItemId = $"{Door.QualifiedItemId}_Chaos" },
                        new ShopItemData() { Id = Painting.QualifiedItemId, ItemId = Painting.QualifiedItemId },
                        new ShopItemData() { Id = SpaceTimeSynchronizer.QualifiedItemId, ItemId = SpaceTimeSynchronizer.QualifiedItemId },
                    });

                });
            }
        }
    }
}

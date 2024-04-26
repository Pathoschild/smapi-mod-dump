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
using static PersonalIndoorFarm.ModEntry;

namespace PersonalIndoorFarm.Lib
{
    public class AssetRequested
    {
        public const string FarmsAsset = "DLX.PIF/Farms";
        public const string SpriteSheetAsset = "DLX.PIF_SpriteSheet";
        public static void Initialize()
        {
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
                    var data = editor.Data;

                    addDoor(data, "Aether", 1000, "Door1.Name", 0);
                    addDoor(data, "Erebus", 15000, "Door2.Name", 6);
                    addDoor(data, "Chaos", 40000, "Door3.Name", 7);

                    addDoor(data, "Attic", 20000, "DoorAttic.Name", 30, Door.SoundWoodStep, 2);
                    addDoor(data, "Loft", 20000, "DoorLoft.Name", 32, Door.SoundWoodStep, 2);
                    addDoor(data, "Cellar", 20000, "DoorCellar.Name", 34, Door.SoundWoodStep, 2);

                    data.Add(Painting.ItemId,
                        "Painting of the Season" + //name
                        "/painting" + //type
                        "/1 2" + //tilesheet size
                        "/1 2" + //bounding box size
                        "/1" + //rotations
                        "/500" + //price
                        "/-1" + //placement restriction
                        $"/[LocalizedText Strings\\Furniture:DLX.PIF.Painting.Name]" + //display name
                        "/1" + //Index in sprite sheet
                        $"/{SpriteSheetAsset}"
                    );
                });

            } else if(e.NameWithoutLocale.IsEquivalentTo("Strings/Furniture")) {
                e.Edit(asset => {
                    var editor = asset.AsDictionary<string, string>();
                    var data = editor.Data;

                    data.Add("DLX.PIF.Door1.Name", Helper.Translation.Get("Door1.Name"));
                    data.Add("DLX.PIF.Door2.Name", Helper.Translation.Get("Door2.Name"));
                    data.Add("DLX.PIF.Door3.Name", Helper.Translation.Get("Door3.Name"));

                    data.Add("DLX.PIF.DoorAttic.Name", Helper.Translation.Get("DoorAttic.Name"));
                    data.Add("DLX.PIF.DoorLoft.Name", Helper.Translation.Get("DoorLoft.Name"));
                    data.Add("DLX.PIF.DoorCellar.Name", Helper.Translation.Get("DoorCellar.Name"));

                    data.Add("DLX.PIF.Painting.Name", Helper.Translation.Get("Painting.Name"));
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

                    editor.Data.Add(VoidSeal.ItemId, new ObjectData() {
                        Name = "Void Seal Liquid",
                        DisplayName = Helper.Translation.Get("VoidSeal.Name"),
                        Description = Helper.Translation.Get("VoidSeal.Description"),
                        Type = "Cooking",
                        Category = 0,
                        Price = 250,
                        Texture = SpriteSheetAsset,
                        SpriteIndex = 15,
                        Edibility = -10,
                        IsDrink = true,
                        Buffs = new List<ObjectBuffData>() { new ObjectBuffData() {
                            BuffId = VoidSeal.BuffId,
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

                    editor.Data.Add(VoidSeal.BuffId, new BuffData {
                        Duration = 30,
                        GlowColor = "MediumPurple",
                        DisplayName = Helper.Translation.Get("VoidSeal.Buff.Name"),
                        Description = Helper.Translation.Get("VoidSeal.Buff.Description"),
                        IconSpriteIndex = 14,
                        IconTexture = "TileSheets\\BuffsIcons",
                        ActionsOnApply = new() { VoidSeal.BuffTriggerAction }
                    });
                });

            } else if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops")) {
                e.Edit(asset => {
                    var editor = asset.AsDictionary<string, ShopData>();
                    editor.Data["Carpenter"].Items.AddRange(new ShopItemData[] {
                        new ShopItemData() { Id = $"{Door.QualifiedItemId}_Aether", ItemId = $"{Door.QualifiedItemId}_Aether" },
                        new ShopItemData() { Id = $"{Door.QualifiedItemId}_Erebus", ItemId = $"{Door.QualifiedItemId}_Erebus" },
                        new ShopItemData() { Id = $"{Door.QualifiedItemId}_Chaos", ItemId = $"{Door.QualifiedItemId}_Chaos" },
                        new ShopItemData() { Id = $"{Door.QualifiedItemId}_Attic", ItemId = $"{Door.QualifiedItemId}_{Door.SoundWoodStep}Attic" },
                        new ShopItemData() { Id = $"{Door.QualifiedItemId}_Loft", ItemId = $"{Door.QualifiedItemId}_{Door.SoundWoodStep}Loft" },
                        new ShopItemData() { Id = $"{Door.QualifiedItemId}_Cellar", ItemId = $"{Door.QualifiedItemId}_{Door.SoundWoodStep}Cellar" },
                        new ShopItemData() { Id = Painting.QualifiedItemId, ItemId = Painting.QualifiedItemId },
                        new ShopItemData() { Id = SpaceTimeSynchronizer.QualifiedItemId, ItemId = SpaceTimeSynchronizer.QualifiedItemId },
                        new ShopItemData() { Id = VoidSeal.QualifiedItemId, ItemId = VoidSeal.QualifiedItemId },
                    });

                });
            }
        }

        public static void addDoor(IDictionary<string, string> data, string name, int price, string translation, int index, string sound = "", int w = 1, int h = 3)
        {
            data.Add($"{Door.ItemId}_{sound}{name}",
                $"Dimension Door - {name}" + //name
                "/painting" + //type
                $"/{w} {h}" + //tilesheet size
                $"/{w} {h}" + //bounding box size
                "/1" + //rotations
                $"/{price}" + //price
                "/-1" + //placement restriction
                $"/[LocalizedText Strings\\Furniture:DLX.PIF.{translation}]" + //display name
                $"/{index}" + //Index in sprite sheet
                $"/{SpriteSheetAsset}"
            );
        }
    }
}

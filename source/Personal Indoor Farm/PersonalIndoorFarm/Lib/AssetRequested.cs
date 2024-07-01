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
using xTile;
using static PersonalIndoorFarm.ModEntry;

namespace PersonalIndoorFarm.Lib
{
    public class AssetRequested
    {
        public const string FarmsAsset = "DLX.PIF/Farms";
        public const string DoorsAsset = "DLX.PIF/Doors";
        public const string SpriteSheetAsset = "DLX.PIF_SpriteSheet";
        public const string ShopId = "DLX.PIF.Magician";
        private static Texture2D _SpriteSheetTexture;
        public static Texture2D SpriteSheetTexture { get => _SpriteSheetTexture ??= Helper.GameContent.Load<Texture2D>(SpriteSheetAsset); set => _SpriteSheetTexture = value; }
        public static void Initialize()
        {
            Helper.Events.Content.AssetRequested += OnAssetRequested;
        }

        private static void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(FarmsAsset)) {
                //We need to provide an empty initial dictionary where others can append their changes
                e.LoadFrom(delegate () {
                    return new Dictionary<string, PersonalFarmModel>() { };
                }, AssetLoadPriority.Exclusive);

            } else if (e.NameWithoutLocale.IsEquivalentTo(DoorsAsset)) {
                e.LoadFrom(delegate () {
                    return new Dictionary<string, DoorAssetModel>() {
                        { $"{Door.ItemId}_Aether", new(DoorSoundEnum.Door, "Aether")},
                        { $"{Door.ItemId}_Erebus", new(DoorSoundEnum.Door, "Erebus") },
                        { $"{Door.ItemId}_Chaos", new(DoorSoundEnum.Door, "Chaos") },

                        { $"{Door.ItemId}_WoodStep_Attic", new(DoorSoundEnum.WoodStep, "WoodStep_Attic") },
                        { $"{Door.ItemId}_WoodStep_Loft", new(DoorSoundEnum.WoodStep, "WoodStep_Loft") },
                        { $"{Door.ItemId}_WoodStep_Cellar", new(DoorSoundEnum.WoodStep, "WoodStep_Cellar") },

                        { $"{Door.ItemId}_Spa", new(DoorSoundEnum.Door, "Spa") },

                        //Vanilla
                        { "DecorativeJojaDoor", new(DoorSoundEnum.Door, "Vanilla.DecorativeJojaDoor") },
                        { "DecorativeWizardDoor", new(DoorSoundEnum.Door, "Vanilla.DecorativeWizardDoor") },
                        { "DecorativeJunimoDoor", new(DoorSoundEnum.Door, "Vanilla.DecorativeJunimoDoor") },
                        { "DecorativeRetroDoor", new(DoorSoundEnum.Door, "Vanilla.DecorativeRetroDoor") },
                        { "DecorativeDoor1", new(DoorSoundEnum.Door, "Vanilla.DecorativeDoor1") },
                        { "DecorativeDoor2", new(DoorSoundEnum.Door, "Vanilla.DecorativeDoor2") },
                        { "DecorativeDoor3", new(DoorSoundEnum.Door, "Vanilla.DecorativeDoor3") },
                        { "DecorativeDoor4", new(DoorSoundEnum.Door, "Vanilla.DecorativeDoor4") },
                        { "DecorativeDoor5", new(DoorSoundEnum.Door, "Vanilla.DecorativeDoor5") },
                        { "DecorativeDoor6", new(DoorSoundEnum.Door, "Vanilla.DecorativeDoor6") },
                        { "DecorativeOakLadder", new(DoorSoundEnum.Door, "Vanilla.DecorativeOakLadder") },
                        { "DecorativeWalnutLadder", new(DoorSoundEnum.Door, "Vanilla.DecorativeWalnutLadder") },
                        { "DecorativeHatch", new(DoorSoundEnum.Door, "Vanilla.DecorativeHatch") },

                        //VMV
                        { "Lumisteria.MtVapius_FurnitureDeluxeSet_Door01", new() },
                        { "Lumisteria.MtVapius_FurnitureDarkDeluxeSet_Door01", new() },
                    };
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
                    addDoor(data, "Chaos", 20000, "Door3.Name", 7);

                    addDoor(data, "WoodStep_Attic", 20000, "DoorAttic.Name", 30, 2);
                    addDoor(data, "WoodStep_Loft", 20000, "DoorLoft.Name", 32, 2);
                    addDoor(data, "WoodStep_Cellar", 20000, "DoorCellar.Name", 34, 2);

                    addDoor(data, "Spa", 20000, "DoorSpa.Name", 36);

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

            } else if (e.NameWithoutLocale.IsEquivalentTo("Strings/Furniture")) {
                e.Edit(asset => {
                    var editor = asset.AsDictionary<string, string>();
                    var data = editor.Data;

                    data.Add("DLX.PIF.Door1.Name", Helper.Translation.Get("Door1.Name"));
                    data.Add("DLX.PIF.Door2.Name", Helper.Translation.Get("Door2.Name"));
                    data.Add("DLX.PIF.Door3.Name", Helper.Translation.Get("Door3.Name"));

                    data.Add("DLX.PIF.DoorAttic.Name", Helper.Translation.Get("DoorAttic.Name"));
                    data.Add("DLX.PIF.DoorLoft.Name", Helper.Translation.Get("DoorLoft.Name"));
                    data.Add("DLX.PIF.DoorCellar.Name", Helper.Translation.Get("DoorCellar.Name"));

                    data.Add("DLX.PIF.DoorSpa.Name", Helper.Translation.Get("DoorSpa.Name"));

                    data.Add("DLX.PIF.Painting.Name", Helper.Translation.Get("Painting.Name"));
                });

            } else if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects")) {
                e.Edit(asset => {
                    var editor = asset.AsDictionary<string, ObjectData>();

                    editor.Data.Add(SpaceTimeSynchronizer.ItemId, new ObjectData() {
                        Name = "DLX.PIF.SpaceTimeSynchronizer",
                        DisplayName = Helper.Translation.Get("Synchronizer.Name"),
                        Description = Helper.Translation.Get("Synchronizer.Description"),
                        Type = "Basic",
                        Category = StardewValley.Object.artisanGoodsCategory,
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
                        Name = "DLX.PIF.VoidSealLiquid",
                        DisplayName = Helper.Translation.Get("VoidSeal.Name"),
                        Description = Helper.Translation.Get("VoidSeal.Description"),
                        Type = "Basic",
                        Category = StardewValley.Object.artisanGoodsCategory,
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

                    editor.Data.Add(Key.ItemId, new ObjectData() {
                        Name = "DLX.PIF.Key",
                        DisplayName = Helper.Translation.Get("Key.Name"),
                        Description = Helper.Translation.Get("Key.Description"),
                        Type = "Basic",
                        Category = 0,
                        Price = 1250,
                        Texture = SpriteSheetAsset,
                        SpriteIndex = 25,
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
                    editor.Data.Add(ShopId, new() {
                        Items = new() {
                        new() { Id = SpaceTimeSynchronizer.QualifiedItemId, ItemId = SpaceTimeSynchronizer.QualifiedItemId },
                        new() { Id = VoidSeal.QualifiedItemId, ItemId = VoidSeal.QualifiedItemId },
                        new() { Id = Painting.QualifiedItemId, ItemId = Painting.QualifiedItemId },
                        new() { Id = Key.QualifiedItemId, ItemId = Key.QualifiedItemId },

                        new() { Id = $"{Door.QualifiedItemId}_Aether", ItemId = $"{Door.QualifiedItemId}_Aether" },
                        new() { Id = $"{Door.QualifiedItemId}_Erebus", ItemId = $"{Door.QualifiedItemId}_Erebus" },
                        new() { Id = $"{Door.QualifiedItemId}_Chaos", ItemId = $"{Door.QualifiedItemId}_Chaos" },
                        new() { Id = $"{Door.QualifiedItemId}_Attic", ItemId = $"{Door.QualifiedItemId}_WoodStep_Attic" },
                        new() { Id = $"{Door.QualifiedItemId}_Loft", ItemId = $"{Door.QualifiedItemId}_WoodStep_Loft" },
                        new() { Id = $"{Door.QualifiedItemId}_Cellar", ItemId = $"{Door.QualifiedItemId}_WoodStep_Cellar" },
                        new() { Id = $"{Door.QualifiedItemId}_Spa", ItemId = $"{Door.QualifiedItemId}_Spa" },
                    }
                    });
                });

            } else if (e.NameWithoutLocale.IsEquivalentTo("Maps/Tunnel")) {
                e.Edit(asset => {
                    var editor = asset.AsMap();
                    var patch = Helper.ModContent.Load<Map>("assets/Shopkeeper.tmx");
                    editor.PatchMap(patch, null, targetArea: new Microsoft.Xna.Framework.Rectangle(10, 6, 1, 2));
                });
            }
        }

        public static void addDoor(IDictionary<string, string> data, string name, int price, string translation, int index, int w = 1, int h = 3)
        {
            data.Add($"{Door.ItemId}_{name}",
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

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Models.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using StardewValley;
using StardewValley.GameData.FishPond;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace ItemResearchSpawner.Components
{
    /**
        MIT License

        Copyright (c) 2018 CJBok

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.
     **/
    internal class ItemRepository
    {
        private const int CustomIDOffset = 1000;

        public IEnumerable<SearchableItem> GetAll()
        {
            IEnumerable<SearchableItem> GetAllRaw()
            {
                for (var q = Tool.stone; q <= Tool.iridium; q++)
                {
                    var quality = q;

                    yield return TryCreate(ItemType.Tool, ToolFactory.axe,
                        _ => ToolFactory.getToolFromDescription(ToolFactory.axe, quality));
                    yield return TryCreate(ItemType.Tool, ToolFactory.hoe,
                        _ => ToolFactory.getToolFromDescription(ToolFactory.hoe, quality));
                    yield return TryCreate(ItemType.Tool, ToolFactory.pickAxe,
                        _ => ToolFactory.getToolFromDescription(ToolFactory.pickAxe, quality));
                    yield return TryCreate(ItemType.Tool, ToolFactory.wateringCan,
                        _ => ToolFactory.getToolFromDescription(ToolFactory.wateringCan, quality));
                    if (quality != Tool.iridium)
                        yield return TryCreate(ItemType.Tool, ToolFactory.fishingRod,
                            _ => ToolFactory.getToolFromDescription(ToolFactory.fishingRod, quality));
                }

                // these don't have any sort of ID, so we'll just assign some arbitrary ones
                yield return TryCreate(ItemType.Tool, CustomIDOffset, _ => new MilkPail());
                yield return TryCreate(ItemType.Tool, CustomIDOffset + 1, _ => new Shears());
                yield return TryCreate(ItemType.Tool, CustomIDOffset + 2, _ => new Pan());
                yield return TryCreate(ItemType.Tool, CustomIDOffset + 3, _ => new Wand());

                {
                    var clothingIds = new HashSet<int>();
                    foreach (var id in Game1.clothingInformation.Keys)
                    {
                        if (id < 0) continue;

                        clothingIds.Add(id);
                        yield return TryCreate(ItemType.Clothing, id, p => new Clothing(p.ID));
                    }

                    for (var id = 1000; id <= 1111; id++)
                        if (!clothingIds.Contains(id))
                            yield return TryCreate(ItemType.Clothing, id, p => new Clothing(p.ID));
                }

                for (var id = 0; id < 112; id++)
                    yield return TryCreate(ItemType.Wallpaper, id,
                        p => new Wallpaper(p.ID) {Category = SObject.furnitureCategory});

                for (var id = 0; id < 56; id++)
                    yield return TryCreate(ItemType.Flooring, id,
                        p => new Wallpaper(p.ID, true) {Category = SObject.furnitureCategory});

                foreach (var id in TryLoad<int, string>("Data\\Boots").Keys)
                    yield return TryCreate(ItemType.Boots, id, p => new Boots(p.ID));

                foreach (var id in TryLoad<int, string>("Data\\hats").Keys)
                    yield return TryCreate(ItemType.Hat, id, p => new Hat(p.ID));


                foreach (var id in TryLoad<int, string>("Data\\weapons").Keys)
                    yield return TryCreate(ItemType.Weapon, id, p => p.ID is >= 32 and <= 34
                        ? new Slingshot(p.ID)
                        : new MeleeWeapon(p.ID)
                    );

                foreach (var id in TryLoad<int, string>("Data\\Furniture").Keys)
                {
                    yield return TryCreate(ItemType.Furniture, id, p => Furniture.GetFurnitureInstance(p.ID));
                }

                foreach (var id in Game1.bigCraftablesInformation.Keys)
                {
                    yield return TryCreate(ItemType.BigCraftable, id, p => new SObject(Vector2.Zero, p.ID));
                }

                foreach (var id in Game1.objectInformation.Keys)
                {
                    var fields = Game1.objectInformation[id]?.Split('/');

                    if (id == 79)
                    {
                        foreach (var secretNoteId in TryLoad<int, string>("Data\\SecretNotes").Keys)
                        {
                            yield return TryCreate(ItemType.Object, CustomIDOffset + secretNoteId, _ =>
                            {
                                var note = new SObject(79, 1);
                                note.name = $"{note.name} #{secretNoteId}";
                                return note;
                            });
                        }
                    }

                    else if (id != 801 && fields?.Length >= 4 && fields[3] == "Ring")
                    {
                        yield return TryCreate(ItemType.Ring, id, p => new Ring(p.ID));
                    }

                    else
                    {
                        SObject item = null;
                        yield return TryCreate(ItemType.Object, id, p =>
                        {
                            return item = p.ID == 812 // roe
                                ? new ColoredObject(p.ID, 1, Color.White)
                                : new SObject(p.ID, 1);
                        });
                        if (item == null)
                            continue;

                        switch (item.Category)
                        {
                            case SObject.FruitsCategory:
                                yield return TryCreate(ItemType.Object, CustomIDOffset * 2 + item.ParentSheetIndex, _ =>
                                    new SObject(348, 1)
                                    {
                                        Name = $"{item.Name} Wine",
                                        Price = item.Price * 3,
                                        preserve = {SObject.PreserveType.Wine},
                                        preservedParentSheetIndex = {item.ParentSheetIndex}
                                    });

                                yield return TryCreate(ItemType.Object, CustomIDOffset * 3 + item.ParentSheetIndex, _ =>
                                    new SObject(344, 1)
                                    {
                                        Name = $"{item.Name} Jelly",
                                        Price = 50 + item.Price * 2,
                                        preserve = {SObject.PreserveType.Jelly},
                                        preservedParentSheetIndex = {item.ParentSheetIndex}
                                    });
                                break;

                            case SObject.VegetableCategory:
                                yield return TryCreate(ItemType.Object, CustomIDOffset * 4 + item.ParentSheetIndex, _ =>
                                    new SObject(350, 1)
                                    {
                                        Name = $"{item.Name} Juice",
                                        Price = (int) (item.Price * 2.25d),
                                        preserve = {SObject.PreserveType.Juice},
                                        preservedParentSheetIndex = {item.ParentSheetIndex}
                                    });

                                yield return TryCreate(ItemType.Object, CustomIDOffset * 5 + item.ParentSheetIndex, _ =>
                                    new SObject(342, 1)
                                    {
                                        Name = $"Pickled {item.Name}",
                                        Price = 50 + item.Price * 2,
                                        preserve = {SObject.PreserveType.Pickle},
                                        preservedParentSheetIndex = {item.ParentSheetIndex}
                                    });
                                break;

                            case SObject.flowersCategory:
                                yield return TryCreate(ItemType.Object, CustomIDOffset * 5 + item.ParentSheetIndex, _ =>
                                {
                                    var honey = new SObject(Vector2.Zero, 340, $"{item.Name} Honey", false, true,
                                        false, false)
                                    {
                                        Name = $"{item.Name} Honey",
                                        preservedParentSheetIndex = {item.ParentSheetIndex}
                                    };
                                    honey.Price += item.Price * 2;
                                    return honey;
                                });
                                break;

                            case SObject.sellAtFishShopCategory when item.ParentSheetIndex == 812:
                            {
                                GetRoeContextTagLookups(out var simpleTags,
                                    out var complexTags);

                                foreach (var pair in Game1.objectInformation)
                                {
                                    var input =
                                        TryCreate(ItemType.Object, pair.Key, p => new SObject(p.ID, 1))
                                            ?.Item as SObject;
                                    var inputTags = input?.GetContextTags();
                                    if (inputTags?.Any() != true)
                                        continue;

                                    if (!inputTags.Any(tag => simpleTags.Contains(tag)) &&
                                        !complexTags.Any(set => set.All(tag => input.HasContextTag(tag))))
                                        continue;

                                    SObject roe = null;
                                    var color = GetRoeColor(input);
                                    yield return TryCreate(ItemType.Object, CustomIDOffset * 7 + item.ParentSheetIndex,
                                        _ =>
                                        {
                                            roe = new ColoredObject(812, 1, color)
                                            {
                                                name = $"{input.Name} Roe",
                                                preserve = {Value = SObject.PreserveType.Roe},
                                                preservedParentSheetIndex = {Value = input.ParentSheetIndex}
                                            };
                                            roe.Price += input.Price / 2;
                                            return roe;
                                        });

                                    if (roe != null && pair.Key != 698)
                                        yield return TryCreate(ItemType.Object,
                                            CustomIDOffset * 7 + item.ParentSheetIndex, _ =>
                                                new ColoredObject(447, 1, color)
                                                {
                                                    name = $"Aged {input.Name} Roe",
                                                    Category = -27,
                                                    preserve = {Value = SObject.PreserveType.AgedRoe},
                                                    preservedParentSheetIndex = {Value = input.ParentSheetIndex},
                                                    Price = roe.Price * 2
                                                });
                                }
                            }
                                break;
                        }
                    }
                }
            }

            return GetAllRaw().Where(p => p != null);
        }

        private static void GetRoeContextTagLookups(out HashSet<string> simpleTags, out List<List<string>> complexTags)
        {
            simpleTags = new HashSet<string>();
            complexTags = new List<List<string>>();

            foreach (var data in Game1.content.Load<List<FishPondData>>("Data\\FishPondData"))
            {
                if (data.ProducedItems.All(p => p.ItemID != 812))
                    continue;

                if (data.RequiredTags.Count == 1 && !data.RequiredTags[0].StartsWith("!"))
                    simpleTags.Add(data.RequiredTags[0]);
                else
                    complexTags.Add(data.RequiredTags);
            }
        }

        private Dictionary<TKey, TValue> TryLoad<TKey, TValue>(string assetName)
        {
            try
            {
                return Game1.content.Load<Dictionary<TKey, TValue>>(assetName);
            }
            catch (ContentLoadException)
            {
                return new Dictionary<TKey, TValue>();
            }
        }

        private SearchableItem TryCreate(ItemType type, int id, Func<SearchableItem, Item> createItem)
        {
            try
            {
                var item = new SearchableItem(type, id, createItem);
                item.Item.getDescription();
                return item;
            }
            catch
            {
                return null;
            }
        }

        private Color GetRoeColor(SObject fish)
        {
            return fish.ParentSheetIndex == 698
                ? new Color(61, 55, 42)
                : TailoringMenu.GetDyeColor(fish) ?? Color.Orange;
        }
    }
}
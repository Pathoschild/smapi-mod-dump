/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SObject = StardewValley.Object;

namespace Chest_Displays.Utility
{
    public static class Utils
    {
        public static float ItemScale = ModEntry.IConfig.ItemScale;
        public static float ItemTransparency = ModEntry.IConfig.Transparency;

        internal static Dictionary<int, string> objectInformation => Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<int, string> bigCraftablesInformation => Game1.content.Load<Dictionary<int, string>>("Data\\BigCraftablesInformation", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<int, string> clothingInformation => Game1.content.Load<Dictionary<int, string>>("Data\\ClothingInformation", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<int, string> hatsInformation => Game1.content.Load<Dictionary<int, string>>("Data\\Hats", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<int, string> furnitureInformation => Game1.content.Load<Dictionary<int, string>>("Data\\Furniture", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<int, string> weaponsInformation => Game1.content.Load<Dictionary<int, string>>("Data\\Weapons", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<int, string> bootsInformation => Game1.content.Load<Dictionary<int, string>>("Data\\Boots", LocalizedContentManager.LanguageCode.en);

        public static Vector2 PointToVector(Point p) => new(p.X, p.Y);

        public static int getItemType(Item i)
        {
            if (i is Hat) return 2;
            else if (i is Boots) return 8;
            else if (i is Clothing) return 7;
            else if (i is Ring) return 4;
            else if (i is Furniture) return 5;
            else if (i is MeleeWeapon || i is Slingshot) return 9;
            else if (i is Tool) return 6;
            else if (i is SObject sobj && sobj.bigCraftable.Value) return 3;

            return 1;
        }

        public static void drawItem(SpriteBatch spriteBatch, Item i, int itemType, int x, int y, Vector2 location, float layerDepth)
        {
            switch (itemType)
            {
                case 1:
                    i.drawInMenu(spriteBatch, location, 0.5f, ItemTransparency, layerDepth, StackDrawType.Hide, Color.White, false);
                    if (ModEntry.IConfig.DisplayQuality && (i as SObject)!.Quality > 0)
                    {
                        float num = (i as SObject)!.Quality < 4 ? 0.0f : (float)((Math.Cos((double)Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
                        spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(12f, 32f + num), new Rectangle?((i as SObject)!.Quality < 4 ? new Rectangle(338 + ((i as SObject)!.Quality - 1) * 8, 400, 8, 8) : new Rectangle(346, 392, 8, 8)), Color.White, 0.0f, new Vector2(4f, 4f), (float)(3.0 * 0.5 * (1.0 + num)), SpriteEffects.None, layerDepth + 0.0000001f);
                    }
                    break;
                case 3:
                    i.drawInMenu(spriteBatch, location, ItemScale + .05f, ItemTransparency, layerDepth, StackDrawType.Hide, Color.White, false);
                    break;
                case 2:
                case 4:
                case 5:
                case 9:
                case 6:
                case 7:
                case 8:
                    i.drawInMenu(spriteBatch, location, ItemScale, ItemTransparency, layerDepth, StackDrawType.Hide, Color.White, false);
                    break;
            }
        }

        public static Vector2 GetLocationFromItemType(int itemType, int x, int y)
        {
            return itemType switch
            {
                2 => Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64) + 1f - 2, (y * 64 - 64 + 21 + 8 + 4 - 1))),
                3 => Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64), (y * 64 - 64 + 21 + 2 + 4 - 1))),
                4 => Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64) - 1f + 4, (y * 64 - 64 + 21 + 8 + 4 - 1))),
                5 => Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64), (y * 64 - 64 + 21 + 4 + 4 - 1))),
                6 => Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64) + 2f, (y * 64 - 64 + 21 + 4 + 4 - 1))),
                7 => Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64) - 2, (y * 64 - 64 + 21 + 2 + 4 - 1))),
                9 => Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64) + 2f, (y * 64 - 64 + 21 + 4 + 4 - 1))),
                _ => Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64) + 8, (y * 64) - 32)),
            };
        }

        public static float GetDepthFromItemType(int itemType, int x, int y)
        {
            return itemType switch
            {
                2 => (float)((double)Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) + x * 9.99999974737875E-06 + 1.99999994947575E-05),
                4 => (float)((double)Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) + x * 9.99999974737875E-06 + 1.99999994947575E-05),
                5 => (float)((double)Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) + x * 9.99999974737875E-06 + 1.99999994947575E-05),
                6 => (float)((double)Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) + x * 9.99999974737875E-06 + 1.99999994947575E-05),
                7 => (float)((double)Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) + x * 9.99999974737875E-06 + 1.99999994947575E-05),
                9 => (float)((double)Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) + x * 9.99999974737875E-06 + 1.99999994947575E-05),
                _ => (float)((double)Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) + x * 9.99999974737875E-06 + 1.99999994947575E-05),
            };
        }

        public static bool InvalidChest(Chest c) => c.chestType.Value == "Monster" || c.chestType.Value == "OreChest" || c.chestType.Value == "dungeon" || c.chestType.Value == "Grand" || c.giftbox.Value || c.fridge.Value;

        public static IEnumerable<SButton> ParseSButton(string btn)
        {
            string[] btns = btn.Split(',');
            for (int i = 0; i < btns.Length; i++)
                if (Enum.TryParse(btns[i].Trim(), out SButton sbtn))
                    yield return sbtn;
        }

        public static Item? getItemFromName(string name, int itemType, int quality = 0, int upgradeLevel = 0, Color? color = null)
        {
            Item? obj = null;
            if (string.IsNullOrWhiteSpace(name)) return null;
            switch (itemType)
            {
                default:
                case 1:
                    if (color is not null)
                        obj = new ColoredObject(objectInformation.First(x => x.Value.Split('/')[0] == name).Key, 1, color.Value) { Quality = quality };
                    else
                        obj = new SObject(objectInformation.First(x => x.Value.Split('/')[0] == name).Key, 1, quality: quality);
                    break;
                case 2:
                    obj = new Hat(hatsInformation.First(x => x.Value.Split('/')[0] == name).Key);
                    break;
                case 3:
                    obj = new SObject(Vector2.Zero, bigCraftablesInformation.First(x => x.Value.Split('/')[0] == name).Key) { Stack = 1 };
                    break;
                case 4:
                    obj = new Ring(objectInformation.First(x => x.Value.Split('/')[0] == name).Key);
                    break;
                case 5:
                    obj = new Furniture(furnitureInformation.First(x => x.Value.Split('/')[0] == name).Key, Vector2.Zero);
                    break;
                case 6:
                    Tool? t = name switch
                    {
                        nameof(Axe) => new Axe(),
                        nameof(Hoe) => new Hoe(),
                        nameof(Pickaxe) => new Pickaxe(),
                        nameof(Shears) => new Shears(),
                        "Fishing Rod" => new FishingRod(),
                        "Watering Can" => new WateringCan(),
                        "Copper Pan" => new Pan(),
                        "Milk Pail" => new MilkPail(),
                        "Return Scepter" => new Wand(),
                        _ => null
                    };
                    if (t is not null)
                    {
                        t.UpgradeLevel = upgradeLevel;
                        obj = t;
                    }
                    break;
                case 7:
                    obj = new Clothing(clothingInformation.First(x => x.Value.Split('/')[0] == name).Key);
                    break;
                case 8:
                    obj = new Boots(bootsInformation.First(x => x.Value.Split('/')[0] == name).Key);
                    break;
                case 9:
                    Func<KeyValuePair<int, string>, bool> query = x => x.Value.Split('/')[0] == name;
                    if (weaponsInformation.Any(query))
                    {
                        if (name.ToLower().Contains("slingshot"))
                            obj = new Slingshot(weaponsInformation.First(query).Key);
                        else
                            obj = new MeleeWeapon(weaponsInformation.First(query).Key);
                    }
                    break;
            }

            return obj;
        }

        public static int GetItemIndexInParentSheet(Item i, int itemType)
        {
            return itemType switch
            {
                2 => (i as Hat)!.which.Value,
                8 => (i as Boots)!.indexInTileSheet.Value,
                9 => i is MeleeWeapon mw ? mw.CurrentParentTileIndex : (i as Slingshot)!.CurrentParentTileIndex,
                _ => i.ParentSheetIndex
            };
        }

        public static string GetItemNameFromIndex(int parentSheetIndex, int itemType)
        {
            return itemType switch
            {
                2 => hatsInformation[parentSheetIndex].Split('/')[0],
                3 => bigCraftablesInformation[parentSheetIndex].Split('/')[0],
                5 => furnitureInformation[parentSheetIndex].Split('/')[0],
                7 => clothingInformation[parentSheetIndex].Split('/')[0],
                8 => bootsInformation[parentSheetIndex].Split('/')[0],
                9 => weaponsInformation[parentSheetIndex].Split('/')[0],
                _ => objectInformation[parentSheetIndex].Split('/')[0],
            };
        }
    }
}

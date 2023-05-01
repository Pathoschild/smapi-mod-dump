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
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace Trading.Utilities
{
    /// <summary>
    /// Item handler imported from Chest Displays
    /// </summary>
    public class ItemFactory
    {
        internal static Dictionary<int, string> objectInformation => Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<int, string> bigCraftablesInformation => Game1.content.Load<Dictionary<int, string>>("Data\\BigCraftablesInformation", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<int, string> clothingInformation => Game1.content.Load<Dictionary<int, string>>("Data\\ClothingInformation", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<int, string> hatsInformation => Game1.content.Load<Dictionary<int, string>>("Data\\Hats", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<int, string> furnitureInformation => Game1.content.Load<Dictionary<int, string>>("Data\\Furniture", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<int, string> weaponsInformation => Game1.content.Load<Dictionary<int, string>>("Data\\Weapons", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<int, string> bootsInformation => Game1.content.Load<Dictionary<int, string>>("Data\\Boots", LocalizedContentManager.LanguageCode.en);

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

        public static Item? getItemFromName(string name, int itemType, int stack = 1, int quality = 0, int upgradeLevel = 0, Color? color = null, ModDataDictionary? modData = null)
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
                    obj.Stack = stack;
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
            if (modData is not null && obj is not null)
                foreach(var key in modData.Keys)
                    obj.modData[key] = modData[key];
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

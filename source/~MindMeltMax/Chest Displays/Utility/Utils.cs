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
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Pants;
using StardewValley.GameData.Shirts;
using StardewValley.GameData.Tools;
using StardewValley.GameData.Weapons;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestDisplays.Utility
{
    public static class Utils
    {
        private static float ItemScale = ModEntry.IConfig.ItemScale;
        private static float ItemTransparency = ModEntry.IConfig.Transparency;

        internal static Dictionary<string, ObjectData> objects = Game1.content.Load<Dictionary<string, ObjectData>>("Data\\Objects", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<string, BigCraftableData> bigCraftables = Game1.content.Load<Dictionary<string, BigCraftableData>>("Data\\BigCraftables", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<string, ShirtData> shirts = Game1.content.Load<Dictionary<string, ShirtData>>("Data\\Shirts", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<string, PantsData> pants = Game1.content.Load<Dictionary<string, PantsData>>("Data\\Pants", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<string, string> hats = Game1.content.Load<Dictionary<string, string>>("Data\\hats", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<string, string> furniture = Game1.content.Load<Dictionary<string, string>>("Data\\Furniture", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<string, WeaponData> weapons = Game1.content.Load<Dictionary<string, WeaponData>>("Data\\Weapons", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<string, ToolData> tools = Game1.content.Load<Dictionary<string, ToolData>>("Data\\Tools", LocalizedContentManager.LanguageCode.en);
        internal static Dictionary<string, string> boots = Game1.content.Load<Dictionary<string, string>>("Data\\Boots", LocalizedContentManager.LanguageCode.en);

        internal static Dictionary<Chest, Item> displayItemsCache = new();

        public static int getItemType(Item i)
        {
            return i switch
            {
                Hat _ => 2,
                Boots _ => 8,
                Clothing _ => 7,
                Ring _ => 4,
                Furniture _ => 5,
                MeleeWeapon _ => 9,
                Slingshot _ => 9,
                Tool _ => 6,
                Object o => o.bigCraftable.Value ? 3 : 1,
                _ => 1
            };
        }

        public static void drawItem(SpriteBatch spriteBatch, Item i, int itemType, Vector2 location, float layerDepth)
        {
            switch (itemType)
            {
                case 1:
                    int quality = (i as Object)!.Quality;
                    i.drawInMenu(spriteBatch, location, 0.5f, ItemTransparency, layerDepth, StackDrawType.Hide, Color.White, false);
                    if (ModEntry.IConfig.DisplayQuality && quality > 0)
                    {
                        float num = quality < 4 ? 0.0f : (float)((Math.Cos((double)Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
                        spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(12f, 32f + num), new Rectangle?(quality < 4 ? new Rectangle(338 + (quality - 1) * 8, 400, 8, 8) : new Rectangle(346, 392, 8, 8)), Color.White, 0.0f, new Vector2(4f, 4f), (float)(3.0 * 0.5 * (1.0 + num)), SpriteEffects.None, layerDepth + 0.0000001f);
                    }
                    break;
                case 3:
                    i.drawInMenu(spriteBatch, location, ItemScale + .05f, ItemTransparency, layerDepth, StackDrawType.Hide, Color.White, false);
                    break;
                default:
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

        public static bool InvalidChest(Chest c) => c.Type != "Crafting" || c.giftbox.Value || c.fridge.Value;

        public static IEnumerable<SButton> ParseSButton(string btn)
        {
            string[] btns = btn.Split(',');
            for (int i = 0; i < btns.Length; i++)
                if (Enum.TryParse(btns[i].Trim(), out SButton sbtn))
                    yield return sbtn;
        }

        internal static void updateCache(Chest c)
        {
            displayItemsCache.Remove(c);
            if (!c.modData.ContainsKey(ModEntry.IHelper.ModRegistry.ModID))
                return;
            var data = JsonConvert.DeserializeObject<ModData>(c.modData[ModEntry.IHelper.ModRegistry.ModID]);
            Item? i = BuildItemFromData(data);
            if (i is null)
                return;
            displayItemsCache.Add(c, i);
        }

        public static Item? BuildItemFromData(ModData? data)
        {
            if (data == null) 
                return null;
            if (string.IsNullOrWhiteSpace(data.ItemId) && !string.IsNullOrWhiteSpace(data.Item))
                data = updateOldModdata(data);
            Item? i = ItemRegistry.Create(data.ItemId, quality: data.ItemQuality, allowNull: true);
            if (i is null)
                return null;
            if (i is ColoredObject co)
                co.color.Value = data.Color!.Value;
            if (i is Tool t)
                t.UpgradeLevel = data.UpgradeLevel;
            return i;
        }


        private static string IndexToQualifiedId(string parentSheetIndex, int itemType, string name = "", int upgradeLevel = -1)
        {
            string strindex = parentSheetIndex;
            return itemType switch
            {
                2 => new Hat(strindex).QualifiedItemId,
                4 => new Ring(strindex).QualifiedItemId,
                5 => new Furniture(strindex, Vector2.Zero).QualifiedItemId,
                6 => ToolNameToQualifiedId(name, upgradeLevel),
                7 => new Clothing(strindex).QualifiedItemId,
                8 => new Boots(strindex).QualifiedItemId,
                9 => ItemRegistry.Create($"(W){strindex}").QualifiedItemId,
                _ => new Object(strindex, 1).QualifiedItemId
            };
        }

        private static string ToolNameToQualifiedId(string name, int upgradeLevel)
        {
            if (string.IsNullOrWhiteSpace(name))
                return tools.Keys.First();
            foreach (var tool in tools)
                if (tool.Value.ClassName.ToLower() == name.ToLower() && tool.Value.UpgradeLevel == upgradeLevel)
                    return tool.Key;
            return tools.Keys.First();
        }

        private static string? IndexFromName(string name, int itemType)
        {
            name = name.ToLower();
            return itemType switch
            {
                2 => hats.FirstOrDefault(x => x.Value.Split('/')[0].ToLower() == name).Key,
                3 => bigCraftables.FirstOrDefault(x => x.Value.Name.ToLower() == name).Key,
                5 => furniture.FirstOrDefault(x => x.Value.Split('/')[0].ToLower() == name).Key,
                7 => getClothingIndex(name),
                8 => boots.FirstOrDefault(x => x.Value.Split('/')[0].ToLower() == name).Key,
                9 => weapons.FirstOrDefault(x => x.Value.Name.ToLower() == name).Key,
                _ => objects.FirstOrDefault(x => x.Value.Name.ToLower() == name).Key
            };
        }


        private static string getClothingIndex(string name)
        {
            var _shirt = shirts.FirstOrDefault(x => x.Value.Name.ToLower() == name).Key;
            var _pants = pants.FirstOrDefault(x => x.Value.Name.ToLower() == name).Key;

            if (string.IsNullOrWhiteSpace(_shirt))
                return _pants;
            return _shirt;
        }

        private static ModData updateOldModdata(ModData data)
        {
            string? index = IndexFromName(data.Item, data.ItemType);
            if (string.IsNullOrWhiteSpace(index) && data.ItemType != 6)
                ModEntry.IMonitor.Log($"Failed to update modData from version 1.2.0 -> 1.2.1, could not find an index for item {data.Item} of type {data.ItemType}");
            else
                data.ItemId = IndexToQualifiedId(index!, data.ItemType, data.Item, data.UpgradeLevel);
            data.Item = "";
            data.ItemType = -1;
            return data;
        }
    }
}

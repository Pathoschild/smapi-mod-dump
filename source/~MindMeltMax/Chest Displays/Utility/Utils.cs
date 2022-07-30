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
    public class Utils
    {
        public static float ItemScale = ModEntry.RequestableConfig.ItemScale;
        public static float ItemTransparency = ModEntry.RequestableConfig.Transparency;

        public static int getItemType(Item i)
        {
            if (i is Hat) return 2;
            else if (i is Boots) return 8;
            else if (i is Clothing) return 7;
            else if (i is Ring) return 4;
            else if (i is Furniture) return 5;
            else if (i is MeleeWeapon || i is Slingshot || i is Tool) return 6;
            else if (i is SObject) if ((i as SObject).bigCraftable) return 3;

            return 1;
        }

        public static void drawItem(SpriteBatch spriteBatch, Item i, int itemType, int x, int y, Vector2 location, float layerDepth)
        {
            switch (itemType)
            {
                case 1:
                    //spriteBatch.Draw(Game1.menuTexture, location + new Vector2(4f, 4f), new Rectangle?(new Rectangle(0, 320, 64, 64)), Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, (float)((double)Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) + (double)x * 9.99999974737875E-06 + 1.99999994947575E-05)); //Discarded, too buggy
                    if (ModEntry.RequestableConfig.DisplayQuality && (i as SObject).quality > 0)
                    {
                        float num = (int)(i as SObject).quality < 4 ? 0.0f : (float)((Math.Cos((double)Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
                        spriteBatch.Draw(Game1.mouseCursors, location + new Vector2(12f, 32f + num), new Rectangle?((int)(i as SObject).quality < 4 ? new Rectangle(338 + ((int)(i as SObject).quality - 1) * 8, 400, 8, 8) : new Rectangle(346, 392, 8, 8)), Color.White, 0.0f, new Vector2(4f, 4f), (float)(3.0 * 0.5 * (1.0 + num)), SpriteEffects.None, layerDepth);
                    }
                    i.drawInMenu(spriteBatch, location, ItemScale, ItemTransparency, layerDepth, StackDrawType.Hide, Color.White, false);
                    break;
                case 2:
                    i.drawInMenu(spriteBatch, location, ItemScale, ItemTransparency, layerDepth, StackDrawType.Hide, Color.White, false);
                    break;
                case 3:
                    i.drawInMenu(spriteBatch, location, ItemScale, ItemTransparency, layerDepth, StackDrawType.Hide, Color.White, false);
                    break;
                case 4:
                    i.drawInMenu(spriteBatch, location, ItemScale, ItemTransparency, layerDepth, StackDrawType.Hide, Color.White, false);
                    break;
                case 5:
                    i.drawInMenu(spriteBatch, location, ItemScale, ItemTransparency, layerDepth, StackDrawType.Hide, Color.White, false);
                    break;
                case 6:
                    i.drawInMenu(spriteBatch, location, ItemScale, ItemTransparency, layerDepth, StackDrawType.Hide, Color.White, false);
                    break;
                case 7:
                    i.drawInMenu(spriteBatch, location, ItemScale, ItemTransparency, layerDepth, StackDrawType.Hide, Color.White, false);
                    break;
                case 8:
                    i.drawInMenu(spriteBatch, location, ItemScale, ItemTransparency, layerDepth, StackDrawType.Hide, Color.White, false);
                    break;
            }
        }

        public static Vector2 GetLocationFromItemType(int itemType, int x, int y)
        {
            switch (itemType)
            {
                default:
                case 1:
                    return Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64) + 2f + 4, (y * 64 - 64 + 21 + 8 + 4 - 1)));
                case 2:
                    return Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64) + 1f - 2, (y * 64 - 64 + 21 + 8 + 4 - 1)));
                case 3:
                    return Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64), (y * 64 - 64 + 21 + 2 + 4 - 1)));
                case 4:
                    return Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64) - 1f + 4, (y * 64 - 64 + 21 + 8 + 4 - 1)));
                case 5:
                    return Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64), (y * 64 - 64 + 21 + 4 + 4 - 1)));
                case 6:
                    return Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64) + 2f, (y * 64 - 64 + 21 + 4 + 4 - 1)));
                case 7:
                    return Game1.GlobalToLocal(Game1.viewport, new Vector2((x * 64) - 2, (y * 64 - 64 + 21 + 2 + 4 - 1)));
            }
        }

        public static float GetDepthFromItemType(int itemType, int x, int y)
        {
            switch (itemType)
            {
                default:
                case 8:
                case 1:
                    return (float)((double)Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) + (double)x * 9.99999974737875E-06 + 1.99999994947575E-05);
                case 2:
                    return (float)((double)Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) + (double)x * 9.99999974737875E-06 + 1.99999994947575E-05);
                case 3:
                    return (float)((double)Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) + (double)x * 9.99999974737875E-06 + 9.99999974737875E-06);
                case 4:
                    return (float)((double)Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) + (double)x * 9.99999974737875E-06 + 1.99999994947575E-05);
                case 5:
                    return (float)((double)Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) + (double)x * 9.99999974737875E-06 + 1.99999994947575E-05);
                case 6:
                    return (float)((double)Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) + (double)x * 9.99999974737875E-06 + 1.99999994947575E-05);
                case 7:
                    return (float)((double)Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) + (double)x * 9.99999974737875E-06 + 1.99999994947575E-05);
            }
        }

        public static bool isZeroType(Item i)
        {
            return (i is MeleeWeapon || i is Slingshot || i is Tool || i is Hat || i is Clothing || i is Boots);
        }

        public static bool nullChest(Chest c)
        {
            return (c.chestType == "Monster" || c.chestType == "OreChest" || c.chestType == "dungeon" || c.chestType == "Grand" || c.giftbox.Value || c.fridge.Value);
        }

        public static bool IsChangeItemKey(SButton button)
        {
            string buttonToString = button.ToString().ToLower();
            return ((IEnumerable<string>)ModEntry.RequestableConfig.ChangeItemKey.ToLower().Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries)).Any((Func<string, bool>)(i => buttonToString.Equals(i.Trim())));
        }

        public static Item getItemFromName(string name, Chest container)
        {
            foreach(Item i in container.items)
                if (i.Name == name)
                    return i;
            return null;
        }
    }
}

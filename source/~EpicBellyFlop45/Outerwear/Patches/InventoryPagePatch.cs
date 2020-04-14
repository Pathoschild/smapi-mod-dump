using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Linq;

namespace Outerwear.Patches
{
    internal class InventoryPagePatch
    {
        internal static void ConstructorPostFix(InventoryPage __instance)
        {
            // add outerwear slot
            __instance.equipmentIcons.Add(new ClickableComponent(new Rectangle(__instance.xPositionOnScreen + 48, __instance.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 4 + 448 - 12, 64, 64), "Outerwear")
            {
                myID = 111,
                upNeighborID = 104,
                rightNeighborID = 109,
                fullyImmutable = true
            });

            // recreate 'boots' slot so neighboors are correct (used for navigation with controllers)
            var bootsEquipmentIcon = __instance.equipmentIcons.Where(icon => icon.name == "Boots").FirstOrDefault();
            if (bootsEquipmentIcon != null)
            {
                __instance.equipmentIcons.Remove(bootsEquipmentIcon);
            }

            __instance.equipmentIcons.Add(new ClickableComponent(new Rectangle(__instance.xPositionOnScreen + 48, __instance.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 4 + 384 - 12, 64, 64), "Boots")
            {
                myID = 104,
                upNeighborID = 103,
                rightNeighborID = 109,
                downNeighborID = 111,
                fullyImmutable = true
            });
        }

        internal static void ReceiveLeftClickPostFix(int x, int y, InventoryPage __instance)
        {
            var outerwearEqupimentIcon = __instance.equipmentIcons.Where(icon => icon.name == "Outerwear").FirstOrDefault();
            if (!outerwearEqupimentIcon.containsPoint(x, y))
                return;

            if (Game1.player.CursorSlotItem == null && ModEntry.EquippedOuterwear != null)
            {
                Game1.player.CursorSlotItem = ModEntry.EquippedOuterwear;
                ModEntry.EquippedOuterwear = null;
            }
            else if (Game1.player.CursorSlotItem is Models.Outerwear outerwear)
            {
                (ModEntry.EquippedOuterwear, Game1.player.CursorSlotItem) = (Game1.player.CursorSlotItem, ModEntry.EquippedOuterwear);
            }
        }

        internal static void DrawPostFix(SpriteBatch b, InventoryPage __instance)
        {
            var overwearEquipmentIcon = __instance.equipmentIcons.Where(icon => icon.name == "Outerwear").FirstOrDefault();

            if (ModEntry.EquippedOuterwear != null)
            {
                b.Draw(
                    texture: Game1.menuTexture,
                    destinationRectangle: overwearEquipmentIcon.bounds,
                    sourceRectangle: Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10, -1, -1),
                    color: Color.White,
                    rotation: 0,
                    origin: new Vector2(0, 0),
                    effects: SpriteEffects.None,
                    layerDepth: 0
                );

                ModEntry.EquippedOuterwear.drawInMenu(b, new Vector2(overwearEquipmentIcon.bounds.X, overwearEquipmentIcon.bounds.Y), overwearEquipmentIcon.scale);
            }
            else
            {
                b.Draw(
                    texture: ModEntry.OuterwearSlotPlaceholder,
                    destinationRectangle: overwearEquipmentIcon.bounds,
                    sourceRectangle: new Rectangle(0, 0, 64, 64),
                    color: Color.White,
                    rotation: 0,
                    origin: new Vector2(0, 0),
                    effects: SpriteEffects.None,
                    layerDepth: 0
                );
            }
        }
    }
}

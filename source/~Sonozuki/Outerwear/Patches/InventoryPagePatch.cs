/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Linq;
using System.Reflection;

namespace Outerwear.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="InventoryPage"/> class.</summary>
    internal class InventoryPagePatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The post fix for the <see cref="InventoryPage(int, int, int, int)"/> constructor.</summary>
        /// <param name="__instance">The current <see cref="InventoryPage"/> instance being patched.</param>
        /// <remarks>This is used to add the new outerwear equipment slot.</remarks>
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

            // set the neighbour of the 'boots' slot
            var bootsEquipmentIcon = __instance.equipmentIcons.FirstOrDefault(icon => icon.name == "Boots");
            if (bootsEquipmentIcon != null)
                bootsEquipmentIcon.downNeighborID = 111;
        }

        /// <summary>The post fix for the <see cref="InventoryPage.performHoverAction(int, int)"/> method.</summary>
        /// <param name="x">The X position of the mouse.</param>
        /// <param name="y">The Y position of the mouse.</param>
        /// <param name="__instance">The current <see cref="InventoryPage"/> instance being patched.</param>
        /// <remarks>This is used to add the tool tip for hovering over the currently equipped outerwear.</remarks>
        internal static void PerformHoverActionPostFix(int x, int y, InventoryPage __instance)
        {
            // ensure the player is hovering on the outerwear slot
            var outerwearEqupimentIcon = __instance.equipmentIcons.FirstOrDefault(icon => icon.name == "Outerwear");
            if (!outerwearEqupimentIcon.containsPoint(x, y))
                return;

            // get the currently equipped outerwear
            var equippedOuterwear = ModEntry.Instance.Api.GetEquippedOuterwear();
            if (equippedOuterwear == null)
                return;

            // set hover values for drawing tool tip
            typeof(InventoryPage).GetField("hoveredItem", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, equippedOuterwear);
            typeof(InventoryPage).GetField("hoverTitle", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, equippedOuterwear.DisplayName);
            typeof(InventoryPage).GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, equippedOuterwear.getDescription());
        }

        /// <summary>The post fix for the <see cref="InventoryPage.receiveLeftClick(int, int, bool)"/> method.</summary>
        /// <param name="x">The X position of the mouse.</param>
        /// <param name="y">The Y postiion of the mouse.</param>
        /// <param name="__instance">The current <see cref="InventoryPage"/> instance being patched.</param>
        internal static void ReceiveLeftClickPostFix(int x, int y, InventoryPage __instance)
        {
            // ensure the player clicked on the outerwear slot
            var outerwearEqupimentIcon = __instance.equipmentIcons.FirstOrDefault(icon => icon.name == "Outerwear");
            if (!outerwearEqupimentIcon.containsPoint(x, y))
                return;

            // get the currently equipped outerwear
            var equippedOuterwear = ModEntry.Instance.Api.GetEquippedOuterwear();

            if (Game1.player.CursorSlotItem == null && equippedOuterwear != null)
            {
                Game1.player.CursorSlotItem = equippedOuterwear;
                ModEntry.Instance.Api.UnequipOuterwear();

                Game1.playSound("dwop");
            }
            else if (Game1.player.CursorSlotItem != null && ModEntry.Instance.Api.IsOuterwear(Game1.player.CursorSlotItem.ParentSheetIndex))
            {
                var oldOuterwear = ModEntry.Instance.Api.GetEquippedOuterwear();

                ModEntry.Instance.Api.EquipOuterwear(Game1.player.CursorSlotItem);
                Game1.player.CursorSlotItem = oldOuterwear;

                Game1.playSound("sandyStep");
            }
        }

        /// <summary>The post fix for the <see cref="InventoryPage.draw(SpriteBatch)"/> method.</summary>
        /// <param name="b">The sprite batch to draw to.</param>
        /// <param name="__instance">The current <see cref="InventoryPage"/> instance being patched.</param>
        /// <remarks>This is used to draw the new outerwear slot.</remarks>
        internal static void DrawPostFix(SpriteBatch b, InventoryPage __instance)
        {
            var overwearEquipmentIcon = __instance.equipmentIcons.FirstOrDefault(icon => icon.name == "Outerwear");
            var equippedOuterwear = ModEntry.Instance.Api.GetEquippedOuterwear();

            if (equippedOuterwear != null)
            {
                // draw plain slot icon
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

                // draw outerwear on slot
                equippedOuterwear.drawInMenu(b, new Vector2(overwearEquipmentIcon.bounds.X, overwearEquipmentIcon.bounds.Y), overwearEquipmentIcon.scale);
            }
            else
            {
                // draw slot with the outerwear placeholder icon
                b.Draw(
                    texture: ModEntry.Instance.OuterwearSlotPlaceholder,
                    destinationRectangle: overwearEquipmentIcon.bounds,
                    sourceRectangle: new Rectangle(0, 0, 64, 64),
                    color: Color.White,
                    rotation: 0,
                    origin: new Vector2(0, 0),
                    effects: SpriteEffects.None,
                    layerDepth: 0
                );
            }

            // redraw the held item so it's drawn over the outerwear slot
            Game1.player.CursorSlotItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 16, Game1.getOldMouseY() + 16), 1f);
        }
    }
}

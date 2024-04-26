/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Menus;
using StardewValley.Objects;
using System.Reflection;
using System.Reflection.Emit;

namespace CustomNames
{
    internal static class ForgeMenuExtensions
    { 
        private static readonly PerScreen<TextBox> textBox = new(() => null!);
        private static readonly PerScreen<bool> isSnappedToTextBox = new(() => false);
        private static readonly PerScreen<string?> textCache = new(() => null);
        private static readonly PerScreen<Item?> leftItem = new(() => null);
        private static readonly PerScreen<bool> isUnforging = new(() => false);
        private static FieldInfo? craftStateField;
        private static FieldInfo? unforgingField;
        private static FieldInfo? craftTimeField;
        private static MethodInfo? updateDescriptionMethod;
        private static MethodInfo? validateCraftMethod;

        public static void Patch(Harmony harmony)
        {
            Init();

            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.GetForgeCost)),
                postfix: new(typeof(ForgeMenuExtensions), nameof(ForgeMenu_GetForgeCost_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.IsValidCraft)),
                postfix: new(typeof(ForgeMenuExtensions), nameof(ForgeMenu_IsValidCraft_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.HighlightItems)),
                postfix: new(typeof(ForgeMenuExtensions), nameof(ForgeMenu_HighlightItems_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), "cleanupBeforeExit"),
                postfix: new(typeof(ForgeMenuExtensions), nameof(ForgeMenu_CleanupBeforeExit_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.CraftItem)),
                prefix: new(typeof(ForgeMenuExtensions), nameof(ForgeMenu_CraftItem_Prefix)),
                postfix: new(typeof(ForgeMenuExtensions), nameof(ForgeMenu_CraftItem_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), "_ValidateCraft"),
                postfix: new(typeof(ForgeMenuExtensions), nameof(ForgeMenu_ValidateCraft_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.receiveKeyPress)),
                prefix: new(typeof(ForgeMenuExtensions), nameof(ForgeMenu_ReceiveKeyPress_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.performHoverAction)),
                postfix: new(typeof(ForgeMenuExtensions), nameof(ForgeMenu_PerformHoverAction_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.update)),
                postfix: new(typeof(ForgeMenuExtensions), nameof(ForgeMenu_Update_Postfix)),
                transpiler: new(typeof(ForgeMenuExtensions), nameof(ForgeMenu_Update_Transpiler))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.gameWindowSizeChanged)),
                postfix: new(typeof(ForgeMenuExtensions), nameof(ForgeMenu_GameWindowSizeChanged_Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.receiveLeftClick)),
                postfix: new(typeof(ForgeMenuExtensions), nameof(ForgeMenu_ReceiveLeftClick_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.draw), [typeof(SpriteBatch)]),
                transpiler: new(typeof(ForgeMenuExtensions), nameof(ForgeMenu_Draw_Transpiler))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), "_leftIngredientSpotClicked"),
                transpiler: new(typeof(ForgeMenuExtensions), nameof(ForgeMenu_LeftIngredientSpotClicked_Transpiler))
            );
        }

        private static void Init()
        {
            textBox.Value = new(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), Game1.staminaRect, Game1.smallFont, Game1.textColor)
            {
                X = Game1.uiViewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2 + 204,
                Y = Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 + 212 - 112,
                Text = null,
                limitWidth = false,
                textLimit = 20
            };
            craftStateField = typeof(ForgeMenu).GetField("_craftState", BindingFlags.Instance | BindingFlags.NonPublic);
            unforgingField = typeof(ForgeMenu).GetField("unforging", BindingFlags.Instance | BindingFlags.NonPublic);
            craftTimeField = typeof(ForgeMenu).GetField("_timeUntilCraft", BindingFlags.Instance | BindingFlags.NonPublic);
            updateDescriptionMethod = typeof(ForgeMenu).GetMethod("_UpdateDescriptionText", BindingFlags.Instance | BindingFlags.NonPublic);
            validateCraftMethod = typeof(ForgeMenu).GetMethod("_ValidateCraft", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        private static void drawExtensions(ForgeMenu menu, SpriteBatch b)
        {
            if (lightUpLeftIngredientSpot(menu))
                menu.leftIngredientSpot.draw(b, Color.White, 0.87f);
            if (menu.leftIngredientSpot.item is null)
                return;
            textBox.Value?.Draw(b, false);
            if (menu.IsValidCraft(menu.leftIngredientSpot.item, menu.rightIngredientSpot.item))
            {
                int num = (menu.GetForgeCost(menu.leftIngredientSpot.item, menu.rightIngredientSpot.item) - 10) / 5;
                if (num >= 0 && num < 3)
                    return;
                ForgeMenu.CraftState craftState = (ForgeMenu.CraftState)craftStateField!.GetValue(menu)!;
                switch (num)
                {
                    case -1:
                        b.Draw(menu.forgeTextures, new Vector2(menu.xPositionOnScreen + 344, menu.yPositionOnScreen + 320), new(142, 48, 6, 10), Color.White * (craftState == ForgeMenu.CraftState.MissingShards ? 0.5f : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
                        b.Draw(menu.forgeTextures, new Vector2(menu.xPositionOnScreen + 344 + 24, menu.yPositionOnScreen + 320), new(153, 48, 6, 10), Color.White * (craftState == ForgeMenu.CraftState.MissingShards ? 0.5f : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
                        break;
                    case 3:
                        b.Draw(menu.forgeTextures, new Vector2(menu.xPositionOnScreen + 344, menu.yPositionOnScreen + 320), new(142, 58, 11, 10), Color.White * (craftState == ForgeMenu.CraftState.MissingShards ? 0.5f : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
                        b.Draw(menu.forgeTextures, new Vector2(menu.xPositionOnScreen + 344 + 44, menu.yPositionOnScreen + 320), new(153, 48, 6, 10), Color.White * (craftState == ForgeMenu.CraftState.MissingShards ? 0.5f : 1f), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.1f);
                        break;
                }
            }
        }

        private static bool isDifferentName(Item item)
        {
            string? text = textBox.Value.Text;
            if (string.IsNullOrWhiteSpace(text) && !(item?.modData.ContainsKey(ModEntry.ModDataKey) ?? false))
                text = item?.DisplayName;
            return text != item?.DisplayName;
        }

        private static void updatePosition()
        {
            textBox.Value.X = Game1.uiViewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2 + 204 - (textBox.Value.Width / 2) + 100;
            textBox.Value.Y = Game1.uiViewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 + 212 - 112;
        }

        private static bool lightUpLeftIngredientSpot(ForgeMenu menu)
        {
            if ((menu.hoveredItem is Trinket || menu.hoveredItem is Ring) && menu.hoveredItem != menu.leftIngredientSpot.item)
                return true;
            if (menu.heldItem is Trinket || menu.heldItem is Ring)
                return true;
            return false;
        }

        private static void tryDoUnforge(ForgeMenu menu)
        {
            if (!ModEntry.IConfig.UnforgeClearsName)
                return;
            Item left = menu.leftIngredientSpot.item;
            if (left.modData.ContainsKey(ModEntry.ModDataKey))
                left.modData.Remove(ModEntry.ModDataKey);
        }

        private static bool canAddToLeftIngredientSpot(ForgeMenu menu)
        {
            return menu.heldItem is Item i && (i is Trinket || i is Tool || i is Ring);
        }

        #region Patches

        private static void ForgeMenu_GetForgeCost_Postfix(Item left_item, Item right_item, ref int __result)
        {
            if (left_item is null)
                return;
            if (right_item is null)
                __result = 0;
            if (isDifferentName(left_item) && !string.IsNullOrWhiteSpace(textBox.Value.Text))
                __result += ModEntry.IConfig.CostToName;
        }

        private static void ForgeMenu_IsValidCraft_Postfix(Item left_item, Item right_item, ref bool __result)
        {
            bool flag1 = left_item is not null;
            bool flag2 = (right_item is null && isDifferentName(left_item)) || (left_item is Tool t && t.CanForge(right_item)) || (left_item is Ring left && right_item is Ring right && left.CanCombine(right));
            bool flag3 = isDifferentName(left_item);//!(left_item?.modData.TryGetValue(ModEntry.ModDataKey, out string value) ?? false) || value != textBox.Value.Text && textBox.Value.Text != left_item.DisplayName;
            __result = flag1 && (flag2 || flag3);
        }

        private static void ForgeMenu_HighlightItems_Postfix(Item i, ref bool __result)
        {
            if (__result)
                return;
            if (i is Tool || i is Ring || i is Trinket)
                __result = true;
        }

        private static void ForgeMenu_CleanupBeforeExit_Postfix() => textBox.Value.Text = "";

        private static bool ForgeMenu_CraftItem_Prefix(Item left_item, Item right_item, ref Item __result, ref string __state)
        {
            __state = textBox.Value.Text;
            if (right_item is not null)
                return true;
            __result = left_item.getOne();
            return false;
        }

        private static void ForgeMenu_CraftItem_Postfix(ref Item __result, ref string __state)
        {
            if (string.IsNullOrWhiteSpace(__state))
            {
                if (__result is not null && __result.modData.TryGetValue(ModEntry.ModDataKey, out _))
                    __result.modData.Remove(ModEntry.ModDataKey);
                return;
            }
            __result.modData[ModEntry.ModDataKey] = __state;
        }

        private static void ForgeMenu_ValidateCraft_Postfix(ForgeMenu __instance)
        {
            if ((ForgeMenu.CraftState)craftStateField?.GetValue(__instance)! == ForgeMenu.CraftState.Valid)
                return;
            Item left = __instance.leftIngredientSpot.item;
            Item right = __instance.rightIngredientSpot.item;
            if (__instance.IsValidCraft(left, right))
            {
                craftStateField.SetValue(__instance, ForgeMenu.CraftState.Valid);
                Item left_one = left.getOne();
                if (right?.QualifiedItemId == "(O)72") //right item is not null and Diamond \\ Probs don't need this, because if right item is not null original will say "Yup, valid", but I honestly don't want to risk it
                {
                    (left_one as Tool)!.AddEnchantment(new DiamondEnchantment());
                    __instance.craftResultDisplay.item = left_one;
                }
                else
                    __instance.craftResultDisplay.item = __instance.CraftItem(left_one, right?.getOne());
            }
            updateDescriptionMethod!.Invoke(__instance, null);
        }

        private static bool ForgeMenu_ReceiveKeyPress_Prefix()
        {
            if (textBox.Value.Selected)
                return false;
            return true;
        }

        private static void ForgeMenu_PerformHoverAction_Postfix(int x, int y) => textBox.Value.Hover(x, y);

        private static void ForgeMenu_Update_Postfix(ForgeMenu __instance)
        {
            textBox.Value.Update();
            if (textBox.Value.Text != textCache.Value)
            {
                if (textCache.Value is not null)
                    validateCraftMethod?.Invoke(__instance, null);
                textCache.Value = textBox.Value.Text;
                textBox.Value.Width = (int)Math.Max(200, textBox.Value.Font.MeasureString(textBox.Value.Text).X + 24);
                textBox.Value.X = textBox.Value.X = Game1.uiViewport.Width / 2 - (800 + IClickableMenu.borderWidth * 2) / 2 + 204 - (textBox.Value.Width / 2) + 100;
            }
        }

        private static void ForgeMenu_GameWindowSizeChanged_Postfix() => updatePosition();

        private static void ForgeMenu_ReceiveLeftClick_Postfix(ForgeMenu __instance, int x, int y)
        {
            if (__instance.leftIngredientSpot is ClickableTextureComponent c && c.containsPoint(x, y))
            {
                if (c.item is not null && leftItem.Value is null)
                {
                    leftItem.Value = c.item;
                    textBox.Value.Text = c.item.DisplayName;
                }

                if (c.item is null && leftItem.Value is not null)
                {
                    leftItem.Value = null;
                    textBox.Value.Text = null;
                }
            }
            if (__instance.unforgeButton is ClickableComponent c2 && c2.containsPoint(x, y))
            {
                if (__instance.rightIngredientSpot.item is null && !__instance.IsValidUnforge())
                {
                    Item left = __instance.leftIngredientSpot.item;
                    if (__instance.HighlightItems(left) && 
                        ModEntry.IConfig.UnforgeClearsName && 
                        left.modData.TryGetValue(ModEntry.ModDataKey, out string value) && 
                        (string.IsNullOrWhiteSpace(textBox.Value.Text) || value == textBox.Value.Text))
                    {
                        unforgingField!.SetValue(__instance, true);
                        craftTimeField!.SetValue(__instance, 1600);
                    }
                }
            }
        }

        #endregion

        #region Transpilers

        private static IEnumerable<CodeInstruction> ForgeMenu_Draw_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var mi_de = AccessTools.Method(typeof(ForgeMenuExtensions), nameof(drawExtensions));
            var mi_ib = AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.IsBusy));
            CodeMatcher matcher = new(instructions);
            matcher.Start()
                   .MatchStartForward([new(OpCodes.Ldarg_0), new(OpCodes.Call, mi_ib)])
                   .Advance(1)
                   .Insert([new(OpCodes.Ldarg_1), new(OpCodes.Call, mi_de), new(OpCodes.Ldarg_0)]);

            return matcher.Instructions();
        }

        private static IEnumerable<CodeInstruction> ForgeMenu_Update_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var mi_tdu = AccessTools.Method(typeof(ForgeMenuExtensions), nameof(tryDoUnforge));
            CodeMatcher matcher = new(instructions);
            matcher.Start()
                   .MatchEndForward([new(OpCodes.Ldarg_0), new(OpCodes.Ldfld, unforgingField), new(OpCodes.Brfalse)])
                   .Insert([new(OpCodes.Ldarg_0), new(OpCodes.Call, mi_tdu)]);

            return matcher.Instructions();
        }

        private static IEnumerable<CodeInstruction> ForgeMenu_LeftIngredientSpotClicked_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var mi_ghi = AccessTools.PropertyGetter(typeof(MenuWithInventory), nameof(MenuWithInventory.heldItem));
            var mi_catlis = AccessTools.Method(typeof(ForgeMenuExtensions), nameof(canAddToLeftIngredientSpot));
            CodeMatcher matcher = new(instructions, generator);

            matcher.Start()
                   .MatchStartForward([new(OpCodes.Ldstr, "stoneStep")])
                   .CreateLabel(out Label l)
                   .Start()
                   .MatchStartForward([new(OpCodes.Ldarg_0), new(OpCodes.Call, mi_ghi), new(OpCodes.Brfalse_S)])
                   .Insert([new(OpCodes.Ldarg_0), new(OpCodes.Call, mi_catlis), new(OpCodes.Brtrue, l)]);

            return matcher.Instructions();
        }

        #endregion
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Slingshots.Patchers;

#region using directives

using System.Reflection;
using System.Text;
using DaLion.Overhaul.Modules.Enchantments.Gemstone;
using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolDrawTooltipPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolDrawTooltipPatcher"/> class.</summary>
    internal ToolDrawTooltipPatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.drawTooltip));
    }

    #region harmony patches

    /// <summary>Draw Slingshot enchantment effects in tooltip.</summary>
    [HarmonyPrefix]
    private static bool ToolDrawTooltipPrefix(
        Tool __instance,
        SpriteBatch spriteBatch,
        ref int x,
        ref int y,
        SpriteFont font,
        float alpha,
        StringBuilder? overrideText)
    {
        if (__instance is not Slingshot slingshot)
        {
            return true; // run original logic
        }

        try
        {
            // write description
            ItemDrawTooltipPatcher.ItemDrawTooltipReverse(
                __instance,
                spriteBatch,
                ref x,
                ref y,
                font,
                alpha,
                overrideText);

            Color co;

            // write bonus damage
            if (slingshot.InitialParentTileIndex != ItemIDs.BasicSlingshot ||
                slingshot.hasEnchantmentOfType<RubyEnchantment>())
            {
                var amount = $"+{slingshot.Get_RelativeDamageModifier():0%}";
                co = new Color(0, 120, 120);
                Utility.drawWithShadow(
                    spriteBatch,
                    Game1.mouseCursors,
                    new Vector2(x + 20, y + 20),
                    new Rectangle(120, 428, 10, 10),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    false,
                    1f);

                Utility.drawTextWithShadow(
                    spriteBatch,
                    I18n.Get("ui.itemhover.damage", new { amount }),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus knockback
            if (slingshot.InitialParentTileIndex != ItemIDs.BasicSlingshot ||
                __instance.hasEnchantmentOfType<AmethystEnchantment>())
            {
                var amount = $"+{slingshot.Get_RelativeKnockbackModifer():0%}";
                co = new Color(0, 120, 120);
                Utility.drawWithShadow(
                    spriteBatch,
                    Game1.mouseCursors,
                    new Vector2(x + 20, y + 20),
                    new Rectangle(70, 428, 10, 10),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    false,
                    1f);

                Utility.drawTextWithShadow(
                    spriteBatch,
                    Game1.content.LoadString("Strings\\UI:ItemHover_Weight", amount),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus crit rate
            if (__instance.hasEnchantmentOfType<AquamarineEnchantment>())
            {
                var amount = $"{slingshot.Get_RelativeCritChanceModifier():0.0%}";
                co = new Color(0, 120, 120);
                Utility.drawWithShadow(
                    spriteBatch,
                    Game1.mouseCursors,
                    new Vector2(x + 20, y + 20),
                    new Rectangle(40, 428, 10, 10),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    false,
                    1f);

                Utility.drawTextWithShadow(
                    spriteBatch,
                    Game1.content.LoadString("Strings\\UI:ItemHover_CritChanceBonus", amount),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write crit power
            if (__instance.hasEnchantmentOfType<JadeEnchantment>())
            {
                var amount = $"{slingshot.Get_RelativeCritPowerModifier():0%}";
                co = new Color(0, 120, 120);
                Utility.drawWithShadow(
                    spriteBatch,
                    Game1.mouseCursors,
                    new Vector2(x + 16, y + 16 + 4),
                    new Rectangle(160, 428, 10, 10),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    false,
                    1f);

                Utility.drawTextWithShadow(
                    spriteBatch,
                    Game1.content.LoadString("Strings\\UI:ItemHover_CritPowerBonus", amount),
                    font,
                    new Vector2(x + 16 + 44, y + 16 + 12),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus fire speed
            if (__instance.hasEnchantmentOfType<EmeraldEnchantment>())
            {
                var amount = $"+{slingshot.Get_RelativeFireSpeed():0%}";
                co = new Color(0, 120, 120);
                Utility.drawWithShadow(
                    spriteBatch,
                    Game1.mouseCursors,
                    new Vector2(x + 20, y + 20),
                    new Rectangle(130, 428, 10, 10),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    false,
                    1f);

                Utility.drawTextWithShadow(
                    spriteBatch,
                    I18n.Get("ui.itemhover.firespeed", new { amount }),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus cooldown reduction
            if (__instance.hasEnchantmentOfType<GarnetEnchantment>())
            {
                var amount = $"-{slingshot.Get_RelativeCooldownReduction():0%}";
                co = new Color(0, 120, 120);
                Utility.drawWithShadow(
                    spriteBatch,
                    Game1.mouseCursors,
                    new Vector2(x + 20, y + 20),
                    new Rectangle(150, 428, 10, 10),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    false,
                    1f);

                Utility.drawTextWithShadow(
                    spriteBatch,
                    I18n.Get("ui.itemhover.cdr", new { amount }),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus defense
            if (__instance.hasEnchantmentOfType<TopazEnchantment>() && EnchantmentsModule.Config.RebalancedForges)
            {
                var amount = CombatModule.IsEnabled && CombatModule.Config.OverhauledDefense
                    ? $"+{slingshot.Get_RelativeResilience():0%}"
                    : slingshot.GetEnchantmentLevel<TopazEnchantment>().ToString();
                co = new Color(0, 120, 120);
                Utility.drawWithShadow(
                    spriteBatch,
                    Game1.mouseCursors,
                    new Vector2(x + 20, y + 20),
                    new Rectangle(110, 428, 10, 10),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    false,
                    1f);

                Utility.drawTextWithShadow(
                    spriteBatch,
                    CombatModule.IsEnabled && CombatModule.Config.OverhauledDefense
                        ? I18n.Get("ui.itemhover.resist", new { amount })
                        : Game1.content.LoadString("ItemHover_DefenseBonus", amount),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);

                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write bonus random forge
            if (__instance.enchantments.Count > 0 && __instance.enchantments[^1] is DiamondEnchantment)
            {
                co = new Color(0, 120, 120);
                var randomForges = __instance.GetMaxForges() - __instance.GetTotalForgeLevels();
                var randomForgeString = randomForges != 1
                    ? Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Plural", randomForges)
                    : Game1.content.LoadString("Strings\\UI:ItemHover_DiamondForge_Singular", randomForges);
                Utility.drawTextWithShadow(
                    spriteBatch,
                    randomForgeString,
                    font,
                    new Vector2(x + 16, y + 28),
                    co * 0.9f * alpha);
                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            // write other enchantments
            co = new Color(120, 0, 210);
            for (var i = 0; i < __instance.enchantments.Count; i++)
            {
                var enchantment = __instance.enchantments[i];
                if (!enchantment.ShouldBeDisplayed())
                {
                    continue;
                }

                Utility.drawWithShadow(
                    spriteBatch,
                    Game1.mouseCursors2,
                    new Vector2(x + 20, y + 20),
                    new Rectangle(127, 35, 10, 10),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    4f,
                    false,
                    1f);
                Utility.drawTextWithShadow(
                    spriteBatch,
                    BaseEnchantment.hideEnchantmentName ? "???" : enchantment.GetDisplayName(),
                    font,
                    new Vector2(x + 68, y + 28),
                    co * 0.9f * alpha);
                y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
            }

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}

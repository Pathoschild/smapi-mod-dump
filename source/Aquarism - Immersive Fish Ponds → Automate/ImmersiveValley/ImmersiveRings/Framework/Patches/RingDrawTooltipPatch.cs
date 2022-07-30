/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

using DaLion.Stardew.Rings.Extensions;

namespace DaLion.Stardew.Rings.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class RingDrawTooltipPatch : Common.Harmony.HarmonyPatch
{
    private static Func<Item, int>? _GetDescriptionWidth;

    /// <summary>Construct an instance.</summary>
    internal RingDrawTooltipPatch()
    {
        Target = RequireMethod<Ring>(nameof(Ring.drawTooltip));
    }

    #region harmony patches

    /// <summary>Draw combined Iridium Band effects in tooltip.</summary>
    [HarmonyPrefix]
    private static bool RingDrawTooltipPrefix(Ring __instance, SpriteBatch spriteBatch, ref int x, ref int y,
        SpriteFont font, float alpha)
    {
        if (__instance is not CombinedRing { ParentSheetIndex: Constants.IRIDIUM_BAND_INDEX_I } iridiumBand ||
            iridiumBand.combinedRings.Count == 0) return true; // run original logic

        float addedDamage = 0f, addedCritChance = 0f, addedCritPower = 0f, addedSwingSpeed = 0f, addedKnockback = 0f, addedPrecision = 0f;
        var addedDefense = 0;
        foreach (var ring in iridiumBand.combinedRings)
            switch (ring.ParentSheetIndex)
            {
                case Constants.RUBY_RING_INDEX_I:
                    addedDamage += 0.1f;
                    break;
                case Constants.AQUAMARINE_RING_INDEX_I:
                    addedCritChance += 0.1f;
                    break;
                case Constants.JADE_RING_INDEX_I:
                    addedCritPower += ModEntry.Config.RebalancedRings ? 0.3f : 0.1f;
                    break;
                case Constants.EMERALD_RING_INDEX_I:
                    addedSwingSpeed += 0.1f;
                    break;
                case Constants.AMETHYST_RING_INDEX_I:
                    addedKnockback += 0.1f;
                    break;
                case Constants.TOPAZ_RING_INDEX_I:
                    if (ModEntry.Config.RebalancedRings) addedDefense += 3;
                    else addedPrecision += 0.1f;
                    break;
            }

        var hasGems = addedDamage + addedCritChance + addedCritPower + addedPrecision + addedSwingSpeed +
            addedKnockback + addedDefense > 0;
        if (!hasGems) return false; // don't run original logic

        if (iridiumBand.IsResonant(out var resonance))
            switch (resonance)
            {
                case Constants.RUBY_RING_INDEX_I:
                    addedDamage += 0.04f;
                    break;
                case Constants.AQUAMARINE_INDEX_I:
                    addedCritChance += 0.04f;
                    break;
                case Constants.JADE_RING_INDEX_I:
                    addedCritPower += 0.12f;
                    break;
                case Constants.EMERALD_RING_INDEX_I:
                    addedSwingSpeed += 0.04f;
                    break;
                case Constants.AMETHYST_RING_INDEX_I:
                    addedKnockback += 0.04f;
                    break;
                case Constants.TOPAZ_RING_INDEX_I:
                    if (ModEntry.Config.RebalancedRings) addedDefense += 1;
                    else addedPrecision += 0.04f;
                    break;
            }

        if (resonance is not null)
        {
            Utility.drawTextWithShadow(spriteBatch, resonance.DisplayName, font, new(x + 16, y + 16 + 4), Color.DarkRed,
                1f, -1f, 2, 2);
            y += (int)font.MeasureString("T").Y;
        }

        // write description
        _GetDescriptionWidth ??=
            typeof(Item).RequireMethod("getDescriptionWidth").CompileUnboundDelegate<Func<Item, int>>();

        var descriptionWidth = _GetDescriptionWidth(__instance);
        Utility.drawTextWithShadow(spriteBatch,
            Game1.parseText(__instance.description, Game1.smallFont, descriptionWidth), font, new(x + 16, y + 20),
            Game1.textColor);
        y += (int)font.MeasureString(Game1.parseText(__instance.description, Game1.smallFont, descriptionWidth)).Y;

        Color co;

        // write bonus damage
        if (addedDamage > 0)
        {
            var amount = $"{addedDamage:p0}";
            co = new(0, 120, 120);
            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(120, 428, 10, 10), Color.White,
                0f, Vector2.Zero, 4f, false, 1f);
            Utility.drawTextWithShadow(spriteBatch, ModEntry.i18n.Get("ui.itemhover.damage", new { amount }), font,
                new(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus crit rate
        if (addedCritChance > 0)
        {
            var amount = $"{addedCritChance:p0}";
            co = new(0, 120, 120);
            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(40, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);
            Utility.drawTextWithShadow(spriteBatch,
                Game1.content.LoadString("Strings\\UI:ItemHover_CritChanceBonus", amount), font, new(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write crit power
        if (addedCritPower > 0)
        {
            var amount = $"{addedCritPower:p0}";
            co = new(0, 120, 120);
            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 16, y + 16 + 4),
                new Rectangle(160, 428, 10, 10), Color.White, 0f, Vector2.Zero, 4f, false, 1f);
            Utility.drawTextWithShadow(spriteBatch,
                Game1.content.LoadString("Strings\\UI:ItemHover_CritPowerBonus", amount), font,
                new(x + 16 + 44, y + 16 + 12), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus precision
        if (addedPrecision > 0)
        {
            var amount = $"{addedPrecision:p0}";
            co = new(0, 120, 120);
            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(110, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);
            Utility.drawTextWithShadow(spriteBatch, ModEntry.i18n.Get("ui.itemhover.precision", new { addedPrecision = amount }), font,
                new(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus charge speed
        if (addedSwingSpeed > 0)
        {
            var amount = $"+{addedSwingSpeed:p0}";
            co = new(0, 120, 120);
            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(130, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);
            Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_Speed", amount),
                font, new(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus knockback
        if (addedKnockback > 0)
        {
            var amount = $"+{addedKnockback:p0}";
            co = new(0, 120, 120);
            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(70, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);
            Utility.drawTextWithShadow(spriteBatch, Game1.content.LoadString("Strings\\UI:ItemHover_Weight", amount),
                font, new(x + 68, y + 28), co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        // write bonus defense
        if (addedDefense > 0)
        {
            co = new(0, 120, 120);
            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new(x + 20, y + 20), new(110, 428, 10, 10),
                Color.White, 0f, Vector2.Zero, 4f, false, 1f);
            Utility.drawTextWithShadow(spriteBatch,
                Game1.content.LoadString("Strings\\UI:ItemHover_DefenseBonus", addedDefense.ToString()), font, new(x + 68, y + 28),
                co * 0.9f * alpha);
            y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
        }

        return false; // don't run original logic
    }

    /// <summary>Fix crab ring tooltip.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? RingDrawTooltipTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        var displayVanillaEffect = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_5)
                )
                .AddLabels(displayVanillaEffect)
                .Insert(
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.RebalancedRings))),
                    new CodeInstruction(OpCodes.Brfalse_S, displayVanillaEffect),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 10),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting custom crabshell tooltip.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
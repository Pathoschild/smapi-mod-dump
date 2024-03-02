/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BirbCore.Attributes;
using BirbCore.Extensions;
using BirbShared;
using HarmonyLib;
using Microsoft.Xna.Framework;
using SpaceCore;
using StardewValley;
using StardewValley.Characters;
using StardewValley.GameData.Characters;
using StardewValley.GameData.GarbageCans;
using StardewValley.Locations;

namespace BinningSkill;

/// <summary>
/// Reclaimer Profession
/// </summary>
[HarmonyPatch(typeof(Utility), nameof(Utility.getTrashReclamationPrice))]
class Utility_GetTrashReclmantionPrice
{
    public static void Postfix(
        Item i,
        Farmer f,
        ref int __result)
    {
        try
        {
            if (__result < 0)
            {
                return;
            }

            if (!f.HasProfession("Reclaimer"))
            {
                return;
            }

            float extraPercentage = ModEntry.Config.ReclaimerExtraValuePercent;
            if (f.HasProfession("Reclaimer"))
            {
                extraPercentage += ModEntry.Config.ReclaimerPrestigeExtraValuePercent;
            }
            int extraPrice = (int)(i.Stack * i.sellToStorePrice() * extraPercentage);

            __result += extraPrice;
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}

/// <summary>
/// Upseller Profession
/// </summary>
[HarmonyPatch(typeof(NPC), nameof(NPC.getGiftTasteForThisItem))]
class NPC_GetGiftTasteForThisItem
{
    public static void Postfix(
        ref int __result)
    {
        try
        {
            if (!Game1.player.HasProfession("Upseller"))
            {
                return;
            }

            if (__result == 4)
            {
                __result = 8;
                return;
            }

            if (__result == 6)
            {
                if (Game1.player.HasProfession("Upseller", true))
                {
                    __result = 8;
                }
                else
                {
                    __result = 4;
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}

/// <summary>
/// Custom animation texture if provided
/// Garbage can level requirements
/// Fix animation for indoor garbage cans
/// Prestige sneak profession
/// Custom noise level and sneak profession
/// TODO: No animation if search failed
/// </summary>
[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.CheckGarbage))]
class GameLocation_CheckGarbage
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            // Level requirements
            if (instruction.LoadsField(AccessTools.DeclaredField(typeof(Game1), nameof(Game1.netWorldState))))
            {
                Label meetsLevel = generator.DefineLabel();
                yield return new CodeInstruction(OpCodes.Ldarg_1).WithLabels(instruction.ExtractLabels()).WithBlocks(instruction.ExtractBlocks());
                yield return new CodeInstruction(OpCodes.Ldarg_3);
                yield return CodeInstruction.Call(typeof(GameLocation_CheckGarbage), nameof(GetMeetsLevelRequirement));
                yield return new CodeInstruction(OpCodes.Brtrue, meetsLevel);
                yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                yield return new CodeInstruction(OpCodes.Ret);
                yield return instruction.WithLabels(meetsLevel);
            }

            // Fix animations for indoor garbage cans
            else if (instruction.Calls(AccessTools.DeclaredMethod(typeof(GameLocation), nameof(GameLocation.GetSeasonIndex))))
            {
                yield return CodeInstruction.Call(typeof(GameLocation_CheckGarbage), nameof(GetSeasonIndexReplacement)).WithLabels(instruction.ExtractLabels()).WithBlocks(instruction.ExtractBlocks());
            }

            // Use custom animations
            else if (instruction.LoadsConstant("LooseSprites\\Cursors2"))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_1).WithLabels(instruction.ExtractLabels()).WithBlocks(instruction.ExtractBlocks());
                yield return CodeInstruction.Call(typeof(GameLocation_CheckGarbage), nameof(GetAnimationReplacement));
            }

            // Do prestiged sneak profession
            else if (instruction.IsLdarg(5))
            {
                Label defaultReactNpcs = generator.DefineLabel();
                yield return new CodeInstruction(OpCodes.Ldarg_1).WithLabels(instruction.ExtractLabels()).WithBlocks(instruction.ExtractBlocks());
                yield return new CodeInstruction(OpCodes.Ldarg_2);
                yield return new CodeInstruction(OpCodes.Ldarg_3);
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return CodeInstruction.Call(typeof(GameLocation_CheckGarbage), nameof(TryPrestigeSneak));
                yield return new CodeInstruction(OpCodes.Brfalse, defaultReactNpcs);
                yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                yield return new CodeInstruction(OpCodes.Starg_S, 5);
                yield return instruction.WithLabels(defaultReactNpcs);
            }

            // Get custom noise level and sneak profession
            else if (instruction.Calls(AccessTools.DeclaredMethod(typeof(Utility), nameof(Utility.GetNpcsWithinDistance))))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_1).WithLabels(instruction.ExtractLabels()).WithBlocks(instruction.ExtractBlocks());
                yield return new CodeInstruction(OpCodes.Ldarg_3);
                yield return CodeInstruction.Call(typeof(GameLocation_CheckGarbage), nameof(GetNpcsWithinDistanceReplacement));
            }
            else
            {
                yield return instruction;
            }
        }
    }

    public static bool GetMeetsLevelRequirement(string garbageCanId, Farmer who)
    {
        GarbageCanData allData = DataLoader.GarbageCans(Game1.content);
        allData.GarbageCans.TryGetValue(garbageCanId, out GarbageCanEntryData data);
        int minLevel = data?.CustomFields?.TryGetInt("drbirbdev.BinningSkill_MinLevel") ?? 0;
        if (who.GetCustomSkillLevel("drbirbdev.Binning") < minLevel)
        {
            if (ModEntry.UnderleveledCheckedGarbage.Value.Contains(garbageCanId))
            {
                return false;
            }
            ModEntry.UnderleveledCheckedGarbage.Value.Add(garbageCanId);
            Game1.showGlobalMessage(ModEntry.Instance.I18n.Get("skill.required_level", new { level = minLevel }));
            return false;
        }
        return true;
    }

    public static int GetSeasonIndexReplacement(GameLocation gameLocation)
    {
        if (gameLocation.IsOutdoors || gameLocation is Summit)
        {
            return gameLocation.GetSeasonIndex();
        }
        return (int)Season.Spring;
    }

    public static string GetAnimationReplacement(string garbageCanId)
    {
        GarbageCanData allData = DataLoader.GarbageCans(Game1.content);
        allData.GarbageCans.TryGetValue(garbageCanId, out GarbageCanEntryData data);
        string textureName = data?.CustomFields?.GetValueOrDefault("drbirbdev.BinningSkill_AnimationTexture", null);
        return textureName ?? "LooseSprites/Cursors2";
    }

    public static bool TryPrestigeSneak(string garbageCanId, Vector2 tile, Farmer who, GameLocation location)
    {
        if (!who.HasProfession("Sneak", true))
        {
            return false;
        }

        GarbageCanData allData = DataLoader.GarbageCans(Game1.content);
        allData.GarbageCans.TryGetValue(garbageCanId, out GarbageCanEntryData data);
        int noiseLevel = data?.CustomFields?.TryGetInt("drbirbdev.BinningSkill_NoiseLevel") ?? 7;
        noiseLevel += ModEntry.Config.PrestigeNoiseIncrease;

        foreach (NPC villager in Utility.GetNpcsWithinDistance(tile, noiseLevel, location))
        {
            if (villager is Horse)
            {
                continue;
            }

            CharacterData charData = villager.GetData();
            int amount = charData?.CustomFields?.TryGetInt("drbirbdev.BinningSkill_DumpsterDivePrestigeSneakFriendshipEffect") ?? 25;
            int? emote = charData?.CustomFields?.TryGetInt("drbirbdev.BinningSkill_DumpsterDivePrestigeSneakEmote");
            Dialogue dialogue = villager.TryGetDialogue("drbirbdev.BinningSkill_DumpsterDivePrestigeSneakComment");
            switch (villager.Age)
            {
                case 2:
                    emote ??= 56;
                    dialogue ??= new Dialogue(villager, ModEntry.Instance.I18n.Get("sneak.prestige.dialogue.child"));
                    break;
                case 1:
                    emote ??= 32;
                    dialogue ??= new Dialogue(villager, ModEntry.Instance.I18n.Get("sneak.prestige.dialogue.teen"));
                    break;
                default:
                    emote ??= 20;
                    dialogue ??= new Dialogue(villager, ModEntry.Instance.I18n.Get("sneak.prestige.dialogue.adult"));
                    break;
            }

            villager.doEmote(emote.Value);
            who.changeFriendship(amount, villager);
            villager.setNewDialogue(dialogue, true, true);
            Game1.drawDialogue(villager);
            break;
        }
        return true;
    }

    public static IEnumerable<NPC> GetNpcsWithinDistanceReplacement(Vector2 centerTile, int tilesAway, GameLocation location, string garbageCanId, Farmer who)
    {
        GarbageCanData allData = DataLoader.GarbageCans(Game1.content);
        allData.GarbageCans.TryGetValue(garbageCanId, out GarbageCanEntryData data);
        int noiseLevel = data?.CustomFields?.TryGetInt("drbirbdev.BinningSkill_NoiseLevel") ?? 7;
        if (who.HasProfession("Sneak"))
        {
            noiseLevel -= ModEntry.Config.NoiseReduction;
        }
        return Utility.GetNpcsWithinDistance(centerTile, noiseLevel, location);
    }
}

/// <summary>
/// Binning Level Bonuses
/// Mega, DoubleMega Level Requirements
/// </summary>
[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.TryGetGarbageItem))]
class GameLocation_TryGetGarbageIItem
{
    public static void Prefix(ref double dailyLuck)
    {
        try
        {
            // Alter dailyLuck, adding more luck depending on level
            dailyLuck += ModEntry.Config.PerLevelBaseDropChanceBonus * Game1.player.GetCustomSkillLevel("drbirbdev.Binning");

        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }

    public static void Postfix(ref Item item, ref GarbageCanItemData selected)
    {
        try
        {
            if (selected == null)
            {
                return;
            }
            // Remove Mega, DoubleMega results if not meeting level requirements
            if ((selected.IsMegaSuccess && Game1.player.GetCustomSkillLevel("drbirbdev.Binning") < ModEntry.Config.MegaMinLevel) ||
                (selected.IsDoubleMegaSuccess && Game1.player.GetCustomSkillLevel("drbirbdev.Binning") < ModEntry.Config.DoubleMegaMinLevel))
            {
                item = null;
                selected = null;
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}

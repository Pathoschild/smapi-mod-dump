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
using BirbShared;
using HarmonyLib;
using SpaceCore;
using StardewValley;
using StardewValley.Characters;
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
/// No Default NPC Reactions
/// Custom animation texture if provided
/// No animation if search failed
/// Fix animation for indoor garbage cans
/// </summary>
[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.CheckGarbage))]
class GameLocation_CheckGarbage
{
    public static void Prefix(ref bool reactNpcs)
    {
        reactNpcs = false;
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.LoadsConstant("LooseSprites\\Cursors2"))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_1).WithLabels(instruction.labels).WithBlocks(instruction.blocks);
                yield return CodeInstruction.Call(typeof(GameLocation_CheckGarbage), nameof(GetAnimationReplacement));
            }
            else if (instruction.Calls(AccessTools.DeclaredMethod(typeof(GameLocation), nameof(GameLocation.TryGetGarbageItem))))
            {
                yield return instruction;

                Label doTrashAnimation = new Label();
                yield return new CodeInstruction(OpCodes.Brtrue, doTrashAnimation);
                yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                yield return new CodeInstruction(OpCodes.Ret);
                yield return new CodeInstruction(OpCodes.Ldc_I4_1).WithLabels(doTrashAnimation);
            }
            else if (instruction.Calls(AccessTools.DeclaredMethod(typeof(GameLocation), nameof(GameLocation.GetSeasonIndex))))
            {
                yield return CodeInstruction.Call(typeof(GameLocation_CheckGarbage), nameof(GetAdjustedSeasonIndex));
            }
            else
            {
                yield return instruction;
            }
        }
    }

    public static string GetAnimationReplacement(string garbageCanId)
    {
        GarbageCanData allData = Game1.content.Load<GarbageCanData>("Data/GarbageCans");
        allData.GarbageCans.TryGetValue(garbageCanId, out GarbageCanEntryData data);
        string textureName = data?.CustomFields?.GetValueOrDefault("drbirbdev.BinningSkill_AnimationTexture", null);
        return textureName ?? "LooseSprites/Cursors2";
    }

    private static int GetAdjustedSeasonIndex(GameLocation gameLocation)
    {
        if (gameLocation.IsOutdoors || gameLocation is Summit)
        {
            return gameLocation.GetSeasonIndex();
        }
        return (int)Season.Spring;
    }
}

/// <summary>
/// Garbage Can Level Requirements
/// Sneak Profession
/// Custom Noise Level
/// Binning Level Bonuses
/// Mega, DoubleMega Level Requirements
/// Change return meaning
///  true = not caught, high enough level
///  false = caught, level too low
/// </summary>
[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.TryGetGarbageItem))]
class GameLocation_TryGetGarbageIItem
{
    public static bool Prefix(string id, ref double dailyLuck, ref Random garbageRandom, GameLocation __instance, ref bool __state)
    {
        try
        {
            __state = true;

            // Check garbage can level
            GarbageCanData allData = Game1.content.Load<GarbageCanData>("Data/GarbageCans");
            allData.GarbageCans.TryGetValue(id, out GarbageCanEntryData data);
            _ = int.TryParse(data?.CustomFields?.GetValueOrDefault("drbirbdev.BinningSkill_MinLevel", null), out int minLevel);
            if (Game1.player.GetCustomSkillLevel("drbirbdev.Binning") < minLevel)
            {
                Game1.showGlobalMessage(ModEntry.Instance.I18n.Get("skill.required_level", new { level = minLevel }));
                garbageRandom = Utility.CreateDaySaveRandom(777 + Utility.GetDeterministicHashCode(id));
                __state = false;
                return false;
            }

            // Sneak Profession, custom sound range
            int noiseLevel = int.TryParse(data?.CustomFields?.GetValueOrDefault("drbirbdev.BinningSkill_NoiseLevel"), out noiseLevel) ? noiseLevel : 7;
            if (Game1.player.HasProfession("Sneak", true))
            {
                noiseLevel += ModEntry.Config.PrestigeNoiseIncrease;
            }
            else if (Game1.player.HasProfession("Sneak"))
            {
                noiseLevel -= ModEntry.Config.NoiseReduction;
            }
            if (noiseLevel > 0 && Utility.isThereAFarmerOrCharacterWithinDistance(Game1.player.Tile, noiseLevel, __instance) is NPC npc && npc is not Horse)
            {
                if (DoCaughtReactions(npc))
                {
                    garbageRandom = Utility.CreateDaySaveRandom(777 + Utility.GetDeterministicHashCode(id));
                    __state = false;
                    __instance.playSound("trashcan");
                    return false;
                }
            }

            // Alter dailyLuck, adding more luck depending on level
            dailyLuck += ModEntry.Config.PerLevelBaseDropChanceBonus * Game1.player.GetCustomSkillLevel("drbirbdev.Binning");

        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
        return true;
    }

    public static void Postfix(ref Item item, ref GarbageCanItemData selected, ref bool __result, bool __state)
    {
        try
        {
            __result = __state;
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

    // TODO: Reverse Patch for vanilla behavior
    public static bool DoCaughtReactions(NPC npc)
    {
        bool isNegativeReaction = true;
        if (npc.Name.Equals("Linus"))
        {
            npc.doEmote(32);
            npc.setNewDialogue("Data\\ExtraDialogue:Town_DumpsterDiveComment_Linus", add: true, clearOnMovement: true);
            Game1.player.changeFriendship(5, npc);
            Game1.Multiplayer.globalChatInfoMessage("LinusTrashCan");
            isNegativeReaction = false;
        }
        else if (Game1.player.HasProfession("Sneak", true))
        {
            npc.doEmote(32);
            Game1.player.changeFriendship(25, npc);
            switch (npc.Age)
            {
                case 1:
                    npc.setNewDialogue(ModEntry.Instance.I18n.Get("sneak.prestige.dialogue.teen"), true, true);
                    break;
                case 2:
                    npc.setNewDialogue(ModEntry.Instance.I18n.Get("sneak.prestige.dialogue.child"), true, true);
                    break;
                default:
                    npc.setNewDialogue(ModEntry.Instance.I18n.Get("sneak.prestige.dialogue.adult"), true, true);
                    break;
            }
            isNegativeReaction = false;
        }
        else
        {
            switch (npc.Age)
            {
                case 2:
                    npc.doEmote(28);
                    npc.setNewDialogue("Data\\ExtraDialogue:Town_DumpsterDiveComment_Child", add: true, clearOnMovement: true);
                    break;
                case 1:
                    npc.doEmote(8);
                    npc.setNewDialogue("Data\\ExtraDialogue:Town_DumpsterDiveComment_Teen", add: true, clearOnMovement: true);
                    break;
                default:
                    npc.doEmote(12);
                    npc.setNewDialogue("Data\\ExtraDialogue:Town_DumpsterDiveComment_Adult", add: true, clearOnMovement: true);
                    break;
            }
            Game1.player.changeFriendship(-25, npc);
        }
        Game1.drawDialogue(npc);

        return isNegativeReaction;
    }
}

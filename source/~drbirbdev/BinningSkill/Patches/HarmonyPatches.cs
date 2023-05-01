/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using HarmonyLib;
using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Locations;
using Netcode;
using SpaceCore;
using Microsoft.Xna.Framework;
using StardewValley.Tools;
using BirbShared;
using StardewValley.Objects;
using System.Reflection;
using xTile.Dimensions;
using System.Reflection.Emit;

namespace BinningSkill
{
    /// <summary>
    /// Binning Skill
    ///  - give binning exp
    ///  - give binning skill bonus drops
    /// </summary>
    [HarmonyPatch(typeof(Town), nameof(Town.checkAction))]
    class Town_CheckAction
    {
        [HarmonyBefore(new string[] { "AairTheGreat.BetterGarbageCans" })]
        public static void Prefix(
            Town __instance,
            ref int[] __state,
            NetArray<bool, NetBool> ___garbageChecked,
            xTile.Dimensions.Location tileLocation,
            Farmer who)
        {

            try
            {
                if (who.mount == null && __instance.map.GetLayer("Buildings").Tiles[tileLocation] != null &&
                    __instance.map.GetLayer("Buildings").Tiles[tileLocation].TileIndex == 78)
                {
                    string s = __instance.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings");
                    int whichCan = ((s != null) ? Convert.ToInt32(s.Split(' ')[1]) : (-1));
                    if (whichCan < 0 || whichCan >= ___garbageChecked.Length)
                    {
                        return;
                    }
                    if (!___garbageChecked[whichCan])
                    {
                        // Remember which can was interacted with, since the game code adjusts garbageChecked array
                        __state = new int[]
                        {
                        whichCan,
                        __instance.debris.Count,
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }

        public static void Postfix(
            ref int[] __state,
            Location tileLocation)
        {
            try
            {
                if (__state != null && __state.Length > 0)
                {
                    Utilities.DoTrashCanCheck("Town", __state[0].ToString(), __state[1], Utilities.GetItemPosition(new Vector2(tileLocation.X, tileLocation.Y)));
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

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
                if (f.HasCustomProfession(BinningSkill.Reclaimer))
                {
                    if (__result < 0)
                    {
                        return;
                    }
                    float extraPercentage = (ModEntry.Config.ReclaimerExtraValuePercent / 100.0f);
                    if (f.HasCustomPrestigeProfession(BinningSkill.Reclaimer))
                    {
                        extraPercentage *= 1.5f;
                    }
                    int extraAmount = 0;
                    if (i.canBeTrashed())
                    {
                        if (i is StardewValley.Object && !(i as StardewValley.Object).bigCraftable)
                        {
                            extraAmount = (int)((float)i.Stack * ((float)(i as StardewValley.Object).sellToStorePrice(-1L) * extraPercentage));
                        }
                        if (i is MeleeWeapon || i is Ring || i is Boots)
                        {
                            extraAmount = (int)((float)i.Stack * ((float)(i.salePrice() / 2) * extraPercentage));
                        }
                    }
                    __result += extraAmount;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch(typeof(NPC), nameof(NPC.getGiftTasteForThisItem))]
    class NPC_GetGiftTasteForThisItem
    {
        public static void Postfix(
            ref int __result)
        {
            try
            {
                if (Game1.player.HasCustomPrestigeProfession(BinningSkill.Upseller))
                {
                    if (__result == 6 || __result == 4)
                    {
                        __result = 8;
                    }
                }
                else if (Game1.player.HasCustomProfession(BinningSkill.Upseller))
                {
                    if (__result == 6)
                    {
                        __result = 4;
                    }
                    else if (__result == 4)
                    {
                        __result = 8;
                    }
                }
            } catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch(typeof(StardewValley.Object), nameof(StardewValley.Object.performObjectDropInAction))]
    class Object_PerformObjectDropInAction
    {
        public static void Prefix(
            StardewValley.Object __instance,
            ref StardewValley.Object __state,
            Item dropInItem,
            bool probe,
            Farmer who)
        {
            try
            {
                // Check heldObject in the prefix.  Need to see if this was null to know if trash has been recycled.
                __state = __instance.heldObject.Value;
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }

        public static void Postfix(
            StardewValley.Object __instance,
            StardewValley.Object __state,
            Item dropInItem,
            bool probe,
            Farmer who)
        {
            try
            {
                if (probe)
                {
                    return;
                }
                if (!__instance.Name.Equals("Recycling Machine"))
                {
                    return;
                }
                if (__instance.isTemporarilyInvisible)
                {
                    return;
                }
                if (dropInItem is not StardewValley.Object)
                {
                    return;
                }
                if (dropInItem is Wallpaper)
                {
                    return;
                }
                StardewValley.Object dropIn = dropInItem as StardewValley.Object;
                if (dropIn.ParentSheetIndex == 872)
                {
                    return;
                }

                if (dropIn.ParentSheetIndex >= 168 && dropIn.ParentSheetIndex <= 172 && __state == null)
                {
                    __instance.heldObject.Value = Utilities.GetSalvagerUpgrade(__instance.heldObject.Value);

                    if (who.HasCustomProfession(BinningSkill.Environmentalist))
                    {
                        if (who.stats.PiecesOfTrashRecycled % ModEntry.Config.RecyclingCountToGainFriendship == 0)
                        {
                            int friendshipGain = ModEntry.Config.RecyclingFriendshipGain;
                            if (who.HasCustomPrestigeProfession(BinningSkill.Environmentalist))
                            {
                                friendshipGain *= 2;
                            }
                            Utility.improveFriendshipWithEveryoneInRegion(who, friendshipGain, 2);
                        }
                    }

                    SpaceCore.Skills.AddExperience(who, "drbirbdev.Binning", ModEntry.Config.ExperienceFromRecycling);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch(typeof(CraftingRecipe), MethodType.Constructor, new Type[] {typeof(string), typeof(bool)})]
    class CraftingRecipe_Constructor
    {
        public static void Postfix(
            CraftingRecipe __instance,
            string name,
            bool isCookingRecipe)
        {
            try
            {
                if (name.Equals("Recycling Machine") && Game1.player.HasCustomProfession(BinningSkill.Recycler))
                {
                    if (Game1.player.HasCustomPrestigeProfession(BinningSkill.Recycler))
                    {
                        __instance.recipeList = new()
                        {
                            { 334, 1 }
                        };
                    }
                    else
                    {
                        __instance.recipeList = new()
                        {
                            { 388, 15 },
                            { 390, 15 },
                            { 334, 1 }
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    // 3rd Party
    [HarmonyPatch("RidgesideVillage.TrashCans", "Trigger")]
    class RidgesideVillage_TrashCans_Trigger
    {
        public static bool Prepare()
        {
            return ModEntry.Instance.Helper.ModRegistry.IsLoaded("Rafseazz.RidgesideVillage");
        }

        public static void Prefix(
            ref object[] __state,
            string tileAction,
            Vector2 position,
            HashSet<Vector2> ___TrashCansTriggeredToday)
        {
            try
            {
                if (___TrashCansTriggeredToday.Contains(position))
                {
                    return;
                }

                __state = new object[]
                {
                    tileAction.Split(' ')[1],
                    Game1.currentLocation.debris.Count,
                };
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }

        public static void Postfix(
            object[] __state,
            Vector2 position)
        {
            try
            {
                if (__state != null && __state.Length > 0)
                {
                    Character c = Sneak_Transpiler.DoSneak(position, 7, Game1.currentLocation);
                    if (c != null && c is NPC && c is not StardewValley.Characters.Horse)
                    {
                        // RSV doesn't have people catch you in trashcans, so just do some vanilla behaviour to add that.

                        // TODO: broadcast chat messages
                        // Game1.multiplayer.globalChatInfoMessage("TrashCan", Game1.player.Name, c.name);
                        int friendshipLoss = -ModEntry.Config.FriendshipRecovery;
                        if (c.Name.Equals("Linus"))
                        {
                            c.doEmote(32);
                            (c as NPC).setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Linus"), add: true, clearOnMovement: true);
                            Game1.player.changeFriendship(5, c as NPC);
                            // Game1.multiplayer.globalChatInfoMessage("LinusTrashCan");
                        }
                        else if ((c as NPC).Age == 2)
                        {
                            c.doEmote(28);
                            (c as NPC).setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Child"), add: true, clearOnMovement: true);
                            Game1.player.changeFriendship(friendshipLoss, c as NPC);
                        }
                        else if ((c as NPC).Age == 1)
                        {
                            c.doEmote(8);
                            (c as NPC).setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Teen"), add: true, clearOnMovement: true);
                            Game1.player.changeFriendship(friendshipLoss, c as NPC);
                        }
                        else
                        {
                            c.doEmote(12);
                            (c as NPC).setNewDialogue(Game1.content.LoadString("Data\\ExtraDialogue:Town_DumpsterDiveComment_Adult"), add: true, clearOnMovement: true);
                            Game1.player.changeFriendship(friendshipLoss, c as NPC);
                        }
                        Game1.drawDialogue(c as NPC);
                    }

                    Utilities.DoTrashCanCheck("RSV", (string)__state[0], (int)__state[1], Utilities.GetItemPosition(position));
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch("Pathoschild.Stardew.Automate.Framework.Machines.Tiles.TrashCanMachine", "GetRandomTrash")]
    class Automate_TrashCanMachine_GetRandomTrash
    {
        public static bool Prepare()
        {
            return ModEntry.Instance.Helper.ModRegistry.IsLoaded("Pathoschild.Automate");
        }

        internal static void Postfix(
            int index,
            ref Item __result)
        {
            try
            {
                if (__result == null)
                {
                    __result = Utilities.GetBonusItem("Town", index.ToString(), Game1.player, false);
                }
                else if (ModEntry.Config.AutomateGrantsXp)
                {
                    Skills.AddExperience(Game1.player, "drbirbdev.Binning", ModEntry.Config.ExperienceFromTrashSuccess);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch("Pathoschild.Stardew.Automate.Framework.Machines.Objects.RecyclingMachine", "SetInput")]
    class Automate_RecyclingMachine_SetInput
    {

        static PropertyInfo Machine;
        public static bool Prepare()
        {
            if (!ModEntry.Instance.Helper.ModRegistry.IsLoaded("Pathoschild.Automate"))
            {
                return false;
            }
            Machine = AccessTools.Property("Pathoschild.Stardew.Automate.Framework.Machines.Objects.RecyclingMachine:Machine");

            return true;
        }

        internal static void Postfix(
            object input,
            bool __result,
            object __instance)
        {
            try
            {
                if (__result)
                {
                    if (ModEntry.Config.AutomateGrantsXp)
                    {
                        Skills.AddExperience(Game1.player, "drbirbdev.Binning", ModEntry.Config.ExperienceFromRecycling);
                    }

                    if (Game1.player.HasCustomProfession(BinningSkill.Environmentalist))
                    {
                        if (Game1.player.stats.PiecesOfTrashRecycled % ModEntry.Config.RecyclingCountToGainFriendship == 0)
                        {
                            int friendshipGain = ModEntry.Config.RecyclingFriendshipGain;
                            if (Game1.player.HasCustomPrestigeProfession(BinningSkill.Environmentalist))
                            {
                                friendshipGain *= 2;
                            }
                            Utility.improveFriendshipWithEveryoneInRegion(Game1.player, friendshipGain, 2);
                        }
                    }

                    if (Game1.player.HasCustomProfession(BinningSkill.Salvager))
                    {
                        StardewValley.Object machine = (StardewValley.Object)Machine.GetValue(__instance);

                        machine.heldObject.Value = Utilities.GetSalvagerUpgrade(machine.heldObject.Value);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch("BetterGarbageCans.GamePatch.GarbageCanOverrider", "CheckForTreasure")]
    class BetterGarbageCans_GarbageCanOverrider_CheckForTreasure
    {
        public static bool Prepare()
        {
            return ModEntry.Instance.Helper.ModRegistry.IsLoaded("AairTheGreat.BetterGarbageCans");
        }

        public static void Prefix(
            ref object[] __state,
            GameLocation location,
            int index
            )
        {
            try
            {
                __state = new object[]
                {
                    index,
                    location.debris.Count,
                };
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }

        public static void Postfix(
            object[] __state,
            Location tileLocation
            )
        {
            try
            {
                if (__state != null && __state.Length > 0)
                {
                    Utilities.DoTrashCanCheck("Town", __state[0].ToString(), (int)__state[1], Utilities.GetItemPosition(tileLocation));
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch("StardewMods.GarbageDay.GarbageCan", "AddLoot")]
    class GarbageDay_GarbageCan_AddLoot
    {


        public static bool Prepare()
        {
            return ModEntry.Instance.Helper.ModRegistry.IsLoaded("furyx639.GarbageDay");
        }

        public static void Prefix(
            ref object[] __state,
            Chest ____chest)
        {
            try
            {
                int whichCan = BirbShared.Utilities.GetIntData(____chest, "furyx639.GarbageDay/WhichCan");
                __state = new object[]
                {
                    whichCan,
                    ____chest.items.Count,
                };
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }

        public static void Postfix(
            ref object[] __state,
            Chest ____chest)
        {
            try
            {
                if (__state != null && __state.Length > 0)
                {
                    int exp = 0;
                    if (____chest.items.Count > (int)__state[1])
                    {
                        exp = ModEntry.Config.ExperienceFromTrashSuccess;
                    } else
                    {
                        int rarity = BirbShared.Utilities.GetRarity(Utilities.GetBinningRarityLevels());
                        if (rarity < 0)
                        {
                            return;
                        }
                        string dropString = BirbShared.Utilities.GetRandomDropStringFromLootTable(ModEntry.Assets.TrashTable, "Town", __state[0].ToString(), rarity.ToString());
                        Item drop = BirbShared.Utilities.ParseDropString(dropString, ModEntry.JsonAssets, ModEntry.DynamicGameAssets);

                        if (rarity < 0)
                        {
                            exp = ModEntry.Config.ExperienceFromTrashFail;
                        }
                        else
                        {
                            exp = ModEntry.Config.ExperienceFromTrashBonus * (int)(Math.Pow(2, rarity));
                        }
                    }
                    int currExp = BirbShared.Utilities.GetIntData(____chest, "drbirbdev.BinningSkill/GarbageDayExp");
                    ____chest.modData.Remove("drbirbdev.BinningSkill/GarbageDayExp");
                    exp += currExp;
                    ____chest.modData.Add("drbirbdev.BinningSkill/GarbageDayExp", exp.ToString());
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch("StardewMods.GarbageDay.GarbageCan", "CheckAction")]
    class GarbageDay_GarbageCan_CheckAction
    {
        public static bool Prepare()
        {
            return ModEntry.Instance.Helper.ModRegistry.IsLoaded("furyx639.GarbageDay");
        }

        public static void Postfix(
            Chest ____chest)
        {
            try
            {
                int exp = BirbShared.Utilities.GetIntData(____chest, "drbirbdev.BinningSkill/GarbageDayExp");
                if (exp > 0)
                {
                    Skills.AddExperience(Game1.player, "drbirbdev.Binning", exp);
                    ____chest.modData.Remove("drbirbdev.BinningSkill/GarbageDayExp");
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch("StardewMods.GarbageDay.GarbageCan", "EmptyTrash")]
    class GarbageDay_GarbageCan_EmptyTrash
    {
        public static bool Prepare()
        {
            return ModEntry.Instance.Helper.ModRegistry.IsLoaded("furyx639.GarbageDay");
        }

        public static void Postfix(
            Chest ____chest)
        {
            try
            {
                ____chest.modData.Remove("drbirbdev.BinningSkill/GarbageDayExp");
            }
            catch (Exception e)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
            }
        }
    }

    [HarmonyPatch("DeluxeGrabberRedux.Grabbers.TownGarbageCanGrabber", "GrabItems")]
    class DeluxeGrabberRedux_TownGarbageCanGrabber_GrabItems
    {
        public static bool Prepare()
        {
            return ModEntry.Instance.Helper.ModRegistry.IsLoaded("ferdaber.DeluxeGrabberRedux");
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instr in instructions)
            {
                if (instr.Is(OpCodes.Call, AccessTools.Method("DeluxeGrabberRedux.MapGrabber:TryAddItem", new Type[] { typeof(Item) })))
                {
                    // Load whichCan to stack
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 5);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DeluxeGrabberRedux_TownGarbageCanGrabber_GrabItems), nameof(DoBinningSkill)));
                }
                yield return instr;
            }
        }

        public static Item DoBinningSkill(Item obj, int whichCan)
        {
            if (obj is not null)
            {
                // Do exp gain and return existing object.
                Skills.AddExperience(Game1.player, "drbirbdev.Binning", ModEntry.Config.ExperienceFromTrashSuccess);
                return obj;
            }

            return Utilities.GetBonusItem("Town", whichCan.ToString(), Game1.player, false);
        }
    }

    // Multi-patch
    [HarmonyPatch]
    class Sneak_Transpiler
    {

        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(Town), nameof(Town.checkAction));

            if (ModEntry.Instance.Helper.ModRegistry.IsLoaded("AairTheGreat.BetterGarbageCans"))
            {
                yield return AccessTools.Method("BetterGarbageCans.GamePatch.GarbageCanOverrider:CheckForNPCMessages");
            }
            if (ModEntry.Instance.Helper.ModRegistry.IsLoaded("furyx639.GarbageDay"))
            {
                if (AccessTools.Method("StardewMods.GarbageDay.GarbageDay:OnButtonPressed") is null)
                {
                    yield return AccessTools.Method("StardewMods.GarbageDay.ModEntry:OnButtonPressed");
                }
                else
                {
                    yield return AccessTools.Method("StardewMods.GarbageDay.GarbageDay:OnButtonPressed");
                }
            }
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instr in instructions)
            {
                if (instr.Is(OpCodes.Call, AccessTools.Method(typeof(Utility), nameof(Utility.isThereAFarmerOrCharacterWithinDistance))))
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Sneak_Transpiler), nameof(DoSneak)));
                }
                else
                {
                    yield return instr;
                }
            }
        }

        public static Character DoSneak(Vector2 tileLocation, int tilesAway, GameLocation location)
        {
            Character c = Utility.isThereAFarmerOrCharacterWithinDistance(tileLocation, tilesAway, location);
            if (c != null && c is NPC && c is not StardewValley.Characters.Horse && c.Name.Equals("Linus"))
            {
                return c;
            }
            if (!Game1.player.HasCustomProfession(BinningSkill.Sneak))
            {
                return c;
            }
            if (Game1.player.HasCustomPrestigeProfession(BinningSkill.Sneak))
            {
                if (c != null && c is NPC npc && !(c is StardewValley.Characters.Horse))
                {
                    // TODO: this plays weird with Garbage Day.
                    c.doEmote(32);
                    Game1.player.changeFriendship(50, npc);
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
                    Game1.drawDialogue(npc);
                }
            }
            return null;
        }
    }
}

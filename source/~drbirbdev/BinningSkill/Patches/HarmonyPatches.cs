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

namespace BinningSkill
{
    /// <summary>
    /// Binning Skill
    ///  - give binning exp
    ///  - give binning skill bonus drops
    /// Sneak Profession
    ///  - negate negative reactions to being caught
    /// </summary>
    [HarmonyPatch(typeof(Town), nameof(Town.checkAction))]
    class Town_CheckAction
    {
        public static void Prefix(
            Town __instance,
            ref int[] __state,
            NetArray<bool, NetBool> ___garbageChecked,
            xTile.Dimensions.Location tileLocation,
            xTile.Dimensions.Rectangle viewport,
            Farmer who)
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

        public static void Postfix(
            bool __result,
            Town __instance,
            ref int[] __state,
            NetArray<bool, NetBool> ___garbageChecked,
            xTile.Dimensions.Location tileLocation,
            xTile.Dimensions.Rectangle viewport,
            Farmer who)
        {
            if (__state != null && __state.Length > 0)
            {
                if (who.HasCustomProfession(BinningSkill.Sneak))
                {
                    // TODO: skip this stuff by transpiling.  Will fix global chat message being displayed with sneak profession
                    Character c = Utility.isThereAFarmerOrCharacterWithinDistance(new Vector2(tileLocation.X, tileLocation.Y), 7, __instance);
                    if (c != null && c is NPC && c is not StardewValley.Characters.Horse)
                    {
                        if (!c.Name.Equals("Linus"))
                        {
                            // Sneak Profession
                            // Undo friendship drop and cancel dialogue/emote
                            c.isEmoting = false;
                            Game1.dialogueUp = false;
                            Game1.activeClickableMenu = null;
                            who.forceCanMove();
                            who.changeFriendship(ModEntry.Config.FriendshipRecovery, c as NPC);
                        }
                    }
                }

                Utilities.DoTrashCanCheck("Town", __state[0].ToString(), __state[1], Utilities.GetItemPosition(new Vector2(tileLocation.X, tileLocation.Y)));
            }
        }
        static void Finalizer(Exception __exception)
        {
            if (__exception != null)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{__exception}");
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
            if (f.HasCustomProfession(BinningSkill.Reclaimer))
            {
                if (__result < 0)
                {
                    return;
                }
                float extraPercentage = (ModEntry.Config.ReclaimerExtraValuePercent / 100.0f);
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

        static void Finalizer(Exception __exception)
        {
            if (__exception != null)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{__exception}");
            }
        }
    }

    [HarmonyPatch(typeof(NPC), nameof(NPC.getGiftTasteForThisItem))]
    class NPC_GetGiftTasteForThisItem
    {
        public static void Postfix(
            Item item,
            ref int __result)
        {
            if (Game1.player.HasCustomProfession(BinningSkill.Upseller))
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
        }

        static void Finalizer(Exception __exception)
        {
            if (__exception != null)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{__exception}");
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
            // Check heldObject in the prefix.  Need to see if this was null to know if trash has been recycled.
            __state = __instance.heldObject.Value;
        }

        public static void Postfix(
            StardewValley.Object __instance,
            StardewValley.Object __state,
            Item dropInItem,
            bool probe,
            Farmer who)
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
                        Utility.improveFriendshipWithEveryoneInRegion(who, ModEntry.Config.RecyclingFriendshipGain, 2);
                    }
                }

                SpaceCore.Skills.AddExperience(who, "drbirbdev.Binning", ModEntry.Config.ExperienceFromRecycling);
            }
        }

        static void Finalizer(Exception __exception)
        {
            if (__exception != null)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{__exception}");
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
            if (name.Equals("Recycling Machine") && Game1.player.HasCustomProfession(BinningSkill.Recycler))
            {
                __instance.recipeList = new()
                {
                    { 388, 15 },
                    { 390, 15 },
                    { 334, 1 }
                };
            }
        }

        static void Finalizer(Exception __exception)
        {
            if (__exception != null)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{__exception}");
            }
        }
    }

    // 3rd Party
    [HarmonyPatch("RidgesideVillage.TrashCans", "Trigger")]
    class RidgesideVillage_TrashCans_Trigger
    {
        public static bool Prepare()
        {
            return ModEntry.RSVLoaded;
        }

        public static void Prefix(
            ref object[] __state,
            string tileAction,
            Vector2 position,
            HashSet<Vector2> ___TrashCansTriggeredToday)
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

        public static void Postfix(
            object[] __state,
            string tileAction,
            Vector2 position)
        {
            if (__state != null && __state.Length > 0)
            {
                if (!Game1.player.HasCustomProfession(BinningSkill.Sneak))
                {
                    Character c = Utility.isThereAFarmerOrCharacterWithinDistance(position, 7, Game1.currentLocation);
                    if (c != null && c is NPC && c is not StardewValley.Characters.Horse)
                    {
                        // Sneak Profession
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
                }

                Utilities.DoTrashCanCheck("RSV", (string)__state[0], (int)__state[1], Utilities.GetItemPosition(position));
            }
        }

        static void Finalizer(Exception __exception)
        {
            if (__exception != null)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{__exception}");
            }
        }
    }

    [HarmonyPatch("Pathoschild.Stardew.Automate.Framework.Machines.Tiles.TrashCanMachine", "GetRandomTrash")]
    class Automate_TrashCanMachine_GetRandomTrash
    {
        public static bool Prepare()
        {
            return ModEntry.AutomateLoaded;
        }

        internal static void Postfix(
            int index,
            ref Item __result)
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

        internal static void Finalizer(Exception __exception)
        {
            if (__exception != null)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{__exception}");
            }
        }
    }

    [HarmonyPatch("Pathoschild.Stardew.Automate.Framework.Machines.Objects.RecyclingMachine", "SetInput")]
    class Automate_RecyclingMachine_SetInput
    {

        static PropertyInfo Machine;
        public static bool Prepare()
        {
            if (!ModEntry.AutomateLoaded)
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
                        Utility.improveFriendshipWithEveryoneInRegion(Game1.player, ModEntry.Config.RecyclingFriendshipGain, 2);
                    }
                }

                if (Game1.player.HasCustomProfession(BinningSkill.Salvager))
                {
                    StardewValley.Object machine = (StardewValley.Object)Machine.GetValue(__instance);

                    machine.heldObject.Value = Utilities.GetSalvagerUpgrade(machine.heldObject.Value);
                }
            }
        }

        internal static void Finalizer(Exception __exception)
        {
            if (__exception != null)
            {
                Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{__exception}");
            }
        }
    }

}

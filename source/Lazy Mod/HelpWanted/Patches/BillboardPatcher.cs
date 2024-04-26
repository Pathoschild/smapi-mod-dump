/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Common.Patch;
using HarmonyLib;
using HelpWanted.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace HelpWanted.Patches;

public class BillboardPatcher : BasePatcher
{
    private static ModConfig config = null!;

    public BillboardPatcher(ModConfig config)
    {
        BillboardPatcher.config = config;
    }

    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            RequireMethod<Billboard>(nameof(Billboard.draw), new[] { typeof(SpriteBatch) }),
            GetHarmonyMethod(nameof(DrawPrefix))
        );
        harmony.Patch(
            RequireMethod<Billboard>(nameof(Billboard.receiveLeftClick)),
            postfix: GetHarmonyMethod(nameof(ReceiveLeftClickPostfix))
        );
    }
    
    private static bool DrawPrefix(bool ___dailyQuestBoard)
    {
        if (!___dailyQuestBoard || Game1.activeClickableMenu.GetType() != typeof(Billboard))
            return true;
        Game1.activeClickableMenu = new HWQuestBoard(config);
        return false;
    }

    private static void ReceiveLeftClickPostfix(Billboard __instance, bool ___dailyQuestBoard, int x, int y)
    {
        if (!___dailyQuestBoard || Game1.activeClickableMenu is not HWQuestBoard)
            return;
        __instance.acceptQuestButton.visible = true;
        if (__instance.acceptQuestButton.containsPoint(x, y))
        {
            Game1.questOfTheDay.daysLeft.Value = config.QuestDays;
            Game1.player.acceptedDailyQuest.Set(false);
            Game1.netWorldState.Value.SetQuestOfTheDay(null);
            HWQuestBoard.QuestDataDictionary.Remove(HWQuestBoard.ShowingQuestID);
            HWQuestBoard.QuestNotes.RemoveAll(option => option.myID == HWQuestBoard.ShowingQuestID);
            HWQuestBoard.ShowingQuest = null;
        }
        else if (__instance.upperRightCloseButton.containsPoint(x, y))
        {
            HWQuestBoard.ShowingQuest = null;
        }
    }
}
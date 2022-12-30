/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Common;

#region using directives

using System.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class NpcCheckForNewCurrentDialoguePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NpcCheckForNewCurrentDialoguePatcher"/> class.</summary>
    internal NpcCheckForNewCurrentDialoguePatcher()
    {
        this.Target = this.RequireMethod<NPC>(nameof(NPC.checkForNewCurrentDialogue));
    }

    #region harmony patches

    /// <summary>Add special custom dialogue.</summary>
    [HarmonyPrefix]
    private static bool NpcCheckForNewCurrentDialoguePrefix(NPC __instance)
    {
        try
        {
            var player = Game1.player;
            switch (__instance.Name)
            {
                case "Clint" when player.hasQuest(Constants.ForgeIntroQuestId):
                    if (player.Read(DataFields.DaysLeftTranslating, -1) > 0)
                    {
                        __instance.CurrentDialogue.Clear();
                        __instance.CurrentDialogue.Push(new Dialogue(
                            I18n.Get("dialogue.clint.blueprint.notdone"),
                            __instance));
                        return false; // don't run original logic
                    }

                    if (player.Read(DataFields.DaysLeftTranslating, int.MaxValue) <= 0)
                    {
                        __instance.CurrentDialogue.Clear();
                        __instance.CurrentDialogue.Push(new Dialogue(
                            I18n.Get("dialogue.clint.blueprint.done"),
                            __instance));
                        player.completeQuest(Constants.ForgeIntroQuestId);
                        player.mailReceived.Add("clintForge");
                        player.Write(DataFields.DaysLeftTranslating, null);
                        return false; // don't run original logic
                    }

                    break;

                case "Wizard" when player.hasQuest(Constants.VirtuesIntroQuestId):
                    __instance.CurrentDialogue.Clear();
                    __instance.CurrentDialogue.Push(new Dialogue(
                        I18n.Get("dialogue.wizard.curse.canthelp"),
                        __instance));
                    return false; // don't run original logic
            }

            return true; // run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}

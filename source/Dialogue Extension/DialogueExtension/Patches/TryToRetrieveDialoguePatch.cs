/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using DialogueExtension.Patches.Parsing;
using DialogueExtension.Patches.Utility;
using HarmonyLib;
using JetBrains.Annotations;
using SDV.Shared.Abstractions;
using SDV.Shared.Abstractions.Utility;
using StardewModdingAPI;
using StardewValley;
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantAssignment

namespace DialogueExtension.Patches
{
  public class TryToRetrieveDialoguePatch : HarmonyPatch
  {
    public TryToRetrieveDialoguePatch(IMonitor monitor, IHarmonyWrapper wrapper, IDialogueLogic dialogueLogic, IWrapperFactory factory) : base(monitor, wrapper)
    {
      HarmonyWrapper.Patch(
       AccessTools.Method(typeof(NPC), "tryToRetrieveDialogue"),
       new HarmonyMethod(typeof(TryToRetrieveDialoguePatch), "Prefix"),
       new HarmonyMethod(typeof(TryToRetrieveDialoguePatch), "Postfix"));

      _dialogueLogic = dialogueLogic;
      _wrapperFactory = factory;
    }
    
    protected override string PatchName => ".tryToRetrieveDialogue";
    private static IDialogueLogic _dialogueLogic;
    private static IWrapperFactory _wrapperFactory;

    [UsedImplicitly]
    private static bool Prefix(ref NPC __instance, ref string preface, ref Dialogue __result)
    {
      var npc = _wrapperFactory.CreateInstance<INPCWrapper>(__instance, Logger);

      __result = _dialogueLogic.GetDialogue(ref npc, !string.IsNullOrEmpty(preface)).GetBaseType;
      if (__result == null) Logger.Log("Value is null", LogLevel.Alert);
      else Logger.Log(__result.getCurrentDialogue(), LogLevel.Alert);
      return true;
    }


    [UsedImplicitly]
    private static void Postfix(ref NPC __instance, ref Dialogue __result)
    {
      if (__result == null) Logger.Log("Value is null", LogLevel.Alert);
      else Logger.Log(__result.getCurrentDialogue(), LogLevel.Alert);
    }
  }
}
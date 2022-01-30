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
using LightInject;

namespace DialogueExtension.Patches.Utility
{
  public class PatchCompositionRoot : ICompositionRoot
  {
    public void Compose(IServiceRegistry serviceRegistry)
    {
      serviceRegistry
        .Register<IConditionRepository, ConditionRepository>(new PerContainerLifetime())
        .Register<IDialogueLogic, DialogueLogic>()
        .Register<IDialogueParser, VanillaDialogueParser>()
        .Register<IHarmonyPatch, TryToRetrieveDialoguePatch>();
    }
  }
}
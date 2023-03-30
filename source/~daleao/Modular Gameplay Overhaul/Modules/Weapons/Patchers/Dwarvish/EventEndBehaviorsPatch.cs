/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Patchers.Dwarvish;

#region using directives

using DaLion.Overhaul.Modules.Weapons.Events;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class EventEndBehaviorsPatch : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="EventEndBehaviorsPatch"/> class.</summary>
    internal EventEndBehaviorsPatch()
    {
        this.Target = this.RequireMethod<Event>(nameof(Event.endBehaviors));
    }

    #region harmony patches

    /// <summary>Subscribe to blueprint translation event.</summary>
    [HarmonyPostfix]
    private static void EventEndBehaviorsPostfix(Event __instance)
    {
        if (__instance.id == (int)Quest.ForgeIntro)
        {
            EventManager.Enable<BlueprintDayStartedEvent>();
        }
    }

    #endregion harmony patches
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System.Collections.Generic;

namespace Slothsoft.Informant;

internal record InformantConfig {
    public Dictionary<string, bool> DisplayIds { get; set; } = new();
    public TooltipTrigger TooltipTrigger { get; set; } = TooltipTrigger.Hover;
    public SButton TooltipTriggerButton { get; set; } = SButton.MouseRight;
    public HideMachineTooltips HideMachineTooltips { get; set; } = HideMachineTooltips.ForNonMachines;
    public bool DecorateLockedBundles { get; set; }
}

internal enum TooltipTrigger {
    Hover,
    ButtonHeld,
}

internal enum HideMachineTooltips {
    ForNonMachines,
    ForChests,
    Never,
}

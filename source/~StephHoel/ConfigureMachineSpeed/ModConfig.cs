/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StephHoel/ModsStardewValley
**
*************************************************/

using StardewModdingAPI;
using Utils.Config;
using static StardewValley.Menus.CharacterCustomization;

namespace ConfigureMachineSpeed;

internal class ModConfig
{
    public uint UpdateInterval { get; set; } = 10u;

    public SButton? ReloadConfigKey { get; set; } = SButton.L;

    public MachineConfig[] Machines { get; set; }

    public ModConfig()
    {
        Machines ??= DefaultMachines();
    }

    private static MachineConfig[] DefaultMachines()
    {
        var machinesDefault = Utils.Machines.GetNewMachines();
        return machinesDefault;
    }

}
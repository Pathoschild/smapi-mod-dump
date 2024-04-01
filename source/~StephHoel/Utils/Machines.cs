/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StephHoel/ModsStardewValley
**
*************************************************/

using Utils.Config;
using Utils.Lists;

namespace Utils;

public class Machines
{
    public static MachineConfig[] GetNewMachines()
    {
        var source = ListMachines.List;
        var newMachines = source.Select((string x) => new MachineConfig(x)).ToArray();
        return newMachines;
    }

    public static MachineConfig[] SetMachines(MachineConfig?[] machines)
    {
        var machinesList = machines.ToList();
        var newMachines = GetNewMachines();

        foreach (var machine in newMachines)
        {
            if (!machinesList.Contains(machine))
                machinesList.Add(machine);
        }

        return machinesList.ToArray()!;
    }
}
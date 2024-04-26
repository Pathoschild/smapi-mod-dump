/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/StephHoel/ModsStardewValley
**
*************************************************/

namespace ConfigureMachineSpeed;

public class Machines
{
    public static MachineConfig[] GetNewMachines()
    {
        var source = MachineList;
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

    private static readonly List<string> MachineList = [
        "Bee House",
        "Bone Mill",
        "Cask",
        "Charcoal Kiln",
        "Cheese Press",
        "Crystalarium",
        "Dehydrator",
        "Fish Smoker",
        "Furnace",
        "Geode Crusher",
        "Heavy Furnace",
        "Heavy Tapper",
        "Incubator",
        "Keg",
        "Lightning Rod",
        "Loom",
        "Mayonnaise Machine",
        "Mushroom Log",
        "Oil Maker",
        "Ostrich Incubator",
        "Preserves Jar",
        "Recycling Machine",
        "Seed Maker",
        "Slime Egg-Press",
        "Slime Incubator",
        "Solar Panel",
        "Tapper",
        "Worm Bin"
    ];
}

public class MachinesComparer : IEqualityComparer<MachineConfig>
{
    public bool Equals(MachineConfig x, MachineConfig y)
    {
        return x.Name == y.Name;
    }

    public int GetHashCode(MachineConfig obj)
    {
        return obj.Name.GetHashCode();
    }
}
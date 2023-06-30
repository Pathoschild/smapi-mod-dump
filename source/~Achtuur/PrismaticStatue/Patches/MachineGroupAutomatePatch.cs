/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Patches;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using Sickhead.Engine.Util;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PrismaticStatue.Patches;

internal class MachineGroupAutomatePatch : GenericPatcher
{
    static Type IMachineGroupType { get; } = AccessTools.TypeByName("Pathoschild.Stardew.Automate.Framework.MachineGroup");
    static Type IJunimoMachineGroupType { get; } = AccessTools.TypeByName("Pathoschild.Stardew.Automate.Framework.JunimoMachineGroup");
    public override void Patch(Harmony harmony)
    {
        harmony.Patch(
            original: IMachineGroupType.GetMethod("Automate", BindingFlags.Public | BindingFlags.Instance),
            postfix: this.GetHarmonyMethod(nameof(Postfix_Automate))
        );
    }

    private static void Postfix_Automate(object __instance)
    {
        try
        {
            if (!Context.IsWorldReady || __instance == null)
                return;


            if (__instance.GetType() == IJunimoMachineGroupType)
            {
                // In junimo group, multiple machine groups are available.
                // Every group will be processed separately, so statues are not shared globally
                IEnumerable<object> machineGroups = GetJunimoGroupMachineGroups(__instance);
                foreach (object machineGroup in machineGroups)
                {
                    ProcessMachineGroup(machineGroup);
                }
            }
            else
            {
                // If not junimoGroup, then __instance is the MachineGroup
                ProcessMachineGroup(__instance);
            }
        }
        catch (Exception e)
        {
            AchtuurCore.Logger.TraceLog(ModEntry.Instance.Monitor, $"Something went wrong when creating and updating machine groups:\n{e}");
        }
    }

    private static IEnumerable<object> GetJunimoGroupMachineGroups(object junimoGroup)
    {
        return (IEnumerable<object>)AccessTools.Method(IJunimoMachineGroupType, "GetAll").Invoke(junimoGroup, null);
    }

    /// <summary>
    /// Processmachine group <paramref name="MachineGroup"/>. Object is of type <see cref="IMachineGroupType"/>
    /// </summary>
    /// <param name="MachineGroup"></param>
    private static void ProcessMachineGroup(object MachineGroup)
    {
        // Get tiles property
        IReadOnlySet<Vector2> tiles = (IReadOnlySet<Vector2>)MachineGroup.GetType().GetField("Tiles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(MachineGroup, null);
        // Get machine list of this machine group
        IMachine[] Machines = (IMachine[])MachineGroup.GetType().GetProperty("Machines").GetValue(MachineGroup, null);

        if (Machines is null || tiles is null)
            return;

        // Get machines that should be sped up and number of statues
        IMachine[] MachinesToSpeedup = Machines.Where(m => m.MachineTypeID != SpeedupStatue.TypeId).ToArray<IMachine>();
        int n_statues = Machines.Where(m => m.MachineTypeID == SpeedupStatue.TypeId).Count();

        // Check if SpedupMachineGroups contains machine group and update it if it does
        bool groupExists = UpdateExistingGroup(MachinesToSpeedup, tiles, n_statues);

        // If no machines to speed up, or group already exists -> do nothing else
        if (
            //MachinesToSpeedup.Length <= 0 ||
            groupExists ||
            n_statues < 1
            )
            return;


        // Add machine group
        SpedUpMachineGroup machineGroup = new SpedUpMachineGroup(MachinesToSpeedup, tiles, n_statues, Machines[0].Location);
        machineGroup.UpdateGroup(MachinesToSpeedup, tiles, n_statues);
        ModEntry.Instance.SpedupMachineGroups.Add(machineGroup);
    }

    /// <summary>
    /// Updates machine group if it already exists
    /// </summary>
    /// <param name="MachinesToSpeedup"></param>
    /// <param name="tiles"></param>
    /// <param name="n_statues"></param>
    /// <returns>true if group exists, false if it doesn't</returns>
    private static bool UpdateExistingGroup(IMachine[] MachinesToSpeedup, IReadOnlySet<Vector2> tiles, int n_statues)
    {
        if (MachinesToSpeedup.Length == 0)
            return false;

        var group = ModEntry.Instance.SpedupMachineGroups.Find(mgroup => mgroup.IsMachineGroup(tiles, MachinesToSpeedup[0].Location));

        if (group is null)
            return false;


        group.UpdateGroup(MachinesToSpeedup, tiles, n_statues);

        // Remove groups if statues is 0
        if (n_statues == 0)
        {
            ModEntry.Instance.SpedupMachineGroups.Remove(group);
        }

        return true;
    }
}

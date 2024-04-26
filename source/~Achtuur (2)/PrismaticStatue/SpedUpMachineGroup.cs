/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Utility;
using Microsoft.Xna.Framework;
using Pathoschild.Stardew.Automate;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrismaticStatue;

internal class SpedUpMachineGroup
{
    internal List<GenericSpedUpMachineWrapper> Machines;
    internal GameLocation Location;

    /// <summary>
    /// Unique Id is created by group locationid + all speedup statues location sum
    /// </summary>
    internal string UniqueId;

    /// <summary>
    /// Tiles of this machinegroup, keep this here in order to know what MachineGroup this is
    /// </summary>
    internal IReadOnlySet<Vector2> Tiles;

    internal int n_statues;

    internal SpedUpMachineGroup(IMachine[] MachinesToSpeedup, IReadOnlySet<Vector2> tiles, int n_statues, GameLocation location)
    {
        this.n_statues = n_statues;
        this.Tiles = tiles;

        // Get machine list of this machine 
        this.Machines = GetMachineList(MachinesToSpeedup);
        this.Location = location;
    }

    /// <summary>
    /// Checks whether <see cref="Tiles"/> are up to date, by checking if the number of actual placed statues match the internal value
    /// </summary>
    /// <returns></returns>
    internal bool TilesMatchNStatues()
    {
        //int placedStatueCount = this.Tiles.Count(tile => ModEntry.GetPossibleStatueIDs().Any(id => tile.ContainsObject(id, Location)));
        int placedStatueCount = this.Tiles.Count(tile => tile.ContainsObject(SpeedupStatue.ID));
        return this.Tiles is not null && placedStatueCount == n_statues;
    }

    internal bool IsMachineGroup(IReadOnlySet<Vector2> GroupTiles, GameLocation GroupLocation)
    {
        if (this.Location != GroupLocation)
            return false;

        // If more Tiles old than new tiles, check if all new tiles are a subset of old tiles
        if (Tiles.Count > GroupTiles.Count)
        {
            return GroupTiles.All(tile => Tiles.Contains(tile));
        }
        // If more (or equal) tiles new than old, check if old tiles are a subset of new tiles
        return Tiles.All(tile => GroupTiles.Contains(tile));
    }

    /// <summary>
    /// Updates machines in this group and speedup state.
    /// </summary>
    /// <param name="MachinesToSpeedup"></param>
    /// <param name="n_statues"></param>
    /// <returns>Returns true if this group should be deleted</returns>
    internal void UpdateGroup(IMachine[] MachinesToSpeedup, IReadOnlySet<Vector2> tiles, int n_statues)
    {
        UpdateMachineList(MachinesToSpeedup, tiles);
        this.UpdateNStatues(n_statues);
    }

    internal GenericSpedUpMachineWrapper GetMachine(IMachine machine)
    {
        return Machines.Find(sm => sm.isSameMachine(machine));
    }

    internal bool ContainsTile(Vector2 tile)
    {
        return this.Tiles is not null && this.Tiles.Contains(tile);
    }


    /// <summary>
    /// Takes in new value of n_statues for this machine group, if it is different then all processing machines in this group are notified
    /// </summary>
    /// <param name="new_n_statues"></param>
    internal void UpdateNStatues(int new_n_statues)
    {
        if (new_n_statues == this.n_statues)
            return;

        foreach (GenericSpedUpMachineWrapper machine in this.Machines)
        {
            machine.OnNStautesChange(new_n_statues);
        }

        this.n_statues = new_n_statues;
    }

    internal void RestoreAllMachines()
    {
        foreach (GenericSpedUpMachineWrapper machineWrapper in this.Machines)
        {
            machineWrapper.RestoreSpeed();
        }
    }

    internal void OnTimeChanged()
    {
        foreach (GenericSpedUpMachineWrapper machine in this.Machines)
        {
            machine.OnTimeChanged();
        }
    }

    internal void OnDayStarted()
    {
        foreach(GenericSpedUpMachineWrapper machine in this.Machines)
        {
            machine.OnDayStarted();
        }
    }



    /// <summary>
    /// Updates list of processing machines in this group and updates state of all machines in list
    /// </summary>
    /// <param name="machines"></param>
    public void UpdateMachineList(IMachine[] machines, IReadOnlySet<Vector2> tiles)
    {
        // Update machine list
        if (machines.Length != this.Machines.Count)
        {
            List<GenericSpedUpMachineWrapper> machines_wrapped = machines
                .Select(m => GetWrapper(m))
                .Where(wrap => !wrap.isNull())
                .ToList();

            //if this.Machines does not contain machine && machine contains machine -> add machine
            List<GenericSpedUpMachineWrapper> AddMachines = machines_wrapped.Where(m => !this.Machines.Any(this_m => this_m.isSameMachine(m))).ToList();

            //if this.Machines contains machine && machine does not contain machine -> remove machine
            List<GenericSpedUpMachineWrapper> RemoveMachines = this.Machines.Where(this_m => !machines_wrapped.Any(m => m.isSameMachine(this_m))).ToList();


            // First restore speed before deleting
            foreach (GenericSpedUpMachineWrapper machine in RemoveMachines)
            {
                machine.RestoreSpeed();
            }

            // Remove and add machines that should be removed and added resp.
            this.Machines = this.Machines.Except(RemoveMachines).Concat(AddMachines).ToList();
        }

        // Update tiles
        if (tiles != null && tiles.Count != this.Tiles.Count)
        {
            this.Tiles = tiles;
        }

        foreach (GenericSpedUpMachineWrapper machine in this.Machines)
        {
            machine.UpdateState();
        }
    }

    public List<GenericSpedUpMachineWrapper> GetMachineList(IMachine[] machines)
    {
        return machines
            .Select(machine => GetWrapper(machine))
            .Where(wrap => !wrap.isNull())
            .ToList();
    }

    internal GenericSpedUpMachineWrapper GetWrapper(IMachine machine)
    {
        if (machine.MachineTypeID == "Cask")
        {
            return new SpedUpCaskWrapper(machine, this.n_statues);
        }
        return new SpedUpMachineWrapper(machine, this.n_statues);
    }

    internal IEnumerable<Vector2> GetStatueTiles()
    {
        //return this.Tiles.Where(tile => ModEntry.GetPossibleStatueIDs().Any(id => tile.ContainsObject(id, Location)));
        return this.Tiles.Where(tile => tile.ContainsObject(SpeedupStatue.ID));
    }

    internal IEnumerable<Vector2> GetMachineTiles()
    {
        return this.Machines.Select(m => m.GetTile())
            .Where(t => t != null)
            .Cast<Vector2>();
        
    }
    
}

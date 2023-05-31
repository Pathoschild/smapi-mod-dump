/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Rings;

/// <summary>The runtime state variable for RNGS.</summary>
internal sealed class State
{
    private int _warriorKillCount;

    internal int WarriorKillCount
    {
        get
        {
            return this._warriorKillCount;
        }

        set
        {
            this._warriorKillCount = Math.Min(value, 20);
        }
    }

    internal int SavageExcitedness { get; set; }

    internal int YobaShieldHealth { get; set; } = -1;

    internal bool CanReceiveYobaShield { get; set; } = true;
}

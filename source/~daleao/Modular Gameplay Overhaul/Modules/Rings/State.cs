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

/// <summary>The runtime state Ring variables.</summary>
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
            Game1.player.attack += value - this._warriorKillCount;
            this._warriorKillCount = value;
        }
    }

    internal int SavageExcitedness { get; set; }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions;

#region using directives

using System.Collections.Generic;
using StardewValley.Monsters;

using Framework;
using Framework.TreasureHunt;
using Framework.Ultimate;

#endregion using directives

internal class PlayerState
{
    private Ultimate _registeredUltimate;

    internal Ultimate RegisteredUltimate
    {
        get => _registeredUltimate;
        set
        {
            if (value is null) _registeredUltimate?.Dispose();
            _registeredUltimate = value;
        }
    }

    internal TreasureHunt ScavengerHunt { get; set; } = new ScavengerHunt();
    internal TreasureHunt ProspectorHunt { get; set; } = new ProspectorHunt();
    internal HudPointer Pointer { get; set; } = new();
    internal Dictionary<int, float> OverchargedBullets { get; } = new();
    internal HashSet<int> BlossomBullets { get; } = new();
    internal HashSet<int> BouncedBullets { get; } = new();
    internal HashSet<int> PiercedBullets { get; } = new();
    internal HashSet<GreenSlime> PipedSlimes { get; } = new();
    internal int[] AppliedPiperBuffs { get; } = new int[12];
    internal int BruteRageCounter { get; set; }
    internal int BruteKillCounter { get; set; }
    internal int SecondsSinceLastCombat { get; set; }
    internal int DemolitionistExcitedness { get; set; }
    internal int SpelunkerLadderStreak { get; set; }
    internal int SlimeContactTimer { get; set; }
    internal bool UsedDogStatueToday { get; set; }
    internal bool[] RevalidatedLevelThisSession { get; set; } = new bool[6];
}
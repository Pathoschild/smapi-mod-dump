/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions;

#region using directives

using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley.Monsters;

using Framework.SuperMode;
using Framework.TreasureHunt;

#endregion using directives

internal class ModState
{
    internal SuperMode SuperMode { get; set; }
    internal TreasureHunt ScavengerHunt { get; set; } = new ScavengerHunt();
    internal TreasureHunt ProspectorHunt { get; set; } = new ProspectorHunt();
    internal Pointer Pointer { get; set; } = new();
    internal bool UsedDogStatueToday { get; set; }
    internal int DemolitionistExcitedness { get; set; }
    internal int SpelunkerLadderStreak { get; set; }
    internal int SlimeContactTimer { get; set; }
    internal Dictionary<SuperModeIndex, HashSet<long>> ActivePeerSuperModes { get; set; } = new();
    internal HashSet<int> MonstersStolenFrom { get; set; } = new();
    internal HashSet<int> AuxiliaryBullets { get; set; } = new();
    internal HashSet<int> BouncedBullets { get; set; } = new();
    internal HashSet<int> PiercedBullets { get; set; } = new();
    internal Dictionary<GreenSlime, float> PipedSlimeScales { get; set; } = new();
    internal ICursorPosition CursorPosition { get; set; }
}
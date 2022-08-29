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

using Framework;
using Framework.TreasureHunts;
using System;
using System.Collections.Generic;

#endregion using directives

internal class ModState
{
    internal Lazy<TreasureHunt> ScavengerHunt { get; } = new(() => new ScavengerHunt());
    internal Lazy<TreasureHunt> ProspectorHunt { get; } = new(() => new ProspectorHunt());
    internal int[] AppliedPiperBuffs { get; } = new int[12];
    internal int BruteRageCounter { get; set; }
    internal int BruteKillCounter { get; set; }
    internal int SecondsOutOfCombat { get; set; }
    internal int DemolitionistExcitedness { get; set; }
    internal int SpelunkerLadderStreak { get; set; }
    internal int SlimeContactTimer { get; set; }
    internal bool UsedDogStatueToday { get; set; }
    internal Queue<ISkill> SkillsToReset { get; } = new();
}
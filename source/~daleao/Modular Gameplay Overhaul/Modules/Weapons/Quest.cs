/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons;

internal enum Quest
{
    /// <summary>The initial quest which introduces the forging mechanic.</summary>
    ForgeIntro = 144701,

    /// <summary>The follow-up forging quest for collecting all Dwarven blueprints.</summary>
    ForgeNext = 144702,

    /// <summary>The initial quest which introduces Viego's curse.</summary>
    CurseIntro = 144703,

    /// <summary>The overarching quest to all the virtue trials.</summary>
    /// <remarks>No longer used since 2.2.4.</remarks>
    HeroJourney = 144704,

    /// <summary>The quest which concludes hero's journey.</summary>
    HeroReward = 144705,
}

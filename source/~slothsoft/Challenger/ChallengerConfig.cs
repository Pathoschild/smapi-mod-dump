/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

namespace Slothsoft.Challenger;

internal record ChallengerConfig {

    public SButton ButtonOpenMenu { get; set; } = SButton.K;
    public bool DisplayEarnMoneyChallenge { get; set; } = false;
}
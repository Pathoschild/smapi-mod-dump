/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using Netcode;
using Newtonsoft.Json;
using StardewValley.Network;

namespace Slothsoft.Challenger.Models;

public sealed class ChallengerState : INetObject<NetFields> {
    public NetRef<ChallengeSelection> ChallengeSelection { get; } = new();

    public NetStringDictionary<EarnMoneyProgress, NetRef<EarnMoneyProgress>> EarnMoneyProgresses = new();
    
    public NetStringDictionary<ChallengeInfo, NetRef<ChallengeInfo>> ChallengeInfos = new();

    public ChallengerState() {
        NetFields.AddFields(
            ChallengeSelection,
            EarnMoneyProgresses,
            ChallengeInfos
        );
    }

    [JsonIgnore]
    public NetFields NetFields { get; } = new();
}
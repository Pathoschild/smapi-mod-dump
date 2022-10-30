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

namespace Slothsoft.Challenger.Models;

public record EarnMoneyProgress : INetObject<NetFields> {
    
    public EarnMoneyProgress() {
        NetFields.AddFields(
            _moneyEarned
        );
    }

    [JsonIgnore]
    public NetFields NetFields { get; } = new();

    private readonly NetInt _moneyEarned = new();
    public int MoneyEarned {
        get => _moneyEarned.Value;
        set => _moneyEarned.Value = value;
    }
}
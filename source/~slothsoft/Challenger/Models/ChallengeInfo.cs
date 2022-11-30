/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Netcode;
using Newtonsoft.Json;
using Slothsoft.Challenger.Api;
using StardewValley.Network;

namespace Slothsoft.Challenger.Models;

public record ChallengeInfo : INetObject<NetFields> {
    
    public ChallengeInfo() {
        NetFields.AddFields(
            _startedOn,
            CompletedOn
        );
    }

    [JsonIgnore]
    public NetFields NetFields { get; } = new();

    private readonly NetInt _startedOn = new(-1);
    public int? StartedOn {
        get => _startedOn.Value < 0 ? null : _startedOn.Value;
        set => _startedOn.Value = value ?? -1;
    }
    
    [JsonIgnore]
    public NetIntDictionary<int, NetInt> CompletedOn { get; } = new();

    public Dictionary<int, int> CompletedOnMap {
        get => Enum.GetValues(typeof(Difficulty))
            .OfType<Difficulty>()
            .Where(d => CompletedOn.ContainsKey((int) d))
            .ToDictionary(d => (int) d, d => CompletedOn[(int) d]);
        set {
            CompletedOn.Clear();
            foreach (var keyValuePair in value) {
                CompletedOn[keyValuePair.Key] = keyValuePair.Value;
            }
        }
    }

    public WorldDate? GetCompletedOnDate(Difficulty difficulty) {
        if (CompletedOn.ContainsKey((int) difficulty)) {
            var completedOn = CompletedOn[(int) difficulty];
            if (completedOn < -1) {
                return null;
            }
            return new WorldDate {
                TotalDays = completedOn
            };
        }
        return null; 
    }
    
    public void SetCompletedOnDate(Difficulty difficulty, WorldDate? date) {
        CompletedOn[(int) difficulty] = date?.TotalDays ?? -1;
    }
}
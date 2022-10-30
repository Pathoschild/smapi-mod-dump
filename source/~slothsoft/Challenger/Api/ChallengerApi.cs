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
using System.Collections.Immutable;
using System.Linq;
using Slothsoft.Challenger.Challenges;
using Slothsoft.Challenger.Models;
using Slothsoft.Challenger.Objects;

namespace Slothsoft.Challenger.Api;

internal class ChallengerApi : IChallengerApi {
    
    private readonly List<IChallenge> _challenges;
    private readonly IModHelper _modHelper;
    
    private IChallenge _activeChallenge;
    private Difficulty _activeDifficulty;

    public ChallengerApi(IModHelper modHelper) {
        _modHelper = modHelper;

        _challenges = new List<IChallenge> {
            new BreweryChallenge(modHelper),
            new HermitChallenge(modHelper),
            new NoCapitalistChallenge(modHelper),
            new VineyardChallenge(modHelper),
        };
        if (ChallengerMod.Instance.Config.DisplayEarnMoneyChallenge) {
            _challenges.Add(new EarnMoneyChallenge(modHelper));
        }
        _challenges.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.CurrentCulture));
        _challenges.Insert(0, new NoChallenge(modHelper));

        _activeChallenge = LoadActiveChallenge();
        Game1.netWorldState.Value.GetChallengerState().ChallengeSelection.fieldChangeEvent += (_, __, newValue) => {
            UpdateChallengeAndDifficulty(FetchChallenge(newValue?.ChallengeId), newValue?.Difficulty ?? Difficulty.Medium);
        };
    }
    
    public bool CanEditChallenges { get; } = Context.IsMainPlayer && Context.IsOnHostComputer;

    private IChallenge LoadActiveChallenge() {
        var dto = Game1.netWorldState.Value.GetChallengerState().ChallengeSelection.GetOrRead(ChallengeSelection.Key) ?? new ChallengeSelection();
        var activeChallenge = FetchChallenge(dto.ChallengeId);
        _activeDifficulty = dto.Difficulty;
        activeChallenge.Start(_activeDifficulty);
        return activeChallenge;
    }

    private IChallenge FetchChallenge(string? challengeId) {
        var activeChallenge = _challenges.SingleOrDefault(c => c.Id == challengeId);
        if (activeChallenge == null) {
            // this can happen if a challenge ID changed or a challenge was removed
            ChallengerMod.Instance.Monitor.Log($"Challenge \"{challengeId}\" was not found.", LogLevel.Debug);
            activeChallenge = new NoChallenge(_modHelper);
        }
        return activeChallenge;
    }

    public IEnumerable<IChallenge> AllChallenges => _challenges.ToImmutableArray();

    public Difficulty ActiveDifficulty {
        get => _activeDifficulty;
        set => UpdateChallengeAndDifficulty(_activeChallenge, value);
    }
    
    public IChallenge ActiveChallenge {
        get => _activeChallenge;
        set => UpdateChallengeAndDifficulty(value, _activeDifficulty);
    }

    private void UpdateChallengeAndDifficulty(IChallenge newActiveChallenge, Difficulty newDifficulty) {
        if (newActiveChallenge == _activeChallenge && newDifficulty == _activeDifficulty) {
            return;
        }
        
        _activeChallenge.Stop();
        ChallengerMod.Instance.Monitor.Log($"Challenge \"{_activeChallenge.DisplayName}\" was stopped.",
            LogLevel.Debug);

        _activeChallenge = newActiveChallenge;
        _activeDifficulty = newDifficulty;

        Game1.netWorldState.Value.GetChallengerState().ChallengeSelection.Write(ChallengeSelection.Key, new ChallengeSelection(_activeChallenge.Id, _activeDifficulty));

        _activeChallenge.Start(newDifficulty);
        ChallengerMod.Instance.Monitor.Log($"Challenge \"{_activeChallenge.DisplayName} ({newDifficulty})\" was started.",
            LogLevel.Debug);
    }

    public void Dispose() {
        _activeChallenge.Stop();
    }
}
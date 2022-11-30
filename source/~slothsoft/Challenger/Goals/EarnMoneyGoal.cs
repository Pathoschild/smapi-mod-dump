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
using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Events;
using Slothsoft.Challenger.Models;
using Slothsoft.Challenger.Objects;

namespace Slothsoft.Challenger.Goals;

public class EarnMoneyGoal : BaseGoal<EarnMoneyProgress> {
    
    private readonly Func<Difficulty, int> _targetMoney;
    private readonly Func<Item, bool> _isCountingAllowed;
    private readonly string _displayNameKey;

    public EarnMoneyGoal(IModHelper modHelper, Func<Difficulty, int> targetMoney) 
        : base(modHelper, "earn-money", Game1.netWorldState.Value.GetChallengerState().EarnMoneyProgresses) {
        _targetMoney = targetMoney;
        _isCountingAllowed = _ => true;
        _displayNameKey = GetType().Name;
    }
    
    public EarnMoneyGoal(IModHelper modHelper, Func<Difficulty, int> targetMoney, string suffix, Func<Item, bool> isCountingAllowed) 
        : base(modHelper, $"earn-money-{suffix}", Game1.netWorldState.Value.GetChallengerState().EarnMoneyProgresses) {
        _targetMoney = targetMoney;
        _isCountingAllowed = isCountingAllowed;
        _displayNameKey = GetType().Name + "." + suffix;
    }

    public override string GetDisplayName(Difficulty difficulty) {
        return ModHelper.Translation.Get(_displayNameKey, new {
            Value = StringifyCurrency(_targetMoney(difficulty)),
        });
    }
    
    private static string StringifyCurrency(int value) {
        return $"$ {value:N0}";
    }
    
    public override void Start() {
        base.Start();
        GlobalMoneyCounter.AddSellEvent(OnItemSold);
    }

    internal bool IsCountingAllowed(Item soldItem) {
        return _isCountingAllowed(soldItem);
    }
    
    private void OnItemSold(Item soldItem, int moneyEarned) {
        if (IsCountingAllowed(soldItem)) {
            Progress.MoneyEarned += moneyEarned;
            WriteProgressType(Progress);
        }
    }

    public override void Stop() {
        base.Stop();
        GlobalMoneyCounter.RemoveSellEvent(OnItemSold);
    }

    public override bool WasStarted() {
        return Progress.MoneyEarned > 0;
    }

    public override string GetProgress(Difficulty difficulty) {
        return $"{StringifyCurrency(Progress.MoneyEarned)} / {StringifyCurrency(_targetMoney(difficulty))}";
    }

    public override bool IsCompleted(Difficulty difficulty) {
        return Progress.MoneyEarned >= _targetMoney(difficulty);
    }
}
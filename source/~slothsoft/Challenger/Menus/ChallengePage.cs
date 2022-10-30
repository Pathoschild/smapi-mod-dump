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
using System.Linq;
using Microsoft.Xna.Framework;
using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Challenges;
using Slothsoft.Challenger.Objects;
using StardewValley.Menus;

namespace Slothsoft.Challenger.Menus;

internal class ChallengePage : BaseOptionsPage {
    private readonly OptionsDropDown _challengeSelection;
    private readonly OptionsDropDown _difficultySelection;
    private readonly OptionsElement _goal;
    private readonly OptionsElement _description;

    private string? _lastUpdatedChallenge;
    
    public ChallengePage(Rectangle bounds) : base(bounds) {
        var modHelper = ChallengerMod.Instance.Helper;
        var title = new OptionsElement(modHelper.Translation.Get("ChallengePage.Title") + ":");
        InitObjectsElement(title);
        
        _difficultySelection = new OptionsDropDown("", -1);
        InitObjectsElement(_difficultySelection);
        AddOption(new MultiOptionsElement(title.bounds, title, _difficultySelection));

        _challengeSelection = new OptionsDropDown("", -1);
        InitObjectsElement(_challengeSelection);
        AddOption(_challengeSelection);
        
        _goal = CreateOptionsElement("\n");
        AddOption(_goal);
        
        _description = CreateOptionsElement();
        AddOption(_description);
        
        AddOption(CreateOptionsElement());
        
        var api = ChallengerMod.Instance.GetApi()!;
        var activeChallenge = api.ActiveChallenge;
        var activeDifficulty = api.ActiveDifficulty;
        var allChallenges = api.AllChallenges.ToArray();

        _difficultySelection.greyedOut = !api.CanEditChallenges;
        _challengeSelection.greyedOut = !api.CanEditChallenges;

        var selectedOption = 0;

        for (var i = 0; i < allChallenges.Length; i++) {
            var challenge = allChallenges[i];
            if (activeChallenge.Id == challenge.Id) {
                selectedOption = i;
            }

            _challengeSelection.dropDownOptions.Add(challenge.Id);
            _challengeSelection.dropDownDisplayOptions.Add(challenge.DisplayName);
        }

        _challengeSelection.selectedOption = selectedOption;
        _challengeSelection.RecalculateBounds();
        
        foreach (var difficulty in (Difficulty[]) Enum.GetValues(typeof(Difficulty))) {
            _difficultySelection.dropDownOptions.Add(difficulty.ToString());
            _difficultySelection.dropDownDisplayOptions.Add(modHelper.Translation.Get("Difficulty." + difficulty));
        }
        _difficultySelection.selectedOption = (int) activeDifficulty;
        _difficultySelection.RecalculateBounds();
        
        RefreshDescriptionLabel(false);
        Game1.netWorldState.Value.GetChallengerState().ChallengeSelection.fieldChangeEvent += (_, _, _) => RefreshDescriptionLabel(false);
    }

    private TElement InitObjectsElement<TElement>(TElement optionsElement) 
        where TElement: OptionsElement {
        optionsElement.bounds = new Rectangle(
            optionsElement.bounds.X,
            optionsElement.bounds.Y,
            width - 3 * optionsElement.bounds.X,
            optionsElement.bounds.Height);
        return optionsElement;
    }

    private static OptionsElement CreateOptionsElement(string label = "") {
        var result = new OptionsElement(label) {
            style = OptionsElement.Style.OptionLabel
        };
        var descriptionSize = Game1.smallFont.MeasureString(result.label);
        result.bounds = new Rectangle(result.bounds.X, result.bounds.Y, (int)descriptionSize.X,
            (int)descriptionSize.Y);
        return result;
    }

    public override void releaseLeftClick(int x, int y) {
        base.releaseLeftClick(x, y);
        RefreshDescriptionLabel(true);
    }

    private void RefreshDescriptionLabel(bool saveAllowed) {
        var api = ChallengerMod.Instance.GetApi()!;
        var allChallenges = api.AllChallenges.ToArray();
        var newChallenge = allChallenges[_challengeSelection.selectedOption];
        var newDifficulty = (Difficulty) _difficultySelection.selectedOption;

        var updatedChallenge = newChallenge.Id + "-" + newDifficulty;

        if (updatedChallenge != _lastUpdatedChallenge) {
            if (newChallenge.Id == NoChallenge.ChallengeId) {
                // no challenge will only display its description as the goal and nothing else
                _goal.label = newChallenge.GetDisplayText(newDifficulty); 
                _description.label = "";
            } else {
                _description.label = newChallenge.GetDisplayText(newDifficulty); 
                
                var goal = ChallengerMod.Instance.Helper.Translation.Get("ChallengePage.Goal");
                _goal.label = $"{goal}: {newChallenge.GetGoalDisplayName(newDifficulty)}";

                if (newChallenge.WasStarted()) {
                    _goal.label += $"\n      ({newChallenge.GetProgress(newDifficulty)})";
                }
            }

            if (saveAllowed) {
                api.ActiveChallenge = newChallenge;
                api.ActiveDifficulty = newDifficulty;
            }
            _lastUpdatedChallenge = updatedChallenge;
        }
    }
}
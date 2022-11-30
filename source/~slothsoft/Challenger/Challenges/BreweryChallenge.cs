/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System.Linq;
using Slothsoft.Challenger.Api;
using Slothsoft.Challenger.Goals;
using Slothsoft.Challenger.Common;
using Slothsoft.Challenger.Restrictions;

namespace Slothsoft.Challenger.Challenges;

public class BreweryChallenge : BaseChallenge {
    public BreweryChallenge(IModHelper modHelper) : base(modHelper, "brewery") {
    }

    protected override IRestriction[] CreateRestrictions(IModHelper modHelper, Difficulty difficulty) {
        return new[] {
            CreateRenameRiceJuice(modHelper),
            CreateIncludeFruitOnly(modHelper),
            VineyardChallenge.CreateExcludeAnimalBuildings(modHelper),
            ChangeGlobalStock.AddRiceInFirstSpring(modHelper),
        };
    }
    
    private static IRestriction CreateRenameRiceJuice(IModHelper modHelper) {
        return new RenameVanillaObject(
            modHelper,
            new RenameVanillaObject.VanillaObject(ObjectIds.Juice, SObject.PreserveType.Juice, ObjectIds.UnmilledRice),
            modHelper.Translation.Get("BreweryChallenge.RenameRiceJuice")
        );
    }

    private static IRestriction CreateIncludeFruitOnly(IModHelper modHelper) {
        var beerSeeds = new [] {
            SeedIds.Rice, SeedIds.Hops, SeedIds.Wheat
        };
        return ChangeGlobalStock.ExcludeSalables(modHelper.Translation.Get("BreweryChallenge.IncludeBeerVegetablesOnly"), s => {
            // everything that is not a basic object is allowed
            if (s is not SObject obj) return false;
            // everything that is a sapling is not allowed
            if (SaplingIds.AllSaplings.Contains(obj.ParentSheetIndex)) return true;
            // everything that is not a seed is allowed
            if (!SeedIds.AllSeeds.Contains(obj.ParentSheetIndex)) return false;
            // everything that is a seed for beer is allowed
            if (beerSeeds.Contains(obj.ParentSheetIndex)) return false;
            // and we won't allow anything else
            return true;
        });
    }
    
    public override MagicalReplacement GetMagicalReplacement(Difficulty difficulty) {
        return difficulty == Difficulty.Hard ? MagicalReplacement.Default : MagicalReplacement.Keg;
    }
    
    protected override IGoal CreateGoal(IModHelper modHelper) {
        var beerIndexes = new[] { ObjectIds.Beer, ObjectIds.PaleAle };
        var goal = new EarnMoneyGoal(ModHelper, EarnMoneyChallenge.CalculateTargetMoney, "Beer", salable => {
            if (beerIndexes.Contains(salable.ParentSheetIndex))
                return true;
            // this is rice "beer"
            if (salable.ParentSheetIndex == ObjectIds.Juice) {
                var obj = salable as SObject;
                return obj != null && obj.preservedParentSheetIndex.Value == ObjectIds.UnmilledRice;
            }
            return false;
        });
        goal.ProgressChanged += (_, _) => ProgressChangedInvoked();
        return goal;
    }
}
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
using Slothsoft.Challenger.Models;
using Slothsoft.Challenger.Restrictions;

namespace Slothsoft.Challenger.Challenges;

public class VineyardChallenge : BaseChallenge {
    public VineyardChallenge(IModHelper modHelper) : base(modHelper, "vineyard") {
    }

    protected override IRestriction[] CreateRestrictions(IModHelper modHelper, Difficulty difficulty) {
        return new[] {
            CreateRenameRiceJuice(modHelper),
            CreateIncludeFruitOnly(modHelper),
            CreateExcludeAnimalBuildings(modHelper),
            ChangeGlobalStock.AddRiceInFirstSpring(modHelper),
        };
    }

    private static IRestriction CreateRenameRiceJuice(IModHelper modHelper) {
        return new RenameVanillaObject(
            modHelper,
            new RenameVanillaObject.VanillaObject(ObjectIds.Juice, SObject.PreserveType.Juice, ObjectIds.UnmilledRice),
            modHelper.Translation.Get("VineyardChallenge.RenameRiceJuice")
        );
    }

    private static IRestriction CreateIncludeFruitOnly(IModHelper modHelper) {
        var fruitIds = new [] {
            SeedIds.Strawberry, SeedIds.Rice, SeedIds.Blueberry, SeedIds.Melon, SeedIds.Starfruit, SeedIds.Cranberries, 
            SeedIds.Grape, SeedIds.AncientFruit, SeedIds.CactusFruit, SeedIds.Pineapple, SeedIds.QiFruit, 
            SeedIds.SweetGemBerry,
        };
        return ChangeGlobalStock.ExcludeSalables(modHelper.Translation.Get("VineyardChallenge.IncludeFruitOnly"), s => {
            // everything that is not a basic object is allowed
            if (s is not SObject obj) return false;
            // everything that is not a seed is allowed
            if (!SeedIds.AllSeeds.Contains(obj.ParentSheetIndex)) return false;
            // everything that is a seed for a fruit or rice is allowed
            if (fruitIds.Contains(obj.ParentSheetIndex)) return false;
            // and we won't allow anything else
            return true;
        });
    }

    internal static IRestriction CreateExcludeAnimalBuildings(IModHelper modHelper) {
        return new ExcludeGlobalCarpenter(modHelper.Translation.Get("VineyardChallenge.ExcludeAnimalBuildings"),  BluePrintNames.Barn, BluePrintNames.Silo, BluePrintNames.Coop);
    }
    
    public override MagicalReplacement GetMagicalReplacement(Difficulty difficulty) {
        return difficulty == Difficulty.Hard ? MagicalReplacement.Default : MagicalReplacement.Keg;
    }
    
    protected override IGoal CreateGoal(IModHelper modHelper) {
        return new EarnMoneyGoal(ModHelper, EarnMoneyChallenge.CalculateTargetMoney, "Wine", salable => {
            if (salable.ParentSheetIndex == ObjectIds.Wine)
                return true;
            // this is rice "wine"
            if (salable.ParentSheetIndex == ObjectIds.Juice) {
                var obj = salable as SObject;
                return obj != null && obj.preservedParentSheetIndex.Value == ObjectIds.UnmilledRice;
            }
            return false;
        });
    }
}
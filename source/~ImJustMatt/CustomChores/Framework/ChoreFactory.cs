using System;
using System.Diagnostics.CodeAnalysis;
using LeFauxMatt.CustomChores.Framework.Chores;
using LeFauxMatt.CustomChores.Models;

namespace LeFauxMatt.CustomChores.Framework
{
    internal class BaseChoreFactory: IChoreFactory
    {
        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        
        public IChore GetChore(ChoreData choreData)
        {
            choreData.Config.TryGetValue("ChoreType", out var choreTypeObject);
            var choreType = (string) choreTypeObject;
            if (choreType.Equals("FeedTheAnimals", StringComparison.CurrentCultureIgnoreCase))
                return new FeedTheAnimalsChore(choreData);
            if (choreType.Equals("LoveThePets", StringComparison.CurrentCultureIgnoreCase))
                return new LoveThePetsChore(choreData);
            if (choreType.Equals("GiveAGift", StringComparison.CurrentCultureIgnoreCase))
                return new GiveAGiftChore(choreData);
            if (choreType.Equals("PetTheAnimals", StringComparison.CurrentCultureIgnoreCase))
                return new PetTheAnimalsChore(choreData);
            if (choreType.Equals("RepairTheFences", StringComparison.CurrentCultureIgnoreCase))
                return new RepairTheFencesChore(choreData);
            if (choreType.Equals("WaterTheCrops", StringComparison.CurrentCultureIgnoreCase))
                return new WaterTheCropsChore(choreData);
            if (choreType.Equals("WaterTheSlimes", StringComparison.CurrentCultureIgnoreCase))
                return new WaterTheSlimesChore(choreData);
            return null;
        }
    }
}

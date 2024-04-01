/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/DailyTaskReportPlus
**
*************************************************/

namespace DailyTasksReport.Tasks
{
    public enum CropsTaskId
    {
        None = -1,
        UnwateredCropFarm = 0,
        UnwateredCropGreenhouse = 1,
        UnharvestedCropFarm = 2,
        UnharvestedCropGreenhouse = 3,
        DeadCropFarm = 4,
        DeadCropGreenhouse = 5,
        FruitTreesFarm = 6,
        FruitTreesGreenhouse = 7,
        UnwateredCropWestIsland = 8,
        UnharvestedCropWestIsland = 9,
        DeadCropWestIsland = 10,
        FruitTreesWestIsland = 11
    }
    public enum AnimalsTaskId
    {
        None = -1,
        UnpettedAnimals = 0,
        AnimalProducts = 1,
        MissingHay = 2,
        PondsNeedingAttention = 3,
        PondsWithItems = 4
    }
    public enum ObjectsTaskId
    {
        None = -1,
        UncollectedCrabpots = 0,
        NotBaitedCrabpots = 1,
        UncollectedMachines = 2
    }

    public enum TerrainTaskId
    {
        BrokenFences = 0
    }
}

namespace DailyTasksReport.UI
{
    internal enum OptionsEnum
    {
        /* Special options*/
        OpenReportKey = 1,
        OpenSettings = 2,
        ToggleBubbles = 3,
        ShowReportButton = 4,

        /* Basic options */
        ShowDetailedInfo = 50,
        DisplayBubbles = 51,

        /* What to report */
        UnwateredCrops = 100,
        UnharvestedCrops = 101,
        DeadCrops = 102,
        UnpettedPet = 103,
        UnfilledPetBowl = 104,
        UnpettedAnimals = 105,
        MissingHay = 106,
        FarmCave = 107,
        UncollectedCrabpots = 108,
        NotBaitedCrabpots = 109,
        NewRecipeOnTv = 110,
        Birthdays = 111,
        TravelingMerchant = 112,
        FruitTrees = 113,

        /* Animal products */
        AllAnimalProducts = 200,
        ChickenEgg = 201,
        DinosaurEgg = 202,
        DuckEgg = 203,
        DuckFeather = 204,
        CowMilk = 205,
        GoatMilk = 206,
        SheepWool = 207,
        Truffle = 208,
        RabbitsWool = 209,
        RabbitsFoot = 210,
        SlimeBall = 211,

        /* Machines */
        AllMachines = 300,
        BeeHouse = 301,
        Cask = 302,
        CharcoalKiln = 303,
        CheesePress = 304,
        Crystalarium = 305,
        Furnace = 306,
        Keg = 307,
        LightningRod = 308,
        Loom = 309,
        MayonnaiseMachine = 310,
        OilMaker = 311,
        PreservesJar = 312,
        RecyclingMachine = 313,
        SeedMaker = 314,
        SlimeEggPress = 315,
        SodaMachine = 316,
        StatueOfEndlessFortune = 317,
        StatueOfPerfection = 318,
        Tapper = 319,
        WormBin = 320,

        /* Draw Bubbles */
        DrawUnwateredCrops = 500,
        DrawUnharvestedCrops = 501,
        DrawDeadCrops = 502,
        DrawUnpettedPet = 503,
        DrawUnpettedAnimals = 504,
        DrawAnimalsWithProduce = 505,
        DrawBuildingsWithProduce = 506,
        DrawBuildingsMissingHay = 507,
        DrawTruffles = 508,
        DrawCrabpotsNotBaited = 509,
        DrawCask = 510
    }
}
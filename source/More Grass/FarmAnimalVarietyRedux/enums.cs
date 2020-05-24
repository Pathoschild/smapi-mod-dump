namespace FarmAnimalVarietyRedux
{
    /// <summary>A valid harvest method for an animal to have.</summary>
    public enum HarvestType
    {
        Lay,
        Tool,
        Forage
    }

    /// <summary>A valid ingame season.</summary>
    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }

    /// <summary>A valid mood for a farm animal.</summary>
    public enum Mood
    {
        NewHome,
        Happy,
        Neutral,
        Unhappy,
        Hungry,
        DisturbedByDog,
        LeftOutAtNight
    }
}

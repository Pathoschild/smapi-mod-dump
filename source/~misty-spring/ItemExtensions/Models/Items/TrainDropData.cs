/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

namespace ItemExtensions.Models.Items;

public class TrainDropData : ExtraSpawn
{
    public CarType Car { get; set; } = CarType.Resource;
    public ResourceType Type { get; set; } = ResourceType.Coal;
}

public enum CarType
{
    Plain,
    Resource,
    Passenger,
    Engine
}

public enum ResourceType
{
    None,
    Coal,
    Metal,
    Wood,
    Compartments,
    Grass,
    Hay,
    Bricks,
    Rocks,
    Packages,
    Presents
}

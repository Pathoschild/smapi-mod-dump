/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace MoreFertilizers.DataModels;

/// <summary>
/// Data model used to save the ID number, to protect against shuffling...
/// </summary>
public class MoreFertilizerIDs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MoreFertilizerIDs"/> class.
    /// </summary>
    public MoreFertilizerIDs()
    {
    }

    /// <summary>
    /// Gets or sets the ID number to store for fruit tree fertilizer.
    /// </summary>
    public int FruitTreeFertilizerID { get; set; } = -1;

    /// <summary>
    /// Gets or sets the ID number to store for deluxe fruit tree fertilizer.
    /// </summary>
    public int DeluxeFruitTreeFertilizerID { get; set; } = -1;

    /// <summary>
    /// Gets or sets the ID number for the fish food.
    /// </summary>
    public int FishFoodID { get; set; } = -1;

    /// <summary>
    /// Gets or sets the ID number for the deluxe fish food.
    /// </summary>
    public int DeluxeFishFoodID { get; set; } = -1;

    /// <summary>
    /// Gets or sets the ID number for the domesticated fish food.
    /// </summary>
    public int DomesticatedFishFoodID { get; set; } = -1;

    /// <summary>
    /// Gets or sets the ID number to store for the paddy crop fertilizer.
    /// </summary>
    public int PaddyFertilizerID { get; set; } = -1;

    /// <summary>
    /// Gets or sets the ID number to store for the Lucky Fertilizer.
    /// </summary>
    public int LuckyFertilizerID { get; set; } = -1;

    /// <summary>
    /// Gets or sets the ID number to store for the bountiful fertilizer.
    /// </summary>
    public int BountifulFertilizerID { get; set; } = -1;

    /// <summary>
    /// Gets or sets the ID number to store for joja fertilizer.
    /// </summary>
    public int JojaFertilizerID { get; set; } = -1;

    /// <summary>
    /// Gets or sets the ID number to store for deluxe joja fertilizer.
    /// </summary>
    public int DeluxeJojaFertilizerID { get; set; } = -1;

    /// <summary>
    /// Gets or sets the ID number for the organic fertilizer.
    /// </summary>
    public int OrganicFertilizerID { get; set; } = -1;
}

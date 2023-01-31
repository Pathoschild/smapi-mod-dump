/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Integrations.GMCMAttributes;

namespace LastDayToPlantRedux.Framework;

/// <summary>
/// The config class for this mod.
/// </summary>
internal sealed class ModConfig
{
    private readonly HashSet<int> allowedSeeds = new();
    private readonly HashSet<int> allowedFertilizers = new();

    private bool seedsNeedReset = true;
    private bool fertilizersNeedReset = true;
    private List<string> allowSeedsList = new();
    private List<string> allowFertilizersList = new();

    private CropOptions cropsToDisplay = CropOptions.Purchaseable;
    private FertilizerOptions fertilizersToDisplay = FertilizerOptions.Seen;

    /// <summary>
    /// Gets or sets a value indicating how to display the info.
    /// </summary>
    public DisplayOptions DisplayOption { get; set; } = DisplayOptions.OnFirstWarp;

    public CropOptions CropsToDisplay
    {
        get => this.cropsToDisplay;
        set
        {
            if (value != this.cropsToDisplay)
            {
                CropAndFertilizerManager.RequestInvalidateCrops();
            }
            this.cropsToDisplay = value;
        }
    }

    public FertilizerOptions FertilizersToDisplay
    {
        get => this.fertilizersToDisplay;
        set
        {
            if (value != this.fertilizersToDisplay)
            {
                CropAndFertilizerManager.RequestInvalidateFertilizers();
            }
            this.fertilizersToDisplay = value;
        }
    }

    /// <summary>
    /// Gets or sets a list of crops (by name) that should always be included.
    /// </summary>
    [GMCMDefaultIgnore]
    public List<string> AllowSeedsList
    {
        get
        {
            return this.allowSeedsList;
        }

        set
        {
            this.seedsNeedReset = true;
            this.allowSeedsList = value;
        }
    }

    [GMCMDefaultIgnore]
    public List<string> AllowFertilizersList
    {
        get
        {
            return this.allowFertilizersList;
        }

        set
        {
            this.fertilizersNeedReset = true;
            this.allowFertilizersList = value;
        }
    }

    internal HashSet<int> GetAllowedSeeds()
    {
        if (this.seedsNeedReset)
        {
            this.allowedSeeds.Clear();

            foreach (string? item in this.AllowSeedsList)
            {
                (int id, int type)? tup = LDUtils.ResolveIDAndType(item);
                if (tup is null)
                {
                    continue;
                }
                int id = tup.Value.id;
                int type = tup.Value.type;

                if (type != SObject.SeedsCategory)
                {
                    continue;
                }

                this.allowedSeeds.Add(id);
            }
            this.seedsNeedReset = false;
        }

        return this.allowedSeeds;
    }

    internal HashSet<int> GetAllowedFertilizers()
    {
        if (this.fertilizersNeedReset)
        {
            this.allowedFertilizers.Clear();

            foreach (string? item in this.AllowFertilizersList)
            {
                (int id, int type)? tup = LDUtils.ResolveIDAndType(item);
                if (tup is null)
                {
                    continue;
                }
                int id = tup.Value.id;
                int type = tup.Value.type;

                if (type != SObject.fertilizerCategory)
                {
                    continue;
                }

                this.allowedFertilizers.Add(id);
            }
            this.fertilizersNeedReset = false;
        }

        return this.allowedFertilizers;
    }
}

public enum CropOptions
{
    All,
    Purchaseable,
    Seen,
}

public enum FertilizerOptions
{
    All,
    Seen,
}

public enum DisplayOptions
{
    None,
    InMailbox,
    OnFirstWarp,
}
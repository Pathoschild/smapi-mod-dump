/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

using AtraShared;
using AtraShared.Utils.Extensions;
using StardewModdingAPI.Utilities;
using StardewValley.Locations;

namespace GingerIslandMainlandAdjustments.AssetManagers;

/// <summary>
/// Enum that represents the special roles on Ginger Island.
/// </summary>
public enum SpecialCharacterType
{
    /// <summary>
    /// Musician.
    /// </summary>
    /// <remarks>requires a beach_towel animation, will cause characters with dance animations to dance near them.</remarks>
    Musician,

    /// <summary>
    /// Bartender, who can tend bar at Ginger Island.
    /// </summary>
    /// <remarks>No shop functionality for anyone not Gus though.</remarks>
    Bartender,
}

/// <summary>
/// Enum that represents groups of people who might want to explore GI together.
/// </summary>
public enum SpecialGroupType
{
    /// <summary>
    /// Groups of people who might go to Ginger Island together.
    /// </summary>
    Groups,

    /// <summary>
    /// Groups of characters who might want to explore Ginger Island more.
    /// </summary>
    Explorers,
}

/// <summary>
/// Class to manage asset loading.
/// </summary>
public sealed class AssetLoader : IAssetLoader
{
    /// <summary>
    /// Primary asset path for this mod. All assets should start with this.
    /// </summary>
    private const string AssetPath = "Mods/atravita_Ginger_Island_Adjustments";

    /// <summary>
    /// Fake asset location for bartenders.
    /// </summary>
    private static readonly string BartenderLocation = PathUtilities.NormalizeAssetName(AssetPath + "_bartenders");

    /// <summary>
    /// Fake asset location for explorers.
    /// </summary>
    private static readonly string ExplorerLocation = PathUtilities.NormalizeAssetName(AssetPath + "_explorers");

    /// <summary>
    /// Fake asset location for musicians.
    /// </summary>
    private static readonly string MusicianLocation = PathUtilities.NormalizeAssetName(AssetPath + "_musicians");

    /// <summary>
    /// Fake asset location for groups.
    /// </summary>
    private static readonly string GroupsLocations = PathUtilities.NormalizeAssetName(AssetPath + "_groups");

    /// <summary>
    /// Fake asset location for exclusions.
    /// </summary>
    private static readonly string ExclusionLocations = PathUtilities.NormalizeAssetName(AssetPath + "_exclusions");

    /// <summary>
    /// Full list of fake assets.
    /// </summary>
    private static readonly string[] MyAssets = new string[]
    {
        BartenderLocation,
        ExplorerLocation,
        MusicianLocation,
        GroupsLocations,
        ExclusionLocations,
    };

    private AssetLoader()
    {
    }

    /// <summary>
    /// Gets the instance of the AssetLoader.
    /// </summary>
    public static AssetLoader Instance { get; } = new();

    /// <inheritdoc />
    [UsedImplicitly]
    public bool CanLoad<T>(IAssetInfo asset)
        => MyAssets.Any((string assetpath) => asset.AssetNameEquals(assetpath));

    /// <inheritdoc />
    [UsedImplicitly]
    public T Load<T>(IAssetInfo asset)
    {
        // default vanilla groupings
        if (asset.AssetNameEquals(GroupsLocations))
        {
            Dictionary<string, string> defaultgroups = Globals.ContentHelper.Load<Dictionary<string, string>>("assets/defaultGroupings.json", ContentSource.ModFolder);
            if (Game1.year > 2 && defaultgroups.TryGetValue("JodiFamily", out string? val))
            {
                Globals.ModMonitor.DebugOnlyLog($"Kent is home, adding Kent");
                defaultgroups["JodiFamily"] = val + ", Kent";
            }
            if (defaultgroups.TryGetValue("barfolk", out string? value) && Game1.getAllFarmers().Any((Farmer farmer) => farmer.eventsSeen.Contains(99210002)))
            {
                defaultgroups["barfolk"] = value + ", Pam"; // A little Pam Tries tie-in?
            }
            return (T)(object)defaultgroups;
        }
        // Load an empty document for everything else
        else if (MyAssets.Any((string assetpath) => asset.AssetNameEquals(assetpath)))
        {
            return (T)(object)new Dictionary<string, string>
            {
            };
        }
        throw new InvalidOperationException($"Should not have tried to load '{asset.AssetName}'");
    }

    /// <summary>
    /// Get the special characters for specific scheduling positions.
    /// </summary>
    /// <param name="specialCharacterType">Which type of special position am I looking for.</param>
    /// <returns>HashSet of possible special characters.</returns>
    /// <exception cref="UnexpectedEnumValueException{SpecialCharacterType}">Recieved an unexpected enum value.</exception>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Reviewed")]
    public static HashSet<NPC> GetSpecialCharacter(SpecialCharacterType specialCharacterType)
    {
        HashSet<NPC> specialCharacters = new();
        string assetLocation = specialCharacterType switch
        {
            SpecialCharacterType.Musician => MusicianLocation,
            SpecialCharacterType.Bartender => BartenderLocation,
            _ => throw new UnexpectedEnumValueException<SpecialCharacterType>(specialCharacterType)
        };
        foreach (string? specialChar in Globals.ContentHelper.Load<Dictionary<string, string>>(assetLocation, ContentSource.GameContent).Keys)
        {
            if (specialChar is null)
            {
                continue;
            }
            if (Game1.getCharacterFromName(specialChar) is NPC npc)
            {
                specialCharacters.Add(npc);
            }
            else
            {
                Globals.ModMonitor.Log(I18n.Assetmanager_SpecialcharNotFound(specialCharacterType, specialChar), LogLevel.Debug);
            }
        }
        return specialCharacters;
    }

    /// <summary>
    /// Fetches a special group type from fake asset.
    /// </summary>
    /// <param name="specialGroupType">Which type of special group am I looking for.</param>
    /// <returns>Dictionary of specialGroupName=>Special Group.</returns>
    /// <exception cref="UnexpectedEnumValueException{SpecialGroupType}">Received an unexpected enum value.</exception>
    public static Dictionary<string, HashSet<NPC>> GetCharacterGroup(SpecialGroupType specialGroupType)
    {
        Dictionary<string, HashSet<NPC>> characterGroups = new();
        string assetLocation = specialGroupType switch
        {
            SpecialGroupType.Explorers => ExplorerLocation,
            SpecialGroupType.Groups => GroupsLocations,
            _ => throw new UnexpectedEnumValueException<SpecialGroupType>(specialGroupType)
        };
        Dictionary<string, string> data = Globals.ContentHelper.Load<Dictionary<string, string>>(assetLocation, ContentSource.GameContent);
        foreach (string? groupname in data.Keys)
        {
            if (groupname is null)
            {
                continue;
            }
            HashSet<NPC> group = new();
            foreach (string charname in data[groupname].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (Game1.getCharacterFromName(charname) is NPC npc)
                {
                    group.Add(npc);
                }
                else
                {
                    Globals.ModMonitor.Log(I18n.Assetmanager_GroupcharNotFound(specialGroupType, charname, groupname), LogLevel.Debug);
                }
            }
            characterGroups[groupname] = group;
        }

        // Add all the spouses as a possible group if there are multiple spouses.
        // (This is mostly for Free Love. Your poly commune can all go to Ginger Island together!)
        if (specialGroupType == SpecialGroupType.Groups)
        {
            HashSet<NPC> allSpouses = new();
            foreach (NPC npc in Utility.getAllCharacters())
            {
                if (npc?.isMarried() == true && IslandSouth.CanVisitIslandToday(npc))
                {
                    allSpouses.Add(npc);
                }
            }
            if (allSpouses.Count > 1)
            {
                characterGroups["allSpouses"] = allSpouses;
            }
        }

        return characterGroups;
    }

    /// <summary>
    /// Fetches an exclusions dictionary from fake asset.
    /// </summary>
    /// <returns>Exclusions dictionary.</returns>
    /// <remarks>Will invalidate the cache every time, so cache it if you need it stored.</remarks>
    public static Dictionary<NPC, string[]> GetExclusions()
    {
        Dictionary<NPC, string[]> exclusions = new();
        Dictionary<string, string> data = Globals.ContentHelper.Load<Dictionary<string, string>>(ExclusionLocations, ContentSource.GameContent);
        foreach (string npcname in data.Keys)
        {
            if (Game1.getCharacterFromName(npcname) is NPC npc)
            {
                exclusions[npc] = data[npcname].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }
            else
            {
                Globals.ModMonitor.Log(I18n.Assetmanager_ExclusionsNotFound(npcname), LogLevel.Debug);
            }
        }
        return exclusions;
    }
}

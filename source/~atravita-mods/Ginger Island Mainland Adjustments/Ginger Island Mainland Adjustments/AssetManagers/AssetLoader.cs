/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Collections;
using AtraBase.Toolkit;

using AtraCore.Framework.Caches;

using AtraShared.Utils;
using AtraShared.Utils.Extensions;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Locations;

namespace GingerIslandMainlandAdjustments.AssetManagers;

/// <summary>
/// Enum that represents the special roles on Ginger Island.
/// </summary>
internal enum SpecialCharacterType
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
internal enum SpecialGroupType
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
internal static class AssetLoader
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
    private static HashSet<string> myAssets = null!;
    private static IAssetName groups = null!;

    /// <summary>
    /// Initialized the myAssets hashset.
    /// </summary>
    /// <param name="helper">game content helper.</param>
    internal static void Init(IGameContentHelper helper)
    {
        groups = helper.ParseAssetName(GroupsLocations);

        myAssets = new(StringComparer.OrdinalIgnoreCase)
        {
            helper.ParseAssetName(BartenderLocation).BaseName,
            helper.ParseAssetName(ExplorerLocation).BaseName,
            helper.ParseAssetName(MusicianLocation).BaseName,
            groups.BaseName,
            helper.ParseAssetName(ExclusionLocations).BaseName,
        };
    }

    /// <summary>
    /// Get the special characters for specific scheduling positions.
    /// </summary>
    /// <param name="specialCharacterType">Which type of special position am I looking for.</param>
    /// <returns>HashSet of possible special characters.</returns>
    /// <exception cref="UnexpectedEnumValueException{SpecialCharacterType}">Recieved an unexpected enum value.</exception>
    internal static HashSet<NPC> GetSpecialCharacter(SpecialCharacterType specialCharacterType)
    {
        HashSet<NPC> specialCharacters = new();
        string assetLocation = specialCharacterType switch
        {
            SpecialCharacterType.Musician => MusicianLocation,
            SpecialCharacterType.Bartender => BartenderLocation,
            _ => TKThrowHelper.ThrowUnexpectedEnumValueException<SpecialCharacterType, string>(specialCharacterType)
        };
        foreach (string? specialChar in Globals.GameContentHelper.Load<Dictionary<string, string>>(assetLocation).Keys)
        {
            if (specialChar is null)
            {
                continue;
            }
            if (NPCCache.GetByVillagerName(specialChar) is NPC npc)
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
    internal static Dictionary<string, HashSet<NPC>> GetCharacterGroup(SpecialGroupType specialGroupType)
    {
        Dictionary<string, HashSet<NPC>> characterGroups = new();
        string assetLocation = specialGroupType switch
        {
            SpecialGroupType.Explorers => ExplorerLocation,
            SpecialGroupType.Groups => GroupsLocations,
            _ => TKThrowHelper.ThrowUnexpectedEnumValueException<SpecialGroupType, string>(specialGroupType)
        };
        Dictionary<string, string> data = Globals.GameContentHelper.Load<Dictionary<string, string>>(assetLocation);
        foreach (string? groupname in data.Keys)
        {
            if (groupname is null)
            {
                continue;
            }
            HashSet<NPC> group = new();
            foreach (string charname in data[groupname].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (NPCCache.GetByVillagerName(charname) is NPC npc)
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
            foreach (NPC npc in NPCHelpers.GetNPCs())
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
    internal static Dictionary<NPC, string[]> GetExclusions()
    {
        Dictionary<NPC, string[]> exclusions = new();
        Dictionary<string, string> data = Globals.GameContentHelper.Load<Dictionary<string, string>>(ExclusionLocations);
        foreach (string npcname in data.Keys)
        {
            if (NPCCache.GetByVillagerName(npcname) is NPC npc)
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

    /// <summary>
    /// Loads default files for this mod.
    /// </summary>
    /// <param name="e">AssetRequestedEventArguments.</param>
    internal static void Load(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(groups))
        {
            e.LoadFrom(GetDefaultGroups, AssetLoadPriority.Low);
        }
        else if (myAssets.Contains(e.NameWithoutLocale.BaseName))
        {
            e.LoadFrom(EmptyContainers.GetEmptyDictionary<string, string>, AssetLoadPriority.Low);
        }
    }

    private static Dictionary<string, string> GetDefaultGroups()
    {
        Dictionary<string, string> defaultgroups = Globals.ModContentHelper.Load<Dictionary<string, string>>("assets/defaultGroupings.json");
        if (Game1.year > 2 && defaultgroups.TryGetValue("JodiFamily", out string? val))
        {
            Globals.ModMonitor.DebugOnlyLog($"Kent is home, adding Kent");
            defaultgroups["JodiFamily"] = val + ", Kent";
        }
        if (defaultgroups.TryGetValue("barfolk", out string? value) && Game1.getAllFarmers().Any((Farmer farmer) => farmer.eventsSeen.Contains(99210002)))
        {
            defaultgroups["barfolk"] = value + ", Pam"; // A little Pam Tries tie-in?
        }
        return defaultgroups;
    }
}

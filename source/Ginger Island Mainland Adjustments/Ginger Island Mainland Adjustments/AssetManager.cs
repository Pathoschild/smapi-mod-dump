/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using GingerIslandMainlandAdjustments.Utils;
using StardewModdingAPI.Utilities;

namespace GingerIslandMainlandAdjustments.ScheduleManager;

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
/// Class to manage assets.
/// </summary>
public class AssetManager : IAssetLoader, IAssetEditor
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

    private static readonly string GeorgeDialogueLocation = PathUtilities.NormalizeAssetName("Characters/Dialogue/George");
    private static readonly string EvelynDialogueLocation = PathUtilities.NormalizeAssetName("Characters/Dialogue/Evelyn");
    private static readonly string SandyDialogueLocation = PathUtilities.NormalizeAssetName("Characters/Dialogue/Sandy");
    private static readonly string WillyDialogueLocation = PathUtilities.NormalizeAssetName("Characters/Dialogue/Willy");

    /// <summary>
    /// Full list of fake assets.
    /// </summary>
    private readonly List<string> myAssets = new() { BartenderLocation, ExplorerLocation, MusicianLocation, GroupsLocations, ExclusionLocations };

    private readonly List<string> dialogueToEdit = new() { GeorgeDialogueLocation, EvelynDialogueLocation, SandyDialogueLocation, WillyDialogueLocation };

    /// <inheritdoc />
    public bool CanEdit<T>(IAssetInfo asset)
    {
        return this.dialogueToEdit.Any((string assetpath) => asset.AssetNameEquals(assetpath));
    }

    /// <inheritdoc />
    public void Edit<T>(IAssetData asset)
    {
        IAssetDataForDictionary<string, string>? editor = asset.AsDictionary<string, string>();
        if (asset.AssetNameEquals(GeorgeDialogueLocation))
        {
            editor.Data["Resort"] = I18n.GeorgeResort();
        }
        else if (asset.AssetNameEquals(EvelynDialogueLocation))
        {
            editor.Data["Resort"] = I18n.EvelynResort();
        }
        else if (asset.AssetNameEquals(WillyDialogueLocation))
        {
            editor.Data["Resort"] = I18n.WillyResort();
        }
        else if (asset.AssetNameEquals(SandyDialogueLocation))
        {
            foreach (string key in new string[] { "Resort", "Resort_Bar", "Resort_Bar_2", "Resort_Wander", "Resort_Shore", "Resort_Pier", "Resort_Approach", "Resort_Left" })
            {
                editor.Data[key] = I18n.GetByKey("Sandy_" + key);
            }
        }
    }

    /// <inheritdoc />
    public bool CanLoad<T>(IAssetInfo asset)
    {
        return this.myAssets.Any((string assetpath) => asset.AssetNameEquals(assetpath));
    }

    /// <inheritdoc />
    public T Load<T>(IAssetInfo asset)
    {
        // default vanilla groupings
        if (asset.AssetNameEquals(GroupsLocations))
        {
            Dictionary<string, string> defaultgroups = Globals.ContentHelper.Load<Dictionary<string, string>>("assets/defaultGroupings.json", ContentSource.ModFolder);
            if (Game1.year > 2 && defaultgroups.ContainsKey("JodiFamily"))
            {
                    Globals.ModMonitor.DebugLog($"Kent is home, adding Kent");
                    defaultgroups["JodiFamily"] += ", Kent";
            }
            if (Globals.Config.AllowGeorgeAndEvelyn)
            {
                defaultgroups["GeorgeFamily"] = "George, Evelyn, Alex";
            }
            if (Globals.Config.AllowWilly)
            {
                defaultgroups["barfolk"] = "Clint, Willy";
            }
            return (T)(object)defaultgroups;
        }
        // Load an empty document for everything else
        else if (this.myAssets.Any((string assetpath) => asset.AssetNameEquals(assetpath)))
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
        Globals.ContentHelper.InvalidateCache(assetLocation);
        foreach (string? specialChar in Globals.ContentHelper.Load<Dictionary<string, string>>(assetLocation, ContentSource.GameContent).Keys)
        {
            if (specialChar is null)
            {
                continue;
            }
            NPC? npc = Game1.getCharacterFromName(specialChar);
            if (npc is not null)
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
        Globals.ContentHelper.InvalidateCache(assetLocation);
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
                NPC? npc = Game1.getCharacterFromName(charname);
                if (npc is not null)
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
                if (npc?.getSpouse() is not null)
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
        Globals.ContentHelper.InvalidateCache(ExclusionLocations);
        Dictionary<string, string> data = Globals.ContentHelper.Load<Dictionary<string, string>>(ExclusionLocations, ContentSource.GameContent);
        foreach (string npcname in data.Keys)
        {
            NPC npc = Game1.getCharacterFromName(npcname);
            if (npc is not null)
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

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using GenericModConfigMenu.Framework.ModOption;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System.Text;

namespace RadialMenu.Gmcm;

internal record GenericModConfigKeybindOption(
    IManifest ModManifest,
    string FieldId,
    Func<string> GetPageTitle,
    Func<string> GetSectionTitle,
    Func<string> GetFieldName,
    Func<string> GetTooltip,
    Func<IEnumerable<SButton>> GetCurrentBinding)
{
    // Used for GMCM; not displayed in UI.
    public string UniqueDisplayName => $"{ModManifest.Name}: {UniqueFieldName}";

    public string UniqueFieldName
    {
        get
        {
            var sb = new StringBuilder();
            var pageTitle = GetPageTitle();
            if (!string.IsNullOrWhiteSpace(pageTitle))
            {
                sb.Append(pageTitle).Append(" > ");
            }
            var sectionTitle = GetSectionTitle();
            if (!string.IsNullOrWhiteSpace(sectionTitle))
            {
                sb.Append(sectionTitle).Append(" > ");
            }
            return sb.Append(GetFieldName()).ToString();
        }
    }

    public bool MatchesBinding(IEnumerable<SButton> otherBinding)
    {
        return GetCurrentBinding().SequenceEqual(otherBinding);
    }

    public bool MatchesBinding(Keybind? otherBinding)
    {
        return otherBinding is not null && MatchesBinding(otherBinding.Buttons);
    }
}

internal class GenericModConfigKeybindings
{
    public static GenericModConfigKeybindings Load()
    {
        var allOptions = new List<GenericModConfigKeybindOption>();
        foreach (var modConfig in GenericModConfigMenu.Mod.instance.ConfigManager.GetAll())
        {
            foreach (var page in modConfig.Pages.Values)
            {
                Func<string> getSectionTitle = () => "";
                foreach (var option in page.Options)
                {
                    if (option is SectionTitleModOption sectionTitle)
                    {
                        getSectionTitle = sectionTitle.Name;
                    }
                    Func<IEnumerable<SButton>>? getValue = option switch
                    {
                        SimpleModOption<SButton> buttonOption => () => [buttonOption.GetValue()],
                        SimpleModOption<KeybindList> keybindListOption => () => keybindListOption
                            .GetValue()?
                            .Keybinds
                            .Where(kb => kb.IsBound)
                            .FirstOrDefault()?
                            .Buttons ?? [],
                        _ => null
                    };
                    if (getValue is not null)
                    {
                        allOptions.Add(new(
                            modConfig.ModManifest,
                            option.FieldId,
                            page.PageTitle,
                            getSectionTitle,
                            option.Name,
                            option.Tooltip,
                            getValue));
                    }
                }
            }
        }
        return new(allOptions);
    }

    public IReadOnlyDictionary<string, IManifest> AllMods { get; }
    public IReadOnlyList<GenericModConfigKeybindOption> AllOptions { get; }

    private readonly ILookup<(string, string), GenericModConfigKeybindOption>
        optionsByModAndFieldName =
            EmptyLookup<(string, string), GenericModConfigKeybindOption>();
    private readonly Dictionary<(string, string), GenericModConfigKeybindOption>
        optionsByModAndFieldId = [];

    private GenericModConfigKeybindings(IReadOnlyList<GenericModConfigKeybindOption> allOptions)
    {
        AllOptions = allOptions;
        AllMods = allOptions
            .Select(opt => opt.ModManifest)
            .DistinctBy(mod => mod.UniqueID)
            .OrderBy(mod => mod.Name)
            .ToDictionary(mod => mod.UniqueID);
        optionsByModAndFieldId = allOptions.ToDictionary(
            opt => (opt.ModManifest.UniqueID, opt.FieldId));
        optionsByModAndFieldName = allOptions.ToLookup(
            opt => (opt.ModManifest.UniqueID, opt.UniqueFieldName));
    }

    public GenericModConfigKeybindOption? Find(
        string modId, string fieldId, string fieldName = "", Keybind? previousBinding = null)
    {
        var nameMatches = optionsByModAndFieldName[(modId, fieldName)];
        GenericModConfigKeybindOption? bestNameMatch = null;
        if (!string.IsNullOrEmpty(fieldName))
        {
            foreach (var nameMatch in nameMatches)
            {
                if (bestNameMatch is null)
                {
                    bestNameMatch = nameMatch;
                }
                else if (nameMatch.FieldId == fieldId
                    || !bestNameMatch.MatchesBinding(previousBinding)
                        && nameMatch.MatchesBinding(previousBinding))
                {
                    bestNameMatch = nameMatch;
                    break;
                }
            }
        }
        return bestNameMatch
            ?? optionsByModAndFieldId.GetValueOrDefault((modId, fieldId))
            // Falling back to exclusively binding-based matching should be unusual, but in the
            // event that it does become necessary, we have to remember that keybindings can be
            // changed at any point while the game is running, so unlike the field ID/name
            // lookups, we don't want to aggressively cache these, otherwise we need a system
            // for keeping the dictionary keys up to date.
            //
            // Since this is pretty rare, and since lookups shouldn't happen that often, and
            // even in a "heavy" loadout there should be a few dozen or maybe a few hundred mods
            // registered, we can just suffer the O(N) search in those instances. This is only
            // really intended to handle the case where a mod author has changed the names
            // around, and once the "link" is "restored", we won't continue to hit this.
            //
            // Of course, if a match is impossible and the link is never re-established, then we
            // will in fact keep hitting this, but at some point we have to trust the user to
            // look at the warnings in the SMAPI console.
            ?? AllOptions.FirstOrDefault(opt =>
                opt.ModManifest.UniqueID == modId
                && opt.MatchesBinding(previousBinding));
    }

    private static ILookup<TKey, TElement> EmptyLookup<TKey, TElement>()
    {
        return Enumerable.Empty<TKey>().ToLookup(key => key, _ => default(TElement)!);
    }
}

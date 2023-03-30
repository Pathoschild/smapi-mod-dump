/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Extensions;

using AtraCore.Framework.Caches;

using AtraShared.Caching;
using AtraShared.Utils.Extensions;

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Locations;

namespace GingerIslandMainlandAdjustments.AssetManagers;

/// <summary>
/// Manages asset editing for this mod.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Reviewed.")]
internal static class AssetEditor
{
    /// <summary>
    /// Pam's mail key.
    /// </summary>
    internal const string PAMMAILKEY = "atravita_GingerIslandMainlandAdjustments_PamMail";

    /// <summary>
    /// The integer key of Pam's heart event.
    /// </summary>
    internal const int PAMEVENT = 99219999;

    private static readonly PerScreen<TickCache<bool>> HasSeenNineHeart = new(
    static () => new(() => Game1.player?.eventsSeen?.Contains(503180) == true));

    private static readonly PerScreen<TickCache<bool>> HasSeenPamEvent = new(
        static () => new(() => Game1.player?.eventsSeen?.Contains(PAMEVENT) == true));

    /// <summary>
    /// The dialogue prefix.
    /// </summary>
    internal static readonly string Dialogue = PathUtilities.NormalizeAssetName("Characters/Dialogue") + "/";

    private static readonly Dictionary<string, Action<IAssetData>> DialoguesToEdit = new(comparer: StringComparer.OrdinalIgnoreCase)
    {
        ["George"] = EditGeorgeDialogue,
        ["Evelyn"] = EditEvelynDialogue,
        ["Sandy"] = EditSandyDialogue,
        ["Willy"] = EditWillyDialogue,
        ["Wizard"] = EditWizardDialogue,
    };

    /// <summary>
    /// Gets the NPCs who have dialogue we should edit.
    /// </summary>
    internal static IEnumerable<string> CharacterDialogues => DialoguesToEdit.Keys;

    // We edit Pam's phone dialogue into Strings/Characters so content packs can target that.
    private static IAssetName phoneStringLocation = null!;

    // A ten heart event and letter are included to unlock the phone. Edit late - I don't really want CP packs changing this.
    private static IAssetName dataEventsSeedshop = null!;
    private static IAssetName dataMail = null!;

    // We edit Pam's nine heart event to set flags to remember which path the player chose.
    // This currently isn't used for anything.
    private static IAssetName dataEventsTrailerBig = null!;

    // This stashes our LocalizedContentManager, should we need to restore a schedule
    private static LocalizedContentManager? contentManager;

    /// <summary>
    /// Initializes the AssetEditor.
    /// </summary>
    /// <param name="parser">GameContentHelper.</param>
    internal static void Initialize(IGameContentHelper parser)
    {
        // phone
        phoneStringLocation = parser.ParseAssetName("Strings/Characters");

        // events
        dataEventsSeedshop = parser.ParseAssetName("Data/Events/SeedShop");
        dataMail = parser.ParseAssetName("Data/mail");
        dataEventsTrailerBig = parser.ParseAssetName("Data/Events/Trailer_Big");
    }

    /// <summary>
    /// Disposes the content manager. Called when our mod instance is disposed.
    /// </summary>
    internal static void DisposeContentManager()
    {
        contentManager?.Dispose();
        contentManager = null;
    }

    /// <summary>
    /// Handles editing assets for this mod.
    /// </summary>
    /// <param name="e">Asset event arguments.</param>
    internal static void Edit(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(phoneStringLocation))
        {
            e.Edit(EditPhone, AssetEditPriority.Early);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(dataMail))
        {
            e.Edit(EditMail, AssetEditPriority.Late);
        }
        else if (HasSeenNineHeart.Value.GetValue() && !HasSeenPamEvent.Value.GetValue() && e.NameWithoutLocale.IsEquivalentTo(dataEventsSeedshop))
        {
            e.Edit(EditSeedShopEvent, AssetEditPriority.Late);
        }
        else if (!HasSeenNineHeart.Value.GetValue() && e.NameWithoutLocale.IsEquivalentTo(dataEventsTrailerBig))
        {
            e.Edit(EditTrailerBig, AssetEditPriority.Late);
        }
        else if (e.NameWithoutLocale.StartsWith("Characters/schedules/", false, false))
        {
            e.Edit(CheckSpringSchedule, AssetEditPriority.Late + 100);
        }
        else if (e.NameWithoutLocale.StartsWith(Dialogue, false, false)
            && Game1.getLocationFromName("IslandSouth") is IslandSouth island && island.resortRestored.Value)
        {
            string npcName = e.NameWithoutLocale.BaseName.GetNthChunk('/', 2).ToString();
            if (DialoguesToEdit.TryGetValue(npcName, out Action<IAssetData>? editor))
            {
                e.Edit(editor, AssetEditPriority.Early);
            }
        }
    }

    private static void CheckSpringSchedule(IAssetData e)
    {
        // SVE removes Sandy's spring schedule for some reason, this can cause issues if she goes to the resort.
        Globals.ModMonitor.DebugOnlyLog($"Checking schedule {e.NameWithoutLocale}", LogLevel.Info);

        IAssetDataForDictionary<string, string> editor = e.AsDictionary<string, string>();
        if (!editor.Data.ContainsKey("spring") && !editor.Data.ContainsKey("default"))
        {
            string character = e.NameWithoutLocale.BaseName.GetNthChunk('/', 2).ToString();
            Globals.ModMonitor.Log($"Found NPC {character} without either a spring or default schedule. This may cause issues.", LogLevel.Info);
            contentManager ??= new(Game1.content.ServiceProvider, Game1.content.RootDirectory);
            try
            {
                Dictionary<string, string> original = contentManager.LoadBase<Dictionary<string, string>>(e.NameWithoutLocale.BaseName);
                if (original.TryGetValue("spring", out string? data))
                {
                    Globals.ModMonitor.Log($"Original spring schedule found: {data}. Adding back", LogLevel.Info);
                    editor.Data["spring"] = data;
                    return;
                }
                else if (original.TryGetValue("default", out string? @default))
                {
                    Globals.ModMonitor.Log($"Original default schedule found: {@default}. Adding back", LogLevel.Info);
                    editor.Data["default"] = @default;
                    return;
                }
            }
            catch (Exception ex)
            {
                Globals.ModMonitor.Log($"Could not find original schedule for {character}:\n\n{ex}");
            }
            Globals.ModMonitor.Log($"Could not restore spring schedule for {character}.", LogLevel.Info);
        }
    }

    private static void EditGeorgeDialogue(IAssetData e)
    {
        IAssetDataForDictionary<string, string>? editor = e.AsDictionary<string, string>();
        editor.Data["Resort"] = I18n.GeorgeResort();
        editor.Data["Resort_IslandNorth"] = I18n.GeorgeResortIslandNorth();
    }

    private static void EditEvelynDialogue(IAssetData e)
    {
        IAssetDataForDictionary<string, string>? editor = e.AsDictionary<string, string>();
        editor.Data["Resort"] = I18n.EvelynResort();
        editor.Data["Resort_IslandNorth"] = I18n.EvelynResortIslandNorth();
    }

    private static void EditWillyDialogue(IAssetData e)
    {
        IAssetDataForDictionary<string, string>? editor = e.AsDictionary<string, string>();
        editor.Data["Resort"] = I18n.WillyResort();
        editor.Data["Resort_IslandNorth"] = I18n.WillyResortIslandNorth();
    }

    private static void EditSandyDialogue(IAssetData e)
    {
        IAssetDataForDictionary<string, string>? editor = e.AsDictionary<string, string>();
        foreach (string key in new string[] { "Resort", "Resort_Bar", "Resort_Bar_2", "Resort_Wander", "Resort_Shore", "Resort_Pier", "Resort_Approach", "Resort_Left", "Resort_IslandNorth" })
        {
            editor.Data[key] = I18n.GetByKey("Sandy_" + key);
        }
    }

    private static void EditWizardDialogue(IAssetData e)
    {
        IAssetDataForDictionary<string, string>? editor = e.AsDictionary<string, string>();
        editor.Data["Resort"] = I18n.WizardResort();
    }

    private static void EditPhone(IAssetData e)
    {
        IAssetDataForDictionary<string, string>? editor = e.AsDictionary<string, string>();
        foreach (string key in new string[] { "Pam_Island_1", "Pam_Island_2", "Pam_Island_3", "Pam_Doctor", "Pam_Other", "Pam_Bus_1", "Pam_Bus_2", "Pam_Bus_3", "Pam_Voicemail_Island", "Pam_Voicemail_Doctor", "Pam_Voicemail_Other", "Pam_Voicemail_Bus", "Pam_Bus_Late" })
        {
            editor.Data[key] = I18n.GetByKey(key);
        }
    }

    private static void EditMail(IAssetData e)
    {
        IAssetDataForDictionary<string, string>? editor = e.AsDictionary<string, string>();
        editor.Data[PAMMAILKEY] = $"{I18n.Pam_Mail_Text()}^^   --{NPCCache.GetByVillagerName("Pam")?.displayName ?? I18n.Pam()}[#]{I18n.Pam_Mail_Title()}";
    }

    private static void EditSeedShopEvent(IAssetData e)
    {
        IAssetDataForDictionary<string, string>? editor = e.AsDictionary<string, string>();
        editor.Data[$"{PAMEVENT}/e 503180/f Pam 2500/v Pam/w rainy/t 1700 2600"] = string.Join(
            separator: string.Empty,
            "sadpiano/-1000 -1000/farmer 35 21 0 Pam 37 18 0/ignoreCollisions farmer/",
            "ignoreCollisions Pam/viewport 37 21 true/move farmer 0 -3 1/faceDirection Pam 3/",
            $"speak Pam \"{I18n._999Pam01a()}\"/faceDirection Pam 0/pause 250/faceDirection Pam 3/",
            $"speak Pam \"{I18n._999Pam01b()}#$b#{I18n._999Pam01c()}\"/pause 500/",
            $"question fork1 \"{I18n._999PamAsk()}#{I18n._999Validate()}#{I18n._999Confront()}\"/",
            $"fork atravita_GIMA_PamInsulted/mail {PAMMAILKEY}/",
            "emote Pam 20/friendship Pam 200/faceDirection Pam 0/pause 250/faceDirection Pam 3/",
            $"speak Pam \"{I18n._999Pam02()}\"/pause 500/faceDirection Pam 0/",
            $"speak Pam \"{I18n._999Pam03()}$s\"/pause 500/faceDirection Pam 3/",
            $"speak Pam \"{I18n._999Pam04()}$s#$b#{I18n._999Pam05()}$u\"/pause 500/faceDirection Pam 2/",
            $"pause 500/faceDirection Pam 3/textAboveHead Pam \"{I18n.Sigh()}\"/speak Pam \"{I18n._999Pam06()}\"/pause 500/",
            $"speak Pam \"{I18n._999Pam07()}\"/pause 500/textAboveHead Pam \"{I18n.Sigh()}\"/pause 1000/",
            $"speak Pam \"{I18n._999Pam08()}\"/pause 1000/fade/viewport -100 -100/end dialogue Pam \"{I18n._999Pam30()}\"");
        editor.Data["atravita_GIMA_PamInsulted"] = $"friendship Pam -250/emote Pam 12/speak Pam \"{I18n._999Pam99()}\"/fade/viewport -100 -100/end invisible Pam";
    }

    private static void EditTrailerBig(IAssetData e)
    {
        // Insert mail flags into the vanilla event
        IAssetDataForDictionary<string, string>? editor = e.AsDictionary<string, string>();
        if (editor.Data.TryGetValue("positive", out string? val))
        {
            editor.Data["positive"] = "addMailReceived atravita_GIMA_PamPositive/" + val;
        }
        foreach (string key in editor.Data.Keys)
        {
            if (key.StartsWith("503180/") && editor.Data[key] is string value)
            {
                int lastslash = value.LastIndexOf('/');
                if (lastslash > 0)
                {
                    editor.Data[key] = value.Insert(lastslash, "/addMailReceived atravita_GIMA_PamInsulted");
                }
                break;
            }
        }
    }
}
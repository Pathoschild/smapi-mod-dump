/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/Ginger-Island-Mainland-Adjustments
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace GingerIslandMainlandAdjustments.AssetManagers;

/// <summary>
/// Manages asset editing for this mod.
/// </summary>
public sealed class AssetEditor : IAssetEditor
{
    /// <summary>
    /// Pam's mail key.
    /// </summary>
    public const string PAMMAILKEY = "atravita_GingerIslandMainlandAdjustments_PamMail";

    // The following dialogue is edited from the code side so each NPC has at least the Resort dialogue.
    // A CP pack will override, since my asset managers are registered in Entry and CP registers in GameLaunched.
    private static readonly string GeorgeDialogueLocation = PathUtilities.NormalizeAssetName("Characters/Dialogue/George");
    private static readonly string EvelynDialogueLocation = PathUtilities.NormalizeAssetName("Characters/Dialogue/Evelyn");
    private static readonly string SandyDialogueLocation = PathUtilities.NormalizeAssetName("Characters/Dialogue/Sandy");
    private static readonly string WillyDialogueLocation = PathUtilities.NormalizeAssetName("Characters/Dialogue/Willy");

    // We edit Pam's phone dialogue into Strings/Characters so content packs can target that.
    private static readonly string PhoneStringLocation = PathUtilities.NormalizeAssetName("Strings/Characters");

    // A ten heart event and letter are included to unlock the phone.
    private static readonly string DataEventsSeedshop = PathUtilities.NormalizeAssetName("Data/Events/SeedShop");
    private static readonly string DataMail = PathUtilities.NormalizeAssetName("Data/mail");

    private static readonly string[] FilesToEdit = new string[]
    {
        GeorgeDialogueLocation,
        EvelynDialogueLocation,
        SandyDialogueLocation,
        WillyDialogueLocation,
        PhoneStringLocation,
        DataEventsSeedshop,
        DataMail,
    };

    private AssetEditor()
    {
    }

    /// <summary>
    /// Gets the instance of the AssetEditor.
    /// </summary>
    public static AssetEditor Instance { get; } = new();

    /// <inheritdoc />
    [UsedImplicitly]
    public bool CanEdit<T>(IAssetInfo asset)
        => FilesToEdit.Any((string assetpath) => asset.AssetNameEquals(assetpath));

    /// <inheritdoc />
    [UsedImplicitly]
    public void Edit<T>(IAssetData asset)
    {
        IAssetDataForDictionary<string, string> editor = asset.AsDictionary<string, string>();
        if (asset.AssetNameEquals(GeorgeDialogueLocation))
        {
            editor.Data["Resort"] = I18n.GeorgeResort();
            editor.Data["Resort_IslandNorth"] = I18n.GeorgeResortIslandNorth();
        }
        else if (asset.AssetNameEquals(EvelynDialogueLocation))
        {
            editor.Data["Resort"] = I18n.EvelynResort();
            editor.Data["Resort_IslandNorth"] = I18n.EvelynResortIslandNorth();
        }
        else if (asset.AssetNameEquals(WillyDialogueLocation))
        {
            editor.Data["Resort"] = I18n.WillyResort();
            editor.Data["Resort_IslandNorth"] = I18n.WillyResortIslandNorth();
        }
        else if (asset.AssetNameEquals(SandyDialogueLocation))
        {
            foreach (string key in new string[] { "Resort", "Resort_Bar", "Resort_Bar_2", "Resort_Wander", "Resort_Shore", "Resort_Pier", "Resort_Approach", "Resort_Left", "Resort_IslandNorth" })
            {
                editor.Data[key] = I18n.GetByKey("Sandy_" + key);
            }
        }
        else if (asset.AssetNameEquals(PhoneStringLocation))
        {
            foreach (string key in new string[] { "Pam_Island_1", "Pam_Island_2", "Pam_Island_3", "Pam_Doctor", "Pam_Other", "Pam_Bus_1", "Pam_Bus_2", "Pam_Bus_3", "Pam_Voicemail_Island", "Pam_Voicemail_Doctor", "Pam_Voicemail_Other", "Pam_Voicemail_Bus", "Pam_Bus_Late" })
            {
                editor.Data[key] = I18n.GetByKey(key);
            }
        }
        else if (asset.AssetNameEquals(DataMail))
        {
            editor.Data[PAMMAILKEY] = $"{I18n.Pam_Mail_Text()}^^   --{Game1.getCharacterFromName("Pam")?.displayName ?? I18n.Pam()}[#]{I18n.Pam_Mail_Title()}";
        }
        else if (asset.AssetNameEquals(DataEventsSeedshop))
        {
            editor.Data["99219999/e 503180/f Pam 2500/v Pam/w rainy/t 1700 2600"] = string.Join(
                separator: string.Empty,
                "sadpiano/-1000 -1000/farmer 35 21 0 Pam 37 18 0/ignoreCollisions farmer/",
                "ignoreCollisions Pam/viewport 37 21 true/move farmer 0 -3 1/faceDirection Pam 3/",
                $"speak Pam \"{I18n._999Pam01a()}\"/faceDirection Pam 0/pause 250/faceDirection Pam 3/",
                $"speak Pam \"{I18n._999Pam01b()}#$b#{I18n._999Pam01c()}\"/pause 500/",
                $"question fork1 \"{I18n._999PamAsk()}#{I18n._999Validate()}#{I18n._999Confront()}\"/",
                "fork atravita_GIMA_PamInsulted/mail atravita_GingerIslandMainlandAdjustments_PamMail/",
                "emote Pam 20/friendship Pam 200/faceDirection Pam 0/pause 250/faceDirection Pam 3/",
                $"speak Pam \"{I18n._999Pam02()}\"/pause 500/faceDirection Pam 0/",
                $"speak Pam \"{I18n._999Pam03()}$s\"/pause 500/faceDirection Pam 3/",
                $"speak Pam \"{I18n._999Pam04()}$s#$b#{I18n._999Pam05()}$u\"/pause 500/faceDirection Pam 2/",
                $"pause 500/faceDirection Pam 3/textAboveHead Pam \"{I18n.Sigh()}\"/speak Pam \"{I18n._999Pam06()}\"/pause 500/",
                $"speak Pam \"{I18n._999Pam07()}\"/pause 500/textAboveHead Pam \"{I18n.Sigh()}\"/pause 1000/",
                $"speak Pam \"{I18n._999Pam08()}\"/pause 1000/fade/viewport -100 -100/end dialogue Pam \"{I18n._999Pam30()}\"");
            editor.Data["atravita_GIMA_PamInsulted"] = $"friendship Pam -250/emote Pam 12/speak Pam \"{I18n._999Pam99()}\"/fade/viewport -100 -100/end invisible Pam";
        }
    }
}
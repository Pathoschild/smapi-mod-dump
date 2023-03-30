/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Combat.Events;

#region using directives

using DaLion.Shared.Content;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class CombatAssetRequestedEvent : AssetRequestedEvent
{
    /// <summary>Initializes a new instance of the <see cref="CombatAssetRequestedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal CombatAssetRequestedEvent(EventManager manager)
        : base(manager)
    {
        this.Edit("Strings/StringsFromCSFiles", new AssetEditor(EditStringsFromCsFiles, AssetEditPriority.Late));
    }

    #region editor callbacks

    /// <summary>Adjust Jinxed debuff description.</summary>
    private static void EditStringsFromCsFiles(IAssetData asset)
    {
        if (!CombatModule.Config.OverhauledDefense)
        {
            return;
        }

        var data = asset.AsDictionary<string, string>().Data;
        data["Buff.cs.465"] = I18n.Get("ui.buffs.jinxed");
    }

    #endregion editor callbacks
}

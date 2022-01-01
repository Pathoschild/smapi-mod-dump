/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.AssetEditors;

public class AchivementsEditor : IAssetEditor
{
    /// <inheritdoc />
    public bool CanEdit<T>(IAssetInfo asset)
    {
        return asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/achievements"));
    }

    /// <inheritdoc />
    public void Edit<T>(IAssetData asset)
    {
        if (!asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/achievements")))
            throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");

        // patch custom prestige achievements
        var data = asset.AsDictionary<int, string>().Data;

        string name =
            ModEntry.ModHelper.Translation.Get("prestige.achievement.name." +
                                               (Game1.player.IsMale ? "male" : "female"));
        var desc = ModEntry.ModHelper.Translation.Get("prestige.achievement.desc");
        
        const string SHOULD_DISPLAY_BEFORE_EARNED_S = "false";
        const string PREREQUISITE_S = "-1";
        const string HAT_INDEX_S = "";

        var newEntry = string.Join("^", name, desc, SHOULD_DISPLAY_BEFORE_EARNED_S, PREREQUISITE_S, HAT_INDEX_S);
        data[name.GetDeterministicHashCode()] = newEntry;
    }
}
/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Framework;

#region using directives

using System;
using System.Globalization;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

#endregion using directives

public class MailEditor : IAssetEditor
{
    /// <inheritdoc />
    public bool CanEdit<T>(IAssetInfo asset)
    {
        return asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/mail"));
    }

    /// <inheritdoc />
    public void Edit<T>(IAssetData asset)
    {
        if (asset.AssetNameEquals(PathUtilities.NormalizeAssetName("Data/mail")))
        {
            if (!ModEntry.Config.ImmersiveGalaxyWeaponConditions) return;

            var data = asset.AsDictionary<string, string>().Data;
            var mail = data["QiChallengeComplete"];
            var tokens = mail.Split('%');
            tokens[1] = "item object 896 1 ";
            mail = string.Join('%', tokens);
            data["QiChallengeComplete"] = mail;
        }
        else
        {
            throw new InvalidOperationException($"Unexpected asset {asset.AssetName}.");
        }
    }
}
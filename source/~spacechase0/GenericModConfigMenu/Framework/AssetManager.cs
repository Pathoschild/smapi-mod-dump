/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

#nullable enable

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace GenericModConfigMenu.Framework;
internal static class AssetManager
{
    private const string BasePath = "Mods/GenericModConfigMenu";
    internal static string ConfigButton { get; } = PathUtilities.NormalizeAssetName(BasePath + "/ConfigButton");
    internal static string KeyboardButton { get; } = PathUtilities.NormalizeAssetName(BasePath + "/KeyboardButton");

    internal static void Apply(AssetRequestedEventArgs e)
    {
        if (e.Name.IsEquivalentTo(ConfigButton))
            e.LoadFromModFile<Texture2D>("assets/config-button.png", AssetLoadPriority.Exclusive);
        else if (e.Name.IsEquivalentTo(KeyboardButton))
            e.LoadFromModFile<Texture2D>("assets/keybindings-button.png", AssetLoadPriority.Exclusive);
    }
}

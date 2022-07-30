/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

#nullable disable

namespace StardewMods.MoreChests;

using Common.Helpers;
using StardewModdingAPI;
using StardewMods.FuryCore.Services;
using StardewMods.MoreChests.Services;

/// <inheritdoc />
public class MoreChests : Mod
{
    /// <summary>
    /// Gets the unique Mod Id.
    /// </summary>
    internal static string ModUniqueId { get; private set; }

    private ModServices Services { get; } = new();

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        MoreChests.ModUniqueId = this.ModManifest.UniqueID;
        Log.Monitor = this.Monitor;

        // Services
        this.Services.Add(
            new AssetHandler(this.Services));
    }

    /// <inheritdoc />
    public override object GetApi()
    {
        return new MoreChestsApi(this.Services);
    }
}
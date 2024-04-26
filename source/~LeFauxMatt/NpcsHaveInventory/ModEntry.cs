/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.NPCsHaveInventory;

using StardewModdingAPI.Events;

/// <inheritdoc />
public sealed class ModEntry : Mod
{
    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        _ = new ModPatches(this.ModManifest);

        // Events
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e) =>
        Utility.ForEachVillager(
            villager =>
            {
                Game1.player.team.GetOrCreateGlobalInventory($"{this.ModManifest.UniqueID}-{villager.Name}");
                return true;
            });
}
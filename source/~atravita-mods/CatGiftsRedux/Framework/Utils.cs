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

using AtraCore.Framework.ItemManagement;
using AtraCore.Framework.QueuePlayerAlert;

using AtraShared.Caching;
using AtraShared.Utils.Extensions;
using AtraShared.Wrappers;

using Microsoft.Xna.Framework;

using StardewValley.Characters;
using StardewValley.Objects;

namespace CatGiftsRedux.Framework;

/// <summary>
/// A utility class for this mod.
/// </summary>
internal static class Utils
{
    private static readonly TickCache<bool> isQiQuestActive = new(() => Game1.player.team.SpecialOrderRuleActive("QI_BEANS"));
    private static readonly TickCache<bool> isPerfectFarm = new(() => Game1.MasterPlayer.mailReceived.Contains("Farm_Enternal"));

    /// <summary>
    /// Gets a value indicating whether Qi's bean quest is active. Only checks once per four ticks.
    /// </summary>
    internal static bool IsQiQuestActive => isQiQuestActive.GetValue();

    /// <summary>
    /// Check if the object should not be given by a random picker. Basically, no golden walnuts (73), qi gems (858), Qi beans or fruit unless the special order is active.
    /// 289 = ostrich egg, 928 is a golden egg.
    /// Or something that doesn't exist in Data/ObjectInformation.
    /// </summary>
    /// <param name="id">int id of the item to check.</param>
    /// <returns>true to forbid it.</returns>
    internal static bool ForbiddenFromRandomPicking(int id)
        => !Game1Wrappers.ObjectInfo.TryGetValue(id, out string? objectData) || id == 73 || id == 858
        || (id is 289 or 928 && !isPerfectFarm.GetValue())
        || (!isQiQuestActive.GetValue() && objectData.GetNthChunk('/', SObject.objectInfoNameIndex).Contains("Qi", StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Gets a random empty tile on a map.
    /// </summary>
    /// <param name="location">The game location to get a random tile from.</param>
    /// <param name="tries">How many times to try.</param>
    /// <returns>Empty tile, or null to indicate failure.</returns>
    internal static Vector2? GetRandomTileImpl(this GameLocation location, int tries = 10)
    {
        do
        {
            Vector2 tile = location.getRandomTile();
            if (location.isWaterTile((int)tile.X, (int)tile.Y))
            {
                continue;
            }

            List<Vector2>? options = Utility.recursiveFindOpenTiles(location, tile, 1);
            if (options.Count > 0)
            {
                return options[0];
            }
        }
        while (tries-- > 0);

        return null;
    }

    /// <summary>
    /// Places the item at the specified tile, and alerts the player.
    /// </summary>
    /// <param name="location">Map.</param>
    /// <param name="tile">Tile to attempt.</param>
    /// <param name="item">Item to place.</param>
    /// <param name="pet">Pet to credit.</param>
    internal static void PlaceItem(this GameLocation location, Vector2 tile, Item item, Pet pet)
    {
        ModEntry.ModMonitor.DebugOnlyLog($"Placing {item.DisplayName} at {location.NameOrUniqueName} - {tile}");

        PlayerAlertHandler.AddMessage(
            message: new PetHudMessage(I18n.PetMessage(pet.Name, item.DisplayName), Color.PaleGreen, 2000, true, item, pet is Cat),
            soundCue: pet is Cat ? "Cowboy_Footstep" : "dog_pant");

        if (item is SObject @object && !@object.bigCraftable.Value
            && DataToItemMap.IsActuallyRing(item.ParentSheetIndex)
            && item.GetType() == typeof(SObject))
        {
            ModEntry.ModMonitor.Log($"Fixing {item.Name} to be an actual ring.");
            item = new Ring(item.ParentSheetIndex);
        }

        if (item.GetType() == typeof(SObject) && !location.Objects.ContainsKey(tile))
        {
            SObject obj = (item as SObject)!;
            if (!obj.bigCraftable.Value)
            {
                obj.IsSpawnedObject = true;
            }

            location.Objects[tile] = obj;

            if (pet is Dog)
            {
                location.makeHoeDirt(tile, ignoreChecks: false);
            }
        }
        else
        {
            Debris? debris = new(item, tile * 64f);
            location.debris.Add(debris);
        }
    }
}
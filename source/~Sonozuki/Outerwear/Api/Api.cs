/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Outerwear.Models.Converted;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System.Linq;

namespace Outerwear
{
    /// <summary>Provides basic outerwear apis.</summary>
    public class Api : IApi
    {
        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public bool IsOuterwear(int objectId) => ModEntry.Instance.OuterwearData.Any(outerwearData => outerwearData.ObjectId == objectId);

        /// <inheritdoc/>
        public OuterwearData GetOuterwearData(int objectId) => ModEntry.Instance.OuterwearData.FirstOrDefault(outerwearData => outerwearData.ObjectId == objectId);

        /// <inheritdoc/>
        public Item GetEquippedOuterwear()
        {
            // ensure context is valid
            if (!Context.IsWorldReady)
                return null;

            var outerwearChest = GetOuterwearChest();
            return outerwearChest.items.Count == 0 ? null : outerwearChest.items[0];
        }

        /// <inheritdoc/>
        public int GetEquippedOuterwearId(Farmer player = null)
        {
            // ensure context is valid
            if (!Context.IsWorldReady)
                return -1;

            player ??= Game1.player;

            // retrieve equipped outerwear
            var equippedOuterwearId = -1;
            if (player.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/equippedOuterwearId", out var equippedOuterwearIdString))
                if (int.TryParse(equippedOuterwearIdString, out var equippedOuterwearIdParsed))
                    equippedOuterwearId = equippedOuterwearIdParsed;

            return equippedOuterwearId;
        }

        /// <inheritdoc/>
        public void EquipOuterwear(Item outerwear)
        {
            // ensure context is valid
            if (!Context.IsWorldReady)
                return;

            // ensure id is valid
            if (outerwear.ParentSheetIndex < 0 || !IsOuterwear(outerwear.ParentSheetIndex))
                return;

            UnequipOuterwear();

            // place outerwear item in the outerwear chest
            var outerwearChest = GetOuterwearChest();
            if (outerwearChest.items.Count == 0)
                outerwearChest.items.Add(outerwear);
            else
                outerwearChest.items[0] = outerwear;
            Game1.player.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/equippedOuterwearId"] = outerwear.ParentSheetIndex.ToString();
        }

        /// <inheritdoc/>
        public void UnequipOuterwear()
        {
            // ensure context is valid
            if (!Context.IsWorldReady)
                return;

            // ensure the player is currently wearing an outerwear
            var equippedOuterwear = GetEquippedOuterwear();
            if (equippedOuterwear == null)
                return;

            // remove the worn outerwear from the player's outerwear chest
            var outerwearChest = GetOuterwearChest();
            if (outerwearChest.items.Count > 0)
                outerwearChest.items[0] = null;
            Game1.player.modData.Remove($"{ModEntry.Instance.ModManifest.UniqueID}/equippedOuterwearId");
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Gets a player's outerwear chest.</summary>
        /// <param name="player">The player whose outerwear chest should be retrieved.</param>
        /// <returns></returns>
        private Chest GetOuterwearChest()
        {
            // try to get an existing outerwear chest linked to the player
            if (Game1.player.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/outerwearChestXPosition", out var outerwearChestXPositionString))
                if (int.TryParse(outerwearChestXPositionString, out var outerwearChestXPositionParsed))
                {
                    var outerwearChestObject = Game1.getFarm().Objects[new Vector2(outerwearChestXPositionParsed, -111)];
                    if (outerwearChestObject is Chest chest)
                    {
                        chest.modData["Pathoschild.ChestsAnywhere/IsIgnored"] = "true";
                        return chest;
                    }
                    else
                        ModEntry.Instance.Monitor.Log("Outerwear chest isn't valid, this may result in losing your equipped outerwear. Outerwear chest will be regenerated.", LogLevel.Error);
                }
                else
                    ModEntry.Instance.Monitor.Log("Failed to location outerwear chest, this may result in losing your equipped outerwear. Outerwear chest will be regenerated.", LogLevel.Error);

            // no valid outerwear chest was linked to player, create a new one
            var outerwearChestXPostition = 0;
            while (Game1.getFarm().Objects.ContainsKey(new Vector2(outerwearChestXPostition, -111)))
                outerwearChestXPostition++;

            var outerwearChest = new Chest(true);
            outerwearChest.modData["Pathoschild.ChestsAnywhere/IsIgnored"] = "true";
            Game1.getFarm().Objects[new Vector2(outerwearChestXPostition, -111)] = outerwearChest;

            Game1.player.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/outerwearChestXPosition"] = outerwearChestXPostition.ToString();
            return outerwearChest;
        }
    }
}

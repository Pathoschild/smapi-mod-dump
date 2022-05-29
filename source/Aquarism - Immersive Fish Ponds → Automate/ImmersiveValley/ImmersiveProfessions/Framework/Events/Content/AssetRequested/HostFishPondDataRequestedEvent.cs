/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Events.Content;

#region using directives

using System.Collections.Generic;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley.GameData.FishPond;

#endregion using directives

[UsedImplicitly]
internal class HostFishPondDataRequestedEvent : AssetRequestedEvent
{
    /// <inheritdoc />
    protected override void OnAssetRequestedImpl(object sender, AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo("Data/FishPondData")) return;

        e.Edit(asset =>
        {
            // patch legendary fish data
            var data = (List<FishPondData>)asset.Data;
            data.InsertRange(data.Count - 2, new List<FishPondData>()
            {
                new() // legendary fish
                {
                    PopulationGates = null,
                    ProducedItems = new()
                    {
                        new()
                        {
                            Chance = 1f,
                            ItemID = 812, // roe
                            MinQuantity = 1,
                            MaxQuantity = 1
                        }
                    },
                    RequiredTags = new() {"fish_legendary"},
                    SpawnTime = int.MaxValue
                }
            });
        });
    }
}
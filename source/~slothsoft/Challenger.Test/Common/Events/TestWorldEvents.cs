/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using System;
using StardewModdingAPI.Events;

namespace ChallengerTest.Common.Events;

public class TestWorldEvents : IWorldEvents {
    public event EventHandler<LocationListChangedEventArgs>? LocationListChanged;
    public event EventHandler<BuildingListChangedEventArgs>? BuildingListChanged;
    public event EventHandler<DebrisListChangedEventArgs>? DebrisListChanged;
    public event EventHandler<LargeTerrainFeatureListChangedEventArgs>? LargeTerrainFeatureListChanged;
    public event EventHandler<NpcListChangedEventArgs>? NpcListChanged;
    public event EventHandler<ObjectListChangedEventArgs>? ObjectListChanged;
    public event EventHandler<ChestInventoryChangedEventArgs>? ChestInventoryChanged;
    public event EventHandler<TerrainFeatureListChangedEventArgs>? TerrainFeatureListChanged;
    public event EventHandler<FurnitureListChangedEventArgs>? FurnitureListChanged;
}
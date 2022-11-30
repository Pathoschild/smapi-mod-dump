/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-informant
**
*************************************************/

using System.Collections.Generic;
using Netcode;
using StardewValley.Network;

namespace StardewTests.Harness.Game; 

public class TestWorldState : IWorldState {
    
    public ServerPrivacy ServerPrivacy { get; set; } = new();
    public WorldDate Date { get; } = new();
    public bool IsTimePaused { get; set; }
    public bool IsPaused { get; set; }
    public bool IsGoblinRemoved { get; set; }
    public bool IsSubmarineLocked { get; set; }
    public int MinesDifficulty { get; set; }
    public int SkullCavesDifficulty { get; set; }
    public int LowestMineLevelForOrder { get; set; }
    public int LowestMineLevel { get; set; }
    public int WeatherForTomorrow { get; set; }
    public Dictionary<string, string> BundleData { get; } = new();
    public NetBundles Bundles { get; } = new();
    public NetIntDictionary<bool, NetBool> BundleRewards { get; } = new();
    public NetVector2Dictionary<int, NetInt> MuseumPieces { get; } = new();
    public NetIntDelta LostBooksFound { get; } = new();
    public NetIntDelta GoldenWalnuts { get; } = new();
    public NetIntDelta GoldenWalnutsFound { get; } = new();
    public NetIntDelta MiniShippingBinsObtained { get; } = new();
    public NetBool GoldenCoconutCracked { get; } = new();
    public NetBool ParrotPlatformsUnlocked { get; } = new();
    public NetStringDictionary<bool, NetBool> FoundBuriedNuts { get; } = new();
    public NetStringDictionary<bool, NetBool> IslandVisitors { get; } = new();
    public NetIntDictionary<LocationWeather, NetRef<LocationWeather>> LocationWeather { get; } = new();
    public int VisitsUntilY1Guarantee { get; set; } = new();
    public Game1.MineChestType ShuffleMineChests { get; set; } = new();
    public NetInt HighestPlayerLimit { get; } = new();
    public NetInt CurrentPlayerLimit { get; } = new();
    public NetRef<SObject> DishOfTheDay { get; } = new();

    public NetFields NetFields { get; } = new();
    
    public void RegisterSpecialCurrencies() {
    }

    public LocationWeather GetWeatherForLocation(GameLocation.LocationContext location_context) {
        return null;
    }

    public Dictionary<string, string> GetUnlocalizedBundleData() {
        return BundleData;
    }

    public void SetBundleData(Dictionary<string, string> data) {
    }

    public bool hasWorldStateID(string id) {
        return false;
    }

    public void addWorldStateID(string id) {
    }

    public void removeWorldStateID(string id) {
    }

    public void UpdateFromGame1() {
    }

    public void WriteToGame1() {
    }

}
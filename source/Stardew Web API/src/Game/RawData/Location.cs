/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.GameData;
using StardewValley.GameData.Locations;
using StardewValley.TokenizableStrings;

namespace StardewWebApi.Game.RawData;

public class Location
{
    private readonly LocationData _locationData;

    public Location(string name, LocationData locationData)
    {
        Name = name;
        _locationData = locationData;

        ParsedDisplayName = TokenParser.ParseText(DisplayName);
    }

    public string Name { get; }
    public string DisplayName => _locationData.DisplayName;
    public string? ParsedDisplayName { get; }
    public Point? DefaultArrivalTile => _locationData.DefaultArrivalTile;
    public bool ExcludeFromNpcPathfinding => _locationData.ExcludeFromNpcPathfinding;
    public CreateLocationData CreateOnLoad => _locationData.CreateOnLoad;
    public List<string> FormerLocationNames => _locationData.FormerLocationNames;
    public bool? CanPlantHere => _locationData.CanPlantHere;
    public bool CanHaveGreenRainSpawns => _locationData.CanHaveGreenRainSpawns;
    public List<ArtifactSpotDropData> ArtifactSpots => _locationData.ArtifactSpots;
    public Dictionary<string, FishAreaData> FishAreas => _locationData.FishAreas;
    public List<SpawnFishData> Fish => _locationData.Fish;
    public List<SpawnForageData> Forage => _locationData.Forage;
    public int MinDailyWeeds => _locationData.MinDailyWeeds;
    public int MaxDailyWeeds => _locationData.MaxDailyWeeds;
    public int FirstDayWeedMultiplier => _locationData.FirstDayWeedMultiplier;
    public int MinDailyForageSpawn => _locationData.MinDailyForageSpawn;
    public int MaxDailyForageSpawn => _locationData.MaxDailyForageSpawn;
    public int MaxSpawnedForageAtOnce => _locationData.MaxSpawnedForageAtOnce;
    public double ChanceForClay => _locationData.ChanceForClay;
    public List<LocationMusicData> Music => _locationData.Music;
    public string MusicDefault => _locationData.MusicDefault;
    public MusicContext MusicContext => _locationData.MusicContext;
    public bool MusicIgnoredInRain => _locationData.MusicIgnoredInRain;
    public bool MusicIgnoredInSpring => _locationData.MusicIgnoredInSpring;
    public bool MusicIgnoredInSummer => _locationData.MusicIgnoredInSummer;
    public bool MusicIgnoredInFall => _locationData.MusicIgnoredInFall;
    public bool MusicIgnoredInFallDebris => _locationData.MusicIgnoredInFallDebris;
    public bool MusicIgnoredInWinter => _locationData.MusicIgnoredInWinter;
    public bool MusicIsTownTheme => _locationData.MusicIsTownTheme;
    public Dictionary<string, string> CustomFields => _locationData.CustomFields;
}
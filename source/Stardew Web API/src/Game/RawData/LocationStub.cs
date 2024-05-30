/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using StardewValley.GameData.Locations;
using StardewValley.TokenizableStrings;

namespace StardewWebApi.Game.RawData;

public class LocationStub
{
    private readonly LocationData _locationData;

    public LocationStub(string name, LocationData locationData)
    {
        Name = name;
        _locationData = locationData;

        DisplayName = TokenParser.ParseText(_locationData.DisplayName);
    }

    public string Name { get; }
    public string DisplayName { get; }

    public string Url => $"/api/v1/data/locations/{Name}";
}
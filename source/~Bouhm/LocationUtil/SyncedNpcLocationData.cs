/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

// Synced NPC positions for multiplayer
using System.Collections.Generic;

internal class SyncedNpcLocationData
{
  public Dictionary<string, LocationData> Locations { get; set; }

  public SyncedNpcLocationData()
  {
    Locations = new Dictionary<string, LocationData>();
  }

  public void AddLocation(string name, LocationData location)
  {
    if (!Locations.ContainsKey(name))
    {
      Locations.Add(name, location);
    }
    else
    {
      Locations[name] = location;
    }
  }
}

// Used for syncing only the necessary data
internal class LocationData
{
  public string LocationName { get; set; }
  public float X { get; set; }
  public float Y { get; set; }

  public LocationData(string locationName, float x, float y)
  {
    this.LocationName = locationName;
    this.X = x;
    this.Y = y;
  }
}
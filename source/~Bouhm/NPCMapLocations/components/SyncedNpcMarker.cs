/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Bouhm/stardew-valley-mods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

public class SyncedNpcMarker
{
  public string DisplayName { get; set; }
  public string LocationName { get; set; }
  public int MapX { get; set; }
  public int MapY { get; set; }
  public bool IsBirthday { get; set; }
  public Character Type { get; set; }
  public SyncedNpcMarker()
  {
    DisplayName = null;
    LocationName = null;
    MapX = -9999;
    MapY = -9999;
    IsBirthday = false;
    Type = Character.Villager;
  }
}


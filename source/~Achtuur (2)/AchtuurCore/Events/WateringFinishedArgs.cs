/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using StardewValley;
using StardewValley.TerrainFeatures;
using System;

namespace AchtuurCore.Events;

public class WateringFinishedArgs : EventArgs
{
    public Farmer farmer;
    public HoeDirt target;
    public WateringFinishedArgs(Farmer _farmer, HoeDirt _target)
    {
        this.farmer = _farmer;
        this.target = _target;
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/ImprovedTracker
**
*************************************************/

class ImprovedTrackerConfig
{
	public bool Berries { get; set; }
	public bool CoconutTrees { get; set; }
	public bool FishingSpots { get; set; }
	public bool Underground { get; set; }

	public ImprovedTrackerConfig()
	{
		Berries = true;
		CoconutTrees = true;
		FishingSpots = true;
		Underground = true;
	}
}
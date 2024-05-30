/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MercuryVN/BaitsDoTheirJobs
**
*************************************************/

using StardewValley.Menus;

namespace BaitsDoTheirJobs;

public enum BaitType
{
    Specific,
    Magnet,
    Wild,
}

public class PseudoBobber
{
    public string whichFish = string.Empty;
    public int fishSize = -1;
    public int fishQuality = -1;
    public int difficulty = -1;
    public bool isPerfect = false;
    public bool isTreasure = false;

    public void Update(BobberBar bar)
    {
        whichFish = bar.whichFish;
        fishSize = bar.fishSize;
        fishQuality = bar.fishQuality;
        difficulty = (int)bar.difficulty;
        isPerfect = bar.perfect;
        isTreasure = bar.treasureCaught;
    }

    public void Clear()
    {
        whichFish = string.Empty;
        fishSize = -1;
        fishQuality = -1;
        difficulty = -1;
        isPerfect = false;
        isTreasure = false;
    }

    public bool IsInitialized()
    {
        return string.IsNullOrEmpty(whichFish)
            && fishSize == -1
            && fishQuality == -1
            && difficulty == -1
            && isPerfect == false
            && isTreasure == false;
    }
}
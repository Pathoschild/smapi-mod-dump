/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

namespace EnhancedRelationships;

public class Multipliers
{
    //Heart Multiplier
    public float[] HeartMultiplier { get; set; } = new float[11]
    {
        1f,
        0.9f,
        0.8f,
        0.7f,
        0.6f,
        0.5f,
        0.4f,
        0.3f,
        0.2f,
        0.15f,
        0.1f
    };
    //Heart Multiplier for Birthdays
    public float[] BirthdayHeartMultiplier { get; set; } = new float[11]
    {
        0.1f,
        0.15f,
        0.2f,
        0.3f,
        0.4f,
        0.5f,
        0.6f,
        0.7f,
        0.8f,
        0.9f,
        1f
    };
    
    //Birthday Multiplier
    public float BirthdayMultiplier { get; set; } = 5f;
}
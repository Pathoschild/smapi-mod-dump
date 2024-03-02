/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

namespace DynamicDialogues.Models;

///<summary>Class which holds animation information (if used for dialogues).</summary>
internal class AnimationData
{
    public bool Enabled { get; set; }
    public string Frames { get; set; }
    public int Interval { get; set; } // milliseconds for each frame

    public AnimationData()
    {
    }

    public AnimationData(AnimationData a)
    {
        Enabled = a.Enabled;
        Frames = a.Frames;
        Interval = a.Interval;
    }
}
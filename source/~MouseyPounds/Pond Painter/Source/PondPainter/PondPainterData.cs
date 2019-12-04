using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PondPainter
{
	// This is the internal data storage for the mod which is a list of Entries.
	// Each entry contains the following:
	//   PackID and LogName -- strings that identify where this data came from
	//   Tags -- list of string tags to match (must match all)
	//   Colors -- a sorted list of color definitions keyed by min population count in descending order
	// Each color definition contains the following:
	//   HasAnimation -- bool denoting whether this is animated (true) or static (false)
	//   StaticColor -- an XNA color structure used when HasAnimation is false
	//   AnimationColors -- a List of XNA color structures, used when HasAnimation is true
	//   AnimationTiming -- unsigned int denoting how many frames between each color change
	//   AnimationFrameCount -- unsigned int to keep track of the animation
	internal class PondPainterDataColorDef
	{
		internal bool HasAnimation { get; set; }
		internal Color StaticColor { get; set; }
		internal List<Color> AnimationColors { get; set; }
		internal uint AnimationTiming { get; set; }
		internal int AnimationCurrentFrame { get; set; }
		
		internal PondPainterDataColorDef(Color StaticColor)
		{
			this.HasAnimation = false;
			this.StaticColor = StaticColor;
			this.AnimationColors = null;
			this.AnimationTiming = 0;
			this.AnimationCurrentFrame = 0;
		}
		
		internal PondPainterDataColorDef(List<Color> AnimationColors)
		{
			this.HasAnimation = true;
			this.StaticColor = Color.White;
			this.AnimationColors = AnimationColors;
			this.AnimationTiming = 10;
			this.AnimationCurrentFrame = 0;
		}

        internal PondPainterDataColorDef(List<Color> AnimationColors, int AnimationTiming)
        {
            this.HasAnimation = true;
            this.StaticColor = Color.White;
            this.AnimationColors = AnimationColors;
            this.AnimationTiming = (uint)AnimationTiming;
            this.AnimationCurrentFrame = 0;
        }

        internal PondPainterDataColorDef(List<Color> AnimationColors, uint AnimationTiming)
		{
			this.HasAnimation = true;
			this.StaticColor = Color.White;
			this.AnimationColors = AnimationColors;
			this.AnimationTiming = AnimationTiming;
			this.AnimationCurrentFrame = 0;
		}
	}
	
    internal class PondPainterDataEntry
    {
		internal string PackID { get; set; }
		internal string LogName { get; set; }
        internal List<string> Tags { get; set; } = new List<string>();
        internal SortedList<int, PondPainterDataColorDef> Colors { get; set; } =
			new SortedList<int, PondPainterDataColorDef>(Comparer<int>.Create((x, y) => y.CompareTo(x)));

        internal PondPainterDataEntry(string PackID, string LogName, List<string> Tags)
        {
			this.PackID = PackID;
			this.LogName = LogName;
            this.Tags = Tags;
			this.Colors = new SortedList<int, PondPainterDataColorDef>(Comparer<int>.Create((x, y) => y.CompareTo(x)));
        }

        internal PondPainterDataEntry(string PackID, string LogName, List<string> Tags, int MinPop, Color ColorToAdd)
        {
			this.PackID = PackID;
			this.LogName = LogName;
            this.Tags = Tags;
            this.Colors.Add(MinPop, new PondPainterDataColorDef(ColorToAdd));
        }

        internal PondPainterDataEntry(string PackID, string LogName, List<string> Tags, int MinPop, List<Color> AnimationColors)
        {
			this.PackID = PackID;
			this.LogName = LogName;
            this.Tags = Tags;
            this.Colors.Add(MinPop, new PondPainterDataColorDef(AnimationColors));
        }

        internal PondPainterDataEntry(string PackID, string LogName, List<string> Tags, int MinPop, List<Color> AnimationColors, int AnimationTiming)
        {
			this.PackID = PackID;
			this.LogName = LogName;
            this.Tags = Tags;
            this.Colors.Add(MinPop, new PondPainterDataColorDef(AnimationColors, AnimationTiming));
        }
	}

    internal class PondPainterData
    {
        internal Color EmptyPondColor { get; set; } = Color.White;
        internal List<PondPainterDataEntry> Entries { get; set; } = new List<PondPainterDataEntry>();
    }
}

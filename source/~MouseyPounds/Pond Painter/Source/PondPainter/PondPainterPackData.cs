using System.Collections.Generic;

namespace PondPainter
{
	// This is the Content Pack data structure.
	// A full pack has a "Format" string and a list of Entries
	// Each Entry has a Tag List, optional LogName string, and a list of Colors
	// Each Color has a Minimum population to trigger and a color definition
	//   which can be specified in a variety of ways.
	
    public class PondPainterPackColor
    {
        public int MinPopulationForColor { get; set; }

        public string ColorName { get; set; }
        //public string ColorName { get; set; }

        public string AnimationType { get; set; }
        public int? AnimationRange { get; set; }
        public int? AnimationFrameDelay { get; set; }
        public int? AnimationTotalFrames { get; set; }

		// Basic constructors which only support static colors.
		// Mainly used for testing defaults.
        public PondPainterPackColor()
        {
            this.MinPopulationForColor = 0;
            this.ColorName = "White";
            this.AnimationType = "none";
        }
		
        public PondPainterPackColor(int MinPop, string CName)
        {
            this.MinPopulationForColor = MinPop;
            this.ColorName = CName;
            this.AnimationType = "none";
        }

    }

    public class PondPainterPackEntry
    {
		public string LogName { get; set; }
        public List<string> Tags { get; set; }
        public List<PondPainterPackColor> Colors { get; set; }
		
        public PondPainterPackEntry()
        {
            this.Tags = new List<string> { "" };
            this.Colors = new List<PondPainterPackColor>();
        }

        public PondPainterPackEntry(string Tag, int MinPop, string CName)
        {
            this.Tags = new List<string> { Tag };
            this.Colors = new List<PondPainterPackColor>() { new PondPainterPackColor(MinPop, CName) };
        }
    }
    public class PondPainterPackData
    {
        public string Format { get; set; }
        public string EmptyPondColor { get; set; }
        public IList<PondPainterPackEntry> Entries { get; set; }

        public PondPainterPackData()
        {
            this.Format = "1.0";
            this.Entries = new List<PondPainterPackEntry>();
        }
    }
}

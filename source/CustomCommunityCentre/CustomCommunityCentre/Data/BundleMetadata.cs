/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CustomCommunityCentre
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StardewValley;

namespace CustomCommunityCentre.Data
{
	public class BundleMetadata
	{
		public class CutsceneActor
		{
			public string ColourName = BundleMetadata.DefaultColourName;
			public Point TileLocation = Point.Zero;
			public string SourceTexture = @"LooseSprites/Cursors";
			public Rectangle SourceRectangle = new (294, 1432, 16, 16);
			public int AnimationFrames = 4;
			public int AnimationInterval = 300;
			public float Scale = Game1.pixelZoom;
			public bool FloatHorizontally = true;
			public bool FloatVertically = false;
			public Color Colour
			{
				get => BundleMetadata.GetXnaColourFromName(this.ColourName ?? BundleMetadata.DefaultColourName);
			}
		}

		public class Cutscene
		{
			public Point CameraTileLocation = Point.Zero;
			public List<CutsceneActor> Actors = new();
			public int Duration = 8000;
			public string Music = "nightTime";
			public string Sound = "junimoMeep1";
			public int SoundInterval = 800;
			public bool DrawEffects = true;
			public string LocationName = null;
		}

		private const LocalizedContentManager.LanguageCode DefaultLanguageCode
			= LocalizedContentManager.LanguageCode.en;

		private const string DefaultColourName = nameof(Color.White);

		public string AreaName;
		public int BundlesRequired;
		public string JunimoColourName = BundleMetadata.DefaultColourName;
		public Rectangle AreaBounds;
		public Point NoteTileLocation;
		public Point JunimoOffsetFromNoteTileLocation;
		public Cutscene AreaCompleteCutscene = null;
		public Dictionary<string, string> AreaDisplayName;
		public Dictionary<string, string> AreaRewardMessage;
		public Dictionary<string, string> AreaCompleteDialogue;
		public Dictionary<string, Dictionary<string, string>> BundleDisplayNames;
		public Color Colour
		{
			get => BundleMetadata.GetXnaColourFromName(this.JunimoColourName ?? BundleMetadata.DefaultColourName);
		}

		private static Color GetXnaColourFromName(string colourName)
		{
			System.Drawing.Color colour = System.Drawing.Color.FromName(colourName);
			return new Color(colour.R, colour.G, colour.B);
		}

		public static string GetLocalisedString(
			Dictionary<string, string> dict,
			string defaultValue = null,
			LocalizedContentManager.LanguageCode? code = null)
		{
			code ??= LocalizedContentManager.CurrentLanguageCode;
			string key = code.ToString();
			return dict.TryGetValue(key, out string translation)
				? translation
				: dict.TryGetValue(BundleMetadata.DefaultLanguageCode.ToString(), out translation)
					? translation
					: defaultValue;
		}
	}
}

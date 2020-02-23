using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	/// <summary>
	/// Contains the information in Data/Crops to randomly distribute
	/// </summary>
	public class CropGrowthInformation
	{
		public List<int> GrowthStages { get; set; } = new List<int>();
		public int TimeToGrow
		{
			get { return GrowthStages.Sum(); }
		}
		public List<Seasons> GrowingSeasons { get; set; } = new List<Seasons>();
		public int GraphicId { get; set; }
		public int CropId { get; set; }
		public int DaysToRegrow { get; set; }
		public bool RegrowsAfterHarvest { get { return DaysToRegrow != -1; } }

		public bool CanScythe { get; set; }
		public int CanScytheInt { get { return CanScythe ? 1 : 0; } }

		public ExtraCropInformation ExtraCropInfo { get; set; }
		public bool IsTrellisCrop { get; set; }
		public TintColorInformation TintColorInfo { get; set; }

		public static Dictionary<int, string> DefaultStringData { get; set; } = new Dictionary<int, string>
		{
			{273, "1 2 2 3/spring/34/271/-1/1/true 1 1 10 .1/false/false" },
			{299,  "1 2 2 2/fall/39/300/-1/1/false/false/false" },
			{301,  "1 1 2 3 3/fall/38/398/3/0/true 1 2 6 0/true/false" },
			{302,  "1 1 2 3 4/summer/37/304/1/0/true 1 2 6 0/true/false" },
			{347,  "2 4 6 6 6/fall/32/417/-1/0/false/false/false" },
			{425,  "1 4 4 3/fall/31/595/-1/0/false/false/true 187 0 255 119 137 255 71 227 255 255 127 144 205 178 255 140 119 255" },
			{427,  "1 1 2 2/spring/26/591/-1/0/false/false/true 255 186 255 223 191 255 255 246 0 255 80 0 255 158 193" },
			{429,  "1 2 2 2/spring/27/597/-1/0/false/false/true 35 127 255 109 131 255 112 207 255 191 228 255 94 121 255 40 150 255" },
			{431,  "1 2 3 2/summer fall/30/421/-1/0/false/false/false" },
			{433,  "1 2 2 3 2/spring summer/40/433/2/0/true 4 6 10 .02/false/false" },
			{453,  "1 2 2 2/summer/28/376/-1/0/false/false/true 255 0 0 254 254 254 255 170 0" },
			{455,  "1 2 3 2/summer/29/593/-1/0/false/false/true 0 208 255 99 255 210 255 212 0 255 144 122 255 0 238 206 91 255" },
			{472,  "1 1 1 1/spring/0/24/-1/0/false/false/false" },
			{473,  "1 1 1 3 4/spring/1/188/3/0/true 1 2 6 0/true/false" },
			{474,  "1 2 4 4 1/spring/2/190/-1/0/false/false/false" },
			{475,  "1 1 1 2 1/spring/3/192/-1/0/true 1 2 8 .2/false/false" },
			{476,  "1 1 1 1/spring/4/248/-1/0/false/false/false" },
			{477,  "1 2 2 1/spring/5/250/-1/1/false/false/false" },
			{478,  "2 2 2 3 4/spring/6/252/-1/0/false/false/false" },
			{479,  "1 2 3 3 3/summer/7/254/-1/0/false/false/false" },
			{480,  "2 2 2 2 3/summer/8/256/4/0/true 1 1 6 .05/false/false" },
			{481,  "1 3 3 4 2/summer/9/258/4/0/true 3 5 5 .02/false/false" },
			{482,  "1 1 1 1 1/summer/10/260/3/0/true 1 1 5 .03/false/false" },
			{483,  "1 1 1 1/summer fall/11/262/-1/1/false/false/false" },
			{484,  "2 1 2 1/summer/12/264/-1/0/false/false/false" },
			{485,  "2 1 2 2 2/summer/13/266/-1/0/false/false/false" },
			{486,  "2 3 2 3 3/summer/14/268/-1/0/false/false/false" },
			{487,  "2 3 3 3 3/summer fall/15/270/4/0/true 1 2 5 0/false/false" },
			{488,  "1 1 1 1 1/fall/16/272/5/0/true 1 2 10 .002/false/false" },
			{489,  "2 2 1 2 1/fall/17/274/-1/0/false/false/false" },
			{490,  "1 2 3 4 3/fall/18/276/-1/0/false/false/false" },
			{491,  "1 1 1 1/fall/19/278/-1/0/false/false/false" },
			{492,  "1 3 3 3/fall/20/280/-1/0/false/false/false" },
			{493,  "1 2 1 1 2/fall/21/282/5/0/true 2 6 5 .1/false/false" },
			{494,  "1 1 2 2/fall/22/284/-1/0/false/false/false" },
			{495,  "3 4/spring/23/16/-1/0/false/false/false" },
			{496,  "3 4/summer/23/396/-1/0/false/false/false" },
			{497,  "3 4/fall/23/404/-1/0/false/false/false" },
			{498,  "3 4/winter/23/412/-1/0/false/false/false" },
			{499,  "2 7 7 7 5/spring summer fall/24/454/7/0/false/false/false" },
			{745,  "1 1 2 2 2/spring/36/400/4/0/true 1 1 5 .02/false/false" },
			{802,  "2 2 2 3 3/spring summer fall winter/41/90/3/0/false/false/false" },
		};

		public static Dictionary<int, CropGrowthInformation> CropIdsToInfo { get; set; }
		static CropGrowthInformation()
		{
			CropIdsToInfo = new Dictionary<int, CropGrowthInformation>();
			foreach (int key in DefaultStringData.Keys)
			{
				CropIdsToInfo.Add(key, ParseString(DefaultStringData[key])); // Add all the default data - randomize it later
			}
		}

		/// <summary>
		/// Parses a string from the Crops data file into a CropGrowthInformation object
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static CropGrowthInformation ParseString(string input)
		{
			CropGrowthInformation cropGrowthInfo = new CropGrowthInformation();
			int result;

			string[] fields = input.Split('/');
			if (fields.Length != 9)
			{
				Globals.ConsoleError($"Invalid string passed when parsing crop info: {input}");
				return null;
			}

			// Growth stages
			string[] growthStages = fields[(int)CropGrowthFields.GrowthStages].Split(' ');
			foreach (string growthStage in growthStages)
			{
				if (int.TryParse(growthStage, out result))
				{
					cropGrowthInfo.GrowthStages.Add(result);
				}
				else
				{
					Globals.ConsoleError($"Tried to parse {growthStage} into a growth stage when parsing: {input}");
					return null;
				}
			}

			// Seasons
			string[] seasonStrings = fields[(int)CropGrowthFields.Seasons].Split(' ');
			foreach (string seasonString in seasonStrings)
			{
				switch (seasonString.ToLower())
				{
					case "spring":
						cropGrowthInfo.GrowingSeasons.Add(Seasons.Spring);
						break;
					case "summer":
						cropGrowthInfo.GrowingSeasons.Add(Seasons.Summer);
						break;
					case "fall":
						cropGrowthInfo.GrowingSeasons.Add(Seasons.Fall);
						break;
					case "winter":
						cropGrowthInfo.GrowingSeasons.Add(Seasons.Winter);
						break;
					default:
						Globals.ConsoleError($"Tries to parse {seasonString} into a season when parsing: {input}");
						return null;
				}
			}

			// Graphic id
			string graphicId = fields[(int)CropGrowthFields.GraphicId];
			if (int.TryParse(graphicId, out result))
			{
				cropGrowthInfo.GraphicId = result;
			}
			else
			{
				Globals.ConsoleError($"Tried to parse {result} into a graphic id when parsing: {input}");
				return null;
			}

			// Crop Id
			string cropId = fields[(int)CropGrowthFields.CropId];
			if (int.TryParse(cropId, out result))
			{
				cropGrowthInfo.CropId = result;
			}
			else
			{
				Globals.ConsoleError($"Tried to parse {result} into a crop id when parsing: {input}");
				return null;
			}

			// Amount per harvest
			string daysToRegrow = fields[(int)CropGrowthFields.DaysToRegrow];
			if (int.TryParse(daysToRegrow, out result))
			{
				cropGrowthInfo.DaysToRegrow = result;
			}
			else
			{
				Globals.ConsoleError($"Tried to parse {result} into the amount per harvest when parsing: {input}");
				return null;
			}

			// Can scythe
			string canScythe = fields[(int)CropGrowthFields.CanScythe];
			if (int.TryParse(canScythe, out result))
			{
				cropGrowthInfo.CanScythe = result == 1;
			}
			else
			{
				Globals.ConsoleError($"Tried to parse {result} into the CanScythe flag id when parsing: {input}");
				return null;
			}

			// Extra crop info
			string extraCropInfoString = fields[(int)CropGrowthFields.ExtraCropInfo];
			cropGrowthInfo.ExtraCropInfo = ExtraCropInformation.ParseString(extraCropInfoString);
			if (cropGrowthInfo.ExtraCropInfo == null)
			{
				return null;
			}

			// Is trellis crop
			string isTrellisCropString = fields[(int)CropGrowthFields.IsTrellisCrop];
			if (bool.TryParse(isTrellisCropString, out bool boolResult))
			{
				cropGrowthInfo.IsTrellisCrop = boolResult;
			}
			else
			{
				Globals.ConsoleError($"Tried to parse {isTrellisCropString} into the isTrellisCrop boolean when parsing: {input}");
				return null;
			}

			// Tint color info
			string tintColorInfoString = fields[(int)CropGrowthFields.TintColorInfo];
			cropGrowthInfo.TintColorInfo = TintColorInformation.ParseString(tintColorInfoString);
			if (cropGrowthInfo.TintColorInfo == null)
			{
				return null;
			}

			return cropGrowthInfo;
		}

		/// <summary>
		/// Gets the string that's part of Data/Crops
		/// </summary>
		/// <returns />
		public override string ToString()
		{
			string growthStagesString = "";
			foreach (int stageTime in GrowthStages)
			{
				growthStagesString += $"{stageTime} ";
			}
			growthStagesString = growthStagesString.Trim();

			return $"{growthStagesString}/{GetSeasonsString()}/{GraphicId}/{CropId}/{(DaysToRegrow == 0 ? -1 : DaysToRegrow)}/{CanScytheInt}/{ExtraCropInfo.ToString()}/{IsTrellisCrop.ToString().ToLower()}/{TintColorInfo.ToString()}";
		}

		/// <summary>
		/// Get the string that's used for seasons
		/// </summary>
		/// <returns>The seasons string</returns>
		public string GetSeasonsString()
		{
			string seasonsString = "";
			foreach (Seasons season in GrowingSeasons)
			{
				seasonsString += $"{season.ToString()} ";
			}
			return seasonsString.Trim().ToLower();
		}

		/// <summary>
		/// Get the string that's used for seasons when displaying the tooltip
		/// </summary>
		/// <returns>The seasons string</returns>
		public string GetSeasonsStringForDisplay(bool useCommaDelimiter = false)
		{
			string seasonsString = "";
			foreach (Seasons season in GrowingSeasons)
			{
				seasonsString += $"{Globals.GetTranslation($"seasons-{season.ToString().ToLower()}")} ";
			}
			seasonsString = seasonsString.Trim().ToLower();
			return seasonsString.Replace(" ", ", ");
		}
	}

	/// <summary>
	/// Contains data about getting extra crops on harvest
	/// </summary>
	public class ExtraCropInformation
	{
		public bool CanGetExtraCrops { get; set; }
		public int MinExtra { get; set; }
		public int MaxExtra { get; set; }
		public int AdditionalExtraPerFramingLevel { get; set; } // Doesn't actually work in the game, but here in case it will
		public double ChanceOfExtra { get; set; }

		public static ExtraCropInformation ParseString(string input)
		{
			ExtraCropInformation extraCropInfo = new ExtraCropInformation();

			string[] extraCropParts = input.Split(' ');
			if (bool.TryParse(extraCropParts[0], out bool boolValue))
			{
				extraCropInfo.CanGetExtraCrops = boolValue;
			}
			else
			{
				Globals.ConsoleError($"Tried to parse {extraCropParts[0]} into a boolean for extra crop info while parsing: {input}");
				return null;
			}

			if (extraCropInfo.CanGetExtraCrops)
			{
				int intValue;
				if (int.TryParse(extraCropParts[1], out intValue))
				{
					extraCropInfo.MinExtra = intValue;
				}
				else
				{
					Globals.ConsoleError($"Tried to parse {extraCropParts[1]} into the minimum extra crops while parsing: {input}");
					return null;
				}

				if (int.TryParse(extraCropParts[2], out intValue))
				{
					extraCropInfo.MaxExtra = intValue;
				}
				else
				{
					Globals.ConsoleError($"Tried to parse {extraCropParts[2]} into the maximum extra crops while parsing: {input}");
					return null;
				}

				if (int.TryParse(extraCropParts[3], out intValue))
				{
					extraCropInfo.AdditionalExtraPerFramingLevel = intValue;
				}
				else
				{
					Globals.ConsoleError($"Tried to parse {extraCropParts[3]} into the additional extra per farming level while parsing: {input}");
					return null;
				}

				if (double.TryParse(extraCropParts[4], out double value))
				{
					extraCropInfo.ChanceOfExtra = value;
				}
				else
				{
					Globals.ConsoleError($"Tried to parse {extraCropParts[4]} into the chance of extra crops while parsing: {input}");
					return null;
				}
			}

			return extraCropInfo;
		}

		public override string ToString()
		{
			if (!CanGetExtraCrops) { return "false"; }

			string chanceOfExtraString = (ChanceOfExtra == 0) ? "0" : ChanceOfExtra.ToString().TrimStart(new char[] { '0' });
			return $"true {MinExtra} {MaxExtra} {AdditionalExtraPerFramingLevel} {chanceOfExtraString}";
		}
	}

	/// <summary>
	/// Contains data about tint color - used for flowers
	/// </summary>
	public class TintColorInformation
	{
		public bool HasTint { get; set; }
		public List<RGBInformation> RGBInfo { get; set; } = new List<RGBInformation>();

		public static TintColorInformation ParseString(string input)
		{
			TintColorInformation tintColorInformation = new TintColorInformation();

			string[] tintColorParts = input.Split(' ');
			if (bool.TryParse(tintColorParts[0], out bool value))
			{
				tintColorInformation.HasTint = value;
			}
			else
			{
				Globals.ConsoleError($"Tried to parse {tintColorParts[0]} into a boolean for tint color info while parsing: {input}");
				return null;
			}

			if (tintColorInformation.HasTint)
			{
				string[] rgbColors = tintColorParts.Skip(1).ToArray();
				if (rgbColors.Length % 3 != 0)
				{
					Globals.ConsoleError($"Invalid number of RGB colors - should be divisible by 3: {input}");
					return null;
				}

				for (int i = 0; i < rgbColors.Length; i += 3)
				{
					if (!int.TryParse(rgbColors[i], out int redValue))
					{
						Globals.ConsoleError($"Tried to parse {rgbColors[i]} into a red value while parsing: {input}");
						return null;
					}

					if (!int.TryParse(rgbColors[i + 1], out int greenValue))
					{
						Globals.ConsoleError($"Tried to parse {rgbColors[i + 1]} into a red value while parsing: {input}");
						return null;
					}

					if (!int.TryParse(rgbColors[i + 2], out int blueValue))
					{
						Globals.ConsoleError($"Tried to parse {rgbColors[i + 2]} into a blue value while parsing: {input}");
						return null;
					}

					tintColorInformation.RGBInfo.Add(new RGBInformation(redValue, greenValue, blueValue));
				}
			}

			return tintColorInformation;
		}

		public override string ToString()
		{
			if (!HasTint) { return "false"; }

			string rgbString = "";
			foreach (RGBInformation rgbInfo in RGBInfo)
			{
				rgbString += $"{rgbInfo.ToString()} ";
			}
			return $"true {rgbString.Trim()}";
		}
	}

	/// <summary>
	/// Contains data about RGB information - used with the tint color info
	/// </summary>
	public class RGBInformation
	{
		public int Red { get; set; }
		public int Green { get; set; }
		public int Blue { get; set; }

		public RGBInformation(int red, int green, int blue)
		{
			Red = red;
			Blue = blue;
			Green = green;
		}

		public override string ToString()
		{
			return $"{Red} {Green} {Blue}";
		}
	}

	/// <summary>
	/// Contains the field indexes for the crop information - the slash-delimited values
	/// </summary>
	public enum CropGrowthFields
	{
		GrowthStages = 0,
		Seasons = 1,
		GraphicId = 2,
		CropId = 3,
		DaysToRegrow = 4,
		CanScythe = 5,
		ExtraCropInfo = 6,
		IsTrellisCrop = 7,
		TintColorInfo = 8
	}
}

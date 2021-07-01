/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/cropbeasts
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace Cropbeasts
{
	public class Mapping
	{
		protected static IModHelper Helper => ModEntry.Instance.Helper;
		protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		protected static ModConfig Config => ModConfig.Instance;

		public readonly int harvestIndex;
		public readonly bool giantCrop;

		private SObject harvestObject_;
		public SObject harvestObject
		{
			get
			{
				harvestObject_ ??= new SObject (harvestIndex, 1);
				return harvestObject_;
			}
		}

		public string harvestName => giantCrop
			? $"Giant {harvestObject.Name}"
			: harvestObject.Name;
		public string harvestDisplayName => giantCrop
			? Helper.Translation.Get ("giantModifier").ToString ()
				.Replace ("{{crop}}", harvestObject.DisplayName)
			: harvestObject.DisplayName;

		public string harvestTextureName => Game1.objectSpriteSheetName;
		public Texture2D harvestTexture => Game1.objectSpriteSheet;
		public Rectangle harvestSourceRect => Game1.getSourceRectForStandardTileSheet
			(harvestTexture, harvestIndex, 16, 16);

		public readonly string beastName;
		public string beastDisplayName => Helper.Translation.Get (beastName);

		public double choiceWeight { get; private set; }

		public readonly Color? primaryColor;
		public readonly Color? secondaryColor;

		public string typeName => $"Cropbeasts.Beasts.{beastName.Replace (" ", "")}";
		public Type type => Type.GetType (typeName);

		public bool available => !Config.ExcludedBeasts.Contains (beastName) &&
			type != null;

		public Mapping (int harvestIndex, bool giantCrop, string beastName,
			double choiceWeight = 0.0, Color? primaryColor = null,
			Color? secondaryColor = null)
		{
			this.harvestIndex = harvestIndex;
			this.giantCrop = giantCrop;
			this.beastName = beastName;
			this.choiceWeight = choiceWeight;
			this.primaryColor = primaryColor;
			this.secondaryColor = secondaryColor;
		}

		public bool matchesFilter (string filter)
		{
			List<string> identifiers = new () { "any", harvestName, beastName };
			return filter == null ||
				Utility.fuzzySearch (filter, identifiers) != null;
		}

		internal void choose ()
		{
			// Only apply the choice weight once per day. The value will be
			// reset for the next day when the mappings are reloaded.
			choiceWeight = 0.0;
		}
	}

	public static class Mappings
	{
		private static IModHelper Helper => ModEntry.Instance.Helper;
		private static IMonitor Monitor => ModEntry.Instance.Monitor;
		// private static ModConfig Config => ModConfig.Instance;

		private static Dictionary<string, string> RawData;
		private static Dictionary<int, Mapping> RegularMappings;
		private static Dictionary<int, Mapping> GiantMappings;

		public static Mapping Get (int harvestIndex, bool giantCrop)
		{
			Load ();
			try
			{
				return giantCrop
					? GiantMappings[harvestIndex]
					: RegularMappings[harvestIndex];
			}
			catch (KeyNotFoundException)
			{
				return null;
			}
		}

		public static void Add (Mapping mapping)
		{
			Load ();
			if (mapping.giantCrop)
				GiantMappings[mapping.harvestIndex] = mapping;
			else
				RegularMappings[mapping.harvestIndex] = mapping;
		}

		public static List<Mapping> GetAll ()
		{
			Load ();
			var regular = RegularMappings.Values;
			var giant = GiantMappings.Values;
			return new IEnumerable<Mapping>[] { regular, giant }
				.SelectMany ((m) => m).ToList ();
		}

		public static List<Mapping> GetForBeast (string name)
		{
			Load ();
			var regular = RegularMappings.Values.Where ((m) => m.beastName == name);
			var giant = GiantMappings.Values.Where ((m) => m.beastName == name);
			return new IEnumerable<Mapping>[] { regular, giant }
				.SelectMany ((m) => m).ToList ();
		}

		internal static void Load (bool reload = false)
		{
			if (!reload && RegularMappings != null) return;

			RawData = Helper.Content.Load<Dictionary<string, string>>
				("Data\\Cropbeasts", ContentSource.GameContent);
			RegularMappings = new Dictionary<int, Mapping> ();
			GiantMappings = new Dictionary<int, Mapping> ();

			foreach (string key in RawData.Keys)
			{
				try
				{
					string[] keyFields = key.Split ('/');
					string[] valueFields = RawData[key].Split ('/');

					int harvestIndex = Convert.ToInt32 (keyFields[0]);
					bool giantCrop = keyFields.Length > 1 &&
						Convert.ToBoolean (keyFields[1]);

					string beastName = valueFields[0].Trim ();
					double choiceWeight = (valueFields.Length > 1)
						? Convert.ToDouble (valueFields[1]) : 0.0;

					Color? primaryColor = (valueFields.Length > 2)
						? ConvertToColor (valueFields[2]) : null;
					Color? secondaryColor = (valueFields.Length > 3)
						? ConvertToColor (valueFields[3]) : null;

					Add (new Mapping (harvestIndex, giantCrop, beastName,
						choiceWeight, primaryColor, secondaryColor));
				}
				catch (Exception e)
				{
					Monitor.Log ($"Could not parse cropbeast mapping for crop {key}: {e.Message}.",
						LogLevel.Warn);
				}
			}
		}

		private static Color? ConvertToColor (string field)
		{
			var converter = System.ComponentModel.TypeDescriptor
				.GetConverter (System.Drawing.Color.White);
			var sysColor = (System.Drawing.Color?)
				converter.ConvertFromInvariantString (field.Trim ());
			return sysColor.HasValue
				? new Color (sysColor.Value.R, sysColor.Value.G, sysColor.Value.B)
				: (Color?) null;
		}
	}
}

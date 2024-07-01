/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Arborsm/PortraiturePlus
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using Portraiture;
using Portraiture.HDP;
using StardewModdingAPI;
using StardewValley;

namespace PortraiturePlus
{
	/// <summary>The mod entry point.</summary>
	// ReSharper disable once ClassNeverInstantiated.Global
	internal sealed class PortraiturePlusMod : Mod
	{
		private static readonly IDictionary<string, string> festivalDates = Game1.content.Load<Dictionary<string, string>>(@"Data\Festivals\FestivalDates", LocalizedContentManager.LanguageCode.en);
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="help">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper help)
		{
			festivalInit();
			harmonyFix();
		}

		private void harmonyFix()
		{
			PortraiturePlusFix.Initialize(monitor: Monitor);
			var harmony = new Harmony(ModManifest.UniqueID);
			harmony.Patch(original: PortraiturePlusFix.TargetMethod(), prefix: new HarmonyMethod(AccessTools.Method(typeof(PortraiturePlusFix), nameof(PortraiturePlusFix.getPortrait_Prefix))));
		}
		
		public static Texture2D? getPortrait(NPC npc, Texture2D tex, List<string> folders, PresetCollection presets, int activeFolder, Dictionary<string, Texture2D> pTextures)
		{
			var name = npc.Name;

			if (!Context.IsWorldReady || folders.Count == 0)
				return null;

			activeFolder = Math.Max(activeFolder, 0);

			if (presets.Presets.FirstOrDefault(pr => pr.Character == name) is { } pre)
				activeFolder = Math.Max(folders.IndexOf(pre.Portraits), 0);

			var folder = folders[activeFolder];

			if (activeFolder == 0 || folders.Count <= activeFolder || folder == "none" || (folder == "HDP" && PortraitureMod.helper.ModRegistry.IsLoaded("tlitookilakin.HDPortraits")))
				return null;

			if (folder == "HDP" && !PortraitureMod.helper.ModRegistry.IsLoaded("tlitookilakin.HDPortraits"))
			{
				try
				{
					var portraits = PortraitureMod.helper.GameContent.Load<MetadataModel>("Mods/HDPortraits/" + name);
					switch (portraits)
					{
						case null:
							return null;
						case var _ when portraits.TryGetTexture(out var texture):
							{
								if (portraits.Animation == null || (portraits.Animation.VFrames == 1 && portraits.Animation.HFrames == 1))
									return ScaledTexture2D.FromTexture(tex, texture, portraits.Size / 64f);
								portraits.Animation.Reset();
								return new AnimatedTexture2D(texture, texture.Width / portraits.Animation.VFrames, texture.Height / portraits.Animation.HFrames, 6, true, portraits.Size / 64f);
							}
						default:
							return null;
					}
				}
				catch
				{
					return null;
				}
			}

			var season = Game1.currentSeason ?? "spring";
			var npcDictionary = pTextures.Keys.Where(key => key.Contains(name) && key.Contains(folder)).ToDictionary(k => k, l => pTextures[l]);
			var dayOfMonth = Game1.dayOfMonth.ToString();
			var festival = GetDayEvent();
			var gl = Game1.currentLocation.Name ?? "";
			var isOutdoors = Game1.currentLocation.IsOutdoors ? "Outdoor" : "Indoor";
			var week = (Game1.dayOfMonth % 7) switch
			{
				0 => "Sunday",
				1 => "Monday",
				2 => "Tuesday",
				3 => "Wednesday",
				4 => "Thursday",
				5 => "Friday",
				6 => "Saturday",
				_ => ""
			};

			if (getTexture2D(npcDictionary, festival) != null)
			{
				return getTexture2D(npcDictionary, festival);
			}
			if (getTexture2D(npcDictionary, gl, season, dayOfMonth) != null)
			{
				return getTexture2D(npcDictionary, gl, season, dayOfMonth);
			}
			if (getTexture2D(npcDictionary, gl, season, week) != null)
			{
				return getTexture2D(npcDictionary, gl, season, week);
			}
			if (getTexture2D(npcDictionary, gl, season) != null)
			{
				return getTexture2D(npcDictionary, gl, season);
			}
			if (getTexture2D(npcDictionary, gl) != null)
			{
				return getTexture2D(npcDictionary, gl);
			}
			if (getTexture2D(npcDictionary, season, isOutdoors) != null)
			{
				return getTexture2D(npcDictionary, season, isOutdoors);
			}
			if (getTexture2D(npcDictionary, season, dayOfMonth) != null)
			{
				return getTexture2D(npcDictionary, season, dayOfMonth);
			}
			if (getTexture2D(npcDictionary, season, week) != null)
			{
				return getTexture2D(npcDictionary, week);
			}
			if (getTexture2D(npcDictionary, season) != null)
			{
				return getTexture2D(npcDictionary, season);
			}

			return pTextures.ContainsKey(folder + ">" + name) ? pTextures[folder + ">" + name] : null;
		}
		
		private static string GetDayEvent()
		{
			if (SaveGame.loaded?.weddingToday ?? Game1.weddingToday || Game1.CurrentEvent != null && Game1.CurrentEvent.isWedding)
				return "Wedding";

			var festival = festivalDates.TryGetValue($"{Game1.currentSeason}{Game1.dayOfMonth}", out var festivalName) ? festivalName : "";
			return festival;
		}
		
		private static Texture2D? getTexture2D(Dictionary<string, Texture2D> npcDictionary, params string[] values)
		{
			return values.Any(text => text == "") ? null : npcDictionary!.GetValueOrDefault(npcDictionary.Keys.FirstOrDefault(key => values.All(v => key.Contains(v, StringComparison.OrdinalIgnoreCase)), ""), null);
		}

		private static void festivalInit()
		{
			foreach (var key in festivalDates.Keys)
			{
				if (festivalDates[key].Contains(' '))
				{
					festivalDates[key] = festivalDates[key].Replace(" ", "");
				}
				if (festivalDates[key].Contains('\''))
				{
					festivalDates[key] = festivalDates[key].Replace("'", "");
				}
				festivalDates[key] = festivalDates[key] switch
				{
					"EggFestival" => "EggF",
					"DanceoftheMoonlightJellies" => "Jellies",
					"StardewValleyFair" => "Fair",
					"FestivalofIce" => "Ice",
					"FeastoftheWinterStar" => "WinterStar",
					_ => festivalDates[key]
				};
			}
		}
	}
}

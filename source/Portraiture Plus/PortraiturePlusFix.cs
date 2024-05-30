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
using System.Reflection;
namespace PortraiturePlus
{
	internal class PortraiturePlusFix
	{
		private static IMonitor Monitor = null!;
		
		internal static void Initialize(IMonitor monitor)
		{
			Monitor = monitor;
		}
		internal static MethodInfo TargetMethod()
		{
			return AccessTools.Method("TextureLoader:getPortrait", new[]
			{
				typeof(NPC), typeof(Texture2D)
			});
		}

		internal static bool getPortrait_Prefix(NPC npc, Texture2D tex, ref Texture2D? __result)
		{
			var folders = Traverse.Create(typeof(PortraitureMod).Assembly.GetType("Portraiture.TextureLoader")).Field<List<string>>("folders").Value;
			var presets = Traverse.Create(typeof(PortraitureMod).Assembly.GetType("Portraiture.TextureLoader")).Field<PresetCollection>("presets").Value;
			var activeFolder = Traverse.Create(typeof(PortraitureMod).Assembly.GetType("Portraiture.TextureLoader")).Field<int>("activeFolder").Value;
			var pTextures = Traverse.Create(typeof(PortraitureMod).Assembly.GetType("Portraiture.TextureLoader")).Field<Dictionary<string, Texture2D>>("pTextures").Value;
			if (folders is { Count: <= 0 })
				return true;
			try
			{
				var name = npc.Name;
				if (!Context.IsWorldReady || folders.Count == 0)
				{
					__result = null;
					return false;
				}

				activeFolder = Math.Max(activeFolder, 0);

				if (presets.Presets.FirstOrDefault(pr => pr.Character == name) is { } pre)
					activeFolder = Math.Max(folders.IndexOf(pre.Portraits), 0);

				var folder = folders[activeFolder];

				if (activeFolder == 0 || folders.Count <= activeFolder || folder == "none" || folder == "HDP" && PortraitureMod.helper.ModRegistry.IsLoaded("tlitookilakin.HDPortraits"))
				{
					__result = null;
					return false;
				}

				if (folder == "HDP" && !PortraitureMod.helper.ModRegistry.IsLoaded("tlitookilakin.HDPortraits"))
				{
					try
					{
						var portraits = PortraitureMod.helper.GameContent.Load<MetadataModel>("Mods/HDPortraits/" + name);
						if (!portraits.TryGetTexture(out var texture))
						{
							__result = null;
							return false;
						}
						if (portraits.Animation == null || (portraits.Animation.VFrames == 1 && portraits.Animation.HFrames == 1))
						{
							__result = ScaledTexture2D.FromTexture(tex, texture, portraits.Size / 64f);
							return false;
						}
						portraits.Animation!.Reset();
						__result = new AnimatedTexture2D(texture, texture.Width / portraits.Animation.VFrames, texture.Height / portraits.Animation.HFrames, 6, true, portraits.Size / 64f);
						return false;
					}
					catch
					{
						__result = null;
						return false;
					}
				}

				var season = Game1.currentSeason ?? "Spring";
				// if (PortraitureMod.config.isFestivalLower) 
				season = season.ToLower();

				if (presets.Presets.FirstOrDefault(p => p.Character == name) is { } preset && folders.Contains(preset.Portraits))
					folder = preset.Portraits;

				if (Game1.isFestival() || Game1.CurrentEvent != null && Game1.CurrentEvent.isWedding)
				{
					var festival = PortriturePlusMod.GetDayEvent();
					if (pTextures.ContainsKey(folder + ">" + name + "_" + festival))
					{
						__result = pTextures[folder + ">" + name + "_" + festival];
						return false;
					}
				}

				if (Game1.currentLocation is { Name: not null } gl)
				{
					if (pTextures.ContainsKey(folder + ">" + name + "_" + gl.Name + "_" + season))
					{
						__result = pTextures[folder + ">" + name + "_" + gl.Name + "_" + season];
						return false;
					}
					if (pTextures.ContainsKey(folders[activeFolder] + ">" + name + "_" + gl.Name))
					{
						__result = pTextures[folder + ">" + name + "_" + gl.Name];
						return false;
					}
				}

				if (pTextures.ContainsKey(folder + ">" + name + "_" + season + "_Indoor") && pTextures.ContainsKey(folder + ">" + name + "_" + season + "_Outdoor"))
				{
					__result = Game1.currentLocation.IsOutdoors ? pTextures[folder + ">" + name + "_" + season + "_Outdoor"] : pTextures[folder + ">" + name + "_" + season + "_Indoor"];
					return false;
				}

				if (pTextures.ContainsKey(folder + ">" + name + "_" + season))
				{
					__result = pTextures[folder + ">" + name + "_" + season];
					return false;
				}

				__result = pTextures.ContainsKey(folder + ">" + name) ? pTextures[folder + ">" + name] : null;
				return false;
			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in {nameof(getPortrait_Prefix)}:\n{ex}", LogLevel.Error);
				return true;
			}
		}
	}
}

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
				__result = PortraiturePlusMod.getPortrait(npc, tex, folders, presets, activeFolder, pTextures);
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

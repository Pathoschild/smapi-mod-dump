/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using Ogg2XNA;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;

namespace TheLion.Stardew.Professions.Framework
{
	public class SoundEffectLoader
	{
		public Dictionary<string, SoundEffect> SfxByName { get; } = new();
		public int Volume { get; set; }

		/// <summary>Construct an instance.</summary>
		/// <param name="modPath">Path to the mod directory.</param>
		public SoundEffectLoader(string modPath)
		{
			foreach (var file in Directory.GetFiles(Path.Combine(modPath, "assets", "sfx"), "*.ogg"))
			{
				try
				{
					// load .wav
					//using var fs = new FileStream(file, FileMode.Open);
					//var soundEffect = SoundEffect.FromStream(fs);

					// load .ogg
					var soundEffect = OggLoader.Load(file);
					if (soundEffect == null) throw new FileLoadException();
					SfxByName.Add(Path.GetFileNameWithoutExtension(file), soundEffect);
				}
				catch (Exception ex)
				{
					ModEntry.Log($"Failed to load {file}. Loader returned {ex}", LogLevel.Error);
				}
			}
		}
	}
}

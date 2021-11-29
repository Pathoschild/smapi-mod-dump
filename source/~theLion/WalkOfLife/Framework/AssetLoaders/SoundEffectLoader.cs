/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Ogg2XNA;
using StardewModdingAPI;

namespace TheLion.Stardew.Professions.Framework.AssetLoaders
{
	public class SoundEffectLoader
	{
		/// <summary>Construct an instance.</summary>
		public SoundEffectLoader(string modPath)
		{
			foreach (var file in Directory.GetFiles(Path.Combine(modPath, "assets", "sfx"), "*.ogg"))
				try
				{
					// load .ogg
					var soundEffect = OggLoader.Load(file);

					if (soundEffect is null) throw new FileLoadException();
					SoundByName.Add(Path.GetFileNameWithoutExtension(file), soundEffect);
				}
				catch (Exception ex)
				{
					ModEntry.Log($"Failed to load {file}. Loader returned {ex}", LogLevel.Error);
				}
		}

		public Dictionary<string, SoundEffect> SoundByName { get; } = new();
	}
}
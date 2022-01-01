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
using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.AssetLoaders;

public class SoundBox
{
    /// <summary>Construct an instance.</summary>
    public SoundBox(string modPath)
    {
        foreach (var file in Directory.GetFiles(Path.Combine(modPath, "assets", "sfx"), "*.wav"))
            try
            {
                // load .wav
                using var fs = new FileStream(file, FileMode.Open);
                var soundEffect = SoundEffect.FromStream(fs);

                if (soundEffect is null) throw new FileLoadException();
                SoundByName.Add(Path.GetFileNameWithoutExtension(file), soundEffect);
            }
            catch (Exception ex)
            {
                ModEntry.Log($"Failed to load {file}. Loader returned {ex}", LogLevel.Error);
            }
    }

    public Dictionary<string, SoundEffect> SoundByName { get; } = new();

    public void Play(string id)
    {
        try
        {
            if (SoundByName.TryGetValue(id, out var sfx))
                sfx.Play(Game1.options.soundVolumeLevel, 0f, 0f);
            else throw new ContentLoadException();
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Couldn't play file 'assets/sfx/{id}.wav'. Make sure the file exists. {ex}",
                LogLevel.Error);
        }
    }
}
/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UIInfoSuite2
**
*************************************************/

using System;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewValley;

namespace UIInfoSuite2.Infrastructure;

public enum Sounds
{
  LevelUp
}

public class SoundHelper
{
  private static readonly Lazy<SoundHelper> _instance = new(() => new SoundHelper());

  private bool _initialized;

  protected SoundHelper() { }

  public static SoundHelper Instance => _instance.Value;

  public void Initialize(IModHelper helper)
  {
    if (_initialized)
    {
      throw new InvalidOperationException("Cannot re-initialize sound helper");
    }

    RegisterSound(helper, Sounds.LevelUp, "LevelUp.wav");

    _initialized = true;
  }

  private static string GetQualifiedSoundName(Sounds sound)
  {
    return $"UIInfoSuite.sounds.{sound.ToString()}";
  }

  private static void RegisterSound(
    IModHelper helper,
    Sounds sound,
    string fileName,
    int instanceLimit = -1,
    CueDefinition.LimitBehavior? limitBehavior = null
  )
  {
    CueDefinition newCueDefinition = new() { name = GetQualifiedSoundName(sound) };

    if (instanceLimit > 0)
    {
      newCueDefinition.instanceLimit = instanceLimit;
      newCueDefinition.limitBehavior = limitBehavior ?? CueDefinition.LimitBehavior.ReplaceOldest;
    } else if (limitBehavior.HasValue)
    {
      newCueDefinition.limitBehavior = limitBehavior.Value;
    }

    SoundEffect audio;
    string filePath = Path.Combine(helper.DirectoryPath, "assets", fileName);
    using (var stream = new FileStream(filePath, FileMode.Open))
    {
      audio = SoundEffect.FromStream(stream);
    }

    newCueDefinition.SetSound(audio, Game1.audioEngine.GetCategoryIndex("Sound"));
    Game1.soundBank.AddCue(newCueDefinition);
    ModEntry.MonitorObject.Log($"Registered Sound: {newCueDefinition.name}");
  }

  public static void Play(Sounds sound)
  {
    Game1.playSound(GetQualifiedSoundName(sound));
  }
}

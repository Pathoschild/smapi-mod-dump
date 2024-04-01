/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using MusicMaster.Streams;
using MusicMaster.Types;
using StardewValley;
using System;
using System.IO;

namespace MusicMaster;

internal record class TrackCue(Track Track, SoundEffectInstance Cue);

internal sealed class Track {
	private static readonly SharedRandom Random = new();

	private readonly Random _random = new(Random.Value);

	internal string Name;
	internal TrackCue[] Tracks;
	internal bool FadeOut = false;
	internal bool RandomPerDay = true;
	internal bool AlwaysStop = false;

	private int LastDay = -1;
	private int? LastDailyTrack = null;

	internal TrackCue CurrentCue => RandomPerDay ? RandomDailyCue : RandomCue;

	private int RandomTrackIndex => _random.Next(0, Tracks.Length);
	internal TrackCue RandomCue => Tracks[RandomTrackIndex];

	private int RandomDailyCueIndex {
		get {
			int currentDay = MusicManager.CurrentDay;

			if (LastDailyTrack is { } lastDailyTrack && LastDay == currentDay) {
				return lastDailyTrack;
			}

			LastDay = currentDay;
			int newIndex = RandomTrackIndex;
			LastDailyTrack = newIndex;
			return newIndex;
		}
	}

	internal TrackCue RandomDailyCue => Tracks[RandomDailyCueIndex];

	private static readonly (string Extension, Func<string, AudioStream?> Getter)[] StreamGetters = {
		new (".ogg", path => new VorbisStream(path)),
		new (".mp3", path => new Mp3Stream(path))
	};

	private static AudioStream? GetAudioStream(string name) {
		var prefix = MusicMaster.Self.Helper.DirectoryPath;
		foreach (var getter in StreamGetters) {
			var path = Path.Combine(prefix, $"{name}{getter.Extension}");
			if (File.Exists(path)) {
				return getter.Getter(path);
			}
		}

		return null;
	}

	private static string GetTrackName(string name, int index) => $"MusicMaster_{name}_[{index}]";

	internal Track(string name, params string[] names) {
		TrackCue GetCueInner(int index) {
			string track = names[index];

			using var audioStream = GetAudioStream(track);
			if (audioStream is null) {
				throw new($"Could not load sound file for '{track}' ({name})");
			}

			using var dataStream = new MemoryStream();
			audioStream.CopyTo(dataStream);

			var effect = new SoundEffect(
				dataStream.GetBuffer(),
				sampleRate: audioStream.Frequency,
				audioStream.Channels
			) {
				Name = name
			};
			if (effect is null) {
				throw new($"Could not load sound file for '{track}' ({name})");
			}

			var instance = effect.CreateInstance();
			instance.IsLooped = true;

			return new(this, instance);
		}

		var result = new TrackCue[names.Length];
		for (int i = 0; i < names.Length; ++i) {
			result[i] = GetCueInner(i);
		}

		Name = name;
		Tracks = result;
	}
}

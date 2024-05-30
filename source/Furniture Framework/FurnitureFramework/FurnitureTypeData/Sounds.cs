/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/FurnitureFramework
**
*************************************************/

using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;

namespace FurnitureFramework
{
	class Sounds
	{
		private enum SoundMode {
			on_turn_on,
			on_turn_off,
			on_click
		}

		#region SoundData

		private struct SoundData
		{
			public readonly bool is_valid = false;
			public readonly string error_msg = "No error";

			public readonly SoundMode mode;
			public readonly string cue_name = "";

			public SoundData(JObject sound_obj)
			{
				string mode_name = JsonParser.parse(sound_obj.GetValue("Mode"), "on_click");
				mode = Enum.Parse<SoundMode>(mode_name);
				if (!Enum.IsDefined(mode))
				{
					error_msg = "Invalid sound Mode.";
					return;
				}

				cue_name = JsonParser.parse(sound_obj.GetValue("Name"), "coin");
				if (!Game1.soundBank.Exists(cue_name))
				{
					error_msg = "Invalid sound Name.";
					return;
				}

				is_valid = true;
			}
		}

		#endregion

		List<SoundData> sound_list = new();

		#region Sounds Parsing

		public Sounds(JToken? sounds_token)
		{
			if (sounds_token is null || sounds_token.Type == JTokenType.Null) return;
			if (sounds_token is not JArray sounds_arr)
			{
				ModEntry.log(
					$"Sounds at {sounds_token.Path} is invalid, must be a list of sounds.",
					LogLevel.Warn
				);
				return;
			}
			
			foreach (JToken sound_token in sounds_arr)
			{
				if (sound_token is JObject sound_obj)
				{
					SoundData new_sound = new(sound_obj);
					if (!new_sound.is_valid)
					{
						ModEntry.log($"Invalid Sound at {sound_obj.Path}:", LogLevel.Warn);
						ModEntry.log($"\t{new_sound.error_msg}", LogLevel.Warn);
						ModEntry.log("Skipping sound.", LogLevel.Warn);
						continue;
					}
					sound_list.Add(new_sound);
				}
			}
		}

		#endregion

		#region Sounds Methods

		public void play(GameLocation location, bool? state = null)
		{
			bool turn_on = state.HasValue && state.Value;
			bool turn_off = state.HasValue && !state.Value;
			foreach (SoundData sound in sound_list)
			{
				if (
					sound.mode == SoundMode.on_click ||
					(sound.mode == SoundMode.on_turn_on && turn_on) ||
					(sound.mode == SoundMode.on_turn_off && turn_off)
				)
				{
					location.playSound(sound.cue_name);
				}
			}
		}

		#endregion
	}
}
/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/FurnitureFramework
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;

namespace FurnitureFramework
{

	class Seats
	{
		#region SeatData

		private class SeatData
		{
			public readonly bool is_valid = false;
			public readonly string? error_msg;

			public readonly Vector2 position = new();
			public readonly int player_dir;
			Depth? depth;

			#region SeatData Parsing

			// When SeatData can have directional source rects
			public static List<SeatData?> make_seats(
				JObject seat_obj, List<string> rot_names
			)
			{
				List<SeatData?> result = new();
				JToken? p_dir_token = seat_obj.GetValue("Player Direction");

				int player_dir = 0;
				if (JsonParser.try_parse(p_dir_token, ref player_dir))
				{
					SeatData seat = new(seat_obj, player_dir);
					result.AddRange(Enumerable.Repeat(seat, rot_names.Count));
					return result;
				}

				List<int?> p_dirs = new();
				if (JsonParser.try_parse(p_dir_token, rot_names, ref p_dirs))
				{
					foreach (int? p_dir_ in p_dirs)
					{
						if (p_dir_ is null) result.Add(null);
						else result.Add(new(seat_obj, p_dir_.Value));
					}
					return result;
				}
				ModEntry.log($"{p_dirs.Count}");

				throw new InvalidDataException("Missing or invalid Player Direction.");
			}
			
			// When SeatData cannot have directional source rects
			public static SeatData make_seat(JObject seat_obj)
			{
				int p_dir = 0;
				if (JsonParser.try_parse(seat_obj.GetValue("Player Direction"), ref p_dir))
				{
					return new(seat_obj, p_dir);
				}

				throw new InvalidDataException("Missing or invalid Player Direction.");
			}

			private SeatData(JObject seat_obj, int player_dir)
			{
				this.player_dir = player_dir;

				// Parsing required seat draw position
				if (!JsonParser.try_parse(seat_obj.GetValue("Position"), ref position))
				{
					error_msg = "Missing or invalid Position.";
					return;
				}

				// Parsing optional player depth
				try { depth = new(seat_obj.GetValue("Depth")); }
				catch (InvalidDataException) { depth = null; }

				is_valid = true;
			}

			#endregion

			#region SeatData Methods

			public float get_player_depth(float top)
			{
				if (depth is null) return -1;	// keep default depth
				else return depth.get_value(top);
			}

			#endregion
		}

		#endregion

		public bool has_seats {get; private set;} = false;
		List<List<SeatData>> seats = new();

		#region Seats Parsing

		public static Seats make_seats(JToken? token, List<string> rot_names)
		{
			int rot_count = 1;
			bool directional = false;
			if (rot_names.Count > 0)
			{
				rot_count = rot_names.Count;
				directional = true;
			}
			Seats result = new(rot_count);

			if (token is JArray seats_arr)
			{
				foreach (JToken seat_token in seats_arr)
				{
					if (seat_token is not JObject seat_obj) continue;
					if (directional)
						result.add_seats(seat_obj, rot_names);
					else
						result.add_seat(seat_obj);
				}
			}

			else if (directional && token is JObject dir_seats_obj)
			{
				foreach ((string key, int rot) in rot_names.Select((value, index) => (value, index)))
				{
					JToken? dir_seats_tok = dir_seats_obj.GetValue(key);
					if (dir_seats_tok is not JArray dir_seats_arr) continue;

					foreach (JToken seat_token in dir_seats_arr)
					{
						if (seat_token is not JObject seat_obj) continue;
						result.add_seat(seat_obj, rot);
					}
				}
			}

			return result;
		}

		private void add_seats(JObject seat_obj, List<string> rot_names)
		{
			List<SeatData?> list;
			try
			{
				list = SeatData.make_seats(seat_obj, rot_names);
			}
			catch (InvalidDataException ex)
			{
				ModEntry.log($"Invalid seat at {seat_obj.Path}:", LogLevel.Warn);
				ModEntry.log($"\t{ex.Message}", LogLevel.Warn);
				ModEntry.log("Skipping Seat.", LogLevel.Warn);
				return;
			}
			
			foreach ((SeatData? seat, int rot) in list.Select((value, index) => (value, index)))
			{
				if (seat is null) continue;
				if (!seat.is_valid)
				{
					ModEntry.log($"Invalid seat at {seat_obj.Path}->{rot_names[rot]}:", LogLevel.Warn);
					ModEntry.log($"\t{seat.error_msg}", LogLevel.Warn);
					ModEntry.log("Skipping Seat.", LogLevel.Warn);
					continue;
				}
				seats[rot].Add(seat);
				has_seats = true;
			}
		}

		private void add_seat(JObject seat_obj, int? rot = null)
		{
			SeatData seat;
			try
			{
				seat = SeatData.make_seat(seat_obj);
			}
			catch (InvalidDataException ex)
			{
				ModEntry.log($"Invalid seat at {seat_obj.Path}:", LogLevel.Warn);
				ModEntry.log($"\t{ex.Message}", LogLevel.Warn);
				ModEntry.log("Skipping Seat.", LogLevel.Warn);
				return;
			}

			if (!seat.is_valid)
			{
				ModEntry.log($"Invalid seat at {seat_obj.Path}:", LogLevel.Warn);
				ModEntry.log($"\t{seat.error_msg}", LogLevel.Warn);
				ModEntry.log("Skipping Seat.", LogLevel.Warn);
				return;
			}

			if (rot is null)
			{
				foreach (List<SeatData> seat_list in seats)
				{
					seat_list.Add(seat);
				}
			}
			else seats[rot.Value].Add(seat);

			has_seats = true;
		}

		private Seats(int rot_count)
		{
			for (int i = 0; i < rot_count; i++)
				seats.Add(new()); 
		}

		#endregion
	
		#region Seats Methods

		public void get_seat_positions(int rot, Vector2 tile_pos, List<Vector2> list)
		{
			if (!has_seats) return;

			foreach (SeatData seat in seats[rot])
			{
				list.Add(tile_pos + seat.position);
			}
		}

		public int get_sitting_direction(int rot, int seat_index)
		{
			if (!has_seats) return -1;

			return seats[rot][seat_index].player_dir;
		}

		public float get_sitting_depth(int rot, int seat_index, float top)
		{
			if (!has_seats) return -1;
			
			return seats[rot][seat_index].get_player_depth(top);
		}

		#endregion
	}
}
/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/FurnitureFramework
**
*************************************************/


using StardewModdingAPI;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Extensions;

namespace FurnitureFramework
{
	class Layers
	{
		#region LayerData

		private class LayerData
		{
			public readonly bool is_valid = false;
			public readonly string? error_msg = "No error";

			SeasonalTexture texture;
			Rectangle source_rect;

			Vector2 draw_pos = Vector2.Zero;
			readonly Depth depth = new(0, 1000);
			
			#region LayerData Parsing

			// When LayerData can have directional source rects
			public static List<LayerData?> make_layers(
				JObject layer_obj, SeasonalTexture source_texture,
				Point rect_offset, List<string> rot_names
			)
			{
				List<LayerData?> result = new();
				JToken? rect_token = layer_obj.GetValue("Source Rect");

				if (JsonParser.try_parse(rect_token, out Rectangle source_rect))
				{
					source_rect.Location += rect_offset;
					LayerData layer = new(layer_obj, source_texture, source_rect);
					result.AddRange(Enumerable.Repeat(layer, rot_names.Count));
					return result;
				}

				List<Rectangle?> source_rects = new();
				if (JsonParser.try_parse(rect_token, rot_names, ref source_rects))
				{
					foreach (Rectangle? rect_ in source_rects)
					{
						if (rect_ is null) result.Add(null);
						else {
							Rectangle rect = rect_.Value;
							rect.Location += rect_offset;
							result.Add(new(layer_obj, source_texture, rect));
						}
					}
					return result;
				}

				throw new InvalidDataException("Missing or invalid Source Rect.");
			}
			
			// When LayerData cannot have directional source rects
			public static LayerData make_layer(
				JObject layer_obj, SeasonalTexture source_texture, Point rect_offset
			)
			{
				if (JsonParser.try_parse(layer_obj.GetValue("Source Rect"), out Rectangle source_rect))
				{
					source_rect.Location += rect_offset;
					return new(layer_obj, source_texture, source_rect);
				}

				throw new InvalidDataException("Missing or invalid Source Rect.");
			}

			private LayerData(JObject layer_obj, SeasonalTexture source_texture, Rectangle rect)
			{
				texture = source_texture;
				source_rect = rect;

				// Parsing optional layer draw position

				JToken? pos_token = layer_obj.GetValue("Draw Pos");
				JsonParser.try_parse(pos_token, ref draw_pos);
				draw_pos *= 4f;	// game rendering scale

				// Parsing optional layer depth

				try { depth = new(layer_obj.GetValue("Depth")); }
				catch (InvalidDataException) { }

				is_valid = true;
			}

			#endregion

			#region LayerData Methods

			public void draw(
				SpriteBatch sprite_batch, Color color,
				Vector2 position, float top,
				bool is_on, Point c_anim_offset
			)
			{
				Rectangle rect = source_rect.Clone();
				if (is_on)
					rect.X += rect.Width;
				rect.Location += c_anim_offset;

				sprite_batch.Draw(
					texture.get_texture(), position + draw_pos, rect,
					color, 0f, Vector2.Zero, 4f, SpriteEffects.None,
					depth.get_value(top)
				);
			}

			#endregion
		}

		#endregion

		public bool has_layers {get; private set;} = false;
		List<List<LayerData>> layers = new();

		#region Layers Parsing

		public static Layers make_layers(JToken? token, List<string> rot_names, SeasonalTexture texture, Point rect_offset)
		{
			int rot_count = 1;
			bool directional = false;
			if (rot_names.Count > 0)
			{
				rot_count = rot_names.Count;
				directional = true;
			}
			Layers result = new(rot_count);

			if (token is JArray layers_arr)
			{
				foreach (JToken layer_token in layers_arr)
				{
					if (layer_token is not JObject layer_obj) continue;
					if (directional)
						result.add_layers(layer_obj, texture, rect_offset, rot_names);
					else
						result.add_layer(layer_obj, texture, rect_offset);
				}
			}

			else if (directional && token is JObject dir_layers_obj)
			{
				foreach ((string key, int rot) in rot_names.Select((value, index) => (value, index)))
				{
					JToken? dir_layers_tok = dir_layers_obj.GetValue(key);
					if (dir_layers_tok is not JArray dir_layers_arr) continue;

					foreach (JToken layer_token in dir_layers_arr)
					{
						if (layer_token is not JObject layer_obj) continue;
						result.add_layer(layer_obj, texture, rect_offset, rot);
					}
				}
			}

			return result;
		}

		private void add_layers(JObject layer_obj, SeasonalTexture texture, Point rect_offset, List<string> rot_names)
		{
			List<LayerData?> list;
			try
			{
				list = LayerData.make_layers(layer_obj, texture, rect_offset, rot_names);
			}
			catch (InvalidDataException ex)
			{
				ModEntry.log($"Invalid layer at {layer_obj.Path}:", LogLevel.Warn);
				ModEntry.log($"\t{ex.Message}", LogLevel.Warn);
				ModEntry.log("Skipping Layer.", LogLevel.Warn);
				return;
			}
			
			foreach ((LayerData? layer, int rot) in list.Select((value, index) => (value, index)))
			{
				if (layer is null) continue;
				if (!layer.is_valid)
				{
					ModEntry.log($"Invalid layer at {layer_obj.Path}->{rot_names[rot]}:", LogLevel.Warn);
					ModEntry.log($"\t{layer.error_msg}", LogLevel.Warn);
					ModEntry.log("Skipping Layer.", LogLevel.Warn);
					continue;
				}
				layers[rot].Add(layer);
				has_layers = true;
			}
		}

		private void add_layer(JObject layer_obj, SeasonalTexture texture, Point rect_offset, int? rot = null)
		{
			LayerData layer;
			try
			{
				layer = LayerData.make_layer(layer_obj, texture, rect_offset);
			}
			catch (InvalidDataException ex)
			{
				ModEntry.log($"Invalid layer at {layer_obj.Path}:", LogLevel.Warn);
				ModEntry.log($"\t{ex.Message}", LogLevel.Warn);
				ModEntry.log("Skipping Layer.", LogLevel.Warn);
				return;
			}

			if (!layer.is_valid)
			{
				ModEntry.log($"Invalid layer at {layer_obj.Path}:", LogLevel.Warn);
				ModEntry.log($"\t{layer.error_msg}", LogLevel.Warn);
				ModEntry.log("Skipping Layer.", LogLevel.Warn);
				return;
			}

			if (rot is null)
			{
				foreach (List<LayerData> layer_list in layers)
				{
					layer_list.Add(layer);
				}
			}
			else layers[rot.Value].Add(layer);

			has_layers = true;
		}

		private Layers(int rot_nb)
		{
			for (int i = 0; i < rot_nb; i++)
				layers.Add(new());
		}

		#endregion

		#region Layers Methods

		public void draw(
			SpriteBatch sprite_batch, Color color,
			Vector2 position, float top,
			int rot, bool is_on, Point c_anim_offset
		)
		{
			if (!has_layers) return;

			foreach (LayerData layer in layers[rot])
			{
				layer.draw(sprite_batch, color, position, top, is_on, c_anim_offset);
			}
		}

		#endregion
	}

}
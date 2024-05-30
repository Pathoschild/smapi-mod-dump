/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/


using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using StardewValley;

namespace Leclair.Stardew.Common;

public record TargetTileFilter(string Property, string? Value);

public record TargetPosition(GameLocation Location, Vector2? Position, int Radius);

public class ArgumentParser {

	public static ArgumentParser New() {
		return new ArgumentParser();
	}


	#region Type Conversion

	public delegate bool ArgumentTypeParserDelegate<TValue>(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out TValue? parsed);

	private delegate bool InternalTypeParserDelegate(Type type, string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out object? parsed);

	private static readonly Dictionary<Type, InternalTypeParserDelegate> TypeConverters = new();

	private static readonly Dictionary<Type, string> UsageArgumentLabel = new();

	public static bool RegisterConverter<TValue>(ArgumentTypeParserDelegate<TValue> converter, string? usageLabel = null) {
		bool WrappedConverter(Type type, string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out object? parsed) {
			if (!type.IsAssignableFrom(typeof(TValue))) {
				consumed = 1;
				error = $"Cannot use {typeof(TValue)} converter to convert {type}.";
				parsed = default;
				return false;
			}
			if (converter(input, index, out consumed, out error, out var value)) {
				parsed = value;
				return true;
			} else {
				parsed = default;
				return false;
			}
		}

		bool result = TypeConverters.TryAdd(typeof(TValue), WrappedConverter);
		if (result && !string.IsNullOrEmpty(usageLabel))
			UsageArgumentLabel[typeof(TValue)] = usageLabel;

		return result;
	}

	public static bool TryConvert<TValue>(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out TValue? parsed) {
		if (TryConvert(typeof(TValue), input, index, out consumed, out error, out object? value)) {
			parsed = (TValue) value;
			return true;
		} else {
			parsed = default;
			return false;
		}
	}

	public static bool TryConvert(Type type, string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out object? parsed) {
		if (!TypeConverters.TryGetValue(type, out var converter)) {
			if (type.IsEnum) {
				converter = ParseEnum;
			} else {
				// TODO: Check for static method to parse from string / ctor?

				error = $"No converter available for type {type}";
				parsed = default;
				consumed = 1;
				return false;
			}
		}

		try {
			return converter(type, input, index, out consumed, out error, out parsed);
		} catch (Exception ex) {
			error = $"Error invoking converter for type {type}: {ex}";
			parsed = default;
			consumed = 1;
			return false;
		}
	}

	static ArgumentParser() {
		RegisterConverter<int>(ParseInt, "<number>");
		RegisterConverter<long>(ParseLong, "<number>");
		RegisterConverter<float>(ParseFloat, "<number>");
		RegisterConverter<double>(ParseDouble, "<number>");
		RegisterConverter<bool>(ParseBool, "<true/false>");
		RegisterConverter<string>(ParseString, "<string>");
		RegisterConverter<Point>(ParsePoint, "<x:number> <y:number>");
		RegisterConverter<Rectangle>(ParseRectangle, "<x:number> <y:number> <width:number> <height:number>");
		RegisterConverter<Vector2>(ParseVector2, "<x:number> <y:number>");
		RegisterConverter<string[]>(ParseRemainder);
		RegisterConverter<IEnumerable<GameLocation>>(ParseLocationContext, "<Location/Context> <Any/Here/ID>");
		RegisterConverter<Farmer>(ParseFarmer, "<Current/Host/Target/ID>");
		RegisterConverter<ParsedFarmers>(ParseFarmers, "<Any/All/Current/Host/Target/ID>");
		RegisterConverter<Color>(ParseColor, "<color>");
		RegisterConverter<IEnumerable<TargetPosition>>(ParseTargetPosition, "<target>");
		RegisterConverter<TargetTileFilter>(ParseTargetTileFilter, "<target-filter>");
	}

	#region Converters

	private enum TargetTileFilterType {
		HasProperty,
		PropertyValue
	}

	private static bool ParseTargetTileFilter(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out TargetTileFilter? value) {

		if (!TryConvert<TargetTileFilterType>(input, index, out consumed, out error, out var type)) {
			value = default;
			return false;
		}

		if (type == TargetTileFilterType.HasProperty) {
			consumed = 2;

			if (!TryConvert(input, index + 1, out _, out error, out string? propertyName)) {
				value = default;
				return false;
			}

			value = new(propertyName, null);
			return true;
		}

		error = "Unsupported type";
		value = default;
		return false;
	}

	private enum TargetType {
		Location,
		Context,
		Player,
		NPC,
		Tile,
		RandomTile
	};

	private static bool ParseTargetPosition(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out IEnumerable<TargetPosition>? value) {

		if (!TryConvert<TargetType>(input, index, out consumed, out error, out var type)) {
			value = default;
			return false;
		}

		// For Location and Context, we just fall back to LocationContext.
		if (type == TargetType.Location || type == TargetType.Context) {
			if (!ParseLocationContext(input, index, out consumed, out error, out var val)) {
				value = default;
				return false;
			}

			value = val.Select(x => new TargetPosition(x, null, 1));
			return true;
		}

		int radius;

		if (type == TargetType.Tile || type == TargetType.RandomTile) {
			// Inputs:
			// Tile [location] [x] [y] [radius]
			// RandomTile [location] [minCount] [maxCount] [minX] [maxX] [minY] [maxY] [minRadius] [maxRadius]
			consumed = type == TargetType.Tile
				? 5
				: 10;

			if (index + consumed > input.Length) {
				error = $"Encountered end of input attempting to read {typeof(TargetType).GetEnumName(type)}";
				value = default;
				return false;
			}

			if (!TryConvert<string>(input, index + 1, out _, out error, out string? locationName)) {
				error = $"Unable to parse {typeof(TargetType).GetEnumName(type)} target: {error}";
				value = default;
				return false;
			}

			IEnumerable<GameLocation> locations;
			if (locationName.Equals("Indoors", StringComparison.OrdinalIgnoreCase) || locationName.Equals("Inside", StringComparison.OrdinalIgnoreCase)) {
				locations = CommonHelper.EnumerateLocations(true)
					.Where(loc => !loc.IsOutdoors);

			} else if (locationName.Equals("Outdoors", StringComparison.OrdinalIgnoreCase) || locationName.Equals("Outside", StringComparison.OrdinalIgnoreCase)) {
				locations = CommonHelper.EnumerateLocations()
					.Where(loc => loc.IsOutdoors);

			} else if (locationName.Equals("Here", StringComparison.OrdinalIgnoreCase) || locationName.Equals("Current", StringComparison.OrdinalIgnoreCase)) {
				if (Game1.currentLocation is null) {
					value = [];
					return true;
				}

				locations = [Game1.currentLocation];

			} else if (locationName.Equals("Any", StringComparison.OrdinalIgnoreCase) || locationName.Equals("All", StringComparison.OrdinalIgnoreCase))
				locations = CommonHelper.EnumerateLocations();

			else {
				var loc = Game1.getLocationFromName(locationName);
				if (loc is null) {
					error = $"Could not find location with name '{locationName}'";
					value = default;
					return false;
				}

				locations = [loc];
			}

			if (type == TargetType.Tile) {
				if (!TryConvert(input, index + 2, out _, out error, out int x) ||
					!TryConvert(input, index + 3, out _, out error, out int y) ||
					!TryConvert(input, index + 4, out _, out error, out radius)
				) {
					error = $"Unable to parse Tile target: {error}";
					value = default;
					return false;
				}

				Vector2 pos = new Vector2(x, y);
				value = locations.Select(x => new TargetPosition(x, pos, radius));
				return true;

			} else {
				if (!TryConvert(input, index + 2, out _, out error, out int minCount) ||
					!TryConvert(input, index + 3, out _, out error, out int maxCount) ||
					!TryConvert(input, index + 4, out _, out error, out int minX) ||
					!TryConvert(input, index + 5, out _, out error, out int maxX) ||
					!TryConvert(input, index + 6, out _, out error, out int minY) ||
					!TryConvert(input, index + 7, out _, out error, out int maxY) ||
					!TryConvert(input, index + 8, out _, out error, out int minRadius) ||
					!TryConvert(input, index + 9, out _, out error, out int maxRadius)
				) {
					error = $"Unable to parse RandomTile target: {error}";
					value = default;
					return false;
				}

				if (minX < 0)
					minX = 0;
				if (minY < 0)
					minY = 0;

				if (maxX < minX)
					maxX = minX;
				if (maxY < minY)
					maxY = minY;

				maxX++;
				maxY++;

				value = locations.Select(x => {
					int count = Game1.random.Next(minCount, maxCount);
					int mX = Math.Min(maxX, x.Map?.Layers?[0]?.LayerWidth ?? maxX);
					int mY = Math.Min(maxY, x.Map?.Layers?[0]?.LayerHeight ?? maxY);

					TargetPosition[] positions = new TargetPosition[count];
					for (int i = 0; i < count; i++) {
						Vector2 pos = new(Game1.random.Next(minX, mX + 1), Game1.random.Next(minY, mY + 1));
						radius = Game1.random.Next(minRadius, maxRadius + 1);
						positions[i] = new(x, pos, radius);
					}

					return positions;
				}).SelectMany(x => x);

				return true;
			}
		}

		if (type != TargetType.Player && type != TargetType.NPC) {
			error = $"Invalid type: {type}";
			value = default;
			return false;
		}

		// Input:
		// Player [All/Current/Host/ID] [radius]
		// NPC [All/Name] [radius]

		consumed = 3;

		if (index + consumed > input.Length) {
			error = $"Encountered end of input attempting to read {typeof(TargetType).GetEnumName(type)}";
			value = default;
			return false;
		}

		if (!TryConvert<string>(input, index + 1, out _, out error, out string? targetName) ||
			!TryConvert<int>(input, index + 2, out _, out error, out radius)
		) {
			error = $"Unable to parse {typeof(TargetType).GetEnumName(type)} target: {error}";
			value = default;
			return false;
		}

		if (type == TargetType.Player) {
			if (targetName.Equals("All", StringComparison.OrdinalIgnoreCase) || targetName.Equals("Any", StringComparison.OrdinalIgnoreCase)) {
				value = Game1.getOnlineFarmers()
					.Where(farmer => farmer?.currentLocation is not null)
					.Select(farmer => new TargetPosition(farmer.currentLocation, farmer.Tile, radius));
				return true;

				// TODO: Figure out a better way to make this useful.
				/*} else if (targetName.Equals("Indoors", StringComparison.OrdinalIgnoreCase) || targetName.Equals("Inside", StringComparison.OrdinalIgnoreCase)) {
					value = Game1.getOnlineFarmers()
						.Where(farmer => !(farmer?.currentLocation?.IsOutdoors ?? true))
						.Select(farmer => new TargetPosition(farmer.currentLocation, farmer.Tile, radius));
					return true;

				} else if (targetName.Equals("Outdoors", StringComparison.OrdinalIgnoreCase) || targetName.Equals("Outside", StringComparison.OrdinalIgnoreCase)) {
					value = Game1.getOnlineFarmers()
						.Where(farmer => farmer?.currentLocation?.IsOutdoors ?? false)
						.Select(farmer => new TargetPosition(farmer.currentLocation, farmer.Tile, radius));
					return true;*/

			} else if (targetName.Equals("Current", StringComparison.OrdinalIgnoreCase)) {
				if (Game1.player?.currentLocation is null)
					value = [];
				else
					value = [new TargetPosition(Game1.player.currentLocation, Game1.player.Tile, radius)];
				return true;

			} else if (targetName.Equals("Host", StringComparison.OrdinalIgnoreCase)) {
				if (Game1.MasterPlayer?.currentLocation is null)
					value = [];
				else
					value = [new TargetPosition(Game1.MasterPlayer.currentLocation, Game1.MasterPlayer.Tile, radius)];
				return true;
			}

			if (!long.TryParse(targetName, out long id) || Game1.getFarmer(id) is not Farmer farmer || farmer.currentLocation is null) {
				error = $"Unable to get farmer with ID: {id}";
				value = default;
				return false;
			}

			value = [new TargetPosition(farmer.currentLocation, farmer.Tile, radius)];
			return true;
		}

		if (targetName.Equals("All", StringComparison.OrdinalIgnoreCase) || targetName.Equals("Any", StringComparison.OrdinalIgnoreCase)) {
			List<TargetPosition> result = [];
			Utility.ForEachCharacter(npc => {
				if (npc.currentLocation != null)
					result.Add(new TargetPosition(npc.currentLocation, npc.Tile, radius));
				return true;
			});
			value = result;
			return true;
		}

		var npc = Game1.getCharacterFromName(targetName);
		if (npc is null || npc.currentLocation is null) {
			error = $"Unable to get character with name '{targetName}'";
			value = default;
			return false;
		}

		value = [new TargetPosition(npc.currentLocation, npc.Tile, radius)];
		return true;
	}


	private enum LocationOrContext {
		Location,
		Context
	};

	private static bool ParseLocationContext(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out IEnumerable<GameLocation>? value) {
		consumed = 2;

		if (!TryConvert<LocationOrContext>(input, index, out _, out error, out var target)) {
			value = default;
			return false;
		}

		if (!ArgUtility.TryGet(input, index + 1, out string? locationName, out error)) {
			value = default;
			return false;
		}

		// Support Current as an alias of Here
		if (locationName.Equals("Current", StringComparison.OrdinalIgnoreCase))
			locationName = "Here";

		// Abort early if we're looking for 'Here' and don't have one.
		if (locationName.Equals("Here", StringComparison.OrdinalIgnoreCase) && Game1.currentLocation is null) {
			value = [];
			return true;
		}

		if (locationName.Equals("Indoors", StringComparison.OrdinalIgnoreCase) || locationName.Equals("Inside", StringComparison.OrdinalIgnoreCase))
			value = CommonHelper.EnumerateLocations(true)
				.Where(loc => !loc.IsOutdoors);

		else if (locationName.Equals("Outdoors", StringComparison.OrdinalIgnoreCase) || locationName.Equals("Outside", StringComparison.OrdinalIgnoreCase))
			value = CommonHelper.EnumerateLocations()
				.Where(loc => loc.IsOutdoors);

		else if (locationName.Equals("All", StringComparison.OrdinalIgnoreCase) || locationName.Equals("Any", StringComparison.OrdinalIgnoreCase))
			value = CommonHelper.EnumerateLocations();

		else if (target == LocationOrContext.Location) {
			if (locationName.Equals("Here", StringComparison.OrdinalIgnoreCase))
				value = [Game1.currentLocation];
			else {
				var loc = Game1.getLocationFromName(locationName);
				if (loc is null) {
					error = $"Could not find location with name '{locationName}'";
					value = null;
					return false;
				}

				value = [loc];
			}

		} else {
			if (locationName.Equals("Here", StringComparison.OrdinalIgnoreCase))
				locationName = Game1.currentLocation.GetLocationContextId();

			else if (!Game1.locationContextData.ContainsKey(locationName)) {
				error = $"Could not find location context with name '{locationName}'";
				value = null;
				return false;
			}

			value = CommonHelper.EnumerateLocations()
				.Where(loc => loc.GetLocationContextId() == locationName);
		}

		return true;
	}

	private enum TargetPlayer {
		Any,
		All,
		Current,
		Host,
		Target
	}

	public static Farmer? TargetFarmer { get; set; }

	private static bool ParseFarmer(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out Farmer? value) {
		consumed = 1;

		if (!ArgUtility.TryGetEnum<TargetPlayer>(input, index, out var target, out error)) {
			if (long.TryParse(input[index], out long id)) {
				if (Game1.getFarmerMaybeOffline(id) is Farmer farmer) {
					error = null;
					value = farmer;
					return true;
				} else {
					error = $"No Farmer with ID '{id}'";
					value = null;
					return false;
				}
			}

			// Set to an invalid value.
			target = (TargetPlayer) int.MaxValue;
		}

		if (target == TargetPlayer.Current)
			value = Game1.player;
		else if (target == TargetPlayer.Host)
			value = Game1.MasterPlayer;
		else if (target == TargetPlayer.Target && TargetFarmer is not null)
			value = TargetFarmer;
		else {
			string targetFarmer = TargetFarmer != null ? ", Target" : "";
			error = $"Unable to parse Farmer. Must be one of: Current, Host{targetFarmer}, [id]";
			value = null;
			return false;
		}

		error = null;
		return true;
	}

	public record ParsedFarmers(bool IsAny, IEnumerable<Farmer> Farmers);

	private static bool ParseFarmers(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out ParsedFarmers? value) {
		consumed = 1;

		if (!ArgUtility.TryGetEnum<TargetPlayer>(input, index, out var target, out error)) {
			if (long.TryParse(input[index], out long id)) {
				if (Game1.getFarmerMaybeOffline(id) is Farmer farmer) {
					error = null;
					value = new(false, [farmer]);
					return true;
				} else {
					error = $"No Farmer with ID '{id}'";
					value = default;
					return false;
				}
			}

			// Set to an invalid value.
			target = (TargetPlayer) int.MaxValue;
		}

		if (target == TargetPlayer.Any || target == TargetPlayer.All)
			value = new(target == TargetPlayer.Any, Game1.getAllFarmers());
		else if (target == TargetPlayer.Current)
			value = new(false, [Game1.player]);
		else if (target == TargetPlayer.Host)
			value = new(false, [Game1.MasterPlayer]);
		else if (target == TargetPlayer.Target && TargetFarmer is not null)
			value = new(false, [TargetFarmer]);
		else {
			string targetFarmer = TargetFarmer != null ? ", Target" : "";
			error = $"Unable to parse Farmer. Must be one of: Current, Host{targetFarmer}, [id]";
			value = default;
			return false;
		}

		error = null;
		return true;
	}

	private static bool ParseEnum(Type type, string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out object? value) {
		if (!type.IsEnum)
			throw new ArgumentException("Type is not enum");

		consumed = 1;

		if (Enum.TryParse(type, input[index], true, out value) && value != null && Enum.IsDefined(type, value)) {
			error = null;
			return true;
		}

		string result = string.Join(", ", Enum.GetNames(type));
		error = $"Unable to parse '{input[index]}' as {type.Name}. Valid: {result}";
		return false;
	}

	private static bool ParseColor(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out Color value) {
		consumed = 1;

		if (CommonHelper.TryParseColor(input[index], out var parsed)) {
			value = parsed.Value;
			error = null;
			return true;
		}

		value = default;
		error = $"Unable to parse '{input[index]}' as color";
		return false;
	}

	private static bool ParseInt(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out int value) {
		consumed = 1;

		if (int.TryParse(input[index], out value)) {
			error = null;
			return true;
		}

		error = $"Unable to parse '{input[index]}' as number";
		return false;
	}

	private static bool ParseFloat(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out float value) {
		consumed = 1;

		if (float.TryParse(input[index], out value)) {
			error = null;
			return true;
		}

		error = $"Unable to parse '{input[index]}' as number";
		return false;
	}

	private static bool ParseLong(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out long value) {
		consumed = 1;

		if (long.TryParse(input[index], out value)) {
			error = null;
			return true;
		}

		error = $"Unable to parse '{input[index]}' as number";
		return false;
	}

	private static bool ParseDouble(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out double value) {
		consumed = 1;

		if (double.TryParse(input[index], out value)) {
			error = null;
			return true;
		}

		error = $"Unable to parse '{input[index]}' as number";
		return false;
	}

	private static bool ParseBool(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out bool value) {
		consumed = 1;

		if (bool.TryParse(input[index], out value)) {
			error = null;
			return true;
		}

		error = $"Unable to parse '{input[index]}' as boolean";
		return false;
	}

	private static bool ParseString(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out string? value) {
		consumed = 1;
		error = null;
		value = input[index];
		return true;
	}

	private static bool ParseRemainder(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out string[]? value) {
		consumed = input.Length - index;
		error = null;

		if (consumed > 0) {
			value = new string[consumed];
			input.CopyTo(value, index);
		} else
			value = Array.Empty<string>();

		return true;
	}

	private static bool ParsePoint(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out Point value) {
		consumed = 2;
		if (index + 1 >= input.Length) {
			error = $"end of input";
			value = default;
			return false;
		}

		if (!ParseInt(input, index, out _, out error, out int x) ||
			!ParseInt(input, index + 1, out _, out error, out int y)
		) {
			error = $"Unable to parse '{input[index]} {input[index + 1]}' as Point: {error}";
			value = default;
			return false;
		}

		error = null;
		value = new Point(x, y);
		return true;
	}

	private static bool ParseVector2(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out Vector2 value) {
		consumed = 2;
		if (index + 1 >= input.Length) {
			error = $"end of input";
			value = default;
			return false;
		}

		if (!ParseFloat(input, index, out _, out error, out float x) ||
			!ParseFloat(input, index + 1, out _, out error, out float y)
		) {
			error = $"Unable to parse '{input[index]} {input[index + 1]}' as Vector2: {error}";
			value = default;
			return false;
		}

		error = null;
		value = new Vector2(x, y);
		return true;
	}

	private static bool ParseRectangle(string[] input, int index, out int consumed, [NotNullWhen(false)] out string? error, [NotNullWhen(true)] out Rectangle value) {
		consumed = 4;
		if (index + 3 >= input.Length) {
			error = $"end of input";
			value = default;
			return false;
		}

		if (!ParseInt(input, index, out _, out error, out int x) ||
			!ParseInt(input, index + 1, out _, out error, out int y) ||
			!ParseInt(input, index + 2, out _, out error, out int width) ||
			!ParseInt(input, index + 3, out _, out error, out int height)
		) {
			error = $"Unable to parse '{input[index]} {input[index + 1]} {input[index + 2]} {input[index + 3]}' as Rectangle: {error}";
			value = default;
			return false;
		}

		error = null;
		value = new Rectangle(x, y, width, height);
		return true;
	}

	#endregion

	#endregion

	public record Argument {

		public Argument(string name, Type type, Action<object> onParse) {
			Name = name;
			Type = type;
			OnParse = onParse;
		}

		public string Name;
		public string? LongName;
		public string? Description;
		public string? Example;

		public bool AllowMultiple;
		public bool IsFlag;
		public bool IsRequired;
		public bool IsFinal;

		public Type Type;

		public Func<object, string?>? Validator;
		public Action<object> OnParse;

	}

	private Argument? LastArg;

	public bool WantsHelp { get; private set; }

	public Dictionary<string, Argument>? Arguments { get; private set; }

	public List<Argument>? PositionalArguments { get; private set; }

	public ArgumentParser AddHelpFlag() {
		var arg = new Argument("-h", typeof(bool), value => WantsHelp = true) {
			LongName = "--help",
			IsFlag = true,
			Description = "View usage information for this action."
		};

		LastArg = arg;

		Arguments ??= new(StringComparer.OrdinalIgnoreCase);
		Arguments.TryAdd("-h", arg);
		Arguments.Add("--help", arg);

		return this;
	}

	public ArgumentParser AddFlag(string shortName, string? longName, Action onParse) {
		var arg = new Argument(shortName, typeof(bool), value => onParse()) {
			LongName = longName,
			IsFlag = true
		};

		LastArg = arg;

		Arguments ??= new(StringComparer.OrdinalIgnoreCase);
		Arguments.Add(shortName, arg);
		if (longName != null)
			Arguments.Add(longName, arg);

		return this;
	}

	public ArgumentParser AddFlag(string shortName, Action onParse) {
		var arg = new Argument(shortName, typeof(bool), value => onParse()) {
			IsFlag = true
		};

		LastArg = arg;

		Arguments ??= new(StringComparer.OrdinalIgnoreCase);
		Arguments.Add(shortName, arg);

		return this;
	}

	public ArgumentParser Add<TValue>(string shortName, Action<TValue> onParse) {
		var arg = new Argument(shortName, typeof(TValue), value => onParse((TValue) value));

		LastArg = arg;

		Arguments ??= new();
		Arguments.Add(shortName, arg);

		return this;
	}

	public ArgumentParser Add<TValue>(string shortName, string? longName, Action<TValue> onParse) {
		var arg = new Argument(shortName, typeof(TValue), value => onParse((TValue) value)) {
			LongName = longName,
		};

		LastArg = arg;

		Arguments ??= new();
		Arguments.Add(shortName, arg);
		if (longName != null)
			Arguments.Add(longName, arg);

		return this;
	}

	public ArgumentParser AddPositional<TValue>(string name, Action<TValue> onParse) {
		var arg = new Argument(name, typeof(TValue), value => onParse((TValue) value));
		LastArg = arg;

		PositionalArguments ??= new();
		PositionalArguments.Add(arg);

		return this;
	}

	public ArgumentParser WithValidation<TValue>(Func<TValue, string> validator) {
		if (LastArg is null)
			throw new ArgumentOutOfRangeException("No previous argument to modify.");

		if (typeof(TValue) != LastArg.Type)
			throw new ArgumentException("Incorrect type for validator.");

		LastArg.Validator = input => validator((TValue) input);
		return this;
	}

	public ArgumentParser WithValidation<TValue>(Func<TValue, bool> validator, string? message = null) {
		if (LastArg is null)
			throw new ArgumentOutOfRangeException("No previous argument to modify.");

		if (typeof(TValue) != LastArg.Type)
			throw new ArgumentException("Incorrect type for validator.");

		LastArg.Validator = input => validator((TValue) input) ? null : message;
		return this;
	}

	public ArgumentParser WithDescription(string description) {
		if (LastArg is null)
			throw new ArgumentOutOfRangeException("No previous argument to modify.");

		LastArg.Description = description;
		return this;
	}

	public ArgumentParser AllowMultiple() {
		if (LastArg is null)
			throw new ArgumentOutOfRangeException("No previous argument to modify.");

		LastArg.AllowMultiple = true;
		return this;
	}

	public ArgumentParser IsRequired() {
		if (LastArg is null)
			throw new ArgumentOutOfRangeException("No previous argument to modify.");

		if (LastArg.IsFlag)
			throw new ArgumentException("Flag arguments cannot be required.");

		LastArg.IsRequired = true;
		return this;
	}

	public ArgumentParser IsFinal() {
		if (LastArg is null)
			throw new ArgumentOutOfRangeException("No previous argument to modify.");

		LastArg.IsFinal = true;
		return this;
	}

	public string Usage {
		get {

			StringBuilder result = new();
			StringBuilder options = new();

			bool had_options = false;

			if (Arguments is not null)
				foreach (var arg in Arguments.Values.Distinct()) {
					string argument;
					if (arg.IsFlag) {
						argument = string.Empty;
					} else {
						if (UsageArgumentLabel.TryGetValue(arg.Type, out string? label))
							argument = label;
						else if (arg.Type.IsEnum)
							argument = "<" + string.Join('/', Enum.GetNames(arg.Type)) + ">";
						else
							argument = "<argument>";
					}

					string name = arg.Name ?? string.Empty;
					if (arg.LongName is not null)
						name = $"{name}, {arg.LongName}";

					options.AppendLine($"    {name} {argument}\t{arg.Description}");

					if (arg.IsRequired)
						result.Append($"{arg.Name ?? arg.LongName} {argument}");
					else
						had_options = true;
				}

			if (PositionalArguments is not null)
				foreach (var arg in PositionalArguments) {
					string argument;
					if (UsageArgumentLabel.TryGetValue(arg.Type, out string? label))
						argument = label;
					else if (arg.Type.IsEnum)
						argument = "<" + string.Join('/', Enum.GetNames(arg.Type)) + ">";
					else
						argument = "<argument>";

					if (arg.AllowMultiple)
						argument += " +";

					if (!arg.IsRequired)
						argument = $"[{argument}]";

					result.Append($"{argument} ");

					// We can't hit any arguments after an AllowMultiple
					if (arg.AllowMultiple)
						break;
				}

			string output = result.ToString();
			if (had_options)
				output = "[options] " + output;

			if (options.Length > 0)
				output = output + "\n  Options:\n" + options.ToString().TrimEnd();

			return output;
		}
	}

	public bool TryParse(string input, [NotNullWhen(false)] out string? error) {
		WantsHelp = false;
		if (!string.IsNullOrEmpty(input))
			return TryParse(ArgUtility.SplitBySpaceQuoteAware(input), out error);

		error = null;
		return true;
	}

	public bool TryParse(string[] input, [NotNullWhen(false)] out string? error) {
		WantsHelp = false;
		int index = 0;
		bool result = TryParse(input, ref index, out error);

		if (result && index < input.Length) {
			error = $"Received unexpected extra argument: {input[index]}";
			return false;
		}

		return result;
	}

	public bool TryParse(string input, ref int index, [NotNullWhen(false)] out string? error) {
		WantsHelp = false;
		if (!string.IsNullOrEmpty(input))
			return TryParse(ArgUtility.SplitBySpaceQuoteAware(input), ref index, out error);

		error = null;
		return true;
	}

	public bool TryParse(string[] input, ref int index, [NotNullWhen(false)] out string? error) {
		WantsHelp = false;
		if ((Arguments?.Count ?? 0) == 0 && (PositionalArguments?.Count ?? 0) == 0) {
			error = null;
			return true;
		}

		int idx = index;
		int positionalIdx = -1;
		Argument? currentArgument = null;

		HashSet<Argument> seenArguments = new();

		if (input != null && input.Length > 0)
			while (idx < input.Length) {

				string token = input[idx];
				if (string.IsNullOrEmpty(token) && idx + 1 == input.Length)
					break;

				// If we can get an argument by name, try doing so.
				if (currentArgument is null && Arguments is not null) {
					// Is this a valid argument?
					if (Arguments.TryGetValue(token, out currentArgument)) {
						if (!seenArguments.Add(currentArgument) && !currentArgument.AllowMultiple) {
							error = $"Received duplicate entry for argument '{currentArgument.LongName ?? currentArgument.Name}'";
							return false;
						}

						// Flags are set by merely being present.
						if (currentArgument.IsFlag) {
							currentArgument.OnParse(true);
							currentArgument = null;
						}

						// We got an argument, so skip to the next token.
						idx++;
						continue;
					}
				}

				if (currentArgument is null && PositionalArguments is not null) {
					// Try using this as a positional argument.
					positionalIdx++;
					if (positionalIdx < PositionalArguments.Count) {
						currentArgument = PositionalArguments[positionalIdx];

						// Don't advance for AllowMultiple arguments.
						if (currentArgument.AllowMultiple)
							positionalIdx--;

						if (!seenArguments.Add(currentArgument) && !currentArgument.AllowMultiple) {
							error = $"Received duplicate entry for argument '{currentArgument.LongName ?? currentArgument.Name}'";
							return false;
						}
					}
				}

				// Still no argument? Danger!
				if (currentArgument is null) {
					error = $"Received unexpected extra argument: {token}";
					return false;
				}

				// Alright, try to parse the argument.
				if (!TryConvert(currentArgument.Type, input, idx, out int consumed, out error, out object? parsed)) {
					error = $"Unable to parse value for argument '{currentArgument.LongName ?? currentArgument.Name}': {error}";
					return false;
				}

				if (currentArgument.Validator != null) {
					string? result = currentArgument.Validator(parsed);
					if (result != null) {
						if (string.IsNullOrEmpty(result))
							result = "invalid valid";
						error = $"Unable to parse value for argument '{currentArgument.LongName ?? currentArgument.Name}': {result}";
						return false;
					}
				}

				currentArgument.OnParse(parsed);
				bool isFinal = currentArgument.IsFinal;
				currentArgument = null;
				idx += consumed;

				if (isFinal)
					break;
			}

		if (WantsHelp) {
			index = idx;
			error = null;
			return true;
		}

		if (currentArgument != null) {
			error = $"Expected value for argument '{currentArgument.LongName ?? currentArgument.Name}', got end of input";
			return false;
		}

		// Check for required arguments.
		if (Arguments is not null)
			foreach (var arg in Arguments.Values) {
				if (seenArguments.Add(arg) && arg.IsRequired) {
					error = $"Missing required argument '{arg.LongName ?? arg.Name}'";
					return false;
				}
			}

		if (PositionalArguments is not null)
			for (int i = 0; i < PositionalArguments.Count; i++) {
				var arg = PositionalArguments[i];
				if (seenArguments.Add(arg) && arg.IsRequired) {
					error = $"Missing required argument '{arg.LongName ?? arg.Name}' at position {i}";
					return false;
				}
			}

		index = idx;

		error = null;
		return true;
	}

}

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
using System.Collections;
using System.Collections.Generic;

using Leclair.Stardew.ThemeManager.VariableSets;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Newtonsoft.Json;

namespace Leclair.Stardew.ThemeManager.Serialization;

public class RealVariableSetConverter : JsonConverter {

	private static readonly Dictionary<Type, string> TypeOwnership = new();

	private readonly string? OtherId;

	public RealVariableSetConverter() { }

	public RealVariableSetConverter(string otherId) {
		OtherId = otherId;
	}

	public override bool CanConvert(Type objectType) {
		if (typeof(IVariableSet).IsAssignableFrom(objectType))
			return true;

		if (OtherId != null)
			return ModEntry.Instance.CanProxy(typeof(IVariableSet), ModEntry.Instance.ModManifest.UniqueID, objectType, OtherId);

		return false;
	}

	public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
		if (reader.TokenType == JsonToken.Null)
			return null;

		if (reader.TokenType != JsonToken.StartObject)
			throw new JsonReaderException($"Cannot parse VariableSet from {reader.TokenType} node (path: {reader.Path}).");

		if (!objectType.IsConstructedGenericType)
			throw new JsonReaderException($"Cannot create IVariableSet directly. Please use IVariableSet<TValue> instead. (Attempting to parse {objectType} from {reader.TokenType} node (path: {reader.Path}).");

		string? modId = OtherId;
		bool is_native = objectType.GetGenericTypeDefinition() == typeof(IVariableSet<>);

		if (!is_native)
			lock ((TypeOwnership as ICollection).SyncRoot) {
				if (TypeOwnership.TryGetValue(objectType, out string? val)) {
					modId = val ?? OtherId;
				} else if (modId != null)
					TypeOwnership[objectType] = modId;
			}

		var types = objectType.GetGenericArguments();
		var ttype = types.Length == 1 ? types[0] : null;

		IVariableSet? result = null;
		Type? outType = null;

		if (ttype == typeof(Color)) {
			result = new ColorVariableSet();
			outType = typeof(IVariableSet<Color>);
		} else if (ttype == typeof(IManagedAsset<SpriteFont>)) {
			result = new FontVariableSet();
			outType = typeof(IVariableSet<IManagedAsset<SpriteFont>>);
		} else if (ttype == typeof(IManagedAsset<Texture2D>)) {
			result = new TextureVariableSet();
			outType = typeof(IVariableSet<IManagedAsset<Texture2D>>);
		} else if (ttype == typeof(IManagedAsset<IBmFontData>)) {
			result = new BmFontVariableSet();
			outType = typeof(IVariableSet<IManagedAsset<IBmFontData>>);
		} else if (TypedVariableSet<object>.CanHandleType(ttype))
			result = TypedVariableSet<object>.CreateInstance(ttype);
		else if (ttype != null && modId != null) {
			// The easy stuff is done. Now we need to start looking into Pintail stuff.

			// Check for IManagedAsset
			if (ttype.IsConstructedGenericType) {
				var ttypes = ttype.GetGenericArguments();
				var matype = ttypes.Length == 1 ? ttypes[0] : null;
				if (matype is not null && ModEntry.Instance.CanProxy(typeof(IManagedAsset<>).MakeGenericType(matype), ttype, modId)) {
					if (matype == typeof(SpriteFont)) {
						result = new FontVariableSet();
						outType = typeof(IVariableSet<IManagedAsset<SpriteFont>>);
					} else if (matype == typeof(Texture2D)) {
						result = new TextureVariableSet();
						outType = typeof(IVariableSet<IManagedAsset<Texture2D>>);
					} else if (matype == typeof(IBmFontData) || (matype != null && ModEntry.Instance.CanProxy<IBmFontData>(matype, modId))) {
						result = new BmFontVariableSet();
						outType = typeof(IVariableSet<IManagedAsset<IBmFontData>>);
					}
				}
			}
		}

		if (result is null)
			throw new JsonReaderException($"Cannot initialize variable set of unknown type {objectType.Name} (path: {reader.Path}).");

		result.RawValues = serializer.Deserialize<Dictionary<string, string>>(reader);

		if (is_native)
			return result;
		else {
			outType ??= typeof(IVariableSet<>).MakeGenericType(ttype!);
			if (modId != null && ModEntry.Instance.TryProxy(result, ModEntry.Instance.ModManifest.UniqueID, objectType, modId, out object? proxy, sourceType: outType)) {
				ModEntry.Instance.CanProxy(objectType, modId, typeof(IVariableSet), ModEntry.Instance.ModManifest.UniqueID);
				return proxy;
			}
			return null;
		}
	}

	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
		if (value is null) {
			writer.WriteNull();
			return;
		}			

		if (value is IVariableSet vs) {
			if (vs.RawValues is not null)
				serializer.Serialize(writer, vs.RawValues);
			else
				writer.WriteNull();
			return;

		}

		string? modId = OtherId;
		Type objectType = value.GetType();

		lock ((TypeOwnership as ICollection).SyncRoot) {
			if (TypeOwnership.TryGetValue(objectType, out string? val)) {
				modId = val ?? OtherId;
			} else if (modId != null)
				TypeOwnership[objectType] = modId;
		}

		if (modId != null && ModEntry.Instance.TryProxyRemote<IVariableSet>(value, modId, out var pvs) && pvs.RawValues is not null) {
			serializer.Serialize(writer, pvs.RawValues);
			return;
		}

		writer.WriteNull();
		return;
	}

}

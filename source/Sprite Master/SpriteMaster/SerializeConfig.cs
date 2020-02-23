using SpriteMaster.Extensions;
using SpriteMaster.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Tomlyn;
using Tomlyn.Syntax;

namespace SpriteMaster {
	internal static class SerializeConfig {
		private const BindingFlags StaticFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

		private static ulong HashClass (Type type) {
			ulong hash = default;

			foreach (var field in type.GetFields(StaticFlags)) {
				hash = Hash.Combine(hash, field.GetValue(null).GetLongHashCode());
			}

			foreach (var child in type.GetNestedTypes(StaticFlags)) {
				hash = Hash.Combine(hash, HashClass(child));
			}

			return hash;
		}

		internal static ulong GetWideHashCode () {
			return HashClass(typeof(Config));
		}

		private static bool Parse (DocumentSyntax Data) {
			try {
				foreach (var table in Data.Tables) {
					string tableName = "";
					try {
						tableName = table.Name.ToString();
						var elements = tableName.Split('.');
						if (elements.Length != 0) {
							elements[0] = null;
						}
						var configClass = typeof(Config);
						string summedClass = configClass.Name.Trim();
						foreach (var element in elements) {
							if (element == null)
								continue;
							var trimmedElement = element.Trim();
							summedClass += $".{trimmedElement}";
							var child = configClass.GetNestedType(trimmedElement, StaticFlags);
							if (child == null || child.IsNestedPrivate || child.GetCustomAttribute<Config.ConfigIgnoreAttribute>() != null)
								throw new InvalidDataException($"Configuration Child Class '{summedClass}' does not exist");
							configClass = child;
						}

						foreach (var value in table.Items) {
							try {
								var keyString = value.Key.ToString().Trim();
								var field = configClass.GetField(keyString, StaticFlags);
								if (field == null || field.IsPrivate || field.IsInitOnly || !field.IsStatic || field.IsLiteral)
									throw new InvalidDataException($"Configuration Value '{summedClass}.{keyString}' does not exist");

								object fieldValue = field.GetValue(null);
								switch (fieldValue) {
									case string v:
										field.SetValue(null, (string)((StringValueSyntax)value.Value).Value.Trim());
										break;
									case sbyte v:
										field.SetValue(null, (sbyte)((IntegerValueSyntax)value.Value).Value);
										break;
									case byte v:
										field.SetValue(null, (byte)((IntegerValueSyntax)value.Value).Value);
										break;
									case short v:
										field.SetValue(null, (short)((IntegerValueSyntax)value.Value).Value);
										break;
									case ushort v:
										field.SetValue(null, (ushort)((IntegerValueSyntax)value.Value).Value);
										break;
									case int v:
										field.SetValue(null, (int)((IntegerValueSyntax)value.Value).Value);
										break;
									case uint v:
										field.SetValue(null, (uint)((IntegerValueSyntax)value.Value).Value);
										break;
									case ulong v:
										field.SetValue(null, unchecked((ulong)((IntegerValueSyntax)value.Value).Value));
										break;
									case float v:
										field.SetValue(null, (float)((FloatValueSyntax)value.Value).Value);
										break;
									case double v:
										field.SetValue(null, (double)((FloatValueSyntax)value.Value).Value);
										break;
									case bool v:
										field.SetValue(null, (bool)((BooleanValueSyntax)value.Value).Value);
										break;
									default:
										if (fieldValue is List<string> slist) {
											var arrayValue = ((ArraySyntax)value.Value).Items;
											var list = new List<string>(arrayValue.ChildrenCount);
											foreach (var obj in arrayValue) {
												var ovalue = obj.Value;
												if (ovalue is StringValueSyntax svalue) {
													list.Add(svalue.Value);
												}
												else if (ovalue is IntegerValueSyntax ivalue) {
													list.Add(ivalue.Value.ToString());
												}
											}
											field.SetValue(null, list);
										}
										else if (fieldValue is List<int> ilist) {
											var arrayValue = ((ArraySyntax)value.Value).Items;
											var list = new List<int>(arrayValue.ChildrenCount);
											foreach (var obj in arrayValue) {
												var ovalue = obj.Value;
												if (ovalue is StringValueSyntax svalue) {
													list.Add(Int32.Parse(svalue.Value));
												}
												else if (ovalue is IntegerValueSyntax ivalue) {
													list.Add((int)ivalue.Value);
												}
											}
											field.SetValue(null, list);
										}
										else if (fieldValue.GetType().IsEnum) {
											var enumNames = fieldValue.GetType().GetEnumNames();
											var values = fieldValue.GetType().GetEnumValues();

											var configValue = ((StringValueSyntax)value.Value).Value.Trim();

											bool found = false;
											foreach (int index in 0..enumNames.Length) {
												if (enumNames[index] == configValue) {
													field.SetValue(null, values.GetValue(index));
													found = true;
													break;
												}
											}
											if (!found)
												throw new InvalidDataException($"Unknown Enumeration Value Type '{summedClass}.{keyString}' = '{value.Value.ToString()}'");
										}
										else {
											throw new InvalidDataException($"Unknown Configuration Value Type '{summedClass}.{keyString}' = '{value.Value.ToString()}'");
										}
										break;
								}
							}
							catch (Exception ex) {
								ex.PrintWarning();
							}
						}
					}
					catch (Exception) {
						throw new InvalidDataException($"Unknown Configuration Table '{tableName}'");
					}
				}
			}
			catch (Exception ex) {
				ex.PrintWarning();
				return false;
			}

			return true;
		}

		private static void SaveClass (Type type, DocumentSyntax document, KeySyntax key = null) {
			key ??= new KeySyntax(type.Name);

			var fields = type.GetFields(StaticFlags);
			var children = type.GetNestedTypes(StaticFlags);

			var table = new TableSyntax(key);
			var tableItems = table.Items;

			foreach (var field in fields) {
				if (field.IsPrivate || field.IsInitOnly || !field.IsStatic || field.IsLiteral)
					continue;

				if (field.GetCustomAttribute<Config.ConfigIgnoreAttribute>() != null) {
					continue;
				}

				ValueSyntax value = null;
				object fieldValue = field.GetValue(null);

				switch (fieldValue) {
					case string v:
						value = new StringValueSyntax(v);
						break;
					case sbyte v:
						value = new IntegerValueSyntax(v);
						break;
					case byte v:
						value = new IntegerValueSyntax(v);
						break;
					case short v:
						value = new IntegerValueSyntax(v);
						break;
					case ushort v:
						value = new IntegerValueSyntax(v);
						break;
					case int v:
						value = new IntegerValueSyntax(v);
						break;
					case uint v:
						value = new IntegerValueSyntax(v);
						break;
					case ulong v:
						value = new IntegerValueSyntax(unchecked((long)v));
						break;
					case float v:
						value = new FloatValueSyntax(v);
						break;
					case double v:
						value = new FloatValueSyntax(v);
						break;
					case bool v:
						value = new BooleanValueSyntax(v);
						break;
					default:
						if (fieldValue is List<string> slist) {
							value = new ArraySyntax(slist.ToArray());
						}
						else if (fieldValue is List<int> ilist) {
							value = new ArraySyntax(ilist.ToArray());
						}
						else if (fieldValue.GetType().IsEnum) {
							value = new StringValueSyntax(fieldValue.GetType().GetEnumName(fieldValue));
						}
						break;
				}

				if (value == null)
					continue;

				var keyValue = new KeyValueSyntax(
					field.Name,
					value
				);

				//if (field.GetAttribute<Config.CommentAttribute>(out var attribute)) {
				//keyValue.GetChildren(Math.Max(0, keyValue.ChildrenCount - 2)).AddComment(attribute.Message);
				//}

				tableItems.Add(keyValue);
			}

			if (table.Items.ChildrenCount != 0) {
				document.Tables.Add(table);
			}

			foreach (var child in children) {
				if (child.IsNestedPrivate)
					continue;
				if (child.GetCustomAttribute<Config.ConfigIgnoreAttribute>() != null) {
					continue;
				}
				var childKey = new KeySyntax(typeof(Config).Name);
				var parentKey = key.ToString().Split('.');
				if (parentKey.Length != 0) {
					parentKey[0] = null;
				}
				foreach (var subKey in parentKey) {
					if (subKey == null)
						continue;
					childKey.DotKeys.Add(new DottedKeyItemSyntax(subKey));
				}
				childKey.DotKeys.Add(new DottedKeyItemSyntax(child.Name));
				SaveClass(child, document, childKey);
			}
		}

		internal static bool Load (MemoryStream stream) {
			try {
				string ConfigText = System.Text.Encoding.UTF8.GetString(stream.ToArray());
				var Data = Toml.Parse(ConfigText);
				return Parse(Data);
			}
			catch (Exception ex) {
				ex.PrintWarning();
				return false;
			}
		}

		internal static bool Load (string ConfigPath) {
			if (!File.Exists(ConfigPath)) {
				return false;
			}

			try {
				string ConfigText = File.ReadAllText(ConfigPath);
				var Data = Toml.Parse(ConfigText, ConfigPath);
				return Parse(Data);
			}
			catch (Exception ex) {
				ex.PrintWarning();
				return false;
			}
		}

		internal static bool Save (MemoryStream stream) {
			try {
				var Document = new DocumentSyntax();

				SaveClass(typeof(Config), Document);

				using var writer = new StreamWriter(stream);
				Document.WriteTo(writer);
				writer.Flush();
			}
			catch (Exception ex) {
				ex.PrintWarning();
				return false;
			}
			return true;
		}

		internal static bool Save (string ConfigPath) {
			try {
				var Document = new DocumentSyntax();

				SaveClass(typeof(Config), Document);

				using var writer = File.CreateText(ConfigPath);
				Document.WriteTo(writer);
				writer.Flush();
			}
			catch (Exception ex) {
				ex.PrintWarning();
				return false;
			}
			return true;
		}
	}
}

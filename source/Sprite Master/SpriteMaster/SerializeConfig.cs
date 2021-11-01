/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

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

		private static void AddTrailingTrivia(this SyntaxNode node, TokenKind kind, string text) {
			node.TrailingTrivia ??= new List<SyntaxTrivia>();
			node.TrailingTrivia.Add(new SyntaxTrivia(kind, text));
		}

		private static void AddLeadingTrivia(this SyntaxNode node, TokenKind kind, string text) {
			node.LeadingTrivia ??= new List<SyntaxTrivia>();
			node.LeadingTrivia.Add(new SyntaxTrivia(kind, text));
		}

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

		private static bool IsValidField(FieldInfo field)
		{
			return !(field == null || field.IsPrivate || field.IsInitOnly || !field.IsStatic || field.IsLiteral || field.HasAttribute<Config.ConfigIgnoreAttribute>());
		}

		// TODO validate that different config elements aren't sharing 'OldName' attributes. That'd be hard to diagnose.

		private static bool Parse (DocumentSyntax Data, bool retain) {
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
							if (child == null || child.IsNestedPrivate || child.HasAttribute<Config.ConfigIgnoreAttribute>())
								throw new InvalidDataException($"Configuration Child Class '{summedClass}' does not exist");
							if (retain && child.HasAttribute<Config.ConfigRetainAttribute>())
							{
								configClass = null;
								break;
							}
							configClass = child;
						}

						if (configClass == null)
						{
							// If this is null, it meant that we are supposed to skip it as that should be the only way to get here.
							if (!retain)
								throw new InvalidOperationException("configClass was null when retain was 'false'");
							continue;
						}

						foreach (var value in table.Items) {
							try {
								var keyString = value.Key.ToString().Trim();
								var field = configClass.GetField(keyString, StaticFlags);
								if (!IsValidField(field))
								{
									var fields = configClass.GetFields(StaticFlags);
									foreach (var subField in fields)
									{
										if (!subField.GetAttribute<Config.ConfigOldNameAttribute>(out var attrib))
										{
											continue;
										}
										if (attrib.Name == keyString && IsValidField(subField))
										{
											field = subField;
											break;
										}
									}
								}
								if (!IsValidField(field))
									throw new InvalidDataException($"Configuration Value '{summedClass}.{keyString}' does not exist");

								if (retain && field.HasAttribute<Config.ConfigRetainAttribute>())
								{
									continue;
								}

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
											foreach (int index in 0.RangeTo(enumNames.Length)) {
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

				if (field.HasAttribute<Config.ConfigIgnoreAttribute>()) {
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

						if (value is ArraySyntax valueSyntax) {
							foreach (var item in valueSyntax.Items) {
								item.AddLeadingTrivia(TokenKind.NewLine, "\n\t");
							}
							if (valueSyntax.Items.ChildrenCount != 0) {
								valueSyntax.CloseBracket.AddLeadingTrivia(TokenKind.NewLine, "\n");
							}
						}
						break;
				}

				if (value == null)
					continue;

				var keyValue = new KeyValueSyntax(
					field.Name,
					value
				);

				if (field.GetAttribute<Config.CommentAttribute>(out var attribute)) {
					keyValue.AddLeadingTrivia(TokenKind.Comment, $"# {attribute.Message}\n");
				}

				tableItems.Add(keyValue);
			}

			if (table.Items.ChildrenCount != 0) {
				if (document.Tables.ChildrenCount != 0) {
					table.AddLeadingTrivia(TokenKind.NewLine, "\n");
				}
				document.Tables.Add(table);
			}

			foreach (var child in children) {
				if (child.IsNestedPrivate)
					continue;
				if (child.HasAttribute<Config.ConfigIgnoreAttribute>()) {
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

		internal static bool Load (MemoryStream stream, bool retain = false) {
			try {
				string ConfigText = System.Text.Encoding.UTF8.GetString(stream.ToArray());
				var Data = Toml.Parse(ConfigText);
				return Parse(Data, retain);
			}
			catch (Exception ex) {
				ex.PrintWarning();
				return false;
			}
		}

		internal static bool Load (string ConfigPath, bool retain = false) {
			if (!File.Exists(ConfigPath)) {
				return false;
			}

			try {
				string ConfigText = File.ReadAllText(ConfigPath);
				var Data = Toml.Parse(ConfigText, ConfigPath);
				return Parse(Data, retain);
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

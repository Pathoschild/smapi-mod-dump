/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Extensions;
using SpriteMaster.Extensions.Reflection;
using SpriteMaster.Hashing;
using SpriteMaster.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Tomlyn;
using Tomlyn.Syntax;

namespace SpriteMaster.Configuration;

internal static class Serialize {
	private const BindingFlags StaticFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

	private const int MaxEnumCommentLength = 80;
	private static void AddTrailingTrivia(this SyntaxNode node, TokenKind kind, string text) {
		node.TrailingTrivia ??= new List<SyntaxTrivia>();
		node.TrailingTrivia.Add(new SyntaxTrivia(kind, text));
	}

	private static void AddLeadingTrivia(this SyntaxNode node, TokenKind kind, string text) {
		node.LeadingTrivia ??= new List<SyntaxTrivia>();
		node.LeadingTrivia.Add(new SyntaxTrivia(kind, text));
	}

	private static ulong HashClass(Type type) {
		ulong hash = default;

		foreach (var field in type.GetFields(StaticFlags)) {
			hash = HashUtility.Combine(hash, field.GetValue(null).GetLongHashCode());
		}

		foreach (var child in type.GetNestedTypes(StaticFlags)) {
			hash = HashUtility.Combine(hash, HashClass(child));
		}

		return hash;
	}

	internal static ulong ConfigHash { get; private set; } = 0UL;

	internal static void RefreshHash() {
		var newHash = HashClass(typeof(Config));
		if (ConfigHash != newHash) {
			ConfigHash = newHash;
			Config.OnConfigChanged();
		}
	}

	private static bool IsClassIgnored(Type? type) => type is not null && (type.HasAttribute<Attributes.IgnoreAttribute>() || IsClassIgnored(type.DeclaringType));

	private static bool IsFieldIgnored(FieldInfo field) => field.HasAttribute<Attributes.IgnoreAttribute>() || IsClassIgnored(field.DeclaringType);

	private static bool IsValidField([NotNullWhen(true)] FieldInfo? field) => !(field is null || field.IsPrivate || field.IsInitOnly || !field.IsStatic || field.IsLiteral || IsFieldIgnored(field));

	// TODO validate that different config elements aren't sharing 'OldName' attributes. That'd be hard to diagnose.

	internal class Category {
		internal readonly Category? Parent;
		internal readonly string Name;
		internal readonly Type Type;
		internal readonly Dictionary<string, Category> Children = new();
		internal readonly Dictionary<string, FieldInfo> Fields = new();

		internal Category(Category? parent, string name, Type type) {
			Parent = parent;
			Name = name;
			Type = type;
		}

		internal ParentTraverseEnumerable ParentTraverser => new(this);

		[StructLayout(LayoutKind.Auto)]
		internal readonly struct ParentTraverseEnumerable : IEnumerable<Category> {
			private readonly Category Current;
			
			internal ParentTraverseEnumerable(Category current) {
				Current = current;
			}

			public ParentTraverseEnumerator GetEnumerator() => new(Current);

			IEnumerator<Category> IEnumerable<Category>.GetEnumerator() => new ParentTraverseEnumerator(Current);

			IEnumerator IEnumerable.GetEnumerator() => new ParentTraverseEnumerator(Current);
		}

		internal struct ParentTraverseEnumerator : IEnumerator<Category> {
			private readonly Category InitialValue;
			private Category? CurrentValue = null;
			private bool PastEnd = false;

			internal ParentTraverseEnumerator(Category current) {
				InitialValue = current;
				CurrentValue = current;
			}

			public Category Current => CurrentValue!;

			object IEnumerator.Current => CurrentValue!;

			public void Dispose() {}

			public bool MoveNext() {
				if (PastEnd) {
					return false;
				}

				CurrentValue = CurrentValue is null ? InitialValue : CurrentValue.Parent;

				if (CurrentValue is not null) {
					return true;
				}

				PastEnd = true;
				return false;
			}

			public void Reset() {
				CurrentValue = InitialValue;
				PastEnd = false;
			}
		}
	}

	internal static readonly Category Root;

	private static bool HasValidFields(Type type) {
		foreach (var field in type.GetFields(StaticFlags)) {
			if (IsFieldIgnored(field)) {
				continue;
			}

			if (field.IsInitOnly || field.IsLiteral) {
				continue;
			}

			if (field.HasAttribute<CompilerGeneratedAttribute>()) {
				continue;
			}

			return true;
		}

		return false;
	}

	static Serialize() {
		Root = new(null, "", typeof(Config));

		void ProcessCategory(Category category) {
			foreach (var field in category.Type.GetFields(StaticFlags)) {
				if (IsFieldIgnored(field)) {
					continue;
				}

				if (field.IsInitOnly || field.IsLiteral) {
					continue;
				}

				if (field.HasAttribute<CompilerGeneratedAttribute>()) {
					continue;
				}

				category.Fields[field.Name.ToLowerInvariant()] = field;
			}
			foreach (var subType in category.Type.GetNestedTypes(StaticFlags)) {
				if (IsClassIgnored(subType) || subType.IsEnum) {
					continue;
				}

				if (subType.HasAttribute<CompilerGeneratedAttribute>()) {
					continue;
				}

				if (!HasValidFields(subType)) {
					continue;
				}

				var subCategory = new Category(category, subType.Name, subType);
				category.Children[subType.Name.ToLowerInvariant()] = subCategory;

				ProcessCategory(subCategory);
			}
		}

		ProcessCategory(Root);
	}

	private static bool Parse(DocumentSyntax data, bool retain) {
		try {
			foreach (var table in data.Tables) {
				if (table.Name is null) {
					continue;
				}

				string tableName = "";
				try {
					tableName = table.Name.ToString();
					string?[] elements = tableName.Split('.');
					if (elements.Length != 0) {
						elements[0] = null;
					}
					var configClass = typeof(Config);
					string summedClass = configClass.Name.Trim();
					foreach (var element in elements) {
						if (element is null) {
							continue;
						}
						var trimmedElement = element.Trim();
						summedClass += $".{trimmedElement}";
						var child = configClass.GetNestedType(trimmedElement, StaticFlags);
						if (child is null || child.IsNestedPrivate || IsClassIgnored(child)) {
							throw new InvalidDataException($"Configuration Child Class '{summedClass}' does not exist");
						}
						if (retain && child.HasAttribute<Attributes.RetainAttribute>()) {
							configClass = null;
							break;
						}
						configClass = child;
					}

					if (configClass is null) {
						// If this is null, it meant that we are supposed to skip it as that should be the only way to get here.
						if (!retain) {
							throw new InvalidOperationException("configClass was null when retain was 'false'");
						}
						continue;
					}

					foreach (var value in table.Items) {
						if (value.Key is null) {
							continue;
						}
						try {
							var keyString = value.Key.ToString().Trim();
							var field = configClass.GetField(keyString, StaticFlags);
							if (!IsValidField(field)) {
								var fields = configClass.GetFields(StaticFlags);
								foreach (var subField in fields) {
									if (!subField.GetAttribute<Attributes.OldNameAttribute>(out var attrib)) {
										continue;
									}
									if (attrib.Name == keyString && IsValidField(subField)) {
										field = subField;
										break;
									}
								}
							}
							if (!IsValidField(field)) {
								throw new InvalidDataException($"Configuration Value '{summedClass}.{keyString}' does not exist");
							}

							if (retain && field.HasAttribute<Attributes.RetainAttribute>()) {
								continue;
							}

							object? fieldValue = field.GetValue(null);
							var switchValue = fieldValue ?? Activator.CreateInstance(field.FieldType);

							switch (switchValue) {
								case string:
									field.SetValue(null, ((StringValueSyntax?)value.Value)?.Value?.Trim().Intern());
									break;
								case sbyte:
									field.SetValue(null, (sbyte?)((IntegerValueSyntax?)value.Value)?.Value);
									break;
								case byte:
									field.SetValue(null, (byte?)((IntegerValueSyntax?)value.Value)?.Value);
									break;
								case short:
									field.SetValue(null, (short?)((IntegerValueSyntax?)value.Value)?.Value);
									break;
								case ushort:
									field.SetValue(null, (ushort?)((IntegerValueSyntax?)value.Value)?.Value);
									break;
								case int:
									field.SetValue(null, (int?)((IntegerValueSyntax?)value.Value)?.Value);
									break;
								case uint:
									field.SetValue(null, (uint?)((IntegerValueSyntax?)value.Value)?.Value);
									break;
								case long:
									field.SetValue(null, (long?)((IntegerValueSyntax?)value.Value)?.Value);
									break;
								case ulong:
									field.SetValue(null, (ulong?)((IntegerValueSyntax?)value.Value)?.Value);
									break;
								case float: {
										if (value.Value is IntegerValueSyntax ivalue) {
											field.SetValue(null, (float)ivalue.Value);
										}
										else {
											field.SetValue(null, (float?)((FloatValueSyntax?)value.Value)?.Value);
										}
									}
									break;
								case double: {
										if (value.Value is IntegerValueSyntax ivalue) {
											field.SetValue(null, (double)ivalue.Value);
										}
										else {
											field.SetValue(null, ((FloatValueSyntax?)value.Value)?.Value);
										}
									}
									break;
								case bool:
									field.SetValue(null, ((BooleanValueSyntax?)value.Value)?.Value);
									break;
								default:
									switch (switchValue) {
										case List<string>: {
												var arrayValue = ((ArraySyntax?)value.Value)?.Items;
												if (arrayValue is null) {
													break;
												}
												var list = new List<string>(arrayValue.ChildrenCount);
												foreach (var obj in arrayValue) {
													var ovalue = obj.Value;
													if (ovalue is StringValueSyntax svalue) {
														if (svalue.Value is not null) {
															list.Add(svalue.Value.Intern());
														}
													}
													else if (ovalue is IntegerValueSyntax ivalue) {
														list.Add(ivalue.Value.ToString().Intern());
													}
												}
												field.SetValue(null, list);
											}
											break;
										case string[]: {
												var arrayValue = ((ArraySyntax?)value.Value)?.Items;
												if (arrayValue is null) {
													break;
												}
												var list = new string[arrayValue.ChildrenCount];
												int i = 0;
												foreach (var obj in arrayValue) {
													var ovalue = obj.Value;
													if (ovalue is StringValueSyntax svalue) {
														if (svalue.Value is not null) {
															list[i++] = svalue.Value.Intern();
														}
													}
													else if (ovalue is IntegerValueSyntax ivalue) {
														list[i++] = ivalue.Value.ToString().Intern();
													}
												}
												Array.Resize(ref list, i);
												field.SetValue(null, list);
											}
											break;
										case List<int>: {
												var arrayValue = ((ArraySyntax?)value.Value)?.Items;
												if (arrayValue is null) {
													break;
												}
												var list = new List<int>(arrayValue.ChildrenCount);
												foreach (var obj in arrayValue) {
													var ovalue = obj.Value;
													if (ovalue is StringValueSyntax svalue) {
														if (svalue.Value is not null) {
															list.Add(int.Parse(svalue.Value));
														}
													}
													else if (ovalue is IntegerValueSyntax ivalue) {
														list.Add((int)ivalue.Value);
													}
												}
												field.SetValue(null, list);
											}
											break;
										case int[]: {
												var arrayValue = ((ArraySyntax?)value.Value)?.Items;
												if (arrayValue is null) {
													break;
												}
												var list = new int[arrayValue.ChildrenCount];
												int i = 0;
												foreach (var obj in arrayValue) {
													var ovalue = obj.Value;
													if (ovalue is StringValueSyntax svalue) {
														if (svalue.Value is not null) {
															list[i++] = int.Parse(svalue.Value);
														}
													}
													else if (ovalue is IntegerValueSyntax ivalue) {
														list[i++] = (int)ivalue.Value;
													}
												}
												field.SetValue(null, list);
											}
											break;
										case Enum: {
												var enumNames = switchValue.GetType().GetEnumNames();
												var values = switchValue.GetType().GetEnumValues();

												var configValue = ((StringValueSyntax?)value.Value)?.Value?.Trim();

												if (configValue is not null) {
													bool found = false;
													for (int index = 0; index < enumNames.Length; ++index) {
														if (enumNames[index] == configValue) {
															field.SetValue(null, values.GetValue(index));
															found = true;
															break;
														}
													}
													if (!found) {
														throw new InvalidDataException($"Unknown Enumeration Value Type '{summedClass}.{keyString}' = '{value.Value}'");
													}
												}
											}
											break;
										default:
											throw new InvalidDataException($"Unknown Configuration Value Type '{summedClass}.{keyString}' = '{value.Value}'");
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

	private static void SaveClass(int depth, Type type, DocumentSyntax document, KeySyntax? key = null) {
		key ??= new(type.Name);
		string indent = new string('\t', depth);

		var fields = type.GetFields(StaticFlags);
		var children = type.GetNestedTypes(StaticFlags);

		var table = new TableSyntax(key);
		var tableItems = table.Items;

		var typeCommentAttributes = type.GetCustomAttributes<Attributes.CommentAttribute>();
		foreach (var attribute in typeCommentAttributes) {
			table.AddLeadingTrivia(TokenKind.Comment, $"{indent}# {attribute.Message}");
		}

		foreach (var field in fields) {
			if (field.IsPrivate || field.IsInitOnly || !field.IsStatic || field.IsLiteral || IsFieldIgnored(field)) {
				continue;
			}

			ValueSyntax? value = null;
			object? fieldValue = field.GetValue(null);

			if (fieldValue is null) {
				continue;
			}

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
				case long v:
					value = new IntegerValueSyntax((long)v);
					break;
				case ulong v:
					value = new IntegerValueSyntax((long)v);
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
					switch (fieldValue) {
						case List<string> stringList:
							value = new ArraySyntax(stringList.ToArray());
							break;
						case string[] stringList:
							value = new ArraySyntax(stringList);
							break;
						case List<int> intList:
							value = new ArraySyntax(intList.ToArray());
							break;
						case int[] intList:
							value = new ArraySyntax(intList);
							break;
						case Enum enumValue:
							var enumName = enumValue.GetType().GetEnumName(fieldValue);
							if (enumName is not null) {
								value = new StringValueSyntax(enumName);
							}
							break;
					}

					if (value is ArraySyntax valueSyntax) {
						foreach (var item in valueSyntax.Items) {
							item.AddLeadingTrivia(TokenKind.NewLine, $"\n\t{indent}");
						}
						if (valueSyntax.Items.ChildrenCount != 0) {
							valueSyntax.CloseBracket?.AddLeadingTrivia(TokenKind.NewLine, "\n");
							if (indent.Length != 0) {
								valueSyntax.CloseBracket?.AddLeadingTrivia(TokenKind.Whitespaces, indent);
							}
						}
					}
					break;
			}

			if (value is null) {
				continue;
			}

			var keyValue = new KeyValueSyntax(
				field.Name,
				value
			);

			var commentAttributes = field.GetCustomAttributes<Attributes.CommentAttribute>();
			foreach (var attribute in commentAttributes) {
				keyValue.AddLeadingTrivia(TokenKind.Comment, $"{indent}# {attribute.Message}\n");
			}

			var subType = field.FieldType.IsEnum ? field.FieldType : field.FieldType.GetElementType();
			if (subType?.IsEnum ?? false) {
				keyValue.AddLeadingTrivia(TokenKind.Comment, $"{indent}# Legal Values: [\n");

				var enumMap = new Dictionary<object, List<string>>();
				foreach (var enumName in Enum.GetNames(subType)) {
					var enumValue = Enum.Parse(subType, enumName);
					if (enumMap.TryGetValue(enumValue, out var enumList)) {
						enumList.Add(enumName);
					}
					else {
						enumMap[enumValue] = new() { enumName };
					}
				}

				var currentLine = new StringBuilder();
				void PushCommentLine() {
					if (currentLine.Length > 0) {
						keyValue.AddLeadingTrivia(TokenKind.Comment, $"{indent}#\t{currentLine}\n");
						currentLine.Clear();
					}
				}
				foreach (var enumNames in enumMap) {
					var enumName = string.Join('/', enumNames.Value);
					if (currentLine.Length > 0) {
						currentLine.Append(", ");
					}
					if (currentLine.Length + enumName.Length >= MaxEnumCommentLength) {
						PushCommentLine();
					}
					currentLine.Append(enumName);
				}
				PushCommentLine();

				keyValue.AddLeadingTrivia(TokenKind.Comment, $"{indent}# ]\n");
			}

			if (indent.Length != 0) {
				keyValue.AddLeadingTrivia(TokenKind.Whitespaces, indent);
			}

			tableItems.Add(keyValue);
		}

		if (table.Items.ChildrenCount != 0) {
			if (document.Tables.ChildrenCount != 0) {
				table.AddLeadingTrivia(TokenKind.NewLine, "\n");
			}
			document.Tables.Add(table);
		}

		if (indent.Length != 0) {
			table.AddLeadingTrivia(TokenKind.Whitespaces, indent);
		}

		foreach (var child in children) {
			if (child.IsNestedPrivate || IsClassIgnored(child)) {
				continue;
			}
			var childKey = new KeySyntax(nameof(Config));

			string?[] parentKey = key.ToString().Split('.');
			if (parentKey.Length != 0) {
				parentKey[0] = null;
			}
			foreach (var subKey in parentKey) {
				if (subKey is null) {
					continue;
				}
				childKey.DotKeys.Add(new(subKey));
			}
			childKey.DotKeys.Add(new(child.Name));
			SaveClass(depth + 1, child, document, childKey);
		}
	}

	internal static bool Load(MemoryStream stream, bool retain = false) {
		try {
			string configText = Encoding.UTF8.GetString(stream.ToArray());
			var data = Toml.Parse(configText);
			return Parse(data, retain);
		}
		catch (Exception ex) {
			ex.PrintWarning();
			return false;
		}
		finally {
			RefreshHash();
		}
	}

	internal static bool Load(string configPath, bool retain = false) {
		if (!File.Exists(configPath)) {
			return false;
		}

		try {
			string configText = File.ReadAllText(configPath);
			var data = Toml.Parse(configText, configPath);
			return Parse(data, retain);
		}
		catch (Exception ex) {
			ex.PrintWarning();
			return false;
		}
		finally {
			RefreshHash();
		}
	}

	internal static bool Save(MemoryStream stream, bool leaveOpen = false) {
		try {
			var document = new DocumentSyntax();

			SaveClass(0, typeof(Config), document);

			using var writer = new StreamWriter(stream, leaveOpen: leaveOpen);
			document.WriteTo(writer);
			writer.Flush();
		}
		catch (Exception ex) {
			ex.PrintWarning();
			return false;
		}
		finally {
			RefreshHash();
		}
		return true;
	}

	internal static bool Save(string configPath) {
		try {
			var document = new DocumentSyntax();

			SaveClass(0, typeof(Config), document);

			using var writer = File.CreateText(configPath);
			document.WriteTo(writer);
			writer.Flush();
		}
		catch (Exception ex) {
			ex.PrintWarning();
			return false;
		}
		finally {
			RefreshHash();
		}
		return true;
	}
}

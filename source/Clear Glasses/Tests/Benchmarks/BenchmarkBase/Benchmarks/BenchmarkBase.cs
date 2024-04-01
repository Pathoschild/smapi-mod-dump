/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using LinqFasterer;
using System.Reflection;

namespace Benchmarks.BenchmarkBase.Benchmarks;

[Orderer(SummaryOrderPolicy.Declared, MethodOrderPolicy.Declared)]
[CsvExporter, HtmlExporter]
[MinColumn, MaxColumn]
public abstract class BenchmarkBase {
}

public abstract class BenchmarkBase<TDataType, TBase> : BenchmarkBase where TDataType : IDataSet<TBase> {
	[JetBrains.Annotations.UsedImplicitly]
	public static List<TDataType> DefaultDataSetsStatic { get; protected set; } = new();

	[JetBrains.Annotations.UsedImplicitly]
	public List<TDataType> DefaultDataSets => DefaultDataSetsStatic;

	private List<TDataType>? GetDataSet(string name) {
		var dataSets = this.GetType()
			.GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
			.WhereF(member => (member is FieldInfo or PropertyInfo) && member.Name.EndsWith("DataSets"));

		foreach (var dataSet in dataSets) {
			var setName = dataSet.Name.Substring(0, dataSet.Name.Length - "DataSets".Length);

			if (setName.Equals(name, StringComparison.InvariantCultureIgnoreCase)) {
				object? value = null;
				switch (dataSet) {
					case FieldInfo field:
						value = field.GetValue(field.IsStatic ? null : this);
						break;
					case PropertyInfo property:
						value = property.GetValue((property.GetGetMethod()?.IsStatic ?? true) ? null : this);
						break;
				}

				if (value is List<TDataType> listValue) {
					return listValue;
				}
			}
		}

		return null;
	}

	[JetBrains.Annotations.UsedImplicitly]
	public List<TDataType> DataSets {
		get {
			if (ProgramBase.CurrentOptions is { } options) {
				switch (options.DataSet.Count) {
					case 0:
						return DefaultDataSetsStatic;
					case 1:
						return GetDataSet(options.DataSet.First()) ?? DefaultDataSetsStatic;
				}

				HashSet<List<TDataType>> dataSetsSet = new(options.DataSet.Count);

				foreach (var setName in options.DataSet) {
					if (GetDataSet(setName) is { } dataSet) {
						dataSetsSet.Add(dataSet);
					}
				}

				if (dataSetsSet.Count == 0) {
					return DefaultDataSetsStatic;
				}

				int sumCount = 0;
				foreach (var dataSet in dataSetsSet) {
					sumCount += dataSet.Count;
				}

				List<TDataType> combinedDataSets = new(sumCount);

				foreach (var dataSet in dataSetsSet) {
					combinedDataSets.AddRange(dataSet);
				}

				return combinedDataSets;
			}

			return DefaultDataSetsStatic;
		}
	}
}

public abstract class BenchmarkBaseImpl<TDataType, TBase> : BenchmarkBase<TDataType, TBase> where TDataType : IDataSet<TBase> {
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Runtime.CompilerServices;
using System.Text;

namespace BoolParse;

public abstract class Common {
	private const int RandSeed = 0x13377113;
	private const int DataSetSize = 0x100_000;

	public static string[] DataSetInterned { get; private set; }
	public static string[] DataSetUninterned { get; private set; }

	public static string[][] DataSets => new[] { DataSetInterned, DataSetUninterned };

	static Common() {
		var words = File.ReadAllLines("D:\\words.txt");

		DataSetInterned = InitializeInternedDataSet(words);
		DataSetUninterned = InitializeUninternedDataSet(words);
	}

	private static volatile string trueString0 = new StringBuilder().Append('t').Append('r').Append('u').Append('e').ToString();
	private static volatile string falseString0 = new StringBuilder().Append('f').Append('a').Append('l').Append('s').Append('e').ToString();
	private static volatile string trueString1 = new StringBuilder().Append('T').Append('r').Append('u').Append('e').ToString();
	private static volatile string falseString1 = new StringBuilder().Append('F').Append('a').Append('l').Append('s').Append('e').ToString();

	private static string[] InitializeInternedDataSet(string[] randomText) {
		var random = new Random(RandSeed);

		var dataSet = new string[DataSetSize];
		for (int i = 0; i < DataSetSize; i++) {
			dataSet[i] = random.Next(6) switch {
				0 => string.Intern(trueString0),
				1 => string.Intern(falseString0),
				2 => string.Intern(trueString1),
				3 => string.Intern(falseString1),
				_ => string.Intern(randomText[random.Next(randomText.Length)])
			};
		}

		return dataSet;
	}

	[CompilationRelaxations(CompilationRelaxations.NoStringInterning)]
	private static string[] InitializeUninternedDataSet(string[] randomText) {
		var random = new Random(RandSeed);

		var dataSet = new string[DataSetSize];
		for (int i = 0; i < DataSetSize; i++) {
			dataSet[i] = random.Next(6) switch {
				0 => trueString0,
				1 => falseString0,
				2 => trueString1,
				3 => falseString1,
				_ => randomText[random.Next(randomText.Length)]
			};
		}

		return dataSet;
	}
}

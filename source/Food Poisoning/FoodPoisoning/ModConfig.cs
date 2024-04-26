/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/seferoni/FoodPoisoning
**
*************************************************/

namespace FoodPoisoning;

#region using directives

using SharedLibrary.Classes;

#endregion

public sealed class ModConfig : ConfigClass
{
	private readonly Dictionary<string, dynamic> _Defaults = new()
	{
		{ "BasePoisoningChance", 30 },
		{ "BaseDuration", 120 },
		{ "HarmfulThreshold", 0 },
		{ "HarmfulChanceOffset", 70 },
		{ "HarmfulDurationOffset", 60 }
	};

	[GMCMIgnore]
	internal override Dictionary<string, dynamic> Defaults
	{
		get
		{
			return _Defaults;
		}
		set{}
	}

	public ModConfig()
	{
		ResetProperties();
	}

	[GMCMRange(0, 100)]
	[GMCMInterval(1)]
	public int BasePoisoningChance { get; set; }

	[GMCMRange(10, 240)]
	[GMCMInterval(2)]
	public int BaseDuration { get; set; }

	[GMCMRange(-300, 100)]
	[GMCMInterval(5)]
	public int HarmfulThreshold { get; set; }

	[GMCMRange(0, 100)]
	[GMCMInterval(1)]
	public int HarmfulChanceOffset { get; set; }

	[GMCMRange(10, 120)]
	[GMCMInterval(2)]
	public int HarmfulDurationOffset { get; set; }
}

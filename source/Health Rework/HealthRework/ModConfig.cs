/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/seferoni/HealthRework
**
*************************************************/

namespace HealthRework;

#region using directives

using SharedLibrary.Classes;

#endregion

public sealed class ModConfig : ConfigClass
{
	private readonly Dictionary<string, object> _Defaults = new()
	{
		{ "HealthRecoveredFromFoodModifier", 0f },
		{ "HealthRecoveredOnSleepOffset", 10 }
	};

	[GMCMIgnore]
	internal override Dictionary<string, object> Defaults
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

	[GMCMRange(0.0f, 1f)]
	[GMCMInterval(0.1f)]
	public float HealthRecoveredFromFoodModifier { get; set; }

	[GMCMRange(0, 100)]
	[GMCMInterval(1)]
	public int HealthRecoveredOnSleepOffset { get; set; }
}

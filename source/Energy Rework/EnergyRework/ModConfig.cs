/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/seferoni/EnergyRework
**
*************************************************/

namespace EnergyRework;

#region using directives

using SharedLibrary.Classes;

#endregion

public sealed class ModConfig : ConfigClass
{
	private readonly Dictionary<string, dynamic> _Defaults = new()
	{
		{ "BaseEnergyLoss", 2f },
		{ "EnergyFloor", 15f },
		{ "MovingEnergyOffset", 3f },
		{ "SittingEnergyOffset", 3f },
		{ "SittingEnergyCeiling", 140f },
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

    [GMCMRange(0f, 15f)]
    [GMCMInterval(1f)]
    public float BaseEnergyLoss { get; set; }

	[GMCMRange(5f, 40f)]
	[GMCMInterval(1f)]
	public float EnergyFloor { get; set; }

    [GMCMRange(0f, 15f)]
    [GMCMInterval(1f)]
    public float MovingEnergyOffset { get; set; }

	[GMCMRange(0f, 15f)]
	[GMCMInterval(1f)]
	public float SittingEnergyOffset { get; set; }

	[GMCMRange(45f, 200f)]
	[GMCMInterval(5f)]
	public float SittingEnergyCeiling { get; set; }
}
/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/qixing-jk/QiXingAutoGrabTruffles
**
*************************************************/

namespace AutoGrabTruffles;

public class ModConfig
{
	public bool EnableAutoGrabTruffles { get; set; } = true;


	public string Collection { get; set; } = "Instantly";


	public bool GainExperience { get; set; } = true;


	public bool ApplyGathererBonus { get; set; } = true;


	public bool ApplyBotanistBonus { get; set; } = true;

}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

namespace Shockah.ProjectFluent
{
	internal interface IFluentValueFactory
	{
		IFluentFunctionValue CreateStringValue(string value);
		IFluentFunctionValue CreateIntValue(int value);
		IFluentFunctionValue CreateLongValue(long value);
		IFluentFunctionValue CreateFloatValue(float value);
		IFluentFunctionValue CreateDoubleValue(double value);
	}
}
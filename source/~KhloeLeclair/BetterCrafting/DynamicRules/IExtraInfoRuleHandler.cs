/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/


using Leclair.Stardew.Common.UI.FlowNode;

namespace Leclair.Stardew.BetterCrafting.DynamicRules;

public interface IExtraInfoRuleHandler {

	IFlowNode[]? GetExtraInfo(object? state);

}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Leclair.Stardew.Common.UI.FlowNode
{
	public interface INodeSlice
	{

		IFlowNode Node { get; }

		float Width { get; }
		float Height { get; }

		bool ForceWrap { get; }

		bool IsEmpty();

	}
}

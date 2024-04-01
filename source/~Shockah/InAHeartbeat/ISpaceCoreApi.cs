/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using System;

namespace Shockah.InAHeartbeat;

public interface ISpaceCoreApi
{
	public event EventHandler<Action<string, Action>> AdvancedInteractionStarted;
}
/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Freaksaus/Tileman-Redux
**
*************************************************/

using System;

namespace TilemanRedux.Exceptions;
public sealed class InvalidOverlayModeException : Exception
{
	public InvalidOverlayModeException(int overlayMode) : base($"Tried to show tile purchase info for invalid overlay mode {overlayMode}")
	{ }
}

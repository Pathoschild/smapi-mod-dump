/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework.Input;

namespace DecidedlyShared.Input;

public record struct KeyWatch(Keys Key, KeyPressType Type, Action? Callback, Action<string>? LogCallback);

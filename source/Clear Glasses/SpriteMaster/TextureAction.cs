/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Types;
using System.Runtime.InteropServices;

namespace SpriteMaster;

[StructLayout(LayoutKind.Auto)]
internal record struct TextureAction(string Name, int Size, ComparableWeakReference<XTexture2D> Reference, Bounds Bounds);

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Metadata;
using SpriteMaster.Types;

namespace SpriteMaster;

internal static class ResourceManager {
	internal static readonly ConcurrentConsumer<ulong> ReleasedTextureMetas =
		new("ReleasedTextureMetas", Texture2DMeta.Cleanup);
	internal static readonly ConcurrentConsumer<ManagedSpriteInstance.CleanupData> ReleasedSpriteInstances =
		new("ReleasedSpriteInstances", ManagedSpriteInstance.Cleanup);
}

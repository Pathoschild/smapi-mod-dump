/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using SpriteMaster.Tasking;
using System.Threading;
using System.Threading.Tasks;

namespace SpriteMaster.Types.MemoryCache;

internal interface ICompressedMemoryCache : ICache {
	private static readonly ThreadedTaskScheduler CompressedMemoryCacheScheduler = new(
		concurrencyLevel: null,
		threadNameFunction: i => $"CompressedMemoryCache Compression Thread {i}",
		useBackgroundThreads: true,
		threadPriority: ThreadPriority.Lowest
	);
	protected static readonly TaskFactory TaskFactory = new(CompressedMemoryCacheScheduler);
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace SpriteMaster.Types;

internal interface IPurgeable {
	internal readonly struct Target {
		[Pure]
		internal readonly ulong CurrentMemoryUsage { get; init; }
		[Pure]
		internal readonly ulong TargetMemoryUsage { get; init; }

		[Pure]
		internal readonly ulong Difference => (ulong)Math.Max(0L, (long)CurrentMemoryUsage - (long)TargetMemoryUsage);
	}

	ulong? OnPurgeHard(Target target, CancellationToken cancellationToken = default);
	ulong? OnPurgeSoft(Target target, CancellationToken cancellationToken = default);

	Task<ulong?> PurgeHardAsync(Target target, CancellationToken cancellationToken = default) =>
		Task.Run(() => OnPurgeHard(target, cancellationToken), cancellationToken);
	Task<ulong?> PurgeSoftAsync(Target target, CancellationToken cancellationToken = default) =>
		Task.Run(() => OnPurgeSoft(target, cancellationToken), cancellationToken);
}

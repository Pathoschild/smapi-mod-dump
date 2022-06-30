/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System;

namespace SpriteMaster.Types;

internal abstract class SpriteMasterObject : IDisposable {
	private IPurgeable? Purgeable => this as IPurgeable;

	internal SpriteMasterObject() {
		if (Purgeable is {} purgeable) {
			MemoryMonitor.Manager.Register(purgeable);
		}
	}

	~SpriteMasterObject() {
		Debug.Trace($"{nameof(SpriteMasterObject)} ({this.GetType().FullName}) was not disposed before finalizer");
	}

	public virtual void Dispose() {
		GC.SuppressFinalize(this);

		if (Purgeable is { } purgeable) {
			MemoryMonitor.Manager.Unregister(purgeable);
		}
	}
}

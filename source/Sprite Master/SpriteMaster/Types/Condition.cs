/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using System.Threading;

namespace SpriteMaster.Types
{
	internal sealed class Condition
	{
		private volatile bool State = false;
		private volatile AutoResetEvent Event = new(false);

		public Condition(bool initialState = false)
		{
			State = initialState;
		}

		public static implicit operator bool(Condition condition) => condition.State;

		// This isn't quite thread-safe, but the granularity of this in our codebase is really loose to begin with. It doesn't need to be entirely thread-safe.
		public bool Wait()
		{
			Event.WaitOne();
			return State;
		}

		public void Set(bool state = true)
		{
			State = state;
			Event.Set();
		}

		// This clears the state without triggering the event.
		public void Clear()
		{
			State = false;
		}
	}
}

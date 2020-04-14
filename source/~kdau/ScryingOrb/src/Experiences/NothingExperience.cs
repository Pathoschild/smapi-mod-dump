using StardewValley;

namespace ScryingOrb
{
	public class NothingExperience : Experience
	{
		protected override bool check ()
		{
			if (!isAvailable)
				return false;
			
			if (Game1.player.CurrentItem != null &&
					!(Game1.player.CurrentItem is Tool))
				return false;

			showMessage ("rejection.nothing");
			return true;
		}

		protected override void doRun ()
		{}
	}
}

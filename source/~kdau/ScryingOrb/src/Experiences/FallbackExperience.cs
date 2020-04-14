using StardewValley;

namespace ScryingOrb
{
	public class FallbackExperience : Experience
	{
		protected override bool check ()
		{
			showRejection ("rejection.unrecognized");
			return true;
		}

		protected override void doRun ()
		{}
	}
}

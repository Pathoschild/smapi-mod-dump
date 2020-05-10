using StardewValley;

namespace ScryingOrb
{
	public class FallbackExperience : Experience
	{
		protected override bool check ()
		{
			return true;
		}

		protected override void doRun ()
		{
			showRejection ("rejection.unrecognized");
		}
	}
}

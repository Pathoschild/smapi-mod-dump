using StardewValley;
using SObject = StardewValley.Object;


namespace QualityScrubber
{
	public interface IQualityScrubberApi
	{
		QualityScrubberController controller { get; }
		bool CanProcess(Item inputItem, SObject machine);
	}


	public class QualityScrubberApi : IQualityScrubberApi
	{
		public QualityScrubberController controller { get; }


		public QualityScrubberApi(QualityScrubberController mod)
		{
			this.controller = mod;
		}


		public bool CanProcess(Item inputItem, SObject machine)
		{
			return controller.CanProcess(inputItem, machine);
		}
	}
}

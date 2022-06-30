/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using StardewValley;
using SObject = StardewValley.Object;


namespace QualityScrubber
{
	public interface IQualityScrubberApi
	{
		public QualityScrubberController Controller { get; }
		public bool CanProcess(Item inputItem, SObject machine);
	}


	public class QualityScrubberApi : IQualityScrubberApi
	{
		public QualityScrubberController Controller { get; }


		public QualityScrubberApi(QualityScrubberController controller)
		{
			this.Controller = controller;
		}


		public bool CanProcess(Item inputItem, SObject machine)
		{
			return Controller.CanProcess(inputItem, machine);
		}
	}
}

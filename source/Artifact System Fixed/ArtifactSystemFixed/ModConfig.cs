/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/ArtifactSystemFixed
**
*************************************************/

namespace ArtifactSystemFixed
{
	public class ModConfig
	{
		//Artifact dig spots
		public double Artifact_AlreadyFoundMultiplier { get; set; } = 0.3;
		public double Artifact_BaseWeightForNothingInPrimaryTable { get; set; } = 0.0;
		public double Artifact_MultiplierForNothingInPrimaryTable { get; set; } = 4.0;

		//Geodes
		public double Geode_AlreadyFoundMultiplier { get; set; } = 0.3;
	}
}

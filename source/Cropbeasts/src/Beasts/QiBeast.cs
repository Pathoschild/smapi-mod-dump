/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/cropbeasts
**
*************************************************/

namespace Cropbeasts.Beasts
{
	public class QiBeast : Berrybeast
	{
		public QiBeast ()
		{ }

		public QiBeast (CropTile cropTile, bool primary)
		: base (cropTile, primary, "Qi Beast")
		{
			// The harvest already has a face.
			faceType.Value = FaceType.Blank;
		}
	}
}

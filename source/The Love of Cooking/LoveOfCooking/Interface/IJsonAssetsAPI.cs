/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/CooksAssistant
**
*************************************************/

namespace LoveOfCooking
{
	public interface IJsonAssetsApi
	{
		void LoadAssets(string path);

		int GetObjectId(string name);
		int GetCropId(string name);
	}
}

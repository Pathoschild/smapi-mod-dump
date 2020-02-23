using System.Collections.Generic;

namespace SailorStyles_Clothing
{
	public interface IJsonAssetsApi
	{
		void LoadAssets(string path);

		int GetHatId(string name);
		int GetClothingId(string name);
		IDictionary<string, int> GetAllHatIds();
		IDictionary<string, int> GetAllClothingIds();
		List<string> GetAllHatsFromContentPack(string cp);
		List<string> GetAllClothingFromContentPack(string cp);
	}
}

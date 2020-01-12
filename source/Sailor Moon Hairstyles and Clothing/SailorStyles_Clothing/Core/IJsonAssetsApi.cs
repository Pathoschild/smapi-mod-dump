using System.Collections.Generic;

namespace SailorStyles_Clothing
{
	public interface IJsonAssetsApi
	{
		void LoadAssets(string path);

		int GetClothingId(string name);
		int GetHatId(string name);
		IDictionary<string, int> GetAllClothingIds();
		IDictionary<string, int> GetAllHatIds();
	}
}

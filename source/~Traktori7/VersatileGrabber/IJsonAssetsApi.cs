using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersatileGrabber
{
	public interface IJsonAssetsApi
	{
		void LoadAssets(string path);
		int GetBigCraftableId(string name);

		event EventHandler IdsAssigned;
	}
}

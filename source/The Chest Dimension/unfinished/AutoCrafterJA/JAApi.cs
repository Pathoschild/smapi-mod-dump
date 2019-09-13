using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCrafterJA
{
    public interface JAApi
    {
        void LoadAssets(string path);
        int GetBigCraftableId(string name);
    }
}

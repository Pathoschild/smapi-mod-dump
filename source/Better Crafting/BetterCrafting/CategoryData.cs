using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCrafting
{
    public class CategoryData
    {
        public Dictionary<string, string> categoryNames { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, List<string>> categories { get; set; } = new Dictionary<string, List<string>>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondTheValleyExpansion.Framework.Alchemy
{
    class CustomAlchemyDataModel
    {
        /// <summary> Read custom item ids from config/AlchemyIDs.json </summary>
        public IDictionary<int, string> itemData = new Dictionary<int, string>();
    }
}

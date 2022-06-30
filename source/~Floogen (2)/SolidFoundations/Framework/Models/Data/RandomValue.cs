/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidFoundations.Framework.Models.Data
{
    public class RandomValue
    {
        public List<object> Values { get; set; }

        public T Get<T>(Random random)
        {
            if (Values is null || Values.Count == 0)
            {
                return default(T);
            }

            return (T)Convert.ChangeType(Values[random.Next(Values.Count)], typeof(T));
        }
    }
}

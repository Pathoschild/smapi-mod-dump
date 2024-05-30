/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.GUI.DragNDrop
{
    public class SavedShapeGroupArea<T>
    {
        protected List<T> contents = new List<T>();

        public virtual List<T> GetAll()
        {
            return contents;
        }

        public virtual void SetAll(List<T> list)
        {
            contents = list;
        }

        public virtual void SetAll(int index, List<T> list)
        {
            contents = list;
        }
    }
}

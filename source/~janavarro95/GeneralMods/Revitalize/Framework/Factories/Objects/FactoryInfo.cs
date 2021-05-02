/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PyTK.CustomElementHandler;
using Revitalize.Framework.Objects;

namespace Revitalize.Framework.Factories.Objects.Furniture
{
    public class FactoryInfo
    {
        /// <summary>
        /// Revitalize's extra info tacked on.
        /// </summary>
        public BasicItemInformation info;
        public CustomObjectData PyTkData;

        public FactoryInfo()
        {

        }

        public FactoryInfo(MultiTiledObject obj)
        {
            this.info = obj.info;
            this.PyTkData = obj.data;
        }

        public FactoryInfo(MultiTiledComponent component)
        {
            this.info = component.info;
            this.PyTkData = component.data;
        }

    }
}

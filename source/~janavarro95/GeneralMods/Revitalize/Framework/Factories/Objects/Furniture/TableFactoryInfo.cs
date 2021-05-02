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
using Revitalize.Framework.Objects;
using Revitalize.Framework.Objects.Furniture;
using Revitalize.Framework.Objects.InformationFiles.Furniture;

namespace Revitalize.Framework.Factories.Objects.Furniture
{
    public class TableFactoryInfo:FactoryInfo
    {
        public TableInformation tableInfo;


        public TableFactoryInfo()
        {

        }

        public TableFactoryInfo(TableTileComponent table):base(table)
        {
            this.tableInfo = table.furnitureInfo;
        }

        public TableFactoryInfo(TableMultiTiledObject table):base(table)
        {
            this.tableInfo = null;
        }

    }
}

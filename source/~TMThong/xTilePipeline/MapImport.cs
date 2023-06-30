/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xTile.Pipeline
{
    public class MapImport
    {
        public MapImport(Map map, string mapDirectory)
        {
            m_map = map;
            m_mapDirectory = mapDirectory;
        }

        public Map Map
        {
            get { return m_map; }
        }

        public string MapDirectory
        {
            get { return m_mapDirectory; }
        }

        private Map m_map;
        private string m_mapDirectory;
    }
}
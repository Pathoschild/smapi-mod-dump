/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpaceCore.Framework
{
    internal class CustomPropertyInfo
    {
        private PropertyInfo propInfo;

        public Type DeclaringType { get; set; }
        public string Name { get; set; }
        public Type PropertyType { get; set; }
        public MethodInfo Getter { get; set; }
        public MethodInfo Setter { get; set; }

        public PropertyInfo GetFakePropertyInfo()
        {
            if ( propInfo == null )
                propInfo = new FakePropertyInfo( this );
            return propInfo;
        }
    }
}

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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Revitalize.Framework.Utilities.Serialization.ContractResolvers
{
    public class NetFieldContract : DefaultContractResolver
    {
        public static NetFieldContract Instance { get; } = new NetFieldContract();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (member.Name == nameof(StardewValley.Item.NetFields))
            {
                property.Ignored = true;
            }
            return property;
        }
    }
}

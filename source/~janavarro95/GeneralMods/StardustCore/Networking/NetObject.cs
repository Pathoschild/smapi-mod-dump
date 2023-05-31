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
using System.Xml.Serialization;
using Netcode;
using Newtonsoft.Json;

namespace Omegasis.StardustCore.Networking
{
    /// <summary>
    /// Class used to make other classes be able to be serialized over the net.
    /// </summary>
    public class NetObject : INetObject<NetFields>
    {
        [XmlIgnore]
        [JsonIgnore]
        public NetFields NetFields { get; } = new NetFields();

        protected virtual void initializeNetFields()
        {

        }
    }
}

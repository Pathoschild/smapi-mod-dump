/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DynamicBodies.Data
{
    [DataContract]
    internal class DGAPackData
    {
        [DataMember(Name = "$ItemType")]
        public string ItemType;
        //Boots
        [DataMember()]
        public string ID;
        [DataMember()]
        public string Texture = "";
        [DataMember()]
        public string FarmerColors = "";
        [DataMember()]
        public string Metadata = "";
        [DataMember()]
        public string TextureMale = "";
        [DataMember()]
        public string TextureMaleColor = "";
        [DataMember()]
        public string TextureMaleOverlay = "";
        [DataMember()]
        public string TextureFemale = "";
        [DataMember()]
        public string TextureFemaleColor = "";
        [DataMember()]
        public string TextureFemaleOverlay = "";
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;

namespace EiTK.Update
{
    public class UpdateData
    {

        public string name { get; }
        public string author { get; }
        public string version { get; }
        public string description { get; }
        public string uniqueID { get; }
        public string entryDll { get; }
        public string MinimumApiVersion { get; }
        public string[] UpdateKeys { get; }
        public string newVersionLink { get; }
        public List<contactData> contactDatas { get; }

        public UpdateData(string name, string author, string version, string description, string uniqueId, string entryDll, string minimumApiVersion, string[] updateKeys, string newVersionLink, List<contactData> contactDatas)
        {
            this.name = name;
            this.author = author;
            this.version = version;
            this.description = description;
            uniqueID = uniqueId;
            this.entryDll = entryDll;
            MinimumApiVersion = minimumApiVersion;
            UpdateKeys = updateKeys;
            this.newVersionLink = newVersionLink;
            this.contactDatas = contactDatas;
        }
    }
}
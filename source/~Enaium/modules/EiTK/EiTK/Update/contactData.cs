/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium/Stardew_Valley_Mods
**
*************************************************/

namespace EiTK.Update
{
    public class contactData
    {
        public string websiteName { get; }
        public string websiteLink { get; }

        public contactData(string websiteName,string websiteLink)
        {
            this.websiteName = websiteName;
            this.websiteLink = websiteLink;
        }
    }
}
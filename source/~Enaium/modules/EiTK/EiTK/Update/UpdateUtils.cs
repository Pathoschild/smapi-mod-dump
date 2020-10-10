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
using System.Net;
using System.Text;

namespace EiTK.Update
{
    public class UpdateUtils
    {
        public static bool isNewVersion(UpdateData updateData)
        {
            
            WebClient MyWebClient = new WebClient();        
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;
            Byte[] pageData = MyWebClient.DownloadData(updateData.newVersionLink); 
            string p = Encoding.Default.GetString(pageData);
            p = new UpdateUtils().TextGainCenter("  \"Version\": \"", "\",", p);
            
            return !updateData.version.Equals(p);
        }
        
        
        private string TextGainCenter(string left, string right, string text) {
            if (string.IsNullOrEmpty(left))
                return ""; 
            if (string.IsNullOrEmpty(right))
                return "";
            if (string.IsNullOrEmpty(text))
                return "";

            int Lindex = text.IndexOf(left);
            
            if (Lindex == -1){
                return "";
            } 
            
            Lindex = Lindex + left.Length;
            
            int Rindex = text.IndexOf(right, Lindex);
           
            if (Rindex == -1){
                return "";    
            } 
            
            return text.Substring(Lindex, Rindex - Lindex);
        }


    }
}
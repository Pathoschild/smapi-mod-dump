/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Windmill-City/InputFix
**
*************************************************/

using System;
using System.Globalization;
using System.Windows;

namespace Patcher
{
    public class LangHelper
    {
        public ResourceDictionary langRd;

        public LangHelper()
        {
            LoadLang();
        }

        public string GetString(string key, params string[] args)
        {
            string result = (string)langRd[key];
            return result == null ? key : string.Format(result.Replace("\\n", "\n"), args);
        }

        private void LoadLang()
        {
            try
            {
                langRd = Application.LoadComponent(new Uri(@"Lang\" + CultureInfo.CurrentUICulture.Name + ".xaml", UriKind.Relative)) as ResourceDictionary;
            }
            catch (Exception)
            {
                langRd = Application.LoadComponent(new Uri(@"Lang\" + "en-US" + ".xaml", UriKind.Relative)) as ResourceDictionary;
            }
        }
    }
}
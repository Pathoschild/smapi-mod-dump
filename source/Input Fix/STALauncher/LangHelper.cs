using System;
using System.Globalization;
using System.Windows;

namespace STALauncher
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
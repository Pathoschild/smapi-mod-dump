/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Linq;

namespace SpaceCore.UI
{
    public class FloatBox : Textbox
    {
        public float Value
        {
            get { return (String == "" || String == "-") ? 0 : float.Parse(String); }
            set { String = value.ToString(); }
        }

        protected override void receiveInput(string str)
        {
            bool hasDot = String.Contains('.');
            bool valid = true;
            for (int i = 0; i < str.Length; ++i)
            {
                char c = str[i];
                if ( !char.IsDigit(c) && !(c == '.' && !hasDot) && !(c == '-' && String == "" && i == 0))
                {
                    valid = false;
                    break;
                }
                if (c == '.')
                    hasDot = true;
            }
            if (!valid)
                return;

            String += str;
            if (Callback != null)
                Callback.Invoke(this);
        }
    }
}

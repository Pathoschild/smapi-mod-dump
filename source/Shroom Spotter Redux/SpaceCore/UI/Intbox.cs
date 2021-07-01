/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

namespace SpaceCore.UI
{
    public class Intbox : Textbox
    {
        public int Value
        {
            get { return (String == "" || String == "-") ? 0 : int.Parse(String); }
            set { String = value.ToString(); }
        }

        protected override void receiveInput(string str)
        {
            bool valid = true;
            for ( int i = 0; i < str.Length; ++i )
            {
                char c = str[i];
                if ( !char.IsDigit(c) && !(c == '-' && String == "" && i == 0) )
                {
                    valid = false;
                    break;
                }
            }
            if (!valid)
                return;

            String += str;
            if (Callback != null)
                Callback.Invoke(this);
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System.Linq;

namespace SpaceShared.UI
{
    internal class FloatBox : Textbox
    {
        public float Value
        {
            get => (this.String == "" || this.String == "-") ? 0 : float.Parse(this.String);
            set => this.String = value.ToString();
        }

        protected override void ReceiveInput(string str)
        {
            bool hasDot = this.String.Contains('.');
            bool valid = true;
            for (int i = 0; i < str.Length; ++i)
            {
                char c = str[i];
                if (!char.IsDigit(c) && !(c == '.' && !hasDot) && !(c == '-' && this.String == "" && i == 0))
                {
                    valid = false;
                    break;
                }
                if (c == '.')
                    hasDot = true;
            }
            if (!valid)
                return;

            this.String += str;
            this.Callback?.Invoke(this);
        }
    }
}

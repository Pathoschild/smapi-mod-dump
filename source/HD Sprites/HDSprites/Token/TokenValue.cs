/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ninthworld/HDSprites
**
*************************************************/

namespace HDSprites.Token
{
    public class TokenValue
    {
        protected string Value { get; set; }
        protected bool Enabled { get; set; }

        public TokenValue(string value, bool enabled)
        {
            this.Value = value;
            this.Enabled = enabled;
        }

        public virtual string GetValue()
        {
            return this.Value;
        }

        public virtual bool IsEnabled()
        {
            return this.Enabled;
        }
    }
}

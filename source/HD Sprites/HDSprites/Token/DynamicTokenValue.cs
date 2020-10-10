/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ninthworld/HDSprites
**
*************************************************/

using System.Collections.Generic;

namespace HDSprites.Token
{
    public class DynamicTokenValue : TokenValue
    {
        private DynamicTokenManager Manager { get; set; }
        private List<TokenEntry> When { get; set; }

        public DynamicTokenValue(string value, bool enabled, DynamicTokenManager manager, List<TokenEntry> when) : base(value, enabled)
        {
            this.Manager = manager;
            this.When = when;
        }

        public override bool IsEnabled()
        {
            if (!this.Enabled) return false;
            foreach (TokenEntry entry in this.When)
            {
                if (!entry.IsEnabled(this.Manager))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

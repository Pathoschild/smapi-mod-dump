/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using SpaceShared;

namespace JsonAssets.Framework.ContentPatcher
{
    internal class IdToken : BaseToken
    {
        public IdToken(string type)
            : base(type, "Id")
        {
        }

        public override IEnumerable<string> GetValidInputs()
        {
            return new string[0];
        }

        public override bool TryValidateInput(string input, out string error)
        {
            error = "";
            return true;
        }

        public override IEnumerable<string> GetValues(string input)
        { 
            if (!this.IsReady())
                return Array.Empty<string>();

            return new[] { input };
        }

        protected override void UpdateContextImpl()
        {
        }
    }
}

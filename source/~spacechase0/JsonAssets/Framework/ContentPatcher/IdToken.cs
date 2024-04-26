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
        private readonly Func<IDictionary<string, string>> IdsFunc;
        private IDictionary<string, string> Ids;

        public IdToken(string type, Func<IDictionary<string,string>> idsFunc)
            : base(type, "Id")
        {
            IdsFunc = idsFunc;
        }

        public override IEnumerable<string> GetValidInputs()
        {
            return this.Ids.Keys;
        }

        public override bool TryValidateInput(string input, out string error)
        {
            error = "";
            if (!this.Ids.ContainsKey(input))
            {
                error = $"Invalid name for {this.Type}: {input}";
                return false;
            }
            return true;
        }

        public override IEnumerable<string> GetValues(string input)
        {
            if (!this.IsReady())
                return Array.Empty<string>();

            if (input == "")
                return this.Ids.Values.Select(p => p.ToString()).ToArray();

            return this.Ids.TryGetValue(input, out string value)
                ? new[] { value }
                : Array.Empty<string>();
        }

        protected override void UpdateContextImpl()
        {
            this.Ids = this.IdsFunc();
        }
    }
}

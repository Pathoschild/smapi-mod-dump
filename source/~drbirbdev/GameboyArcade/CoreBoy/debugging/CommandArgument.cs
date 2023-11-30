/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreBoy.debugging
{
    public class CommandArgument
    {

        private readonly string _name;

        private readonly bool _required;

        private readonly ICollection<string> _allowedValues;

        public CommandArgument(string name, bool required)
        {
            this._name = name;
            this._required = required;
            this._allowedValues = new List<string>();
        }

        public CommandArgument(string name, bool required, ICollection<string> allowedValues)
        {
            this._name = name;
            this._required = required;
            this._allowedValues = allowedValues ?? new List<string>();
        }

        public string GetName()
        {
            return this._name;
        }

        public bool IsRequired()
        {
            return this._required;
        }

        public ICollection<string> GetAllowedValues()
        {
            return this._allowedValues;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            if (!this._required)
            {
                builder.Append('[');
            }

            if (this._allowedValues != null)
            {
                builder.Append('{');
                builder.Append(string.Join(",", this._allowedValues.ToArray()));
                builder.Append('}');
            }
            else
            {
                builder.Append(this._name.ToUpper());
            }

            if (!this._required)
            {
                builder.Append(']');
            }

            return builder.ToString();
        }
    }
}
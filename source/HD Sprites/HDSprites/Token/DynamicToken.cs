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
    public class ValueExt
    {
        public string Value { get; set; }
        public List<string> SubValues { get; set; }

        public ValueExt(string value, List<string> subValues)
        {
            this.Value = value;
            this.SubValues = subValues;
        }
    }

    public class DynamicToken
    {
        public string Name { get; set; }
        protected List<TokenValue> Values { get; set; }

        public DynamicToken(string name)
        {
            this.Name = name;
            this.Values = new List<TokenValue>();
        }

        public virtual void AddValue(TokenValue value)
        {
            this.Values.Add(value);
        }

        public virtual string GetValue()
        {
            for (int i = this.Values.Count - 1; i >= 0; --i)
            {
                if (this.Values[i].IsEnabled())
                {
                    return this.Values[i].GetValue();
                }
            }
            return "";
        }

        public virtual List<ValueExt> GetValues()
        {
            List<ValueExt> enabledValues = new List<ValueExt>();
            foreach (TokenValue value in this.Values)
            {
                if (value.IsEnabled())
                {
                    enabledValues.Add(new ValueExt(value.GetValue(), new List<string>()));
                }
            }
            return enabledValues;
        }

        public virtual List<string> GetSimpleValues()
        {
            List<string> values = new List<string>();
            foreach (var entry in this.GetValues())
            {
                values.Add(entry.Value);
            }
            return values;
        }
    }
}

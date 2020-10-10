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
using System.Linq;

namespace HDSprites.Token.Global
{
    public abstract class GlobalToken : DynamicToken
    {
        protected string GlobalValue { get; set; }
        protected List<ValueExt> GlobalValues { get; set; }

        public GlobalToken(string name) : base(name)
        {
            this.GlobalValue = "";
            this.GlobalValues = new List<ValueExt>();
        }
        
        public override string GetValue()
        {
            return this.GlobalValue;
        }

        public override List<ValueExt> GetValues()
        {
            return this.GlobalValues;
        }

        public void UpdateToken()
        {
            if (!this.Update())
                this.SetValue("");
        }

        protected abstract bool Update();

        protected virtual bool SetValue(string value)
        {
            this.GlobalValue = value ?? "";
            this.GlobalValues = new List<ValueExt> { new ValueExt(this.GlobalValue, new List<string>()) };
            return true;
        }

        protected virtual bool SetValues(IEnumerable<string> values)
        {
            this.GlobalValue = "";
            this.GlobalValues = values?.Select(value => new ValueExt(value, new List<string>())).ToList() ?? new List<ValueExt>();
            return true;
        }

        protected virtual bool SetValues(IEnumerable<int> values)
        {
            return this.SetValues(values?.Select(p => p.ToString()));
        }
    }
}

using System.Collections.Generic;

namespace HDSprites.Token.Global
{
    public class GlobalToken : DynamicToken
    {
        protected string GlobalValue { get; set; }
        protected List<ValueExt> GlobalValues { get; set; }

        public GlobalToken(string name) : base(name)
        {
            this.GlobalValue = "";
            this.GlobalValues = new List<ValueExt>();
            this.Update();
        }
        
        public override string GetValue()
        {
            return this.GlobalValue;
        }

        public override List<ValueExt> GetValues()
        {
            return this.GlobalValues;
        }

        public virtual void Update()
        {
        }
    }
}

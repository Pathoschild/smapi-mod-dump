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

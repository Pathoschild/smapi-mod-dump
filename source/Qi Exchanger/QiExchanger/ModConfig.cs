using Microsoft.Xna.Framework.Input;
namespace QiExchanger
{
   internal class ModConfig
    {
        public string ActivationKey { get; set; } = Keys.F9.ToString();
        public int ExchangeRate { get; set; } = 10;
    }
}

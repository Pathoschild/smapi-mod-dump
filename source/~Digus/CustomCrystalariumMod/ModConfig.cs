using Microsoft.Xna.Framework.Input;

namespace CustomCrystalariumMod
{
    internal class ModConfig
    {
        public bool DisableLetter;
        public bool GetObjectBackOnChange;
        public bool GetObjectBackImmediately;

        public ModConfig()
        {
            DisableLetter = false;
            GetObjectBackOnChange = false;
            GetObjectBackImmediately = false;
            
        }
    }
}

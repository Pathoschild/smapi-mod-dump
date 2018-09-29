using Microsoft.Xna.Framework.Input;

namespace CustomCaskMod
{
    internal class ModConfig
    {
        public bool DisableLetter;
        public bool EnableCasksAnywhere;

        public ModConfig()
        {
            this.DisableLetter = false;
            this.EnableCasksAnywhere = false;
        }
    }
}

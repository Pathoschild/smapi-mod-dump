using CustomNPCFramework.Framework.NPCS;
using Microsoft.Xna.Framework.Graphics;

namespace CustomNPCFramework.Framework.ModularNpcs
{
    /// <summary>Used as a wrapper for npc portraits.</summary>
    public class Portrait
    {
        /// <summary>Used to display the npc portrait.</summary>
        public Texture2D portrait;

        /// <summary>Used to hold the path to the texture to use for the npc portrait.</summary>
        public string relativePath;

        /// <summaryConstruct an instance.</summary>
        /// <param name="path">The full path to the file.</param>
        public Portrait(string path)
        {
            this.relativePath = Class1.getRelativeDirectory(path);
            this.portrait = Class1.ModHelper.Content.Load<Texture2D>(path);
        }

        /// <summary>Sets the npc's portrait to be this portrait texture.</summary>
        public void setCharacterPortraitFromThis(ExtendedNpc npc)
        {
            npc.Portrait = this.portrait;
        }

        /// <summary>Reloads the texture for the NPC portrait.</summary>
        public void reload()
        {
            this.portrait = Class1.ModHelper.Content.Load<Texture2D>(this.relativePath);
        }
    }
}

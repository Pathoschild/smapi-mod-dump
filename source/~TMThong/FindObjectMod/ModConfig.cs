/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace FindObjectMod
{
    public class ModConfig
    {
         
        public bool FindQuestObject { get; set; } = false;

         
        public bool DrawArea { get; set; } = false;

        
        public bool InitiatesObjectSelectionModeForMobile { get; set; } = false;

         
        public bool FindAllNPC { get; set; } = false;

         
        public bool FindAllObject { get; set; } = false;

        
        public bool SearchMode { get; set; } = true;

         
        public SButton KeyOpenMenu { get; set; } = SButton.L;

        
        public SButton KeySelectObject { get; set; } = SButton.MouseRight;

         
        public Color NPC { get; set; } = Color.SeaGreen;

         
        public Color Monsters { get; set; } = Color.DarkRed;

         
        public Color Object { get; set; } = Color.BlueViolet;

         
        public Color QuestObject { get; set; } = Color.Gold;

        
        public Dictionary<string, Dictionary<string, Color>> ObjectToFind { get; set; } = new Dictionary<string, Dictionary<string, Color>>();

         
        public Dictionary<string, Dictionary<string, Color>> FindCharacter { get; set; } = new Dictionary<string, Dictionary<string, Color>>();

        
        public void reset()
        {
            this.FindQuestObject = false;
            this.DrawArea = false;
            this.InitiatesObjectSelectionModeForMobile = false;
            this.FindAllNPC = false;
            this.FindAllObject = false;
            this.SearchMode = true;
            this.KeyOpenMenu = (SButton.L);
            this.KeySelectObject = SButton.MouseRight;
            this.NPC = Color.SeaGreen;
            this.Monsters = Color.DarkRed;
            this.Object = Color.BlueViolet;
            this.QuestObject = Color.Gold;
            this.ObjectToFind[Utilities.SaveKey] = new Dictionary<string, Color>();
            this.FindCharacter[Utilities.SaveKey] = new Dictionary<string, Color>();
        }
    }
}

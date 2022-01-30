/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gaiadin/Stardew-Screenshot-Everywhere
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace Stardew_Screenshot_Everywhere
{
    class ModConfig
    {
        public bool Debug { get; set; }
        public KeybindList HotKey { get; set; }
        public bool CameraFlash { get; set; }
        public bool NotifyCapture { get; set; }
        public bool ShutterSound { get; set; }
        public bool Farm { get; set; }
        public bool FarmHouse { get; set; }
        public bool Greenhouse { get; set; }
        public bool Town { get; set; }
        public bool Cellar { get; set; }
        public bool Coop { get; set; }
        public bool Barn { get; set; }
        public bool Shed { get; set; }
        public bool Beach { get; set; }
        public bool IslandFarm { get; set; }
        public bool Desert { get; set; }
        public bool Forest { get; set; }

        public string ScreenShotLocations { get; set; }

        public ModConfig()
        {
            this.Debug = false;
            this.HotKey = new KeybindList(SButton.OemTilde);
            this.CameraFlash = true;
            this.NotifyCapture = true;
            this.ShutterSound = true;
            this.Farm = false;
            this.FarmHouse = false;
            this.Greenhouse = false;
            this.Beach = false;
            this.Shed = false;
            this.Coop = false;
            this.Barn = false;
            this.Cellar = false;
            this.IslandFarm = false;
            this.Town = false;
            this.Desert = false;
            this.Forest = false;
            this.ScreenShotLocations = "";
        }
    }
}

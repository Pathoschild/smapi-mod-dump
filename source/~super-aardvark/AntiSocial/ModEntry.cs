using Harmony;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SuperAardvark.AntiSocial
{
    public class ModEntry : StardewModdingAPI.Mod
    {

        public override void Entry(IModHelper helper)
        {
            AntiSocialManager.DoSetupIfNecessary(this);
        }

    }

}

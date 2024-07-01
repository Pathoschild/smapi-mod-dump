/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValleyMod.Shared;
using StardewValleyMod.Shared.FastHarmony;
namespace CloudSave.Framework.Patch
{
    internal class PatchManager : AbstractPatchManager<Config>
    {
        public PatchManager(IModHelper helper, IManifest manifest, Config config) : base(helper, manifest, config)
        {
            
        }

        public override void Init()
        {
            this.Patches.Add(new LoadGameMenuPatch());
        }
    }
}

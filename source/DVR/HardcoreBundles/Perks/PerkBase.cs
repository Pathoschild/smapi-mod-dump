/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/captncraig/StardewMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using System.Linq;

namespace HardcoreBundles.Perks
{
    public class PerkBase
    {
        public int BundleID;
        public IModHelper Helper;
        public IMonitor Monitor;

        public virtual bool ShouldEnable()
        {
            return true;
        }

        public void EnableIfCompleted()
        {
            if (bundleDone())
            {
                Enable();
            }
        }

        protected bool bundleDone()
        {
            return Game1.netWorldState.Value.Bundles[BundleID].All(x => x);
        }

        // called on game load, or when mail is sent initially. Hook events here
        public virtual void Enable()
        {

        }

        // called on game unload. Unhook all events.
        public virtual void Disable()
        {

        }
    }
}

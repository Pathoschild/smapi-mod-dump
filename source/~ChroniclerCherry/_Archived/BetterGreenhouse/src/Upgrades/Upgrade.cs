/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;

namespace GreenhouseUpgrades.Upgrades
{
    public enum UpgradeTypes
    {
        AutoWaterUpgrade, SizeUpgrade, AutoHarvestUpgrade
    }
    public abstract class Upgrade
    {
        public abstract UpgradeTypes Type { get; }
        public virtual string Name => Type.ToString();
        public abstract bool Active { get; set; }
        public abstract bool Unlocked { get; set; }
        public virtual int Cost => Main.Config.UpgradeCosts[Type];

        public virtual bool DisableOnFarmhand { get; } = false;
        public virtual void Start()
        {
            if (!Context.IsMainPlayer && DisableOnFarmhand) return;
            if (!Unlocked) return;
            Active = true;
        }
        public abstract void Stop();

        public string TranslatedName => Helper.Translation.Get($"{Name}.Name");
        public string TranslatedDescription => Helper.Translation.Get($"{Name}.Description");


        public IModHelper Helper;
        public IMonitor Monitor;
        public virtual void Initialize(IModHelper helper, IMonitor monitor)
        {
            Helper = helper;
            Monitor = monitor;
        }
    }

}

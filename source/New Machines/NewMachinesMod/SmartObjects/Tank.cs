/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.Data;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;
using StardewValley;
using StardewValley.Tools;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public sealed class FullTank : Tank
    {
        public FullTank() : base(ClassMapperService.Instance.GetCraftableID<FullTank>()) { }
    }

    public class Tank : CustomMachineBase
    {
        private static readonly int TankID = ClassMapperService.Instance.GetCraftableID<Tank>();

        public Tank() : this(TankID) { }

        protected Tank(int id) : base(id) { }

        protected bool IsEmpty
        {
            get { return (ID == TankID); }
            set { ID = TankID + (value? 0 : 1); }
        }

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            var result = base.minutesElapsed(minutes, environment);
            if (readyForHarvest) IsEmpty = true;
            return result;
        }

        protected override void OnWateringCanAction(WateringCan wateringCan)
        {
            if (!IsEmpty) return;
            PlaySound(Sound.Bubbles);
            IsEmpty = false;
        }

        protected override bool OnPickaxeAction(Pickaxe pickaxe)
        {
            if (!IsEmpty) PlaySound(Sound.Bubbles);
            IsEmpty = true;
            return base.OnPickaxeAction(pickaxe);
        }

        protected override bool OnAxeAction(Axe axe)
        {
            if (!IsEmpty) PlaySound(Sound.Bubbles);
            IsEmpty = true;
            return base.OnAxeAction(axe);
        }

        protected override MachineInformation MachineInformation => NewMachinesMod.Config.Tank;

        protected override bool PerformDropIn(Object dropInItem, Farmer farmer)
        {
            if (!IsEmpty) return base.PerformDropIn(dropInItem, farmer);
            Game1.showRedMessage(NewMachinesMod.Config.LocalizationStrings[NewMachinesModConfig.LocalizationString.TankRequiresWater]);
            return false;
        }
    }
}
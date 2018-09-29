using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.Data;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Modules;
using Microsoft.Xna.Framework;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public sealed class Churn : MachineModule
    {
        public Churn() : base(ClassMapperService.Instance.GetCraftableID<Churn>()) { }

        protected override MachineInformation MachineInformation => NewMachinesMod.Config.Churn;
        private double Frame { get; set; }

        protected override void GetSpriteDeltaAndColor(out int spriteDelta, out Color? color)
        {
            color = null;
            if (State == MachineState.Working)
            {
                Frame += 0.05;
                if (Frame >= 4) Frame -= 4;
            }
            else
            {
                Frame = 0;
            }
            spriteDelta = (int)Frame;
        }
        
        protected override Vector2 GetScale(bool change = true) => Vector2.Zero;
    }
}
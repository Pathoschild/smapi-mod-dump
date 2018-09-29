using System;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.Data;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Object = StardewValley.Object;
using XColor = Microsoft.Xna.Framework.Color;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public sealed class Mixer : CustomMachineBase
    {
        public Mixer() : base(ClassMapperService.Instance.GetCraftableID<Mixer>()) { }

        private Object FirstDroppedItem { get; set; }
        private Object SecondDroppedItem { get; set; }

        protected override MachineInformation MachineInformation => NewMachinesMod.Config.Mixer;

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            var result = base.minutesElapsed(minutes, environment);
            if (readyForHarvest)
            {
                FirstDroppedItem = null;
                SecondDroppedItem = null;
            }
            return result;
        }

        protected override bool PerformDropIn(Object dropInItem, Farmer farmer)
        {
            if (FirstDroppedItem == null)
            {
                FirstDroppedItem = dropInItem;
                PlaySound(Sound.Ship);
            }
            else
            {
                SecondDroppedItem = dropInItem;
                var q1 = FirstDroppedItem.quality;
                var q2 = SecondDroppedItem.quality;

                var itemQuality = Math.Max(0, q1 + q2 - 1);
                var itemPrice = (FirstDroppedItem.price * (4 + q1) + SecondDroppedItem.price * (4 + q2)) / 4;

                var random = GetRandom();
                var postfix = $"{FirstDroppedItem.Name.First()}{SecondDroppedItem.Name.First()}-{(char)random.Next('A', 'Z')}{random.Next(1, 9)}{random.Next(0, 9)}";
                var hue = random.Next(0, 359); // TODO: Not random color.
                var color = RawColor.FromHSL(hue, 0.75, 0.55).ToXnaColor();

                PutItem(NewMachinesModConfig.ExperementalLiquidID, 1, itemQuality, $"{{0}} {postfix}", itemPrice, color); // TODO: From config?
                minutesUntilReady = GetMinutesUntilReady(GetOutputItem(SecondDroppedItem)); // TODO: From config?

                PlaySound(Sound.Ship);
                PlaySound(Sound.Bubbles);
            }
            return true;
        }

        protected override void DrawDetails(SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            DrawObjectRaw(spriteBatch, x, y, alpha, FirstDroppedItem.GetColor() ?? XColor.White, +1);
            DrawObjectRaw(spriteBatch, x, y, alpha, SecondDroppedItem.GetColor() ?? XColor.White, +2);
        }
    }
}
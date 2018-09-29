using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Utils;
using Igorious.StardewValley.NewMachinesMod.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Base
{
    public abstract class CustomMachineBase : MachineBase
    {
        protected CustomMachineBase(int id) : base(id) { }

        protected abstract MachineInformation MachineInformation { get; }
        protected override MachineOutputInformation Output => MachineInformation.Output;

        protected void DrawObjectRaw(SpriteBatch spriteBatch, int x, int y, float alpha, Color? color = null, int sheetIndexDelta = 0)
        {
            base.DrawObject(spriteBatch, x, y, alpha, color, sheetIndexDelta);
        }

        protected override void DrawObject(SpriteBatch spriteBatch, int x, int y, float alpha, Color? color = null, int sheetIndexDelta = 0)
        {
            GetSpriteDeltaAndColor(out sheetIndexDelta, out color);

            if (color == null)
            {
                base.DrawObject(spriteBatch, x, y, alpha, null, sheetIndexDelta);
            }
            else
            {
                base.DrawObject(spriteBatch, x, y, alpha);
                base.DrawObject(spriteBatch, x, y, alpha, color, sheetIndexDelta);
            }
        }

        private MachineDraw GetDrawInfo()
        {
            if (heldObject == null) return MachineInformation.Draw;

            var itemDraw = MachineInformation.Output.Items.Values
                .FirstOrDefault(i => i?.ID == heldObject.ParentSheetIndex)?.Draw;
            if (itemDraw != null) return itemDraw;

            return MachineInformation.Draw;
        }

        protected virtual void GetSpriteDeltaAndColor(out int spriteDelta, out Color? color)
        {
            var draw = GetDrawInfo();
            color = null;
            spriteDelta = 0;

            switch (State)
            {
                case MachineState.Empty:
                    spriteDelta = draw?.Empty ?? 0;
                    break;

                case MachineState.Working:
                    spriteDelta = draw?.Working ?? 0;
                    color = ConvertColor(draw?.WorkingColor);
                    break;

                case MachineState.Ready:
                    spriteDelta = draw?.Ready ?? 0;
                    color = ConvertColor(draw?.ReadyColor);
                    break;
            }
        }

        private Color? ConvertColor(string color)
        {
            if (color == null) return null;
            if (color != "@") return RawColor.FromHex(color).ToXnaColor();
            if (heldObject == null) return null;
            var heldColor = heldObject.GetColor();
            if (heldColor != null) return heldColor;
            return DominantColorFinder.GetDominantColor(heldObject.ParentSheetIndex, Game1.objectSpriteSheet, 16, 16);
        }
    }
}

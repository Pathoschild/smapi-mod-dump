using StardewConfigFramework;
using StardewValley;
using Microsoft.Xna.Framework;

namespace StardewConfigMenu.Panel.Components.ModOptions {
	internal class ModPlusMinusComponent: PlusMinusComponent {
		readonly ModOptionStepper Option;

		public override bool enabled => Option.enabled;

		public override string label => Option.LabelText;

		public override decimal min => Option.min;
		public override decimal max => Option.max;
		public override decimal stepSize => Option.stepSize;
		public override decimal Value {
			get {
				return Option.Value;
			}
			protected set {
				Option.Value = value;
			}
		}

		internal ModPlusMinusComponent(ModOptionStepper option, int x, int y) : base(option.LabelText, option.min, option.max, option.stepSize, option.Value, x, y, option.type, option.enabled) {
			this.Option = option;
		}

		internal ModPlusMinusComponent(ModOptionStepper option) : base(option.LabelText, option.min, option.max, option.stepSize, option.Value, option.type, option.enabled) {
			this.Option = option;
		}
	}
}

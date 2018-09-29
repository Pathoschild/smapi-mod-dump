using StardewConfigFramework;
using StardewValley;
using Microsoft.Xna.Framework;

namespace StardewConfigMenu.Panel.Components.ModOptions {
	internal class ModSliderComponent: SliderComponent {

		readonly ModOptionRange Option;

		public override bool enabled => Option.enabled;

		public override string label => Option.LabelText;

		protected override decimal min => Option.min;
		protected override decimal max => Option.max;
		protected override decimal stepSize => Option.stepSize;
		protected override bool showValue => Option.showValue;
		protected override decimal Value {
			get {
				return Option.Value;
			}
			set {
				Option.Value = value;
			}
		}

		internal ModSliderComponent(ModOptionRange option, int x, int y) : base(option.LabelText, option.min, option.max, option.stepSize, option.Value, option.showValue, x, y, option.enabled) {
			this.Option = option;
		}

		internal ModSliderComponent(ModOptionRange option) : this(option, 0, 0) { }

	}
}

using StardewConfigFramework;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Menus;

namespace StardewConfigMenu.Panel.Components.ModOptions {
	internal class ModButtonComponent: ButtonComponent {
		readonly ModOptionTrigger Option;

		public override bool enabled {
			get {
				return Option.enabled;
			}
		}

		public override string label {
			get {
				return Option.LabelText;
			}
		}

		internal ModButtonComponent(ModOptionTrigger option, int x, int y) : base(option.LabelText, option.type, x, y, option.enabled) {
			this.Option = option;
		}

		internal ModButtonComponent(ModOptionTrigger option) : base(option.LabelText, option.type, option.enabled) {
			this.Option = option;
		}

		public override OptionActionType ActionType {
			get {
				if (Option == null)
					return _ActionType;

				return this.Option.type;
			}
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true) {
			base.receiveLeftClick(x, y, playSound);

			if (this.button.containsPoint(x, y) && enabled && this.IsAvailableForSelection()) {
				if (playSound)
					Game1.playSound("breathin");
				this.Option.Trigger();
			}
		}

		/*
		protected override void leftClicked(int x, int y)
		{
			base.leftClicked(x, y);

			if (this.bounds.Contains(x, y) && enabled && this.IsAvailableForSelection())
			{
				Game1.playSound("breathin");
				this.Option.Trigger();
			}
		} */
	}
}

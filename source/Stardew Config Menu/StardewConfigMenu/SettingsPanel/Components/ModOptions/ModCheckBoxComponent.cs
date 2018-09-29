using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewConfigFramework;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework.Input;

namespace StardewConfigMenu.Panel.Components.ModOptions {

	internal class ModCheckBoxComponent: CheckBoxComponent {
		//
		// Static Fields
		//
		//public const int pixelsHigh = 11;

		//
		// Fields
		//
		readonly private ModOptionToggle Option;

		public override bool IsChecked {
			get { return Option.IsOn; }
			protected set {
				if (Option == null)
					return; // used to ignore base class assignment
				this.Option.IsOn = value;
			}
		}

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

		internal ModCheckBoxComponent(ModOptionToggle option, int x, int y) : base(option.LabelText, option.IsOn, x, y, option.enabled) {
			this.Option = option;
		}

		internal ModCheckBoxComponent(ModOptionToggle option) : base(option.LabelText, option.IsOn, option.enabled) {
			this.Option = option;
		}

	}

}


/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Leclair.Stardew.Almanac.Menus;
using Leclair.Stardew.Common.UI.SimpleLayout;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.Almanac.Pages {
	public abstract class BasePage : IAlmanacPage, ITab {

		public readonly AlmanacMenu Menu;
		public readonly ModEntry Mod;

		public bool Active { get; private set; }

		public BasePage(AlmanacMenu menu, ModEntry mod) {
			Menu = menu;
			Mod = mod;
		}

		#region IAlmanacPage

		public virtual PageType Type => typeof(ICalendarPage).IsAssignableFrom(GetType()) ? PageType.Calendar : PageType.Blank;

		public virtual bool IsMagic => false;

		public virtual void Activate() {
			Active = true;
		}

		public virtual void Deactivate() {
			Active = false;
		}

		public virtual void DateChanged(WorldDate oldDate, WorldDate newDate) {

		}

		public virtual void UpdateComponents() {

		}

		private bool IsComponent(Type type) {
			return type == typeof(ClickableComponent) || type.IsSubclassOf(typeof(ClickableComponent));
		}

		public virtual List<ClickableComponent> GetComponents() {
			List<ClickableComponent> result = new();

			Type type = GetType();

			foreach (FieldInfo field in type.GetFields()) {
				if (field.GetCustomAttributes(typeof(SkipForClickableAggregation), true).Length != 0)
					continue;

				Type ftype = field.FieldType;

				if (IsComponent(ftype)) {
					if (field.GetValue(this) is ClickableComponent cmp)
						result.Add(cmp);

				} else if (ftype.IsGenericType && typeof(IEnumerable).IsAssignableFrom(ftype) && IsComponent(ftype.GetGenericArguments()[0])) {
					if (field.GetValue(this) is IEnumerable enumerable)
						foreach (object obj in enumerable) {
							if (obj is ClickableComponent cmp)
								result.Add(cmp);
						}
				}
			}

			return result;
		}

		public virtual bool ReceiveGamePadButton(Buttons b) {
			return false;
		}

		public virtual bool ReceiveKeyPress(Keys key) {
			return false;
		}

		public virtual bool ReceiveScroll(int x, int y, int direction) {
			return false;
		}

		public virtual bool ReceiveLeftClick(int x, int y, bool playSound) {
			return false;
		}

		public virtual bool ReceiveRightClick(int x, int y, bool playSound) {
			return false;
		}

		public virtual void PerformHover(int x, int y) {

		}

		public virtual void Draw(SpriteBatch b) {

		}

		#endregion

		#region ITab

		public virtual bool TabMagic => IsMagic;

		public virtual int SortKey => 50;
		public virtual bool TabVisible => TabTexture != null;
		public virtual string TabSimpleTooltip => null;
		public virtual ISimpleNode TabAdvancedTooltip => null;
		public virtual Texture2D TabTexture => null;
		public virtual Rectangle? TabSource => null;
		public virtual float? TabScale => 3f;

		#endregion

	}
}

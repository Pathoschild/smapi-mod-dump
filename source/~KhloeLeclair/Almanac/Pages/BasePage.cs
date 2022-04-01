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
using Leclair.Stardew.Common.UI;
using Leclair.Stardew.Common.UI.FlowNode;
using Leclair.Stardew.Common.UI.SimpleLayout;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;

namespace Leclair.Stardew.Almanac.Pages {

	public class BaseState {
		public int LeftFlowScroll;
		public int LeftFlowStep;
		public int RightFlowScroll;
		public int RightFlowStep;
	}

	public abstract class BasePage<T> : IAlmanacPage, ITab where T : BaseState, new() {

		// Id
		public string Id { get; }

		public readonly AlmanacMenu Menu;
		public readonly ModEntry Mod;

		// Flow State
		private IEnumerable<IFlowNode> LeftFlow;
		private int LeftFlowStep;
		private int LeftFlowScroll;
		private bool LeftFlowRestore = false;

		private IEnumerable<IFlowNode> RightFlow;
		private int RightFlowStep;
		private int RightFlowScroll;
		private bool RightFlowRestore = false;

		// Update State
		protected WorldDate LastDate;

		public bool Active { get; private set; }

		public BasePage(AlmanacMenu menu, ModEntry mod) {
			Id = GetType().Name;
			Menu = menu;
			Mod = mod;
		}

		public BasePage(string id, AlmanacMenu menu, ModEntry mod) {
			Id = id;
			Menu = menu;
			Mod = mod;
		}

		#region Update Logic

		public virtual bool WantDateUpdates => false;

		public virtual void Update() {
			LastDate = new(Menu.Date);
		}

		public IEnumerable<IFlowNode> GetLeftFlow() {
			return LeftFlow;
		}

		public void SetLeftFlow(FlowBuilder builder, int step = 4, int scroll = 0) {
			SetLeftFlow(builder.Build(), step, scroll);
		}

		public void SetLeftFlow(IEnumerable<IFlowNode> flow, int step = 4, int scroll = 0) {
			LeftFlow = flow;
			LeftFlowStep = step;

			if (LeftFlowRestore)
				LeftFlowRestore = false;
			else if (scroll >= 0 || scroll == -1)
				LeftFlowScroll = scroll;

			if (Active)
				Menu.SetLeftFlow(LeftFlow, LeftFlowStep, LeftFlowScroll);
		}

		public IEnumerable<IFlowNode> GetRightFlow() {
			return RightFlow;
		}

		public void SetRightFlow(FlowBuilder builder, int step = 4, int scroll = 0) {
			SetRightFlow(builder?.Build(), step, scroll);
		}

		public void SetRightFlow(IEnumerable<IFlowNode> flow, int step = 4, int scroll = 0) {
			RightFlow = flow;
			RightFlowStep = step;

			if (RightFlowRestore)
				RightFlowRestore = false;
			else if (scroll >= 0 || scroll == -1)
				RightFlowScroll = scroll;

			if (Active)
				Menu.SetRightFlow(RightFlow, RightFlowStep, RightFlowScroll);
		}

		public object GetState() {
			return SaveState();
		}

		public virtual T SaveState() {
			if (Active) {
				LeftFlowScroll = Menu.GetLeftFlowScroll();
				RightFlowScroll = Menu.GetRightFlowScroll();
			}

			return new T() {
				LeftFlowScroll = LeftFlowScroll,
				LeftFlowStep = LeftFlowStep,
				RightFlowScroll = RightFlowScroll,
				RightFlowStep = RightFlowStep,
			};
		}

		public void LoadState(object state) {
			if (state is T tstate)
				LoadState(tstate);
		}

		public virtual void LoadState(T state) {
			LeftFlowStep = state.LeftFlowStep;
			LeftFlowScroll = state.LeftFlowScroll;
			RightFlowStep = state.RightFlowStep;
			RightFlowScroll = state.RightFlowScroll;
			LeftFlowRestore = true;
			RightFlowRestore = true;

			if (Active) {
				Menu.SetLeftFlow(LeftFlow, LeftFlowStep, LeftFlowScroll);
				Menu.SetRightFlow(RightFlow, RightFlowStep, RightFlowScroll);
			}
		}

		#endregion

		#region IAlmanacPage

		public virtual PageType Type => typeof(ICalendarPage).IsAssignableFrom(GetType()) ? PageType.Calendar : PageType.Blank;

		public virtual bool IsMagic => false;

		public virtual void Refresh() {
			Update();
		}

		public virtual void ThemeChanged() {

		}

		public virtual void Activate() {
			Active = true;
			if (LastDate != Menu.Date && (WantDateUpdates || Menu.Date.Season != LastDate?.Season))
				Update();

			if (LeftFlow != null)
				Menu.SetLeftFlow(LeftFlow, LeftFlowStep, LeftFlowScroll);

			if (RightFlow != null)
				Menu.SetRightFlow(RightFlow, RightFlowStep, RightFlowScroll);
		}

		public virtual void Deactivate() {
			Active = false;
			if (LeftFlow != null)
				LeftFlowScroll = Menu.GetLeftFlowScroll();

			if (RightFlow != null)
				RightFlowScroll = Menu.GetRightFlowScroll();
		}

		public virtual void DateChanged(WorldDate oldDate, WorldDate newDate) {
			if (Active && (WantDateUpdates || newDate.Season != LastDate?.Season))
				Update();
		}

		public virtual void UpdateComponents() {

		}

		private static bool IsComponent(Type type) {
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

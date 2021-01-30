/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using Bookcase.Events;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Events;
using System;
using System.Reflection;
using xTile.Dimensions;

namespace Bookcase.Lib {

    internal class StardewModHooksWrapper : ModHooks {

        private readonly ModHooks previousHooks;

        internal static StardewModHooksWrapper CreateWrapper() {

            try {

                FieldInfo hooksField = BookcaseMod.reflection.GetField<ModHooks>(typeof(Game1), "hooks").FieldInfo;
                StardewModHooksWrapper wrapper = new StardewModHooksWrapper((ModHooks)hooksField.GetValue(null));
                hooksField.SetValue(null, wrapper);
                BookcaseMod.logger.Debug("This mod has wrapped Game1.hooks!");
                return wrapper;
            }

            catch (Exception e) {

                BookcaseMod.logger.Error($"Could not create StardewModHooksWrapper. Failed with {e.Message}");
            }

            return null;
        }

        private StardewModHooksWrapper(ModHooks previous) {

            this.previousHooks = previous;
        }

        public override void OnGame1_PerformTenMinuteClockUpdate(Action action) {

            this.previousHooks.OnGame1_PerformTenMinuteClockUpdate(action);
        }

        public override void OnGame1_NewDayAfterFade(Action action) {

            this.previousHooks.OnGame1_NewDayAfterFade(action);
        }

        public override void OnGame1_ShowEndOfNightStuff(Action action) {

            this.previousHooks.OnGame1_ShowEndOfNightStuff(action);
        }

        public override void OnGame1_UpdateControlInput(ref KeyboardState keyboardState, ref MouseState mouseState, ref GamePadState gamePadState, Action action) {

            this.previousHooks.OnGame1_UpdateControlInput(ref keyboardState, ref mouseState, ref gamePadState, action);
        }

        public override void OnGameLocation_ResetForPlayerEntry(GameLocation location, Action action) {

            this.previousHooks.OnGameLocation_ResetForPlayerEntry(location, action);
        }

        public override bool OnGameLocation_CheckAction(GameLocation location, Location tileLocation, Rectangle viewport, Farmer who, Func<bool> action) {

            return this.previousHooks.OnGameLocation_CheckAction(location, tileLocation, viewport, who, action);
        }

        public override FarmEvent OnUtility_PickFarmEvent(Func<FarmEvent> action) {

            FarmEvent original = this.previousHooks.OnUtility_PickFarmEvent(action);
            SelectFarmEvent selectEvent = new SelectFarmEvent(original);
            return BookcaseEvents.SelectFarmEvent.Post(selectEvent) ? null : selectEvent.SelectedEvent;
        }
    }
}
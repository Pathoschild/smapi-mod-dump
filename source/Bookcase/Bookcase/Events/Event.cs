/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using System;

namespace Bookcase.Events {

    /// <summary>
    /// Base class for all bookcase events.
    /// </summary>
    public class Event {

        /// <summary>
        /// Whether or not the event has been canceled.
        /// </summary>
        private bool canceled = false;

        /// <summary>
        /// Checks if the event has been canceled.
        /// </summary>
        /// <returns></returns>
        public bool IsCanceled() {

            return this.canceled;
        }

        /// <summary>
        /// Checks if the event can be canceled.
        /// </summary>
        /// <returns>Whether or not the event can be canceled.</returns>
        public virtual bool CanCancel() {

            return false;
        }

        /// <summary>
        /// Cancels an event. Can also uncancel if the optional parameter is set to false.
        /// This will throw an InvalidOperationException if the event can not be canceled.
        /// </summary>
        /// <param name="cancel">Optional parameter to set the cancel state of the event.</param>
        public void Cancel(bool cancel = true) {

            if (!this.CanCancel()) {

                throw new InvalidOperationException($"Tried to cancel an event which can not be canceled! {this.GetType().FullName}");
            }

            this.canceled = cancel;
        }
    }
}
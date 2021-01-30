/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using Bookcase.Registration;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace Bookcase.Mail {

    public class Letter : Identifiable {

        /// <summary>
        /// The contents of the mail. This is the text that the player will read. This uses the vanilla mail format, meaning 
        /// any special character formatting will apply here as well.
        /// </summary>
        public String Contents { get; private set; }

        /// <summary>
        /// A list of gift items to give to the reader. If this is null/empty it will be ignored.
        /// </summary>
        public List<Item> Gifts { get; private set; }

        /// <summary>
        /// Allows code to be executed before the message has been created. The resulting string is what will be used in place
        /// of the value defined for constants. This allows for you to define custom replacement and formatting tokens in your
        /// mail text. Params are the letter and the mail string, return is the results.
        /// </summary>
        public Func<Letter, String, String> PreProcessor;

        /// <summary>
        /// A callback hook for after the letter has been closed. This allows mail to do additional things like track data or
        /// modify the player.
        /// </summary>
        public Action<Letter, LetterViewerMenu> Callback;

        /// <summary>
        /// The color of the text to be displayed in the mail. This is a color id, and not a packed integer. 
        /// </summary>
        public int TextColor;

        /// <summary>
        /// The texture to use for the backround of the mail. If this is null the original background will be used. 
        /// </summary>
        public Texture2D Background;

        public Letter(String id, String contents) : this(new Identifier(id), contents) {

            // String based constructor for lazy people (IE: me).
        }

        public Letter(String id, String contents, int color = -1, Texture2D background = null, Func<Letter, String, String> preProcessor = null, Action<Letter, LetterViewerMenu> callback = null) : this(new Identifier(id), contents) {

            this.TextColor = color;
            this.Background = background;
            this.PreProcessor = preProcessor;
            this.Callback = callback;
        }

        public Letter(Identifier id, String contents) {

            this.Identifier = id;
            this.Contents = contents;
            this.Gifts = new List<Item>();
            this.TextColor = -1;
        }

        /// <summary>
        /// Checks if a farmer has recieved this letter before or not.
        /// </summary>
        /// <param name="farmer">The farmer to check for.</param>
        /// <returns>Whether or not the farmer has recieved this letter in the past.</returns>
        public bool HasRecieved(Farmer farmer) {

            return farmer.mailReceived.Contains(this.Identifier.ToString());
        }

        /// <summary>
        /// Delivers this letter to the specified player.
        /// </summary>
        /// <param name="farmer">The player to deliver the letter to.</param>
        /// <param name="allowRepeats">Should the player be allowed to recieve this letter more than once ever.</param>
        /// <param name="allowDuplicates">Should the player be able to recieve multiple copies of this letter at the same time?</param>
        /// <param name="immediately">Should the player recieve the letter immediately, or on the next day?</param>
        /// <returns>Whether or not the mail was successfuly delivered.</returns>
        public bool DeliverMail(Farmer farmer, bool allowRepeats = false, bool allowDuplicates = false, bool immediately = false) {

            String mailName = this.Identifier.FullString;

            if (!allowRepeats && this.HasRecieved(farmer)) {

                return false;
            }

            if (!allowDuplicates && (farmer.mailForTomorrow.Contains(mailName) || farmer.mailbox.Contains(mailName))) {

                return false;
            }

            else {

                if (immediately) {

                    farmer.mailbox.Add(mailName);
                }

                else {

                    farmer.mailForTomorrow.Add(mailName);
                }

                return true;
            }
        }
    }
}
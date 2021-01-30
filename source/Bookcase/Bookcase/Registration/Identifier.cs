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

namespace Bookcase.Registration {

    /// <summary>
    /// A general name spaced ID object. You should try to avoid creating lots of these objects and save memory where possible.
    /// </summary>
    public class Identifier {

        /// <summary>
        /// The owner of the ID.
        /// </summary>
        public String OwnerId { get; private set; }

        /// <summary>
        /// The ID of the specific thing.
        /// </summary>
        public String ObjectId { get; private set; }

        /// <summary>
        /// A combination of the full ID using the appointed seperator.
        /// </summary>
        internal String FullString;

        /// <summary>
        /// Constructs a new identifier.
        /// </summary>
        /// <param name="joined">The ID as a string. This expects a modid:objid format.</param>
        public Identifier(String joined, bool warn = true) {

            if (joined == null) {

                BookcaseMod.logger.Error("Failed to create Identifier. The joined parameter was null!");
                throw new ArgumentNullException("joined");
            }

            // Split the string on the seperator character.
            String[] parts = joined.Split(Seperator());

            // If parts are not exactly two, display an error and return an error instance.
            if (parts.Length != 2) {

                this.OwnerId = "error";
                this.ObjectId = "error";

                if (warn) {

                    BookcaseMod.logger.Error($"Could not read identifier from {joined}, expected format is owner{Seperator()}object");
                }
            }

            // Set the owner and object id normally.
            else {

                this.OwnerId = parts[0];
                this.ObjectId = parts[1];
            }

            // Construct the full string to save time later.
            this.FullString = $"{OwnerId}{Seperator()}{ObjectId}";
        }

        /// <summary>
        /// Constructs an identifier.
        /// </summary>
        /// <param name="ownerId">The owner of the id.</param>
        /// <param name="objectId">The specifci id.</param>
        public Identifier(String ownerId, String objectId) {

            this.OwnerId = ownerId;
            this.ObjectId = objectId;
            this.FullString = $"{OwnerId}{Seperator()}{ObjectId}";
        }

        /// <summary>
        /// Gets the character used for seperating the owner id from the object id. Usually a colon.
        /// </summary>
        /// <returns>The character used for splitting the id.</returns>
        public virtual char Seperator() {

            return ':';
        }

        public override int GetHashCode() {

            // Use the hash code of the full string.
            return this.FullString.GetHashCode();
        }

        public override bool Equals(object obj) {

            // Compare full strings of the ids. 
            Identifier other = obj as Identifier;
            return other != null && this.FullString.Equals(other.FullString);
        }

        public override string ToString() {

            return FullString;
        }
    }
}
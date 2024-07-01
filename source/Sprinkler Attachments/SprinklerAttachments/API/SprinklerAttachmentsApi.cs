/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mushymato/StardewMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewObject = StardewValley.Object;
using SprinklerAttachments.Framework;

namespace SprinklerAttachments.API
{
    public class SprinklerAttachmentsAPI : ISprinklerAttachmentsAPI
    {
        /// <summary>
        /// Perform planting of seeds and fertilizer around a sprinkler object, if it has an attachment for this mod
        /// </summary>
        /// <param name="sprinkler">possible sprinkler object</param>
        public void ApplySowing(StardewObject sprinkler)
        {
            SprinklerAttachment.ApplySowing(sprinkler);
        }

        /// <summary>
        /// Get held object if the object is a sprinkler and the held object is a sprinkler attachment
        /// </summary>
        /// <param name="sprinkler">possible sprinkler object</param>
        /// <param name="attachment">resulting attachment</param>
        /// <returns>True if attachment is not null</returns>
        public bool TryGetSprinklerAttachment(StardewObject sprinkler, [NotNullWhen(true)] out StardewObject? attachment)
        {
            return SprinklerAttachment.TryGetSprinklerAttachment(sprinkler, out attachment);
        }

        /// <summary>
        /// Get held object's held chest if the object is a sprinkler and the held object is a sprinkler attachment with chest
        /// </summary>
        /// <param name="sprinkler">possible sprinkler object</param>
        /// <param name="attachment">resulting chest</param>
        /// <returns>True if attachment is not null</returns>
        public bool TryGetIntakeChest(StardewObject sprinkler, [NotNullWhen(true)] out StardewObject? attachment)
        {
            return SprinklerAttachment.TryGetSprinklerAttachment(sprinkler, out attachment);
        }

        /// <summary>
        /// Check if an attachment object has CustomField for sowing functionality
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns>True if this attachment can sow seeds and fertilizer</returns>
        public bool IsSowing(StardewObject attachment)
        {
            if (ItemRegistry.GetData(attachment.QualifiedItemId)?.RawData is ObjectData data)
                return ModFieldHelper.IsSowing(data);
            return false;
        }

        /// <summary>
        /// Check if an object has CustomField for sowing functionality
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns>True if this attachment can pressurize</returns>
        public bool IsPressurize(StardewObject attachment)
        {
            if (ItemRegistry.GetData(attachment.QualifiedItemId)?.RawData is ObjectData data)
                return ModFieldHelper.IsPressurize(data);
            return false;
        }
    }
}

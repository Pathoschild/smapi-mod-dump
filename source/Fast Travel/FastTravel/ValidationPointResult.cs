/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DeathGameDev/SDV-FastTravel
**
*************************************************/

namespace FastTravel
{
    public struct ValidationPointResult
    {
        public string messageKeyId { get; }
        public bool isValid { get; }
        
        public ValidationPointResult(bool isValid, string messageKeyId)
        {
            this.messageKeyId = messageKeyId;
            this.isValid = isValid;
        }
    }
}

/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using static SolidFoundations.Framework.Models.ContentPack.Actions.SpecialAction;

namespace SolidFoundations.Framework.Models.ContentPack.Actions
{
    public class MessageAction
    {
        public string Text { get; set; } = "Nothing interesting happens";
        public MessageType Icon { get; set; } = MessageType.Quest;

        public MessageAction()
        {

        }

        public MessageAction(string message, MessageType icon)
        {
            Text = message;
            Icon = icon;
        }
    }
}

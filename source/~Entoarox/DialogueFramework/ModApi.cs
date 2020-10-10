/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley;

namespace DialogueFramework
{
    public class ModApi : IModApi
    {
        public event EventHandler<DialogueEventArgs> ChoiceDialogueOpened;

        internal void FireChoiceDialogueOpened(List<Response> responses)
        {
            ChoiceDialogueOpened?.Invoke(this, new DialogueEventArgs(responses));
        }
    }
}

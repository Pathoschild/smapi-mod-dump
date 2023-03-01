/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace MailFrameworkMod.Api
{
    public interface IMailFrameworkModApi
    {
        public void RegisterContentPack(IContentPack contentPack);

        public void RegisterLetter(ILetter iLetter, Func<ILetter, bool> condition, Action<ILetter> callback = null, Func<ILetter, List<Item>> dynamicItems = null);

        public ILetter GetLetter(string id);

        public string GetMailDataString(string id);
    }
}

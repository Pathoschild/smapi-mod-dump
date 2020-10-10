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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagicJunimoPet
{
    public abstract class Ability
    {
        public SmartPet Pet { get; internal set; }

        public abstract string Id { get; }
        public virtual string GetName()
        {
            return MJPModEntry.SHelper.Translation.Get("Ability." + this.Id + ".Name", new { PetName = this.Pet.Name });
        }
        public virtual string GetDescription()
        {
            return MJPModEntry.SHelper.Translation.Get("Ability." + this.Id + ".Description", new { PetName = this.Pet.Name });
        }
        public abstract void DrawIcon(Rectangle region, SpriteBatch batch);
    }
}

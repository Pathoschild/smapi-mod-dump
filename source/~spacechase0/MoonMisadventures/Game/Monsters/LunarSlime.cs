/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

namespace MoonMisadventures.Game.Monsters
{
    [XmlType("Mods_spacechase0_MoonMisadventures_LunarSlime")]
    public class LunarSlime : GreenSlime
    {
        public LunarSlime() { }
        public LunarSlime( Vector2 pos )
            : base( pos, 0 )
        {
            this.Name = "Lunar Slime";
            this.reloadSprite();
            this.Sprite.SpriteHeight = 24;
            this.Sprite.UpdateSourceRect();
            this.color.Value = Color.White;

            parseMonsterInfo("Tiger Slime");
            Health = MaxHealth = 450;
            DamageToFarmer = 32;
            displayName = "Lunar Slime";
            objectsToDrop.Clear();
        }

        public override void reloadSprite()
        {
            this.Sprite = new AnimatedSprite(Mod.instance.Helper.ModContent.GetInternalAssetName( "assets/enemies/slime" + (Game1.random.Next(2)+1) + ".png").BaseName, 0, 16, 16);
        }
    }
}

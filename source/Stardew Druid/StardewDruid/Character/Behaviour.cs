/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace StardewDruid.Character
{
    public class Behaviour : StardewValley.NPC
    {

        public bool busy;

        public bool follow;

        public List<string> mode;

        public Behaviour(Vector2 position, string map, string Name)
            : base(
            StardewDruid.Map.CharacterData.CharacterSprite(Name),
            position,
            map,
            2,
            Name,
            new(),
            StardewDruid.Map.CharacterData.CharacterPortrait(Name),
            false
            )
        {

        }



    }

}
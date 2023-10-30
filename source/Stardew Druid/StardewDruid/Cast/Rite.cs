/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Cast
{
    public class Rite
    {

        public Dictionary<string, bool> spawnIndex;

        public string castType;

        public string castDisplay;

        public int castLevel;

        public int castCycle;

        public Vector2 castVector;

        public StardewValley.Farmer caster;

        public StardewValley.GameLocation castLocation;

        public int direction;

        public int castDamage;

        public int combatModifier;

        public Dictionary<string, int> castTask;

        public Dictionary<string, int> castToggle;

        public Random randomIndex { get; set; }

        public Rite()
        {

            castLevel = -1;

            castType = "earth";

            caster = Game1.player;

            castLocation = caster.currentLocation;

            castVector = caster.getTileLocation();

            spawnIndex = Map.SpawnData.SpawnIndex(castLocation);

            castTask = new();

            castToggle = new();

            direction = 0;

            castDamage = 10;

            combatModifier = 1;

            randomIndex = new Random();

        }

    }

}

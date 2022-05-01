/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NormanPCN/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasierMonsterEradication
{
    public interface IEasierMonsterEradicationApi
    {
        /// <summary>Return the modified monster eradication goal value. returns -1 if the passed monster could not be identified.
        /// A good place to access the Api could be in the OnSaveLoaded SMAPI event. The goal values will not change during gameplay.
        /// </summary>
        /// <param name="nameOfMonster">You pass the generic monster name as indentified by the game code.
        /// "Slimes", "DustSprites", "Bats", "Serpent", "VoidSpirits", "MagmaSprite", "CaveInsects", "Mummies", "RockCrabs", "Skeletons", "PepperRex", "Duggies".
        /// You can also pass specific game monster names like "Green Slime" if that is more convenient.
        /// </param>
        public int GetMonsterGoal(string nameOfMonster);

    }
}

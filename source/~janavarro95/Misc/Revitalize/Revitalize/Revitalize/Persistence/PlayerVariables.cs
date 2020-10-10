/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Persistence
{
   public class PlayerVariables
    {
        public static int MaxMagic;
        public static int CurrentMagic;
        public static float MagicProficiency;
        public static int MagicExperience;
        public static int MagicExperienceToLevelUp;
        public static int MagicLevel; //used as direct reduction to costs
        public static float ExpCurve;
        public static int MagicToGainUponLevelUp;

        public const float magicProficiencyToGainUponLevelUp=0.01f;
        

        public PlayerVariables()
        {
           
        }

        public static void initializePlayerVariables()
        {
            MaxMagic = 100;
            CurrentMagic = 100;
            MagicProficiency = 0.0f;
            MagicExperience = 0;
            MagicExperienceToLevelUp = 100;
            MagicLevel = 0;
            ExpCurve = 1.05f;
            MagicToGainUponLevelUp = 5;

        }


        public static void savePlayerVariables()
        {


        }

        public static void loadPlayerVariables()
        {

        }

    }
}

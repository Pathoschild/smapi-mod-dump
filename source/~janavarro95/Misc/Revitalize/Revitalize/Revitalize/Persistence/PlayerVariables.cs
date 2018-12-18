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

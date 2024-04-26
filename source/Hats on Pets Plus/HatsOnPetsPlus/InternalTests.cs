/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SymaLoernn/Stardew_HatsOnPetsPlus
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HatsOnPetsPlus
{
    internal class InternalTests
    {

        internal static void InitializeTestData()
        {
            //Regex for transforming this test data to json (without checking for flipped) : 
            //Select : ([\d.\-]*);([\d.\-]*);([\d.\-]*);([\d.\-]*);([\d.\-]*);
            //Replace : {\n"SpriteId":$1,\n"HatOffsetX":$2,\n"HatOffsetY":$3,\n"Direction":$4,\n"Scale":$5,\n"Flipped":false\n},
            String testData = @"0;0;20;2;1.4;
1;0;24;2;1.4;
2;0;20;2;1.4;
3;0;24;2;1.4;
4;4;20;1;1.4;
5;8;20;1;1.4;
6;4;20;1;1.4;
7;8;20;1;1.4;
8;0;20;0;1.4;
9;0;24;0;1.4;
10;0;20;0;1.4;
11;0;24;0;1.4;
12;-8;20;3;1.4;
13;-4;20;3;1.4;
14;-8;20;3;1.4;
15;-4;20;3;1.4;
16;0;20;2;1.4;
17;0;20;-1;1.4;
18;0;20;-1;1.4;
19;0;28;-1;1.4;
20;0;28;-1;1.4;
21;0;25;-1;1.4;
22;0;22;-1;1.4;
23;0;25;-1;1.4;
24;6;24;1;1.4;
25;8;26;1;1.4;
26;10;29;1;1.4;
27;10;30;1;1.4;
28;-3;29;2;1.4;
29;-3;29;2;1.4;
30;14;24;1;1.4;
31;14;24;1;1.4;
24;-6;24;3;1.4;f
25;-8;26;3;1.4;f
26;-10;29;3;1.4;f
27;-10;30;3;1.4;f
28;-3;29;2;1.4;f
29;-3;29;2;1.4;f
30;-14;24;3;1.4;f
31;-14;24;3;1.4;f";
            using (StringReader reader = new StringReader(testData))
            {
                string line;
                int spriteId = -1;
                float? hatOffsetX = null;
                float? hatOffsetY = null;
                int? direction = null;
                float? scale = null;
                bool flipped = false;
                PetData testPet = new PetData();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] data = line.Split(';');
                    // First is sprite ID
                    spriteId = int.Parse(data[0]);
                    // Second is X
                    hatOffsetX = float.Parse(data[1], CultureInfo.InvariantCulture.NumberFormat);
                    // Third is Y
                    hatOffsetY = float.Parse(data[2], CultureInfo.InvariantCulture.NumberFormat);
                    // Fourth is direction (-1 means null)
                    direction = int.Parse(data[3]);
                    direction = ((direction == -1) ? null : direction);
                    // Fifth is scale (0 means null)
                    scale = float.Parse(data[4], CultureInfo.InvariantCulture.NumberFormat);
                    scale = ((scale == 0) ? null : scale);
                    // Sixth is either nothing or f for flipped
                    flipped = (data.Length > 5 && data[5].Equals("f"));

                    SpriteData sprite = new SpriteData(hatOffsetX, hatOffsetY, direction, scale, null);
                    testPet.sprites[new Tuple<int, bool>(spriteId, flipped)] = sprite;
                }
                PetHatsPatch.addPetToDictionnary("Cat", "3", testPet);
            }
        }
    }
}
